using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;

namespace TracerX
{
    // There should be one of these per encrypted output file, used under lock.
    internal class Encryptor
    {
        private AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider();
        private BinaryWriter _logfile;
        private byte[] _encryptionKey;
        private byte[] _pad = new byte[16];

        public Encryptor(BinaryWriter logfile, byte[] encryptionKey)
        {
            aesProvider.Padding = PaddingMode.None; // We do our own padding.
            _logfile = logfile;
            _encryptionKey = encryptionKey;
        }

        /// <summary>
        /// Encrypts the string and writes it to the log file.
        /// If useCache is true, will try to avoid encryption overhead by
        /// looking up the results of a previous encryption of the string in a cache.
        /// Use the cache for things like logger names that are repeated often in the log.
        /// </summary>
        public void Encrypt(string msg)
        {
            if (msg.Length == 0)
            {
                _logfile.Write((int)0);
            }
            else
            {
                using (ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor(_encryptionKey, _encryptionKey))
                using (MemoryStream memStream = new MemoryStream())
                using (CryptoStream cryptoStream = new CryptoStream(memStream, cryptoTransform, CryptoStreamMode.Write))
                {
                    byte[] msgbytes = Encoding.UTF8.GetBytes(msg);

                    Debug.Assert(msgbytes.Length > 0);

                    // Write the length of cleartext bytes to help the decryption code in the viewer.
                    // It infers the length of the cipher bytes from this.
                    _logfile.Write(msgbytes.Length);

                    cryptoStream.Write(msgbytes, 0, msgbytes.Length);

                    // If padding is required, we add our own.
                    if (msgbytes.Length % 16 != 0)
                    {
                        cryptoStream.Write(_pad, 0, 16 - msgbytes.Length % 16);
                    }

                    // This must be called to write all the bytes to _cipherBytes, but
                    // can only be called once.
                    cryptoStream.FlushFinalBlock();

                    _logfile.Write(memStream.GetBuffer(), 0, (int)memStream.Length);
                }
            }
        }
        //public void Encrypt(string msg, bool useCache)
        //{
        //    byte[] result;
        //    //useCache = false;

        //    if (msg.Length == 0)
        //    {
        //        _logfile.Write((int)0);
        //    }
        //    else if (useCache && _cache.TryGetValue(msg, out result))
        //    {
        //        _logfile.Write(result);
        //    }
        //    else
        //    {
        //        using (ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor(_encryptionKey, _encryptionKey))
        //        using (MemoryStream memStream = new MemoryStream())
        //        using (CryptoStream cryptoStream = new CryptoStream(memStream, cryptoTransform, CryptoStreamMode.Write))
        //        {
        //            byte[] msgbytes = Encoding.UTF8.GetBytes(msg);

        //            Debug.Assert(msgbytes.Length > 0);

        //            // Write the length of cleartext bytes to help the decryption code in the viewer.
        //            // It infers the length of the cipher bytes from this.
        //            _logfile.Write(msgbytes.Length);

        //            cryptoStream.Write(msgbytes, 0, msgbytes.Length);

        //            // If padding is required, we add our own.
        //            if (msgbytes.Length % 16 != 0)
        //            {
        //                cryptoStream.Write(_pad, 0, 16 - msgbytes.Length % 16);
        //            }

        //            // This must be called to write all the bytes to _cipherBytes, but
        //            // can only be called once.
        //            cryptoStream.FlushFinalBlock();

        //            _logfile.Write(memStream.GetBuffer(), 0, (int)memStream.Length);

        //            if (useCache)
        //            {
        //                MemoryStream tempStream = new MemoryStream();
        //                BinaryWriter tempWriter = new BinaryWriter(tempStream);
        //                tempWriter.Write(msgbytes.Length);
        //                tempWriter.Write(memStream.ToArray());

        //                _cache[msg] = tempStream.ToArray();
        //            }
        //        }
        //    }
        //}
    }
}
