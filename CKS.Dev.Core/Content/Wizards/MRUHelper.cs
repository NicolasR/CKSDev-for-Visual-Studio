using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards
{
    /// <summary>
    /// The MRU Helper (borrowed from Microsoft.VisualStudio.SharePoint.Internal)
    /// </summary>
    internal sealed class MRUHelper
    {
        #region Fields

        /// <summary>
        /// The max entries
        /// </summary>
        private const int MaxEntries = 10;

        /// <summary>
        /// The URL format
        /// </summary>
        private const string UrlFormat = "SpUrl{0}";

        /// <summary>
        /// The _mru URL list
        /// </summary>
        private List<string> _mruUrlList;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the MRU URL list.
        /// </summary>
        /// <value>
        /// The MRU URL list.
        /// </value>
        public List<string> MruUrlList
        {
            get
            {
                if (this._mruUrlList == null)
                {
                    this._mruUrlList = this.GetUrlsFromRegistry();
                }
                return this._mruUrlList;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="MRUHelper" /> class.
        /// </summary>
        public MRUHelper()
        {

        }

        /// <summary>
        /// Adds to top of MRU list.
        /// </summary>
        /// <param name="urlString">The URL string.</param>
        private void AddToTopOfMruList(string urlString)
        {
            if (this.MruUrlList.IndexOf(urlString) != 0)
            {
                this.MruUrlList.Remove(urlString);
                while (this.MruUrlList.Count >= 10)
                {
                    this.MruUrlList.RemoveAt(this.MruUrlList.Count - 1);
                }
                this.MruUrlList.Insert(0, urlString);
                return;
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// Clears the MRU entries from registry.
        /// </summary>
        private void ClearMruEntriesFromRegistry()
        {
            RegistryKey registryKey = VSRegistry.RegistryRoot(__VsLocalRegistryType.RegType_UserSettings, false);
            using (registryKey)
            {
                if (registryKey != null)
                {
                    RegistryKey registryKey1 = registryKey.OpenSubKey("SharePointTools", true);
                    using (registryKey1)
                    {
                        if (registryKey1 != null)
                        {
                            for (int i = 1; i <= 10; i++)
                            {
                                object[] objArray = new object[1];
                                objArray[0] = i;
                                string str = string.Format(CultureInfo.InvariantCulture, "SpUrl{0}", objArray);
                                registryKey1.DeleteValue(str, false);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the default URL.
        /// </summary>
        /// <returns></returns>
        public string GetDefaultUrl()
        {
            return this.MruUrlList[0];
        }

        /// <summary>
        /// Gets the local host URL.
        /// </summary>
        /// <returns></returns>
        private static string GetLocalHostUrl()
        {
            UriBuilder uriBuilder = new UriBuilder("http", System.Environment.MachineName.ToLowerInvariant());
            return uriBuilder.ToString();
        }

        /// <summary>
        /// Gets the urls from registry.
        /// </summary>
        /// <returns></returns>
        private List<string> GetUrlsFromRegistry()
        {
            string empty = string.Empty;
            List<string> strs = new List<string>();
            RegistryKey registryKey = VSRegistry.RegistryRoot(__VsLocalRegistryType.RegType_UserSettings, false);
            using (registryKey)
            {
                if (registryKey != null)
                {
                    RegistryKey registryKey1 = registryKey.OpenSubKey("SharePointTools", false);
                    using (registryKey1)
                    {
                        if (registryKey1 != null)
                        {
                            for (int i = 1; i <= 10; i++)
                            {
                                object[] objArray = new object[1];
                                objArray[0] = i;
                                string str = string.Format(CultureInfo.InvariantCulture, "SpUrl{0}", objArray);
                                string value = registryKey1.GetValue(str, empty) as string;
                                if (value != null && !(value == empty) && WizardHelpers.IsValidUriString(value, UriKind.Absolute))
                                {
                                    strs.Add(value);
                                }
                            }
                        }
                    }
                }
            }
            if (strs.Count == 0)
            {
                strs.Add(MRUHelper.GetLocalHostUrl());
            }
            return strs;
        }

        /// <summary>
        /// Saves the MRU list to registry.
        /// </summary>
        private void SaveMruListToRegistry()
        {
            RegistryKey registryKey = VSRegistry.RegistryRoot(__VsLocalRegistryType.RegType_UserSettings, true);
            using (registryKey)
            {
                if (registryKey != null)
                {
                    RegistryKey registryKey1 = registryKey.CreateSubKey("SharePointTools");
                    using (registryKey1)
                    {
                        if (registryKey1 != null)
                        {
                            int num = 1;
                            foreach (string mruUrlList in this.MruUrlList)
                            {
                                object[] objArray = new object[1];
                                int num1 = num;
                                num = num1 + 1;
                                objArray[0] = num1;
                                string str = string.Format(CultureInfo.InvariantCulture, "SpUrl{0}", objArray);
                                registryKey1.SetValue(str, mruUrlList, RegistryValueKind.String);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Saves the URL to MRU list.
        /// </summary>
        /// <param name="url">The URL.</param>
        public void SaveUrlToMruList(Uri url)
        {
            string absoluteUri = url.AbsoluteUri;
            this.AddToTopOfMruList(absoluteUri);
            this.ClearMruEntriesFromRegistry();
            this.SaveMruListToRegistry();
        }

        #endregion
    }
}
