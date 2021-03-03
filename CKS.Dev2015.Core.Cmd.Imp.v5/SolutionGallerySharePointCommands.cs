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
    /// The solution gallery commands.
    /// </summary>
    class SolutionGallerySharePointCommands
    {
        /// <summary>
        /// Gets data for each solution on the SharePoint site, and returns an array of 
        /// serializable objects that contain the data.
        /// </summary>
        /// <param name="context">The command context</param>
        /// <returns>The solution infos</returns>
        [SharePointCommand(SolutionGallerySharePointCommandIds.GetSolutions)]
        private static FileNodeInfo[] GetSolutions(ISharePointCommandContext context)
        {
            List<FileNodeInfo> nodeInfos = new List<FileNodeInfo>();
            try
            {
                context.Logger.WriteLine(Resources.SolutionGallerySharePointCommands_TryingToRetrieveAvailableSolutions, LogCategory.Status);

                SPListItemCollection solutions = context.Web.GetCatalog(SPListTemplateType.SolutionCatalog).GetItems(
                    new SPQuery
                    {
                        ViewXml = "<View />"
                    }
                );
                nodeInfos = solutions.ToFileNodeInfo();

                context.Logger.WriteLine(Resources.SolutionGallerySharePointCommands_RetrievingException, LogCategory.Status);
            }
            catch (Exception ex)
            {
                context.Logger.WriteLine(String.Format(Resources.SolutionGallerySharePointCommands_RetrievingException,
                          ex.Message,
                          Environment.NewLine,
                          ex.StackTrace), LogCategory.Error);
            }

            return nodeInfos.ToArray();
        }

        /// <summary>
        /// Gets additional property data for the solution gallery.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="nodeInfo">The node info.</param>
        /// <returns></returns>
        [SharePointCommand(SolutionGallerySharePointCommandIds.GetSolutionGalleryProperties)]
        private static Dictionary<string, string> GetSolutionGalleryProperties(ISharePointCommandContext context,
            SolutionGalleryNodeInfo nodeInfo)
        {
            return SharePointCommandServices.GetProperties(context.Site.GetCatalog(SPListTemplateType.SolutionCatalog));
        }

        /// <summary>
        /// Get the default view url for the solution gallery.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The default view of the solution gallery</returns>
        [SharePointCommand(SolutionGallerySharePointCommandIds.GetSolutionGalleryAllItemsUrl)]
        private static string GetSolutionGalleryAllItemsUrl(ISharePointCommandContext context)
        {
            return context.Site.GetCatalog(SPListTemplateType.SolutionCatalog).DefaultViewUrl;
        }
    }
}
