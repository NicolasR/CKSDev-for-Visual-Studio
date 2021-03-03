using CKS.Dev2015.VisualStudio.SharePoint.Commands.Info;
using Microsoft.SharePoint;
using Microsoft.VisualStudio.SharePoint.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CKS.Dev2015.VisualStudio.SharePoint.Commands
{
    /// <summary>
    /// The site columns SharePoint commands.
    /// </summary>
    public static class SiteColumnsSharePointCommands
    {
        /// <summary>
        /// Gets the site columns groups.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        [SharePointCommand(SiteColumnsSharePointCommandIds.GetSiteColumnsGroups)]
        private static string[] GetSiteColumnsGroups(ISharePointCommandContext context)
        {
            SPFieldCollection fields = context.Site.RootWeb.Fields;
            IEnumerable<string> allSiteColumnsGroups = (from SPField field
                                                        in fields
                                                        select field.Group);

            string[] siteColumnsGroups = allSiteColumnsGroups.Distinct().ToArray();

            return siteColumnsGroups;
        }

        /// <summary>
        /// Gets the site columns from group.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <returns></returns>
        [SharePointCommand(SiteColumnsSharePointCommandIds.GetSiteColumnsFromGroup)]
        private static FieldNodeInfo[] GetSiteColumnsFromGroup(ISharePointCommandContext context, string groupName)
        {
            SPFieldCollection fields = context.Site.RootWeb.Fields;
            FieldNodeInfo[] siteColumnsFromGroup = (from SPField field
                                                    in fields
                                                    where field.Group == groupName
                                                    select new FieldNodeInfo
                                                    {
                                                        Id = field.Id,
                                                        IsHidden = field.Hidden,
                                                        Title = field.Title
                                                    }).ToArray();

            return siteColumnsFromGroup;
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        [SharePointCommand(SiteColumnsSharePointCommandIds.GetProperties)]
        public static Dictionary<string, string> GetProperties(ISharePointCommandContext context, FieldNodeInfo field)
        {
            Dictionary<string, string> properties = null;

            if (field.Id != Guid.Empty)
            {
                properties = SharePointCommandServices.GetProperties(context.Site.RootWeb.Fields[field.Id]);
            }

            return properties;
        }
    }
}
