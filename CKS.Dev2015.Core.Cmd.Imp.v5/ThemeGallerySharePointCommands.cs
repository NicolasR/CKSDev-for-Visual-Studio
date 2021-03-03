using CKS.Dev2015.Core.Cmd.Imp.v5.Properties;
using CKS.Dev2015.VisualStudio.SharePoint.Commands.Common.ExtensionMethods;
using CKS.Dev2015.VisualStudio.SharePoint.Commands.Info;
using Microsoft.SharePoint;
using Microsoft.VisualStudio.SharePoint.Commands;
using System;
using System.Collections.Generic;

namespace CKS.Dev2015.VisualStudio.SharePoint.Commands
{
    /// <summary>
    /// The theme gallery commands.
    /// </summary>
    class ThemeGallerySharePointCommands
    {
        /// <summary>
        /// Gets data for each Theme on the SharePoint site, and returns an array of 
        /// serializable objects that contain the data.
        /// </summary>
        /// <param name="context">The command context</param>
        /// <returns>The web part infos</returns>
        [SharePointCommand(ThemeGallerySharePointCommandIds.GetThemes)]
        private static FileNodeInfo[] GetThemes(ISharePointCommandContext context)
        {
            List<FileNodeInfo> nodeInfos = new List<FileNodeInfo>();
            try
            {
                context.Logger.WriteLine(Resources.ThemeGallerySharePointCommands_TryingToRetrieveAvailableThemes, LogCategory.Status);

                SPListItemCollection themes = context.Web.GetCatalog(SPListTemplateType.ThemeCatalog).GetItems(
                    new SPQuery
                    {
                        ViewXml = "<View />"
                    }
                );
                nodeInfos = themes.ToFileNodeInfo();

                context.Logger.WriteLine(Resources.ThemeGallerySharePointCommands_RetrievingException, LogCategory.Status);
            }
            catch (Exception ex)
            {
                context.Logger.WriteLine(String.Format(Resources.ThemeGallerySharePointCommands_RetrievingException,
                          ex.Message,
                          Environment.NewLine,
                          ex.StackTrace), LogCategory.Error);
            }

            return nodeInfos.ToArray();
        }

        /// <summary>
        /// Gets additional property data for the theme gallery.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="nodeInfo">The node info.</param>
        /// <returns></returns>
        [SharePointCommand(ThemeGallerySharePointCommandIds.GetThemeGalleryProperties)]
        private static Dictionary<string, string> GetThemeGalleryProperties(ISharePointCommandContext context,
            ThemeGalleryNodeInfo nodeInfo)
        {
            return SharePointCommandServices.GetProperties(context.Site.GetCatalog(SPListTemplateType.ThemeCatalog));
        }

        /// <summary>
        /// Get the default view url for the theme gallery.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The default view of the theme gallery</returns>
        [SharePointCommand(ThemeGallerySharePointCommandIds.GetThemeGalleryAllItemsUrl)]
        private static string GetThemeGalleryAllItemsUrl(ISharePointCommandContext context)
        {
            return context.Site.GetCatalog(SPListTemplateType.ThemeCatalog).DefaultViewUrl;
        }
    }
}
