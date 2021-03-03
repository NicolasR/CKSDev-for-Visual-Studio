using CKS.Dev2015.VisualStudio.SharePoint.Commands;
using CKS.Dev2015.VisualStudio.SharePoint.Commands.Info;
using CKS.Dev2015.VisualStudio.SharePoint.Environment;
using Microsoft.VisualStudio.SharePoint;
using Microsoft.VisualStudio.SharePoint.Explorer;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using CKSProperties = CKS.Dev2015.Core.Properties.Resources;

namespace CKS.Dev2015.VisualStudio.SharePoint.Explorer
{
    /// <summary>
    /// Represents an extension of SharePoint nodes in Server Explorer for solutions. 
    /// </summary>
    // Enables Visual Studio to discover and load this extension.
    [Export(typeof(IExplorerNodeTypeProvider))]
    // Indicates that this class extends SharePoint nodes in Server Explorer.
    [ExplorerNodeType(ExplorerNodeIds.SolutionNode)]
    internal class SolutionNodeTypeProvider : IExplorerNodeTypeProvider
    {
        #region Methods

        /// <summary>
        /// Initialise the node.
        /// </summary>
        /// <param name="typeDefinition">The node type.</param>
        public void InitializeType(IExplorerNodeTypeDefinition typeDefinition)
        {
            typeDefinition.DefaultIcon = CKSProperties.SolutionNode.ToBitmap();
            typeDefinition.IsAlwaysLeaf = true;

            typeDefinition.NodePropertiesRequested += NodePropertiesRequested;
            typeDefinition.NodeMenuItemsRequested += NodeMenuItemsRequested;
        }

        /// <summary>
        /// Register the menu items for the solution.
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">The ExplorerNodeMenuItemsRequestedEventArgs object</param>
        private void NodeMenuItemsRequested(object sender, ExplorerNodeMenuItemsRequestedEventArgs e)
        {
            e.MenuItems.Add(CKSProperties.SolutionNodeTypeProvider_Export, 4).Click += SolutionNodeTypeProvider_ExportClick;
        }

        /// <summary>
        /// Retrieves properties that are displayed in the Properties window when
        /// a solution node is selected.
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">The ExplorerNodePropertiesRequestedEventArgs object</param>
        private void NodePropertiesRequested(object sender, ExplorerNodePropertiesRequestedEventArgs e)
        {
            IExplorerNode solutionNode = e.Node;
            FileNodeInfo solution = solutionNode.Annotations.GetValue<FileNodeInfo>();
            Dictionary<string, string> solutionProperties = solutionNode.Context.SharePointConnection.ExecuteCommand<FileNodeInfo, Dictionary<string, string>>(SolutionSharePointCommandIds.GetSolutionProperties, solution);
            object propertySource = solutionNode.Context.CreatePropertySourceObject(solutionProperties);
            e.PropertySources.Add(propertySource);
        }

        /// <summary>
        /// Export the wsp file.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The MenuItemEventArgs object.</param>
        void SolutionNodeTypeProvider_ExportClick(object sender, MenuItemEventArgs e)
        {
            IMenuItem menuItem = sender as IMenuItem;

            IExplorerNode owner = (IExplorerNode)e.Owner;

            FileNodeInfo info = owner.Annotations.GetValue<FileNodeInfo>();

            if (info != null)
            {
                ProcessUtilities utils = new ProcessUtilities();
                utils.ExecuteBrowserUrlProcess(new Uri(owner.Context.SiteUrl + info.ServerRelativeUrl.TrimStart(@"/".ToCharArray())).AbsoluteUri);
            }
        }

        #endregion
    }
}

