using CKS.Dev.VisualStudio.SharePoint.Commands.Info;
using Microsoft.SharePoint.Publishing;
using Microsoft.VisualStudio.SharePoint.Commands;
using System.Collections.Generic;
using System.Linq;

namespace CKS.Dev.VisualStudio.SharePoint.Commands
{
    /// <summary>
    /// The site commands
    /// </summary>
    public static class SiteCommands
    {
        /// <summary>
        /// Determines whether [is publishing site] [the specified context].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///   <c>true</c> if [is publishing site] [the specified context]; otherwise, <c>false</c>.
        /// </returns>
        [SharePointCommand(SiteCommandIds.IsPublishingSiteCommandId)]
        private static bool IsPublishingSite(ISharePointCommandContext context)
        {
            bool isPublishingSite = PublishingWeb.IsPublishingWeb(context.Web);

            return isPublishingSite;
        }

        /// <summary>
        /// Gets the publishing pages.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        [SharePointCommand(SiteCommandIds.GetPublishingPagesCommandId)]
        private static List<PublishingPageInfo> GetPublishingPages(ISharePointCommandContext context)
        {
            List<PublishingPageInfo> pages = new List<PublishingPageInfo>();

            PublishingWeb publishingWeb = PublishingWeb.GetPublishingWeb(context.Web);
            PublishingPageCollection publishingPages = publishingWeb.GetPublishingPages();
            pages = (from PublishingPage page
                    in publishingPages
                     select new PublishingPageInfo
                     {
                         Name = page.Name,
                         ServerRelativeUrl = page.Uri.AbsolutePath,
                         Title = page.Title,
                     }).ToList();

            return pages;
        }
    }
}
