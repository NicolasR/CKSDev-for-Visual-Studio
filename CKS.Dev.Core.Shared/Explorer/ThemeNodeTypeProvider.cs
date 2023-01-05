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
    /// Represents an extension of SharePoint nodes in Server Explorer for themes. 
    /// </summary>
    // Enables Visual Studio to discover and load this extension.
    [Export(typeof(IExplorerNodeTypeProvider))]
    // Indicates that this class extends SharePoint nodes in Server Explorer.
    [ExplorerNodeType(ExplorerNodeIds.ThemeNode)]
    internal class ThemeNodeTypeProvider : FileNodeTypeProvider
    {
        #region Methods

        /// <summary>
        /// Initialise the node.
        /// </summary>
        /// <param name="typeDefinition">The node type.</param>
        public override void InitializeType(IExplorerNodeTypeDefinition typeDefinition)
        {
            base.InitializeType(typeDefinition);

            typeDefinition.DefaultIcon = CKSProperties.ThemeNode.ToBitmap();
            typeDefinition.IsAlwaysLeaf = true;


        }

        /// <summary>
        /// Register the menu items for the theme.
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">The ExplorerNodeMenuItemsRequestedEventArgs object</param>
        protected override void NodeMenuItemsRequested(object sender, ExplorerNodeMenuItemsRequestedEventArgs e)
        {
            base.NodeMenuItemsRequested(sender, e);

            e.MenuItems.Add(CKSProperties.ThemeNodeTypeProvider_Export, 4).Click += ThemeNodeTypeProvider_ExportClick;
        }

        /// <summary>
        /// Export the thmx file.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The MenuItemEventArgs object.</param>
        void ThemeNodeTypeProvider_ExportClick(object sender, MenuItemEventArgs e)
        {
            IMenuItem menuItem = sender as IMenuItem;

            IExplorerNode owner = (IExplorerNode)e.Owner;

            FileNodeInfo info = owner.Annotations.GetValue<FileNodeInfo>();

            if (info != null)
            {
                ProcessUtilities utils = new ProcessUtilities();
                utils.ExecuteBrowserUrlProcess(new Uri(owner.Context.SiteUrl + info.ServerRelativeUrl.TrimStart(@"/".ToCharArray())));
            }
        }

        /// <summary>
        /// Retrieves properties that are displayed in the Properties window when
        /// a theme node is selected.
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">The ExplorerNodePropertiesRequestedEventArgs object</param>
        protected override void NodePropertiesRequested(object sender, ExplorerNodePropertiesRequestedEventArgs e)
        {
            IExplorerNode themeNode = e.Node;
            FileNodeInfo theme = themeNode.Annotations.GetValue<FileNodeInfo>();
            Dictionary<string, string> masterPageProperties = themeNode.Context.SharePointConnection.ExecuteCommand<FileNodeInfo, Dictionary<string, string>>(ThemeSharePointCommandIds.GetThemeProperties, theme);
            object propertySource = themeNode.Context.CreatePropertySourceObject(masterPageProperties);
            e.PropertySources.Add(propertySource);
        }

        #endregion
    }
}
