using CKS.Dev2015.VisualStudio.SharePoint.Commands;
using Microsoft.VisualStudio.SharePoint.Explorer;
using System;
using System.ComponentModel.Composition;
using CKSProperties = CKS.Dev2015.Core.Properties.Resources;

namespace CKS.Dev2015.VisualStudio.SharePoint.Explorer
{
    /// <summary>
    /// Extension for the Publishing Pages on the Site node.
    /// </summary>
    [Export(typeof(IExplorerNodeTypeExtension))]
    [ExplorerNodeType(ExplorerNodeTypes.SiteNode)]
    public class PublishingPagesSiteExtension : IExplorerNodeTypeExtension
    {
        #region Methods

        /// <summary>
        /// Initializes the specified node type.
        /// </summary>
        /// <param name="nodeType">Type of the node.</param>
        public void Initialize(IExplorerNodeType nodeType)
        {
            nodeType.NodeChildrenRequested += new EventHandler<ExplorerNodeEventArgs>(nodeType_NodeChildrenRequested);
        }

        /// <summary>
        /// Handles the NodeChildrenRequested event of the nodeType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ExplorerNodeEventArgs" /> instance containing the event data.</param>
        void nodeType_NodeChildrenRequested(object sender, ExplorerNodeEventArgs e)
        {
            IExplorerNode siteNode = e.Node;
            if (siteNode.Context.SharePointConnection.ExecuteCommand<bool>(SiteCommandIds.IsPublishingSiteCommandId))
            {
                IExplorerNode pages = siteNode.ChildNodes.AddFolder("Pages", CKSProperties.PagesNode.ToBitmap(), new Action<IExplorerNode>(PublishingPageNodeTypeProvider.CreatePublishingPageNodes));
            }
        }

        #endregion
    }
}
