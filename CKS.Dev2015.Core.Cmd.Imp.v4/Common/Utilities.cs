using System;

namespace CKS.Dev2015.VisualStudio.SharePoint.Commands.Common
{
    /// <summary>
    /// Utilities for the commands.
    /// </summary>
    internal static class Utilities
    {
        #region Methods

        /// <summary>
        /// Combine the string elements into a url.
        /// </summary>
        /// <param name="urls">The elements.</param>
        /// <returns>The combined url.</returns>
        public static string CombineUrl(params string[] urls)
        {
            if (urls == null)
            {
                throw new ArgumentNullException("urls");
            }

            for (int i = 0; i < urls.Length; i++)
            {
                string s = urls[i];
                if (s != null && s.StartsWith("/") || s.EndsWith("/"))
                {
                    urls[i] = s.Trim('/');
                }
            }

            string url = String.Join("/", urls);

            if (!url.StartsWith("/"))
            {
                url = String.Format("/{0}", url);
            }

            return url;
        }

        #endregion
    }
}
