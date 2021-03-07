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
    /// Represents an extension of SharePoint site nodes in Server Explorer for solution gallery. 
    /// </summary>
    // Enables Visual Studio to discover and load this extension.
    [Export(typeof(IExplorerNodeTypeExtension))]
    // Indicates that this class extends SharePoint site nodes in Server Explorer.
    [ExplorerNodeType(ExplorerNodeTypes.GenericFolderNode)]
    internal class SolutionGalleryFolderNodeExtension : IExplorerNodeTypeExtension
    {
        #region Methods

        /// <summary>
        /// Initialise the node.
        /// </summary>
        /// <param name="nodeType">The node type.</param>
        public void Initialize(IExplorerNodeType nodeType)
        {
            nodeType.NodeMenuItemsRequested += SolutionGallery_NodeMenuItemsRequested;
            nodeType.NodePropertiesRequested += SolutionGallery_NodePropertiesRequested;
        }

        /// <summary>
        /// Get the properties for the solution gallery
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">The ExplorerNodePropertiesRequestedEventArgs object</param>
        void SolutionGallery_NodePropertiesRequested(object sender, ExplorerNodePropertiesRequestedEventArgs e)
        {
            //Check this is the solution gallery node
            if (e.Node.Text == CKSProperties.SiteNodeExtension_SolutionGalleryNodeName)
            {
                var solutionGalleryNodeInfo = e.Node.Annotations.GetValue<SolutionGalleryNodeInfo>();

                // Call the custom SharePoint command to get the solution gallery properties.
                Dictionary<string, string> properties =
                    e.Node.Context.SharePointConnection.ExecuteCommand<
                    SolutionGalleryNodeInfo, Dictionary<string, string>>(
                    SolutionGallerySharePointCommandIds.GetSolutionGalleryProperties, solutionGalleryNodeInfo);

                object propertySource = e.Node.Context.CreatePropertySourceObject(properties);
                e.PropertySources.Add(propertySource);
            }
        }

        /// <summary>
        /// Register the menu items for the solution gallery.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The ExplorerNodeMenuItemsRequestedEventArgs object.</param>
        void SolutionGallery_NodeMenuItemsRequested(object sender, ExplorerNodeMenuItemsRequestedEventArgs e)
        {
            //Check this is the solution gallery node
            if (e.Node.Text == CKSProperties.SiteNodeExtension_SolutionGalleryNodeName)
            {
                //Register the view in browser menu item
                e.MenuItems.Add(CKSProperties.SolutionGalleryFolderNodeExtension_ViewInBrowser).Click += SolutionGalleryGenericFolderNodeExtension_Click;
            }
        }

        /// <summary>
        /// Open the solution gallery in a browser.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The MenuItemEventArgs object.</param>
        void SolutionGalleryGenericFolderNodeExtension_Click(object sender, MenuItemEventArgs e)
        {
            IExplorerNode owner = (IExplorerNode)e.Owner;

            string allItemsUrl =
            owner.Context.SharePointConnection.ExecuteCommand<string>(SolutionGallerySharePointCommandIds.GetSolutionGalleryAllItemsUrl);

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