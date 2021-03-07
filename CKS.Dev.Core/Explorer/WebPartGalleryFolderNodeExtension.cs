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
    /// Represents an extension of SharePoint site nodes in Server Explorer for web part gallery. 
    /// </summary>
    // Enables Visual Studio to discover and load this extension.
    [Export(typeof(IExplorerNodeTypeExtension))]
    // Indicates that this class extends SharePoint site nodes in Server Explorer.
    [ExplorerNodeType(ExplorerNodeTypes.GenericFolderNode)]
    internal class WebPartGalleryFolderNodeExtension : IExplorerNodeTypeExtension
    {
        #region Methods

        /// <summary>
        /// Initialise the node.
        /// </summary>
        /// <param name="nodeType">The node type.</param>
        public void Initialize(IExplorerNodeType nodeType)
        {
            nodeType.NodeMenuItemsRequested += WebPartGallery_NodeMenuItemsRequested;
            nodeType.NodePropertiesRequested += WebPartGallery_NodePropertiesRequested;
        }

        /// <summary>
        /// Get the properties for the web part gallery
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">The ExplorerNodePropertiesRequestedEventArgs object</param>
        void WebPartGallery_NodePropertiesRequested(object sender, ExplorerNodePropertiesRequestedEventArgs e)
        {
            //Check this is the web part gallery node
            if (e.Node.Text == CKSProperties.SiteNodeExtension_WebPartGalleryNodeName)
            {
                var webPartGalleryNodeInfo = e.Node.Annotations.GetValue<WebPartGalleryNodeInfo>();

                // Call the custom SharePoint command to get the Web Part Gallery Properties.
                Dictionary<string, string> properties =
                    e.Node.Context.SharePointConnection.ExecuteCommand<
                    WebPartGalleryNodeInfo, Dictionary<string, string>>(
                    WebPartGallerySharePointCommandIds.GetWebPartGalleryProperties, webPartGalleryNodeInfo);

                object propertySource = e.Node.Context.CreatePropertySourceObject(properties);
                e.PropertySources.Add(propertySource);
            }
        }

        /// <summary>
        /// Register the menu items for the web part gallery
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">The ExplorerNodeMenuItemsRequestedEventArgs object</param>
        void WebPartGallery_NodeMenuItemsRequested(object sender, ExplorerNodeMenuItemsRequestedEventArgs e)
        {
            //Check this is the web part gallery node
            if (e.Node.Text == CKSProperties.SiteNodeExtension_WebPartGalleryNodeName)
            {
                //Register the view in browser menu item
                e.MenuItems.Add(CKSProperties.WebPartGalleryFolderNodeExtension_ViewInBrowser).Click += WebPartGalleryGenericFolderNodeExtension_Click;
            }
        }

        /// <summary>
        /// Open the web part gallery in a browser
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">The MenuItemEventArgs object</param>
        void WebPartGalleryGenericFolderNodeExtension_Click(object sender, MenuItemEventArgs e)
        {
            IExplorerNode owner = (IExplorerNode)e.Owner;

            string allItemsUrl =
            owner.Context.SharePointConnection.ExecuteCommand<string>(WebPartGallerySharePointCommandIds.GetWebPartGalleryAllItemsUrl);

            if (allItemsUrl.StartsWith(@"/"))
            {
                allItemsUrl = allItemsUrl.TrimStart(@"/".ToCharArray());
            }

            ProcessUtilities utils = new ProcessUtilities();
            utils.ExecuteBrowserUrlProcess(new Uri(owner.Context.SiteUrl + allItemsUrl));
        }

        #endregion
    }
}
