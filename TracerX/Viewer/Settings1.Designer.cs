﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.832
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TracerX.Viewer {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "8.0.0.0")]
    internal sealed partial class Settings1 : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings1 defaultInstance = ((Settings1)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings1())));
        
        public static Settings1 Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.Specialized.StringCollection MRU {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["MRU"]));
            }
            set {
                this["MRU"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public int[] ColWidths {
            get {
                return ((int[])(this["ColWidths"]));
            }
            set {
                this["ColWidths"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public int[] ColIndices {
            get {
                return ((int[])(this["ColIndices"]));
            }
            set {
                this["ColIndices"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public bool[] ColSelections {
            get {
                return ((bool[])(this["ColSelections"]));
            }
            set {
                this["ColSelections"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string OpenDir {
            get {
                return ((string)(this["OpenDir"]));
            }
            set {
                this["OpenDir"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool DuplicateTimes {
            get {
                return ((bool)(this["DuplicateTimes"]));
            }
            set {
                this["DuplicateTimes"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool RelativeTime {
            get {
                return ((bool)(this["RelativeTime"]));
            }
            set {
                this["RelativeTime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool LineNumSeparator {
            get {
                return ((bool)(this["LineNumSeparator"]));
            }
            set {
                this["LineNumSeparator"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ExpandNewlines {
            get {
                return ((bool)(this["ExpandNewlines"]));
            }
            set {
                this["ExpandNewlines"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public char IndentChar {
            get {
                return ((char)(this["IndentChar"]));
            }
            set {
                this["IndentChar"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3")]
        public int IndentAmount {
            get {
                return ((int)(this["IndentAmount"]));
            }
            set {
                this["IndentAmount"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.Specialized.StringCollection FindStrings {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["FindStrings"]));
            }
            set {
                this["FindStrings"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int AutoRefreshInterval {
            get {
                return ((int)(this["AutoRefreshInterval"]));
            }
            set {
                this["AutoRefreshInterval"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool KeepFilter {
            get {
                return ((bool)(this["KeepFilter"]));
            }
            set {
                this["KeepFilter"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int VersionCheckInterval {
            get {
                return ((int)(this["VersionCheckInterval"]));
            }
            set {
                this["VersionCheckInterval"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2007-12-15")]
        public global::System.DateTime VersionLastChecked {
            get {
                return ((global::System.DateTime)(this["VersionLastChecked"]));
            }
            set {
                this["VersionLastChecked"] = value;
            }
        }
    }
}
