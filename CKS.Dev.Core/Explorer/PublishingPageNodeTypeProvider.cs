using CKS.Dev.VisualStudio.SharePoint.Commands;
using CKS.Dev.VisualStudio.SharePoint.Commands.Info;
using Microsoft.VisualStudio.SharePoint;
using Microsoft.VisualStudio.SharePoint.Explorer;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using CKSProperties = CKS.Dev.Core.Properties.Resources;

namespace CKS.Dev.VisualStudio.SharePoint.Explorer
{
    /// <summary>
    /// Publishing page node type provider.
    /// </summary>
    [Export(typeof(IExplorerNodeTypeProvider))]
    [ExplorerNodeType(ExplorerNodeIds.PublishingPageNodeTypeId)]
    public class PublishingPageNodeTypeProvider : IExplorerNodeTypeProvider
    {
        #region Methods

        /// <summary>
        /// Initializes the new node type.
        /// </summary>
        /// <param name="typeDefinition">The definition of the new node type.</param>
        public void InitializeType(IExplorerNodeTypeDefinition typeDefinition)
        {
            typeDefinition.IsAlwaysLeaf = true;
            typeDefinition.DefaultIcon = CKSProperties.PageNode.ToBitmap();
            typeDefinition.NodePropertiesRequested += new EventHandler<ExplorerNodePropertiesRequestedEventArgs>(typeDefinition_NodePropertiesRequested);
            typeDefinition.NodeMenuItemsRequested += new EventHandler<ExplorerNodeMenuItemsRequestedEventArgs>(typeDefinition_NodeMenuItemsRequested);
        }

        /// <summary>
        /// Handles the NodeMenuItemsRequested event of the typeDefinition control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ExplorerNodeMenuItemsRequestedEventArgs" /> instance containing the event data.</param>
        void typeDefinition_NodeMenuItemsRequested(object sender, ExplorerNodeMenuItemsRequestedEventArgs e)
        {
            IMenuItem exportMenuItem = e.MenuItems.Add("Export");
            exportMenuItem.Click += new EventHandler<MenuItemEventArgs>(exportMenuItem_Click);

            IMenuItem getTemplatePageMenuItem = e.MenuItems.Add("Get Template Page");
            getTemplatePageMenuItem.Click += new EventHandler<MenuItemEventArgs>(getTemplatePageMenuItem_Click);
        }

        /// <summary>
        /// Handles the Click event of the getTemplatePageMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MenuItemEventArgs" /> instance containing the event data.</param>
        void getTemplatePageMenuItem_Click(object sender, MenuItemEventArgs e)
        {
            DTEManager.CreateNewTextFile("TemplatePage.aspx", CKSProperties.TemplatePage);
        }

        /// <summary>
        /// Handles the Click event of the exportMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MenuItemEventArgs" /> instance containing the event data.</param>
        void exportMenuItem_Click(object sender, MenuItemEventArgs e)
        {
            IExplorerNode pageNode = e.Owner as IExplorerNode;
            if (pageNode != null)
            {
                PublishingPageInfo pageInfo = pageNode.Annotations.GetValue<PublishingPageInfo>();
                if (pageInfo != null)
                {
                    string pageXml = pageNode.Context.SharePointConnection.ExecuteCommand<PublishingPageInfo, string>(PublishingPageCommandIds.ExportToXml, pageInfo);
                    DTEManager.CreateNewTextFile(String.Format("{0}.xml", pageInfo.Name), pageXml);
                }
            }
        }

        /// <summary>
        /// Handles the NodePropertiesRequested event of the typeDefinition control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ExplorerNodePropertiesRequestedEventArgs" /> instance containing the event data.</param>
        void typeDefinition_NodePropertiesRequested(object sender, ExplorerNodePropertiesRequestedEventArgs e)
        {
            IExplorerNode pageNode = e.Node;
            if (pageNode != null)
            {
                IDictionary<string, string> publishingPageProperties = pageNode.Context.SharePointConnection.ExecuteCommand<PublishingPageInfo, Dictionary<string, string>>(PublishingPageCommandIds.GetProperties, pageNode.Annotations.GetValue<PublishingPageInfo>());
                object propertySource = e.Node.Context.CreatePropertySourceObject(publishingPageProperties);
                e.PropertySources.Add(propertySource);
            }
        }

        /// <summary>
        /// Creates the publishing page nodes.
        /// </summary>
        /// <param name="pagesFolder">The pages folder.</param>
        internal static void CreatePublishingPageNodes(IExplorerNode pagesFolder)
        {
            List<PublishingPageInfo> publishingPages = pagesFolder.Context.SharePointConnection.ExecuteCommand<List<PublishingPageInfo>>(SiteCommandIds.GetPublishingPagesCommandId);
            foreach (PublishingPageInfo publishingPage in publishingPages)
            {
                CreateNode(pagesFolder, publishingPage);
            }
        }

        /// <summary>
        /// Creates the node.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="publishingPage">The publishing page.</param>
        /// <returns></returns>
        public static IExplorerNode CreateNode(IExplorerNode parentNode, PublishingPageInfo publishingPage)
        {
            return parentNode.ChildNodes.Add(ExplorerNodeIds.PublishingPageNodeTypeId,
                String.IsNullOrEmpty(publishingPage.Name) ? publishingPage.Title : publishingPage.Name,
                new Dictionary<object, object> {
                    { typeof(PublishingPageInfo), publishingPage }
                });
        }

        #endregion
    }
}
