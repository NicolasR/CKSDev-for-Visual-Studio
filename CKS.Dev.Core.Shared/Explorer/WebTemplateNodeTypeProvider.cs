using CKS.Dev.VisualStudio.SharePoint.Commands;
using CKS.Dev.VisualStudio.SharePoint.Commands.Info;
using Microsoft.VisualStudio.SharePoint.Explorer;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using CKSProperties = CKS.Dev.Core.Properties.Resources;

namespace CKS.Dev.VisualStudio.SharePoint.Explorer
{
    /// <summary>
    /// Represents an extension of SharePoint nodes in Server Explorer for a web template. 
    /// </summary>
    // Enables Visual Studio to discover and load this extension.
    [Export(typeof(IExplorerNodeTypeProvider))]
    // Indicates that this class extends SharePoint nodes in Server Explorer.
    [ExplorerNodeType(ExplorerNodeIds.WebTemplateNode)]
    internal class WebTemplateNodeTypeProvider : IExplorerNodeTypeProvider
    {
        #region Methods

        /// <summary>
        /// Initialise the node.
        /// </summary>
        /// <param name="typeDefinition">The node type.</param>
        public void InitializeType(IExplorerNodeTypeDefinition typeDefinition)
        {
            typeDefinition.DefaultIcon = CKSProperties.WebTemplateNode.ToBitmap();
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
            //TODO: decide if there is something to do here
            //e.MenuItems.Add(Resources.SolutionNodeTypeProvider_Export, 4).Click += SolutionNodeTypeProvider_ExportClick;
        }

        /// <summary>
        /// Retrieves properties that are displayed in the Properties window when
        /// a solution node is selected.
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">The ExplorerNodePropertiesRequestedEventArgs object</param>
        private void NodePropertiesRequested(object sender, ExplorerNodePropertiesRequestedEventArgs e)
        {
            IExplorerNode webTemplateNode = e.Node;
            WebTemplateInfo webTemplate = webTemplateNode.Annotations.GetValue<WebTemplateInfo>();
            Dictionary<string, string> webTemplateProperties = webTemplateNode.Context.SharePointConnection.ExecuteCommand<WebTemplateInfo, Dictionary<string, string>>(WebTemplateSharePointCommandIds.GetWebTemplateProperties, webTemplate);
            object propertySource = webTemplateNode.Context.CreatePropertySourceObject(webTemplateProperties);
            e.PropertySources.Add(propertySource);
        }

        #endregion
    }
}
