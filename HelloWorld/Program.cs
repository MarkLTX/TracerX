using TracerX;

namespace HelloWorld
{
    class Program
    {
        // Create/get a Logger named after this class (could be anything).
        static Logger Log = Logger.GetLogger("Program");

        static void Main(string[] args)
        {
            // Open the output file using default settings.
            Logger.BinaryFileLogging.Open();

            // Log a string.
            Log.Info("Hello, World!");
        }
    }
}
