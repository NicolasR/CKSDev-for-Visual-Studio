using CKS.Dev2015.VisualStudio.SharePoint.Commands;
using CKS.Dev2015.VisualStudio.SharePoint.Commands.Info;
using CKS.Dev2015.VisualStudio.SharePoint.Environment.Options;
using Microsoft.VisualStudio.SharePoint;
using Microsoft.VisualStudio.SharePoint.Explorer;
using Microsoft.VisualStudio.SharePoint.Explorer.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using CKSProperties = CKS.Dev2015.Core.Properties.Resources;

namespace CKS.Dev2015.VisualStudio.SharePoint.Explorer
{
    /// <summary>
    /// The content type group node type provider.
    /// </summary>
    // Enables Visual Studio to discover and load this extension.
    [Export(typeof(IExplorerNodeTypeProvider))]
    // Indicates that this class extends SharePoint nodes in Server Explorer.
    [ExplorerNodeType(ExplorerNodeIds.ContentTypeGroupNode)]
    public class ContentTypeGroupNodeTypeProvider : IExplorerNodeTypeProvider
    {
        /// <summary>
        /// Initializes the new node type.
        /// </summary>
        /// <param name="typeDefinition">The definition of the new node type.</param>
        public void InitializeType(IExplorerNodeTypeDefinition typeDefinition)
        {
            typeDefinition.NodeChildrenRequested += new EventHandler<ExplorerNodeEventArgs>(typeDefinition_NodeChildrenRequested);
            typeDefinition.NodeMenuItemsRequested += new EventHandler<ExplorerNodeMenuItemsRequestedEventArgs>(typeDefinition_NodeMenuItemsRequested);
        }

        /// <summary>
        /// Handles the NodeChildrenRequested event of the typeDefinition control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.VisualStudio.SharePoint.Explorer.ExplorerNodeEventArgs"/> instance containing the event data.</param>
        void typeDefinition_NodeChildrenRequested(object sender, ExplorerNodeEventArgs e)
        {
            ContentTypeNodeInfo[] contentTypes = e.Node.Context.SharePointConnection.ExecuteCommand<string, ContentTypeNodeInfo[]>(ContentTypeSharePointCommandIds.GetContentTypesFromGroup, e.Node.Text);

            if (contentTypes != null)
            {
                foreach (var contentType in contentTypes)
                {
                    var annotations = new Dictionary<object, object>
                        {
                            { typeof(IContentTypeNodeInfo), contentType }
                        };


                    e.Node.ChildNodes.Add(ExtensionNodeTypes.ContentTypeNode, contentType.Name, annotations);
                }
            }
        }

        /// <summary>
        /// Handles the NodeMenuItemsRequested event of the typeDefinition control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.VisualStudio.SharePoint.Explorer.ExplorerNodeMenuItemsRequestedEventArgs"/> instance containing the event data.</param>
        void typeDefinition_NodeMenuItemsRequested(object sender, ExplorerNodeMenuItemsRequestedEventArgs e)
        {
            if (EnabledExtensionsOptionsPage.GetSetting<bool>(EnabledExtensionsOptions.ImportContentTypeGroup, true))
            {
                if (DTEManager.ActiveSharePointProject != null)
                {
                    IMenuItem importContentTypesMenuItem = e.MenuItems.Add(CKSProperties.ContentTypeGroupNodeTypeProvider_ImportContentTypes);
                    importContentTypesMenuItem.Click += new EventHandler<MenuItemEventArgs>(importContentTypesMenuItem_Click);
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the importContentTypesMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.VisualStudio.SharePoint.MenuItemEventArgs"/> instance containing the event data.</param>
        void importContentTypesMenuItem_Click(object sender, MenuItemEventArgs e)
        {
            IExplorerNode contentTypeGroupNode = e.Owner as IExplorerNode;
            if (contentTypeGroupNode != null &&
                contentTypeGroupNode.ChildNodes != null &&
                contentTypeGroupNode.ChildNodes.Count() > 0)
            {
                //This will be valid if a user has already expanded the group node
                foreach (IExplorerNode childNode in contentTypeGroupNode.ChildNodes)
                {
                    ContentTypeNodeExtension.ImportContentType(childNode);
                }
            }
            else if (contentTypeGroupNode != null &&
                contentTypeGroupNode.ChildNodes != null)
            {
                //This is valid if the user has not expanded the group node but we still need the ct's
                ContentTypeNodeInfo[] contentTypes = contentTypeGroupNode.Context.SharePointConnection.ExecuteCommand<string, ContentTypeNodeInfo[]>(ContentTypeSharePointCommandIds.GetContentTypesFromGroup, contentTypeGroupNode.Text);

                if (contentTypes != null)
                {
                    foreach (ContentTypeNodeInfo contentTypeNodeInfo in contentTypes)
                    {
                        ContentTypeInfo contentTypeInfo = contentTypeGroupNode.Context.SharePointConnection.ExecuteCommand<string, ContentTypeInfo>(ContentTypeSharePointCommandIds.GetContentTypeImportProperties, contentTypeNodeInfo.Name);

                        ContentTypeNodeExtension.ImportContentType(contentTypeInfo);
                    }
                }
            }

        }
    }
}
