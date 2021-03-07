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
    /// Represents an extension of SharePoint site nodes in Server Explorer for design catalog. 
    /// </summary>
    // Enables Visual Studio to discover and load this extension.
    [Export(typeof(IExplorerNodeTypeExtension))]
    // Indicates that this class extends SharePoint site nodes in Server Explorer.
    [ExplorerNodeType(ExplorerNodeTypes.GenericFolderNode)]
    internal class DesignCatalogFolderNodeExtension : IExplorerNodeTypeExtension
    {
        #region Methods

        /// <summary>
        /// Initialise the node.
        /// </summary>
        /// <param name="nodeType">The node type.</param>
        public void Initialize(IExplorerNodeType nodeType)
        {
            nodeType.NodeMenuItemsRequested += DesignCatalog_NodeMenuItemsRequested;
            nodeType.NodePropertiesRequested += DesignCatalog_NodePropertiesRequested;
        }

        /// <summary>
        /// Handles the NodePropertiesRequested event of the Design Catalog control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ExplorerNodePropertiesRequestedEventArgs" /> instance containing the event data.</param>
        void DesignCatalog_NodePropertiesRequested(object sender, ExplorerNodePropertiesRequestedEventArgs e)
        {
            //TODO: this needs a sp version detection and working out what to call down into
            //TODO: this also needs the command fixing to the library
            //Check this is the style library node
            if (e.Node.Text == CKSProperties.SiteNodeExtension_DesignCatalogNodeName)
            {
                var designCatalogNodeInfo = e.Node.Annotations.GetValue<DesignCatalogNodeInfo>();

                // Call the custom SharePoint command to get the design catalog properties.
                Dictionary<string, string> properties =
                    e.Node.Context.SharePointConnection.ExecuteCommand<
                    DesignCatalogNodeInfo, Dictionary<string, string>>(
                    DesignCatalogSharePointCommandIds.GetDesignCatalogProperties, designCatalogNodeInfo);

                object propertySource = e.Node.Context.CreatePropertySourceObject(properties);
                e.PropertySources.Add(propertySource);
            }
        }

        /// <summary>
        /// Register the menu items for the design catalog.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The ExplorerNodeMenuItemsRequestedEventArgs object.</param>
        void DesignCatalog_NodeMenuItemsRequested(object sender, ExplorerNodeMenuItemsRequestedEventArgs e)
        {
            //Check this is the style library node
            if (e.Node.Text == CKSProperties.SiteNodeExtension_DesignCatalogNodeName)
            {
                //Register the view in browser menu item
                e.MenuItems.Add(CKSProperties.DesignCatalogFolderNodeExtension_ViewInBrowser).Click += DesignCatalogGenericFolderNodeExtension_Click;
            }
        }

        /// <summary>
        /// Open the style library in a browser.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The MenuItemEventArgs object.</param>
        void DesignCatalogGenericFolderNodeExtension_Click(object sender, MenuItemEventArgs e)
        {
            IExplorerNode owner = (IExplorerNode)e.Owner;

            string allItemsUrl =
            owner.Context.SharePointConnection.ExecuteCommand<string>(DesignCatalogSharePointCommandIds.GetDesignCatalogAllItemsUrl);

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