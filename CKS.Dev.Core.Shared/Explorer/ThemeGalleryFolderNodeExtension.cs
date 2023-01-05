using CKS.Dev.VisualStudio.SharePoint.Commands;
using CKS.Dev.VisualStudio.SharePoint.Commands.Info;
using CKS.Dev.VisualStudio.SharePoint.Environment;
using Microsoft.VisualStudio.SharePoint;
using Microsoft.VisualStudio.SharePoint.Explorer;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using CKSProperties = CKS.Dev.Core.Properties.Resources;

namespace CKS.Dev.VisualStudio.SharePoint.Explorer
{
    /// <summary>
    /// Represents an extension of SharePoint site nodes in Server Explorer for theme gallery. 
    /// </summary>
    // Enables Visual Studio to discover and load this extension.
    [Export(typeof(IExplorerNodeTypeExtension))]
    // Indicates that this class extends SharePoint site nodes in Server Explorer.
    [ExplorerNodeType(ExplorerNodeTypes.GenericFolderNode)]
    internal class ThemeGalleryFolderNodeExtension : IExplorerNodeTypeExtension
    {
        #region Methods

        /// <summary>
        /// Initialise the node.
        /// </summary>
        /// <param name="nodeType">The node type.</param>
        public void Initialize(IExplorerNodeType nodeType)
        {
            nodeType.NodeMenuItemsRequested += ThemeGallery_NodeMenuItemsRequested;
            nodeType.NodePropertiesRequested += ThemeGallery_NodePropertiesRequested;
        }

        /// <summary>
        /// Get the properties for the theme gallery
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">The ExplorerNodePropertiesRequestedEventArgs object</param>
        void ThemeGallery_NodePropertiesRequested(object sender, ExplorerNodePropertiesRequestedEventArgs e)
        {
            //Check this is the theme gallery node
            if (e.Node.Text == CKSProperties.SiteNodeExtension_ThemeGalleryNodeName)
            {
                var themeGalleryNodeInfo = e.Node.Annotations.GetValue<ThemeGalleryNodeInfo>();

                // Call the custom SharePoint command to get the Web Part properties.
                Dictionary<string, string> properties =
                    e.Node.Context.SharePointConnection.ExecuteCommand<
                    ThemeGalleryNodeInfo, Dictionary<string, string>>(
                    ThemeGallerySharePointCommandIds.GetThemeGalleryProperties, themeGalleryNodeInfo);

                object propertySource = e.Node.Context.CreatePropertySourceObject(properties);
                e.PropertySources.Add(propertySource);
            }
        }

        /// <summary>
        /// Register the menu items for the theme gallery.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The ExplorerNodeMenuItemsRequestedEventArgs object.</param>
        void ThemeGallery_NodeMenuItemsRequested(object sender, ExplorerNodeMenuItemsRequestedEventArgs e)
        {
            //Check this is the theme gallery node
            if (e.Node.Text == CKSProperties.SiteNodeExtension_ThemeGalleryNodeName)
            {
                //Register the view in browser menu item
                e.MenuItems.Add(CKSProperties.ThemeGalleryFolderNodeExtension_ViewInBrowser).Click += ThemeGalleryGenericFolderNodeExtension_Click;
            }
        }

        /// <summary>
        /// Open the theme gallery in a browser.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The MenuItemEventArgs object.</param>
        void ThemeGalleryGenericFolderNodeExtension_Click(object sender, MenuItemEventArgs e)
        {
            IExplorerNode owner = (IExplorerNode)e.Owner;

            string allItemsUrl =
            owner.Context.SharePointConnection.ExecuteCommand<string>(ThemeGallerySharePointCommandIds.GetThemeGalleryAllItemsUrl);

            if (allItemsUrl.StartsWith(@"/"))
            {
                allItemsUrl = allItemsUrl.TrimStart(@"/".ToCharArray());
            }

            ProcessUtilities utils = new ProcessUtilities();
            utils.ExecuteBrowserUrlProcess(new Uri(owner.Context.SiteUrl + allItemsUrl).AbsoluteUri);
        }

        #endregion
    }
}
