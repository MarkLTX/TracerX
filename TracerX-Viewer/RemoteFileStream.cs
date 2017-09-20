using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Diagnostics;
using System.Net;

namespace TracerX
{
    // This class implements a Stream around a connection to the TracerX-Service,
    // allowing the Reader class to read files from the service.
    internal class RemoteFileStream : Stream
    {
        // The file path is something that will be valid on the remote host.
        // The hostName has the form "host[:port]".
        // The credentials can be null for current user's credentials.
        public RemoteFileStream(string path, string hostAndPort, NetworkCredential credentials)
        {
            _filePath = path;
            _hostAndPort = hostAndPort;
            _credentials = credentials;

            OpenRemoteFile();
        }

        private string _filePath;
        private string _hostAndPort;
        private NetworkCredential _credentials;
        private long _fileLength;

        private ProxyFileReader _service;
        private bool _disposed;

        private const int _maxFill = 100000;
        private const int _minFill = 1000;
        private int _nextFill = _maxFill;

        // A cached block of bytes read from the remote file.
        private byte[] _buffer = _emptyBuffer;
        private static readonly byte[] _emptyBuffer = new byte[0];

        // The physical file position corresponding to the first byte of _buffer.
        private long _bufferPos;

        // The current read position in _buffer.
        private int _bufferOffset;

        public bool HasWrapped
        {
            // Should only be set by the "user".

            private get;
            set;
        }
 
        public int ServiceVersion
        {
            get;
            private set;
        }
       
        public override bool CanRead
        {
            get { return _service != null && !_disposed; }
        }

        public override bool CanSeek
        {
            get { return _service != null && !_disposed; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        /// Discards the current _buffer so the next Read() is forced to fetch fresh data from the remote file
        /// and specifies how many bytes the next Read() will fetch.
        /// </summary>
        public void SetNextFill(int nextFill)
        {
            _buffer = _emptyBuffer;
            _nextFill = Math.Min(_maxFill, nextFill);
            Logger.Current.Debug("SetNextFill() set _nextFill to ", _nextFill);
        }

        /// <summary>
        /// Discards the current _buffer so the next Read() is forced to fetch fresh data from the remote file.
        /// Also sets _nextFill depending on whether we've ever wrapped.
        /// </summary>
        public override void Flush()
        {
            // This method should be called in response to a file-changed event to force 
            // fresh data to be fetched from the remote file upon the next Read().

            _buffer = _emptyBuffer;

            if (HasWrapped)
            {
                // We're probably reading from the middle of the file after the logger wrote a record or
                // three. 90% of the time, we can get all the new records with a relatively small read.

                _nextFill = _minFill;
            }
            else
            {
                // Read to EOF or _maxFill bytes, whichever is least.

                _nextFill = _maxFill;
            }
            
            Logger.Current.Debug("Flush() set _nextFill to ", _nextFill);
        }

        public override long Length
        {
            get 
            {
                if (_disposed) throw new ObjectDisposedException("RemoteFileStream");
                long result = _service.GetLength();
                Debug.WriteLine("Got length {0}", result);
                _fileLength = result;
                return result;
            }
        }

        public override long Position
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException("RemoteFileStream");
                return _bufferPos + _bufferOffset;
            }

            set
            {
                if (_disposed) throw new ObjectDisposedException("RemoteFileStream");

                //if (value < Position)
                //{
                //    _nextFill = _minFill;
                //}
                
                Debug.Print("Position set to {0} ({1})", value, value - Position);
                _bufferOffset = (int)(value - _bufferPos);
            }
        }

        /// <summary>
        /// Emulates FileStream.Read(), which may return less than the requested number of bytes,
        /// returns 0 bytes when at the end of the stream, and does not throw an exception at the
        /// end of the stream.
        /// </summary>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_disposed) throw new ObjectDisposedException("RemoteFileStream");
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (offset + count > buffer.Length) throw new ArgumentException("The sum of offset and count is larger than the input buffer length.");
            if (offset < 0 || count < 0) throw new ArgumentOutOfRangeException("offset");
            if (count < 0) throw new ArgumentOutOfRangeException("count");

            // We may already have the data in our buffer.

            if (_bufferOffset < 0 || _bufferOffset + count > _buffer.Length)
            {
                FillLocalBuffer(Position, Math.Max(count, _nextFill));

                // If caller needs more bytes before calling Flush() or SetNextFill(), let's get double the bytes.
                _nextFill = Math.Min(_nextFill * 2, _maxFill);

                // If near end-of-file, we still may not have enough data.

                if (count > _buffer.Length)
                {
                    // We can't return the requested number of bytes.
                    // We might even return 0. The caller, which should
                    // be BinaryReader, will determine if this is an error or not.

                    Logger.Current.Info("RemoteFileStream.Read() returning ", _buffer.Length, " instead of the requested ", count, " bytes.");
                    count = _buffer.Length;
                }
            }

            if (count > 0)
            {
                Array.Copy(_buffer, _bufferOffset, buffer, offset, count);
                _bufferOffset += count;
            }
            else
            {
                // I'm pretty sure returning 0 causes the caller (which is usually BinaryReader) to throw an end-of-file exception.

                Logger.Current.Info("RemoteFileStream.Read() returning ", count, " bytes.");
                Debug.WriteLine("Returning count of {0}", count);
            }

            return count;
        }

        public override int ReadByte()
        {
            if (_disposed) throw new ObjectDisposedException("RemoteFileStream");

            if (_bufferOffset < 0 || _bufferOffset >= _buffer.Length)
            {
                FillLocalBuffer(Position, _nextFill);

                // If caller needs more bytes before calling Flush() or SetNextFill(), let's get double the bytes.
                _nextFill = Math.Min(_nextFill * 2, _maxFill);

                // If near end-of-file, we still may not have enough data.

                if (_buffer.Length == 0)
                {
                    Logger.Current.Info("ReadByte() was unable to return a byte!");
                    return -1;
                }
            }

            int result = _buffer[_bufferOffset];
            ++_bufferOffset;

            return result;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (_disposed) throw new ObjectDisposedException("RemoteFileStream");

            long newPos = Position;

            switch (origin)
            {
                case SeekOrigin.Begin:
                    newPos = offset;
                    break;
                case SeekOrigin.Current:
                    newPos = Position + offset;
                    break;
                case SeekOrigin.End:
                    newPos = Length + offset;
                    break;
            }

            if (newPos < 0) throw new IOException("Seek position is less than 0.");
            Position = newPos;
            return newPos;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                if (disposing && _service != null)
                {
                    var temp = _service;
                    _service = null;

                    temp.CloseFile();
                    temp.Dispose();
                }

                base.Dispose(disposing);                
            }
        }

        private void OpenRemoteFile()
        {
            Logger.Current.Info("Open FileReader service on ", _hostAndPort, " for file: ", _filePath);
            _service = new ProxyFileReader();
            _service.SetHost(_hostAndPort);
            _service.SetCredentials(_credentials);
            ServiceVersion = _service.ExchangeVersion(1);
            _service.OpenFile(_filePath);
        }

        private void FillLocalBuffer(long filePos, int count)
        {
            if (filePos < _fileLength ||
                _buffer == _emptyBuffer // Means Flush() was just called to force a fetch, or this is the very first read.
               )
            {
                try
                {
                    // The actual number of bytes read may be less than count.
                    Logger.Current.Debug("Fetching ", count, " bytes at pos ", filePos);
                    _buffer = _service.ReadBytes(filePos, count, out _fileLength);
                }
                catch (CommunicationException ex)
                {
                    // The connection probably timed out because the file didn't change for a long time.
                    // Make one retry attempt to re-open the connection.  
                    Logger.Current.Info("Got a CommunicationException calling remote ReadBytes(), will try to reconnect: ", ex.Message);
                    OpenRemoteFile();
                    _buffer = _service.ReadBytes(filePos, count, out _fileLength);
                }

                Logger.Current.DebugFormat("Requested {0} bytes, got {1}, file length is {2}", count, _buffer.Length, _fileLength);

                if (_buffer.Length < count)
                {
                    // Supposedly, this can happen even when count bytes are available.
                }

                if (filePos + _buffer.Length >= _fileLength)
                {
                    Logger.Current.Info("Looks like we hit EOF on the server.");
                    Debug.Print("Hit EOF on server.");

                    // For some reason the server sometimes returns a length that is less than the actual length.
                    // If we successfully read Len bytes from position Pos the length must be at least Pos + Len.
                    // If we don't correct it the next read may fail due to appearing to read a position greater
                    // than the file length (has happened).

                    _fileLength = filePos + _buffer.Length;
                }

                // _bufferPos is the physical position in the file that maps
                // to the first byte of _buffer.
                _bufferPos = filePos;
                _bufferOffset = 0;
            }
            else
            {
                // This will indicate the caller has attempted to read past end of file.

                _buffer = _emptyBuffer;
                _bufferPos = Position;
                _bufferOffset = 0;

                Logger.Current.Info("Attempted to read file position ", filePos, " which is greater than file length ", _fileLength);
            }
        }
    }
}
