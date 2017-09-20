using System;
namespace TracerX
{
    public interface IFileWatcher
    {
        event System.IO.FileSystemEventHandler Changed;
        event System.IO.RenamedEventHandler Renamed;
        bool Stopped { get; }
        void Stop();
    }
}
