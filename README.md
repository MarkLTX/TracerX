# TracerX
TracerX is a logging framework for .NET.  It has three major components.

1. **TracerX-Logger.dll** - The DLL you must use in your application to send logging to various destinations, primarily a file.  This is available as a NuGet package called TracerX-Logger.
1. **TracerX-Viewer.exe** - A Windows Forms GUI application for viewing log files with powerful filtering, coloring, and navigation features.  It has a "start page" that lists all the logs available on your computer or a remotely connected computer.  Just click one to view it.
1. **TracerX-Service.exe** - This is an optional component that enables viewing logs on remote servers without using shares. When you connect to the service with the viewer you see all available log files just as you do on your local computer.

The best documentation currently available is the CodeProject article at https://www.codeproject.com/Articles/23424/TracerX-Logger-and-Viewer-for-NET.  

Loving VisualSVN as of 2019-06-01!
