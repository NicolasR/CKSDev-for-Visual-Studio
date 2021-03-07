using CKS.Dev.VisualStudio.SharePoint.Commands;
using CKS.Dev.VisualStudio.SharePoint.Commands.Info;
using Microsoft.VisualStudio.SharePoint.Explorer;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using CKSProperties = CKS.Dev.Core.Properties.Resources;

namespace CKS.Dev.VisualStudio.SharePoint.Explorer
{
    /// <summary>
    /// Represents an extension of SharePoint master page nodes in Server Explorer 
    /// </summary>
    // Enables Visual Studio to discover and load this extension.
    [Export(typeof(IExplorerNodeTypeProvider))]
    // Indicates that this class extends SharePoint master page nodes in Server Explorer.
    [ExplorerNodeType(ExplorerNodeIds.MasterPageNode)]
    internal class MasterPageNodeTypeProvider : FileNodeTypeProvider
    {
        #region Methods

        /// <summary>
        /// Initializes the new node type.
        /// </summary>
        /// <param name="typeDefinition">The definition of the new node type.</param>
        public override void InitializeType(IExplorerNodeTypeDefinition typeDefinition)
        {
            base.InitializeType(typeDefinition);

            typeDefinition.DefaultIcon = CKSProperties.MasterPageNode.ToBitmap();
            typeDefinition.IsAlwaysLeaf = true;
        }

        /// <summary>
        /// Nodes the properties requested.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ExplorerNodePropertiesRequestedEventArgs" /> instance containing the event data.</param>
        protected override void NodePropertiesRequested(object sender, ExplorerNodePropertiesRequestedEventArgs e)
        {
            IExplorerNode masterPageNode = e.Node;
            FileNodeInfo masterPage = masterPageNode.Annotations.GetValue<FileNodeInfo>();
            Dictionary<string, string> masterPageProperties = masterPageNode.Context.SharePointConnection.ExecuteCommand<FileNodeInfo, Dictionary<string, string>>(MasterPageGallerySharePointCommandIds.GetMasterPagesOrPageLayoutPropertiesCommand, masterPage);
            object propertySource = masterPageNode.Context.CreatePropertySourceObject(masterPageProperties);
            e.PropertySources.Add(propertySource);
        }

        #endregion
    }
}