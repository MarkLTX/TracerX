using System;
using System.Collections.Generic;
using System.Linq;

namespace TracerX.Properties
{
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    internal sealed partial class Settings
    {
        public Settings()
        {
            // // To add event handlers for saving and changing settings, uncomment the lines below:
            //
            // this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
            //
        }

        protected override void OnSettingsLoaded(object sender, System.Configuration.SettingsLoadedEventArgs e)
        {
            Logger.Current.Debug("Settings were loaded, removing old file view times.");
            RemoveOldViewTimes();
            base.OnSettingsLoaded(sender, e);
        }

        /// <summary>
        /// Remove all ViewedPath with ViewTimes older than the cutoff.
        /// </summary>
        private void RemoveOldViewTimes()
        {
            try
            {
                DateTime cutoff = DateTime.Now.AddDays(-this.DaysToSaveViewTimes);

                if (this.RecentFiles == null)
                {
                    this.RecentFiles = new ViewedPath[0];
                }
                else
                {
                    this.RecentFiles = this.RecentFiles.Where(x => x.ViewTime > cutoff).ToArray();
                }

                if (this.RecentFolders == null)
                {
                    this.RecentFolders = new ViewedPath[0];
                }
                else
                {
                    this.RecentFolders = this.RecentFolders.Where(x => x.ViewTime > cutoff).ToArray();
                }

                if (this.SavedRemoteServers == null)
                {
                    this.SavedRemoteServers = new SavedServer[0];
                }
                else
                {
                    foreach (SavedServer ss in this.SavedRemoteServers)
                    {
                        if (ss.ViewedFiles != null)
                        {
                            ss.ViewedFiles.RemoveAll(x => x.ViewTime < cutoff);
                        }

                        if (ss.ViewedFolders != null)
                        {
                            ss.ViewedFolders.RemoveAll(x => x.ViewTime < cutoff);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Should never happen because we're very careful. Right?
                Logger.Current.Error("Exception in RemoveOldViewTimes: ", ex);
            }
        }
    }
}
