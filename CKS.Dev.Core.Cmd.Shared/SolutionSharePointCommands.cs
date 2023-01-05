using CKS.Dev.VisualStudio.SharePoint.Commands.Info;
using Microsoft.SharePoint;
using Microsoft.VisualStudio.SharePoint.Commands;
using System.Collections.Generic;

namespace CKS.Dev.VisualStudio.SharePoint.Commands
{
    /// <summary>
    /// The solution commands.
    /// </summary>
    internal class SolutionSharePointCommands
    {
        #region Methods

        /// <summary>
        /// Gets additional property data for a specific solution.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="nodeInfo">The node info</param>
        /// <returns>The properties</returns>
        [SharePointCommand(SolutionSharePointCommandIds.GetSolutionProperties)]
        private static Dictionary<string, string> GetSolutionProperties(ISharePointCommandContext context,
            FileNodeInfo nodeInfo)
        {
            SPList solutions = context.Site.GetCatalog(SPListTemplateType.SolutionCatalog);
            SPListItem solution = solutions.Items[nodeInfo.UniqueId];

            return SharePointCommandServices.GetProperties(solution);
        }

        #endregion
    }
}
