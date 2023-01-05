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
    /// Represents an extension of SharePoint site nodes in Server Explorer for style library. 
    /// </summary>
    // Enables Visual Studio to discover and load this extension.
    [Export(typeof(IExplorerNodeTypeExtension))]
    // Indicates that this class extends SharePoint site nodes in Server Explorer.
    [ExplorerNodeType(ExplorerNodeTypes.GenericFolderNode)]
    internal class StyleLibraryFolderNodeExtension : IExplorerNodeTypeExtension
    {
        #region Methods

        /// <summary>
        /// Initialise the node.
        /// </summary>
        /// <param name="nodeType">The node type.</param>
        public void Initialize(IExplorerNodeType nodeType)
        {
            nodeType.NodeMenuItemsRequested += StyleLibrary_NodeMenuItemsRequested;
            nodeType.NodePropertiesRequested += StyleLibrary_NodePropertiesRequested;
        }

        /// <summary>
        /// Handles the NodePropertiesRequested event of the StyleLibrary control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ExplorerNodePropertiesRequestedEventArgs" /> instance containing the event data.</param>
        void StyleLibrary_NodePropertiesRequested(object sender, ExplorerNodePropertiesRequestedEventArgs e)
        {
            //TODO: this needs a sp version detection and working out what to call down into
            //TODO: this also needs the command fixing to the library
            //Check this is the style library node
            if (e.Node.Text == CKSProperties.SiteNodeExtension_StyleLibraryNodeName)
            {
                var styleLibraryNodeInfo = e.Node.Annotations.GetValue<StyleLibraryNodeInfo>();

                // Call the custom SharePoint command to get the style library properties.
                Dictionary<string, string> properties =
                    e.Node.Context.SharePointConnection.ExecuteCommand<
                    StyleLibraryNodeInfo, Dictionary<string, string>>(
                    StyleLibrarySharePointCommandIds.GetStyleLibraryProperties, styleLibraryNodeInfo);

                object propertySource = e.Node.Context.CreatePropertySourceObject(properties);
                e.PropertySources.Add(propertySource);
            }
        }

        /// <summary>
        /// Register the menu items for the style library.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The ExplorerNodeMenuItemsRequestedEventArgs object.</param>
        void StyleLibrary_NodeMenuItemsRequested(object sender, ExplorerNodeMenuItemsRequestedEventArgs e)
        {
            //Check this is the style library node
            if (e.Node.Text == CKSProperties.SiteNodeExtension_StyleLibraryNodeName)
            {
                //Register the view in browser menu item
                e.MenuItems.Add(CKSProperties.StyleLibraryFolderNodeExtension_ViewInBrowser).Click += StyleLibraryGenericFolderNodeExtension_Click;
            }
        }

        /// <summary>
        /// Open the style library in a browser.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The MenuItemEventArgs object.</param>
        void StyleLibraryGenericFolderNodeExtension_Click(object sender, MenuItemEventArgs e)
        {
            IExplorerNode owner = (IExplorerNode)e.Owner;

            string allItemsUrl =
            owner.Context.SharePointConnection.ExecuteCommand<string>(StyleLibrarySharePointCommandIds.GetStyleLibraryAllItemsUrl);

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