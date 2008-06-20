#region Copyright & License
// This is a modified copy of a file in the Apache Log4Net project.
// The modifications include changes to the namespace and comments as well as significant code
// changes that reflect differences in how TracerX is configured vs. Log4Net.
// The remainder of this comment is taken verbatim from the original Apache file.
//
// Copyright 2001-2005 The Apache Software Foundation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//       
#endregion
using System;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Threading;

namespace TracerX {
    public partial class Logger {
        /// <summary>
        /// Use this class to initialize the TracerX environment using an Xml tree.
        /// </summary>
        public static class Xml {
            #region Configure methods
            /// <summary>
            /// Automatically configures the TracerX system based on the 
            /// application's configuration settings.  Does not open the log file
            /// </summary>
            /// <remarks>
            /// <para>
            /// Each application has a configuration file. This has the
            /// same name as the application with '.config' appended.
            /// This file is XML and calling this function prompts the
            /// configurator to look in that file for a section called
            /// <c>TracerX</c> that contains the configuration data.
            /// </para>
            /// <para>
            /// To use this method to configure TracerX you must specify 
            /// the "ConfigurationSectionHandler" section
            /// handler for the <c>TracerX</c> configuration section. 
            /// </para>
            /// </remarks>
            /// <returns>
            /// Returns true if no errors or warnings occurred. If false is returned,
            /// look in the application event log for events logged by TracerX.
            /// </returns>
            static public bool Configure() {
                bool ret = false;
                try {
                    object o = System.Configuration.ConfigurationManager.GetSection("TracerX");
                    XmlElement configElement = o as XmlElement;
                    if (configElement == null) {
                        // Failed to load the TracerX element.
                        EventLogging.Log("XmlConfig: Failed to find configuration section 'TracerX' in the application's .config file. Check your .config file for the <TracerX> and <configSections> elements. The configuration section should look like: <section name=\"TracerX\" type=\"TracerX.XmlConfigSectionHandler, TracerX\" />", EventLogging.XmlConfigError);
                    } else {
                        // Configure using the xml loaded from the config file
                        ret = ConfigureFromXml(configElement);
                    }
                } catch (System.Configuration.ConfigurationException confEx) {
                    if (confEx.BareMessage.IndexOf("Unrecognized element") >= 0) {
                        // Looks like the XML file is not valid
                        EventLogging.Log("XmlConfig: Failed to parse config file. Check your .config file is well formed XML." + confEx.ToString(), EventLogging.XmlConfigError);
                    } else {
                        // This exception is typically due to the assembly name not being correctly specified in the section type.
                        string configSectionStr = "<section name=\"TracerX\" type=\"TracerX.XmlConfigSectionHandler, TracerX\" />\n\n";
                        EventLogging.Log("XmlConfig: Failed to parse config file. Is the <configSections> specified as: " + configSectionStr + confEx.ToString(), EventLogging.XmlConfigError);
                    }
                }

                return ret;
            }

            static public bool Configure(string configFilePath) {
                return Configure(new FileInfo(configFilePath));
            }

            /// <summary>
            /// Configures TracerX using the specified configuration file.
            /// </summary>
            /// <param name="configFile">The XML file to load the configuration from.</param>
            /// <remarks>
            /// The configuration file must be valid XML. It must contain
            /// at least one element called <c>TracerX</c> that holds
            /// the TracerX configuration data.
            /// </remarks>
            static public bool Configure(FileInfo configFile) {
                bool ret = false;

                if (configFile == null) {
                    EventLogging.Log("XmlConfig: Configure called with null 'configFile' parameter", EventLogging.XmlConfigError);
                } else {
                    // Have to use File.Exists() rather than configFile.Exists()
                    // because configFile.Exists() caches the value, not what we want.
                    if (File.Exists(configFile.FullName)) {
                        // Open the file for reading
                        FileStream fs = null;

                        // Try hard to open the file
                        for (int retry = 5; --retry >= 0; ) {
                            try {
                                fs = configFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
                                break;
                            } catch (IOException ex) {
                                if (retry == 0) {
                                    EventLogging.Log("XmlConfig: Failed to open XML config file [" + configFile.Name + "]\n" + ex.ToString(), EventLogging.XmlConfigError);

                                    // The stream cannot be valid
                                    fs = null;
                                }
                                System.Threading.Thread.Sleep(250);
                            }
                        }

                        if (fs != null) {
                            try {
                                // Load the configuration from the stream
                                ret = Configure(fs);
                            } finally {
                                // Force the file closed whatever happens
                                fs.Close();
                            }
                        }
                    } else {
                        EventLogging.Log("XmlConfig: config file [" + configFile.FullName + "] not found. Configuration unchanged.", EventLogging.XmlConfigError);
                    }
                }

                return ret;
            }

            /// <summary>
            /// Configures TracerX using the specified configuration data stream.
            /// </summary>
            /// <param name="configStream">A stream to load the XML configuration from.</param>
            /// <remarks>
            /// <para>
            /// The configuration data must be valid XML. It must contain
            /// at least one element called <c>TracerX</c> that holds
            /// the TracerX configuration data.
            /// </para>
            /// <para>
            /// Note that this method will NOT close the stream parameter.
            /// </para>
            /// </remarks>
            static public bool Configure(Stream configStream) {
                bool ret = false;

                if (configStream == null) {
                    EventLogging.Log("XmlConfig: Configure called with null 'configStream' parameter", EventLogging.XmlConfigError);
                } else {
                    // Load the config file into a document
                    XmlDocument doc = new XmlDocument();
                    try {
                        // Allow the DTD to specify entity includes
                        XmlReaderSettings settings = new XmlReaderSettings();
                        settings.ProhibitDtd = false;

                        // Create a reader over the input stream
                        XmlReader xmlReader = XmlReader.Create(configStream, settings);

                        // load the data into the document
                        doc.Load(xmlReader);
                    } catch (Exception ex) {
                        EventLogging.Log("XmlConfig: Error while loading XML configuration.\n" + ex.ToString(), EventLogging.XmlConfigError);

                        // The document is invalid
                        doc = null;
                    }

                    if (doc != null) {
                        // Configure using the 'TracerX' element
                        XmlNodeList configNodeList = doc.GetElementsByTagName("TracerX");
                        if (configNodeList.Count == 0) {
                            EventLogging.Log("XmlConfig: XML configuration does not contain an <TracerX> element. Configuration Aborted.", EventLogging.XmlConfigError);
                        } else if (configNodeList.Count > 1) {
                            EventLogging.Log("XmlConfig: XML configuration contains [" + configNodeList.Count + "] <TracerX> elements. Only one is allowed. Configuration Aborted.", EventLogging.XmlConfigError);
                        } else {
                            ret = ConfigureFromXml(configNodeList[0] as XmlElement);
                        }
                    }
                }

                return ret;
            }

            /// <summary>
            /// Configures TracerX using an <c>TracerX</c> element.
            /// </summary>
            /// <param name="element">The element to parse.</param>
            /// <remarks>
            /// <para>
            /// Loads the TracerX configuration from the XML element
            /// supplied as <paramref name="element"/>.
            /// </para>
            /// <para>
            /// This method is ultimately called by one of the Configure methods 
            /// to load the configuration from an <see cref="XmlElement"/>.
            /// </para>
            /// </remarks>
            static public bool ConfigureFromXml(XmlElement element) {
                if (element == null) {
                    EventLogging.Log("XmlConfig: ConfigureFromXml called with null 'element' parameter", EventLogging.XmlConfigError);
                    return false;
                } else {
                    // Copy the xml data into the root of a new document
                    // this isolates the xml config data from the rest of
                    // the document.
                    XmlDocument newDoc = new XmlDocument();
                    XmlElement newElement = (XmlElement)newDoc.AppendChild(newDoc.ImportNode(element, true));
                    element = null;

                    foreach (XmlNode node in newElement.ChildNodes) {
                        //System.Diagnostics.Debug.Print("Node type " + node.NodeType + " with name " + node.Name);
                        switch (node.NodeType) {
                            case XmlNodeType.Element:
                                // Should be FileLogging, MaxEventNumber, or Logger.
                                XmlElement e = (XmlElement)node;
                                switch (e.Name.ToLower()) {
                                    case "logfile":
                                        ParseLogFile(e);
                                        break;
                                    case "maxeventnumber":
                                    case "maxinternaleventnumber":
                                        Logger.EventLogging.MaxInternalEventNumber = GetUintVal(e, "value", Logger.EventLogging.MaxInternalEventNumber);
                                        break;
                                    case "internaleventoffset":
                                        Logger.EventLogging.InternalEventOffset = GetUintVal(e, "value", Logger.EventLogging.InternalEventOffset);
                                        break;
                                    case "eventlogformatstring":
                                        Logger.EventLogging.FormatString = e.GetAttribute("value");
                                        break;
                                    case "consoleformatstring":
                                        Logger.ConsoleLogging.FormatString = e.GetAttribute("value");
                                        break;
                                    case "debugformatstring":
                                        Logger.DebugLogging.FormatString = e.GetAttribute("value");
                                        break;
                                    case "maxunhandledexceptionslogged":
                                        Logger.MaxUnhandledExceptionsLogged = GetUintVal(e, "value", Logger.MaxUnhandledExceptionsLogged);
                                        break;
                                    //case "treatrecycledthreadsasnew":
                                    //    Configuration.TreatRecycledThreadsAsNew = GetBoolVal(e, "value", Configuration.TreatRecycledThreadsAsNew);
                                    //    break;
                                    case "logger":
                                        ParseLogger(e);
                                        break;
                                    default:
                                        // Unexpected element.
                                        string msg = "The '{0}' XML element contains unexpected child element '{1}'.\n\n";
                                        _warnings += string.Format(msg, newElement.Name, node.Name);
                                        System.Diagnostics.Debug.Print("Unexpected XML element in TracerX element: " + e.Name);
                                        break;
                                }
                                break;
                            case XmlNodeType.EndElement:
                            case XmlNodeType.Comment:
                                break;
                            default:
                                // Unexpected node type
                                string msg3 = "The '{0}' XML element contains unexpected child '{1}' of type '{2}'.\n\n";
                                _warnings += string.Format(msg3, newElement.Name, node.Name, node.NodeType);
                                System.Diagnostics.Debug.Print("Unexpected XML node type " + node.NodeType + " with name " + node.Name);
                                break;
                        } // switch (NodeType)
                    } // foreach node in TracerX.

                    if (_warnings != string.Empty) {
                        // Log the warnings and reset _warnings.
                        EventLogging.Log("The following warnings occurred while processing the TracerX XML configuration.\n\n" + _warnings, EventLogging.XmlConfigWarning);
                        _warnings = string.Empty;
                        return false;
                    }

                    return true;
                }
            }

            #endregion Configure static methods

            #region ConfigureAndWatch 

            /// <summary>
            /// Configures TracerX using the file specified, monitors the file for changes 
            /// and reloads the configuration if a change is detected.
            /// </summary>
            /// <param name="configFile">The XML file to load the configuration from.</param>
            /// <remarks>
            /// <para>
            /// The configuration file must be valid XML. It must contain
            /// at least one element called <c>TracerX</c> that holds
            /// the configuration data.
            /// </para>
            /// <para>
            /// The configuration file will be monitored using a <see cref="FileSystemWatcher"/>
            /// and depends on the behavior of that class.
            /// </para>
            /// </remarks>
            static public bool ConfigureAndWatch(FileInfo configFile) {
                bool ret = false;

                if (configFile == null) {
                    EventLogging.Log("XmlConfig: ConfigureAndWatch called with null 'configStream' parameter", EventLogging.XmlConfigError);
                } else {
                    // Configure TracerX now
                    ret = Configure(configFile);

                    try {
                        // Create a watch handler that will reload the
                        // configuration whenever the config file is modified.
                        ConfigureAndWatchHandler.StartWatching(configFile);
                    } catch (Exception ex) {
                        EventLogging.Log("XmlConfig: Failed to initialize configuration file watcher for file [" + configFile.FullName + "]\n" + ex.ToString(), EventLogging.XmlConfigWarning);
                    }
                }

                return ret;
            }

            #endregion ConfigureAndWatch 

            #region Privates
            // Warnings are collected here and logged all at once.
            static string _warnings = string.Empty;

            // Get a uint attribute value from an element.  Append a warning message if problems occur.
            static private uint GetUintVal(XmlElement element, string valName, uint curval) {
                string val = element.GetAttribute(valName);
                try {
                    return uint.Parse(val);
                } catch {
                    string msg = "XML element '{0}' does not contain required attribute '{1}' or the value could not be converted to uint.\n\n";
                    _warnings += string.Format(msg, element.Name, valName);
                    return curval;
                }
            }

            // Get a bool attribute value from an element.  Append a warning message if problems occur.
            static private bool GetBoolVal(XmlElement element, string valName, bool curval) {
                string val = element.GetAttribute(valName);
                try {
                    return bool.Parse(val);
                } catch {
                    string msg = "XML element '{0}' does not contain required attribute '{1}' or the value could not be converted to bool.\n\n";
                    _warnings += string.Format(msg, element.Name, valName);
                    return curval;
                }
            }

            // Get a string attribute value from an element.  Append a warning message if problems occur.
            static private bool GetStrVal(XmlElement element, string valName, bool curval) {
                string val = element.GetAttribute(valName);
                try {
                    return bool.Parse(val);
                } catch {
                    string msg = "XML element '{0}' does not contain required attribute '{1}' or the value could not be converted to bool.\n\n";
                    _warnings += string.Format(msg, element.Name, valName);
                    return curval;
                }
            }

            // Parse the FileLogging element.
            static private void ParseLogFile(XmlElement element) {
                foreach (XmlNode node in element.ChildNodes) {
                    //System.Diagnostics.Debug.Print("Node type " + node.NodeType + " with name " + node.Name);
                    switch (node.NodeType) {
                        case XmlNodeType.Element:
                            XmlElement subElement = (XmlElement)node;
                            switch (node.Name.ToLower()) {
                                case "directory":
                                case "logdirectory":
                                    Logger.FileLogging.Directory = subElement.GetAttribute("value");
                                    break;
                                case "maxsizemb":
                                    Logger.FileLogging.MaxSizeMb = GetUintVal(subElement, "value", Logger.FileLogging.MaxSizeMb);
                                    break;
                                case "archives":
                                    Logger.FileLogging.Archives = GetUintVal(subElement, "value", Logger.FileLogging.Archives);
                                    break;
                                case "circularstartsizekb":
                                    Logger.FileLogging.CircularStartSizeKb = GetUintVal(subElement, "value", Logger.FileLogging.CircularStartSizeKb);
                                    break;
                                case "circularstartdelayseconds":
                                    Logger.FileLogging.CircularStartDelaySeconds = GetUintVal(subElement, "value", Logger.FileLogging.CircularStartDelaySeconds);
                                    break;
                                case "addtolistofrecentlycreatedfiles":
                                    Logger.FileLogging.AddToListOfRecentlyCreatedFiles = GetBoolVal(subElement, "value", Logger.FileLogging.AddToListOfRecentlyCreatedFiles);
                                    break;
                                case "logpotentiallysensitivedataatstartup":
                                    _warnings += "LogPotentiallySensitiveDataAtStartup is deprecated.\n\n";
                                    System.Diagnostics.Debug.Print("LogPotentiallySensitiveDataAtStartup is deprecated.");
                                    break;
                                default:
                                    // Unexpected element.
                                    string msg = "XML element '{0}' contains unexpected sub-element '{1}'.\n\n";
                                    _warnings += string.Format(msg, element.Name, subElement.Name);
                                    System.Diagnostics.Debug.Print("Unexpected XML element in FileLogging element: " + node.Name);
                                    break;
                            }
                            break;
                        case XmlNodeType.EndElement:
                        case XmlNodeType.Comment:
                            break;
                        default:
                            // Unexpected node type.
                            string msg2 = "XML element '{0}' contains unexpected child '{1}' of type '{2}'.\n\n";
                            _warnings += string.Format(msg2, element.Name, node.Name, node.NodeType);
                            System.Diagnostics.Debug.Print("Unexpected XML node type " + node.NodeType + " with name " + node.Name);
                            break;
                    } // switch (NodeType)
                } // foreach node in TracerX.
            }

            static private void ParseLogger(XmlElement element) {
                string name = element.GetAttribute("name");
                string levelStr;
                TraceLevel level;
                Logger logger;

                if (name == null || name == string.Empty) {
                    string msg = "XML element '{0}' has a missing or invalid 'Name' attribute.  This element will be ignored.\n\n";
                    _warnings += string.Format(msg, element.Name);
                    return;
                }

                logger = Logger.GetLogger(name);

                foreach (XmlNode node in element.ChildNodes) {
                    //System.Diagnostics.Debug.Print("Node type " + node.NodeType + " with name " + node.Name);
                    switch (node.NodeType) {
                        case XmlNodeType.Element:
                            // Should be one of the trace level elements with a 'value' attribute. 
                            // Parse out the value.
                            try {
                                XmlElement levelElement = (XmlElement)node;
                                levelStr = levelElement.GetAttribute("value");
                                level = (TraceLevel)Enum.Parse(typeof(TraceLevel), levelStr, true);
                            } catch {
                                string msg = "The 'level' element under the 'logger' element with name='{0}' has a missing or invalid 'value' attribute.\n\n";
                                _warnings += string.Format(msg, name);
                                level = TraceLevel.Undefined;
                            }

                            switch (node.Name.ToLower()) {
                                case "level":
                                case "filetracelevel":
                                    logger.FileTraceLevel = level;
                                    break;
                                case "consoletracelevel":
                                    logger.ConsoleTraceLevel = level;
                                    break;
                                case "debugtracelevel":
                                    logger.DebugTraceLevel = level;
                                    break;
                                case "eventlogtracelevel":
                                    logger.EventLogTraceLevel = level;
                                    break;
                                default:
                                    // Unexpected element.
                                    string msg2 = "The 'logger' XML element with name='{0}' has an unexpected child named '{1}'.\n\n";
                                    _warnings += string.Format(msg2, name, node.Name);
                                    System.Diagnostics.Debug.Print("Unexpected XML element in Logger element: " + node.Name);
                                    break;
                            }
                            break;
                        case XmlNodeType.EndElement:
                        case XmlNodeType.Comment:
                            break;
                        default:
                            // Unexpected node type.
                            string msg3 = "The 'logger' XML element with name='{0}' contains unexpected child '{1}' of type '{2}'.\n\n";
                            _warnings += string.Format(msg3, name, node.Name, node.NodeType);
                            System.Diagnostics.Debug.Print("Unexpected XML node type " + node.NodeType + " with name " + node.Name);
                            break;
                    } // switch (NodeType)
                } // foreach node in Logger.
            } // Parse Logger

            #region ConfigureAndWatchHandler

            /// <summary>
            /// Class used to watch config files.
            /// </summary>
            /// <remarks>
            /// <para>
            /// Uses the <see cref="FileSystemWatcher"/> to monitor
            /// changes to a specified file. Because multiple change notifications
            /// may be raised when the file is modified, a timer is used to
            /// compress the notifications into a single event. The timer
            /// waits for <see cref="TimeoutMillis"/> time before delivering
            /// the event notification. If any further <see cref="FileSystemWatcher"/>
            /// change notifications arrive while the timer is waiting it
            /// is reset and waits again for <see cref="TimeoutMillis"/> to
            /// elapse.
            /// </para>
            /// </remarks>
            private sealed class ConfigureAndWatchHandler {
                /// <summary>
                /// Watch a specified config file used to configure TracerX
                /// </summary>
                /// <param name="configFile">The configuration file to watch.</param>
                internal static void StartWatching(FileInfo configFile) {
                    new ConfigureAndWatchHandler(configFile);
                }

                /// <summary>
                /// Holds the FileInfo used to configure the XmlConfigurator
                /// </summary>
                private FileInfo m_configFile;

                /// <summary>
                /// The timer used to compress the notification events.
                /// </summary>
                private Timer m_timer;

                /// <summary>
                /// The default amount of time to wait after receiving notification
                /// before reloading the config file.
                /// </summary>
                private const int TimeoutMillis = 500;

                /// <summary>
                /// Initializes a new instance of the <see cref="ConfigureAndWatchHandler" /> class.
                /// </summary>
                /// <param name="configFile">The configuration file to watch.</param>
                /// <remarks>
                /// <para>
                /// Initializes a new instance of the <see cref="ConfigureAndWatchHandler" /> class.
                /// </para>
                /// </remarks>
                private ConfigureAndWatchHandler(FileInfo configFile) {
                    m_configFile = configFile;

                    // Create a new FileSystemWatcher and set its properties.
                    FileSystemWatcher watcher = new FileSystemWatcher();

                    watcher.Path = m_configFile.DirectoryName;
                    watcher.Filter = m_configFile.Name;

                    // Set the notification filters
                    watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName;

                    // Add event handlers. OnChanged will do for all event handlers that fire a FileSystemEventArgs
                    watcher.Changed += new FileSystemEventHandler(ConfigureAndWatchHandler_OnChanged);
                    watcher.Created += new FileSystemEventHandler(ConfigureAndWatchHandler_OnChanged);
                    watcher.Deleted += new FileSystemEventHandler(ConfigureAndWatchHandler_OnChanged);
                    watcher.Renamed += new RenamedEventHandler(ConfigureAndWatchHandler_OnRenamed);

                    // Begin watching.
                    watcher.EnableRaisingEvents = true;

                    // Create the timer that will be used to deliver events. Set as disabled
                    m_timer = new Timer(new TimerCallback(OnWatchedFileChange), null, Timeout.Infinite, Timeout.Infinite);
                }

                /// <summary>
                /// Event handler used by <see cref="ConfigureAndWatchHandler"/>.
                /// </summary>
                /// <param name="source">The <see cref="FileSystemWatcher"/> firing the event.</param>
                /// <param name="e">The argument indicates the file that caused the event to be fired.</param>
                /// <remarks>
                /// <para>
                /// This handler reloads the configuration from the file when the event is fired.
                /// </para>
                /// </remarks>
                private void ConfigureAndWatchHandler_OnChanged(object source, FileSystemEventArgs e) {
                    EventLogging.Log("ConfigureAndWatchHandler: " + e.ChangeType + " [" + m_configFile.FullName + "]", EventLogging.ConfigFileChanged);

                    // Deliver the event in TimeoutMillis time
                    // timer will fire only once
                    m_timer.Change(TimeoutMillis, Timeout.Infinite);
                }

                /// <summary>
                /// Event handler used by <see cref="ConfigureAndWatchHandler"/>.
                /// </summary>
                /// <param name="source">The <see cref="FileSystemWatcher"/> firing the event.</param>
                /// <param name="e">The argument indicates the file that caused the event to be fired.</param>
                /// <remarks>
                /// <para>
                /// This handler reloads the configuration from the file when the event is fired.
                /// </para>
                /// </remarks>
                private void ConfigureAndWatchHandler_OnRenamed(object source, RenamedEventArgs e) {
                    EventLogging.Log("ConfigureAndWatchHandler: " + e.ChangeType + " [" + m_configFile.FullName + "]", EventLogging.ConfigFileChanged);

                    // Deliver the event in TimeoutMillis time
                    // timer will fire only once
                    m_timer.Change(TimeoutMillis, Timeout.Infinite);
                }

                /// <summary>
                /// Called by the timer when the configuration has been updated.
                /// </summary>
                /// <param name="state">null</param>
                private void OnWatchedFileChange(object state) {
                    Configure(m_configFile);
                }
            }

            #endregion ConfigureAndWatchHandler

            #endregion Privates
        }
    }
}