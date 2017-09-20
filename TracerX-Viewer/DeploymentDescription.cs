using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using TracerX.ExtensionMethods;

namespace TracerX
{
    // Singleton Helper class for parsing information out of a ClickOnce deployment manifest.
    internal class DeploymentDescription
    {

        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        public static DeploymentDescription Instance
        {
            get
            {
                if (_initRequired)
                {
                    _instance = GetDeploymentDescription();
                }

                return _instance;
            }
        }

        public string Publisher
        {
            get { return publisher; }
        }

        public string SuiteName
        {
            get { return suiteName; }
        }

        public string Product
        {
            get { return product; }
        }

        public string Shortcut
        {
            get { return shortcut; }
        }

        private static readonly Logger Log = Logger.GetLogger("DeploymentDescription");
        private static DeploymentDescription _instance;
        private static bool _initRequired = true;

        private const string descriptionElement = "description";
        private const string publisherAttribute = "publisher";
        private const string suiteNameAttribute = "suiteName";
        private const string productAttribute = "product";
        private string publisher;
        private string suiteName;
        private string product;
        private string shortcut;

        public static void StartNewViewer(string file = null, string server = null)
        {
            using (Log.InfoCall())
            {
                // If this is a ClickOnce app we can't just 
                // call Process.Start(Application.ExecutablePath) because the new process won't use the
                // same application settings file as the current process.  To use the same settings file we need to
                // start the new process via the same shortcut that started the current process.
                // Here we try to determine that shortcut.

                DeploymentDescription clickOnceInfo = Instance;
                string arguments = "";

                Log.Info("file = ", file);
                Log.Info("server = ", server);
                Log.Info("clickOnceInfo = ", clickOnceInfo);

                if (clickOnceInfo == null)
                {
                    // Not a ClickOnce app.  Just invoke the .exe directly.

                    if (file != null)
                    {
                        arguments = "\"" + file + "\"";
                    }

                    if (!server.NullOrWhiteSpace()) 
                    {
                        arguments += " -server:\"" + server + "\"";
                    }

                    Process.Start(System.Windows.Forms.Application.ExecutablePath, arguments);
                }
                else
                {
                    // We're a ClickOnce app. Invoke process via the ClickOnce shortcut.  In this case the argument string cannot
                    // have embedded spaces or double-quotes. Use the '&' char where we would normally use a space to separate
                    // the arguments and percent-encode each individual argument in case they contain spaces or '&'s.

                    if (file != null)
                    {
                        //arguments = "%22" + file + "%22"; // Truncated at first blank.
                        //arguments = "\"" + file + "\""; // Quotes are removed, embedded blanks look like multiple args.
                        //arguments = "file:" + ArgParser.PercentEncode(file); // Invalid URI and invalid file.
                        //arguments = "-file:\"" + file + "\""; // Truncated at first quote.

                        // The Uri class will correctly percent-encode the file path so there are no embedded spaces.

                        Uri uri = new Uri(file);
                        arguments = uri.AbsoluteUri;
                    }

                    if (!server.NullOrWhiteSpace())
                    {
                        arguments += "&-server:" + ArgParserTX.PercentEncode(server);
                    }

                    Log.Info("argument string: ", arguments);
                    Process.Start(clickOnceInfo.Shortcut, arguments);
                }
            }
        }

        public override string ToString()
        {
            return Shortcut;
        }

        /// <summary>
        ///  Constructor is private because this class is a singleton.  Use the static Instance member.
        /// </summary>
        private DeploymentDescription()
        {
        }
        
        // This gets the application's deployment manifest and parses out the info needed to
        // determine the shortcut path.  Returns null if it fails or this isn't a ClickOnce app.
        private static DeploymentDescription GetDeploymentDescription()
        {
            using (Log.InfoCall())
            {
                DeploymentDescription result = null;

                try
                {
                    if (ApplicationDeployment.IsNetworkDeployed)
                    {
                        result = new DeploymentDescription();

                        Log.Info("Getting ClickOnce deployment manifest: ", ApplicationDeployment.CurrentDeployment.UpdateLocation);

                        using (WebClient client = new WebClient())
                        {
                            string manifest = client.DownloadString(ApplicationDeployment.CurrentDeployment.UpdateLocation);

                            Log.Info("Got manifest of length ", manifest.Length, ", parsing it now.");

                            using (var stringReader = new StringReader(manifest))
                            {
                                var xmlReader = new XmlTextReader(stringReader);
                                result.ExtractDescriptions(xmlReader);
                                Log.Info("Constructed shortcut is ", result.shortcut);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    result = null;
                }

                return result;
            }
        }

        private void ExtractDescriptions(XmlReader appManifest)
        {
            while (appManifest.Read())
            {
                if (appManifest.NodeType == XmlNodeType.Element)
                {
                    if (appManifest.Name == descriptionElement)
                    {
                        appManifest.MoveToFirstAttribute();
                        do
                        {
                            if (appManifest.Name.Contains(publisherAttribute))
                                publisher = appManifest.Value;
                            else if (appManifest.Name.Contains(suiteNameAttribute))
                                suiteName = appManifest.Value;
                            else if (appManifest.Name.Contains(productAttribute))
                                product = appManifest.Value;
                        } while (appManifest.MoveToNextAttribute());

                        shortcut = Environment.GetFolderPath(Environment.SpecialFolder.Programs);

                        if (!publisher.NullOrWhiteSpace()) shortcut = shortcut.AddPath(publisher);
                        if (!suiteName.NullOrWhiteSpace()) shortcut = shortcut.AddPath(suiteName);
                        if (!product.NullOrWhiteSpace()) shortcut = shortcut.AddPath(product);

                        shortcut += ".appref-ms";
                        return;
                    }
                }
            }

        }
    }
}
