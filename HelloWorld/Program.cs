using TracerX;

namespace HelloWorld
{
    class Program
    {
        // Create a Logger named after this class.
        static Logger Log = Logger.GetLogger("Program");

        static void Main(string[] args)
        {
            // Open the output file using default settings.
            Logger.BinaryFileLogging.Open();

            // Log a string.
            Log.Info("Hello, World!");
            System.Threading.Thread.CurrentThread.Name = "x";
        }
    }
}
