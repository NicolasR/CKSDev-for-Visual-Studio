using CKS.Dev2015.VisualStudio.SharePoint.Commands;
using CKS.Dev2015.VisualStudio.SharePoint.Commands.Info;
using CKS.Dev2015.VisualStudio.SharePoint.Environment.Options;
using Microsoft.VisualStudio.SharePoint.Explorer;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using CKSProperties = CKS.Dev2015.Core.Properties.Resources;

namespace CKS.Dev2015.VisualStudio.SharePoint.Explorer
{
    /// <summary>
    /// Represents an extension of SharePoint site nodes in Server Explorer for content types. 
    /// </summary>
    // Enables Visual Studio to discover and load this extension.
    [Export(typeof(IExplorerNodeTypeExtension))]
    // Indicates that this class extends SharePoint site nodes in Server Explorer.
    [ExplorerNodeType(ExplorerNodeTypes.SiteNode)]
    internal class MasterPageGallerySiteNodeExtension : IExplorerNodeTypeExtension
    {
        #region Methods

        /// <summary>
        /// Initializes the node extension.
        /// </summary>
        /// <param name="nodeType">The node type that is being extended.</param>
        public void Initialize(IExplorerNodeType nodeType)
        {
            nodeType.NodeChildrenRequested += NodeChildrenRequested;
        }

        /// <summary>
        /// Nodes the children requested.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ExplorerNodeEventArgs" /> instance containing the event data.</param>
        private void NodeChildrenRequested(object sender, ExplorerNodeEventArgs e)
        {
            IExplorerNode siteNode = e.Node;
            IExplorerSiteNodeInfo siteInfo = siteNode.Annotations.GetValue<IExplorerSiteNodeInfo>();
            if (siteInfo != null && siteInfo.IsConnectionRoot)
            {
                Uri siteUrl = null;
                siteUrl = siteInfo.Url;
            }

            if (EnabledExtensionsOptionsPage.GetSetting<bool>(EnabledExtensionsOptions.ViewMasterPageAndPageLayoutGallery, true))
            {
                e.Node.ChildNodes.AddFolder(CKSProperties.MasterPageGallerySiteNodeExtension_NodeName, CKSProperties.MasterPagesNode.ToBitmap(), CreateMasterPagesAndPageLayoutsNodes);
            }
        }

        /// <summary>
        /// Creates the master pages and page layouts nodes.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        private void CreateMasterPagesAndPageLayoutsNodes(IExplorerNode parentNode)
        {
            FileNodeInfo[] masterPagesAndPageLayouts = parentNode.Context.SharePointConnection.ExecuteCommand<FileNodeInfo[]>(MasterPageGallerySharePointCommandIds.GetMasterPagesAndPageLayoutsCommand);

            if (masterPagesAndPageLayouts != null)
            {
                foreach (FileNodeInfo masterPageOrPageLayout in masterPagesAndPageLayouts)
                {
                    var annotations = new Dictionary<object, object>
                    {
                        { typeof(FileNodeInfo), masterPageOrPageLayout }
                    };

                    string nodeTypeId = ExplorerNodeIds.PageLayoutNode;

                    if (masterPageOrPageLayout.FileType.Equals("master", StringComparison.InvariantCultureIgnoreCase))
                    {
                        nodeTypeId = ExplorerNodeIds.MasterPageNode;
                    }

                    IExplorerNode masterPageOrPageLayoutNode = parentNode.ChildNodes.Add(nodeTypeId, masterPageOrPageLayout.Name, annotations);

                    if (masterPageOrPageLayout.IsCheckedOut)
                    {
                        if (masterPageOrPageLayout.FileType.Equals("master", StringComparison.InvariantCultureIgnoreCase))
                        {
                            masterPageOrPageLayoutNode.Icon = CKSProperties.MasterPageNodeCheckedOut.ToBitmap();
                        }
                        else
                        {
                            masterPageOrPageLayoutNode.Icon = CKSProperties.PageNodeCheckedOut.ToBitmap();
                        }
                    }
                }
            }
        }

        #endregion
    }
}
