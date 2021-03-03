using CKS.Dev2015.VisualStudio.SharePoint.Commands;
using CKS.Dev2015.VisualStudio.SharePoint.Environment.Options;
using Microsoft.VisualStudio.SharePoint.Explorer;
using System;
using System.ComponentModel.Composition;
using System.Linq;
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
    internal class ContentTypeSiteNodeExtension : IExplorerNodeTypeExtension
    {
        #region Methods

        /// <summary>
        /// Initializes the node extension.
        /// </summary>
        /// <param name="nodeType">The node type that is being extended.</param>
        public void Initialize(IExplorerNodeType nodeType)
        {
            nodeType.NodeChildrenRequested += new EventHandler<ExplorerNodeEventArgs>(nodeType_NodeChildrenRequested);
        }

        /// <summary>
        /// Handles the NodeChildrenRequested event of the nodeType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.VisualStudio.SharePoint.Explorer.ExplorerNodeEventArgs"/> instance containing the event data.</param>
        void nodeType_NodeChildrenRequested(object sender, ExplorerNodeEventArgs e)
        {
            //Only perform this action if the site is not hanging off the connection node
            //The connection node already has a content types node
            if (EnabledExtensionsOptionsPage.GetSetting<bool>(EnabledExtensionsOptions.ContentTypesGroupedView, true))
            {
                if (e.Node.ParentNode.NodeType.Id != ExplorerNodeTypes.ConnectionNode)
                {
                    e.Node.ChildNodes.AddFolder(CKSProperties.ContentTypesSiteNodeExtension_ContentTypesNode, CKSProperties.ContentTypes.ToBitmap(), AddContentTypeGroups);
                }
                else
                {
                    AddContentTypeGroupsToExistingContentTypesNode(e.Node);
                }
            }
        }

        /// <summary>
        /// Adds the content type groups.
        /// </summary>
        /// <param name="contentTypesFolder">The content types folder.</param>
        private void AddContentTypeGroups(IExplorerNode contentTypesFolder)
        {
            if (contentTypesFolder.ParentNode != null &&
                contentTypesFolder.ParentNode.NodeType.Name == ExplorerNodeTypes.SiteNode)
            {
                string[] contentTypeGroups = GetContentTypeGroups(contentTypesFolder);
                if (contentTypeGroups != null)
                {
                    foreach (string groupName in contentTypeGroups)
                    {
                        IExplorerNode contentTypeGroup = contentTypesFolder.ChildNodes.Add(ExplorerNodeIds.ContentTypeGroupNode, groupName, null, -1);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the content type groups to existing content types node.
        /// </summary>
        /// <param name="contentTypesFolder">The content types folder.</param>
        private void AddContentTypeGroupsToExistingContentTypesNode(IExplorerNode contentTypesFolder)
        {
            if (contentTypesFolder.ParentNode != null &&
                contentTypesFolder.ParentNode.NodeType.Name == ExplorerNodeTypes.ConnectionNode)
            {
                IExplorerNode contentTypesFolder1 = contentTypesFolder.ChildNodes.FirstOrDefault(c => c.Text == CKSProperties.ContentTypesSiteNodeExtension_ContentTypesNode);

                string[] contentTypeGroups = GetContentTypeGroups(contentTypesFolder);
                if (contentTypeGroups != null)
                {
                    foreach (string groupName in contentTypeGroups)
                    {
                        IExplorerNode contentTypeGroup = contentTypesFolder1.ChildNodes.Add(ExplorerNodeIds.ContentTypeGroupNode, groupName, null, -1);
                    }
                }
            }
        }
        /// <summary>
        /// Gets the content type groups.
        /// </summary>
        /// <param name="contentTypesFolder">The content types folder.</param>
        /// <returns></returns>
        private string[] GetContentTypeGroups(IExplorerNode contentTypesFolder)
        {
            if (contentTypesFolder == null)
            {
                throw new ArgumentNullException("contentTypesFolder");
            }

            return contentTypesFolder.Context.SharePointConnection.ExecuteCommand<string[]>(ContentTypeSharePointCommandIds.GetContentTypeGroups);
        }

        #endregion
    }
}