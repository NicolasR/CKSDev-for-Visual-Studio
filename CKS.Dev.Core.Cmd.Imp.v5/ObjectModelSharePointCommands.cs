using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.VisualStudio.SharePoint.Commands;
using System;
using System.Collections.Generic;

namespace CKS.Dev.VisualStudio.SharePoint.Commands
{
    class ObjectModelSharePointCommands
    {
        /// <summary>
        /// Get
        /// </summary>
        /// <param name="context">The context object</param>
        /// <returns>True if the content type is a built in one</returns>
        [SharePointCommand(ObjectModelSharePointCommandIds.GetSPBasePermissions)]
        private static Dictionary<string, string> GetSPBasePermissions(ISharePointCommandContext context)
        {
            Dictionary<string, string> options = new Dictionary<string, string>();

            options.Add(SPBasePermissions.AddAndCustomizePages.ToString(),
                "Add, change, or delete HTML pages or Web Part Pages, and edit the Web site using a Windows SharePoint Services–compatible editor.");

            options.Add(SPBasePermissions.AddDelPrivateWebParts.ToString(),
                "Add or remove personal Web Parts on a Web Part Page.");

            options.Add(SPBasePermissions.AddListItems.ToString(),
                "Add items to lists, add documents to document libraries, and add Web discussion comments.");

            options.Add(SPBasePermissions.ApplyStyleSheets.ToString(),
                "Apply a style sheet (.css file) to the Web site.");

            options.Add(SPBasePermissions.ApplyThemeAndBorder.ToString(),
                "Apply a theme or borders to the entire Web site.");

            options.Add(SPBasePermissions.ApproveItems.ToString(),
                "Approve a minor version of a list item or document.");

            options.Add(SPBasePermissions.BrowseDirectories.ToString(),
                "Enumerate files and folders in a Web site using Microsoft Office SharePoint Designer 2007 and WebDAV interfaces.");

            options.Add(SPBasePermissions.BrowseUserInfo.ToString(),
                "View information about users of the Web site.");

            options.Add(SPBasePermissions.CancelCheckout.ToString(),
                "Discard or check in a document which is checked out to another user.");

            options.Add(SPBasePermissions.CreateAlerts.ToString(),
                " Create e-mail alerts.");

            options.Add(SPBasePermissions.CreateGroups.ToString(),
                "Create a group of users that can be used anywhere within the site collection.");

            options.Add(SPBasePermissions.CreateSSCSite.ToString(),
                "Create a Web site using Self-Service Site Creation.");

            options.Add(SPBasePermissions.DeleteListItems.ToString(),
                "Delete items from a list, documents from a document library, and Web discussion comments in documents.");

            options.Add(SPBasePermissions.DeleteVersions.ToString(),
                "Delete past versions of a list item or document.");

            options.Add(SPBasePermissions.EditListItems.ToString(),
                "Edit items in lists, edit documents in document libraries, edit Web discussion comments in documents, and customize Web Part Pages in document libraries.");

            options.Add(SPBasePermissions.EditMyUserInfo.ToString(),
                "Allows a user to change his or her user information, such as adding a picture.");

            options.Add(SPBasePermissions.EmptyMask.ToString(),
                "Has no permissions on the Web site. Not available through the user interface.");

            options.Add(SPBasePermissions.EnumeratePermissions.ToString(),
                "Enumerate permissions on the Web site, list, folder, document, or list item.");

            options.Add(SPBasePermissions.FullMask.ToString(),
                " Has all permissions on the Web site. Not available through the user interface.");

            options.Add(SPBasePermissions.ManageAlerts.ToString(),
                "Manage alerts for all users of the Web site.");

            options.Add(SPBasePermissions.ManageLists.ToString(),
                "Create and delete lists, add or remove columns in a list, and add or remove public views of a list.");

            options.Add(SPBasePermissions.ManagePermissions.ToString(),
                "Create and change permission levels on the Web site and assign permissions to users and groups.");

            options.Add(SPBasePermissions.ManagePersonalViews.ToString(),
                "Create, change, and delete personal views of lists.");

            options.Add(SPBasePermissions.ManageSubwebs.ToString(),
                "Create subsites such as team sites, Meeting Workspace sites, and Document Workspace sites.");

            options.Add(SPBasePermissions.ManageWeb.ToString(),
                "Grant the ability to perform all administration tasks for the Web site as well as manage content. Activate, deactivate, or edit properties of Web site scoped Features through the object model or through the user interface (UI). When granted on the root Web site of a site collection, activate, deactivate, or edit properties of site collection scoped Features through the object  model. To browse to the Site Collection Features page and activate or deactivate site collection scoped Features through the UI, you must be a site collection administrator.");

            options.Add(SPBasePermissions.Open.ToString(),
                "Allow users to open a Web site, list, or folder to access items inside that container.");

            options.Add(SPBasePermissions.OpenItems.ToString(),
                "View the source of documents with server-side file handlers.");

            options.Add(SPBasePermissions.UpdatePersonalWebParts.ToString(),
                "Update Web Parts to display personalized information.");

            options.Add(SPBasePermissions.UseClientIntegration.ToString(),
                "Use features that launch client applications; otherwise, users must work on documents locally and upload changes.");

            options.Add(SPBasePermissions.UseRemoteAPIs.ToString(),
                "Use SOAP, WebDAV, or Microsoft Office SharePoint Designer 2007 interfaces to access the Web site.");

            options.Add(SPBasePermissions.ViewFormPages.ToString(),
                "View forms, views, and application pages, and enumerate lists.");

            options.Add(SPBasePermissions.ViewListItems.ToString(),
                "View items in lists, documents in document libraries, and view Web discussion comments.");

            options.Add(SPBasePermissions.ViewPages.ToString(),
                "View pages in a Web site.");

            options.Add(SPBasePermissions.ViewUsageData.ToString(),
                "View reports on Web site usage.");

            options.Add(SPBasePermissions.ViewVersions.ToString(),
                "View past versions of a list item or document.");

            return options;
        }

        /// <summary>
        /// Gets the full SP root folder path.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="folderName">Name of the folder.</param>
        /// <returns>The full file path or the specified folder within the SP Root.</returns>
        [SharePointCommand(ObjectModelSharePointCommandIds.GetFullSPRootFolderPath)]
        private static string GetFullSPRootFolderPath(ISharePointCommandContext context,
            string folderName)
        {
            string sharePointSetupPath = String.Empty;

            sharePointSetupPath = SPUtility.GetVersionedGenericSetupPath(folderName, 15);

            return sharePointSetupPath;
        }
    }
}
