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
    /// The site column group node type provider.
    /// </summary>
    // Enables Visual Studio to discover and load this extension.
    [Export(typeof(IExplorerNodeTypeProvider))]
    // Indicates that this class extends SharePoint nodes in Server Explorer.
    [ExplorerNodeType(ExplorerNodeIds.SiteColumnsGroupNode)]
    internal class SiteColumnGroupNodeTypeProvider : IExplorerNodeTypeProvider
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
            FieldNodeInfo[] fields = e.Node.Context.SharePointConnection.ExecuteCommand<string, FieldNodeInfo[]>(SiteColumnsSharePointCommandIds.GetSiteColumnsFromGroup, e.Node.Text);

            if (fields != null)
            {
                foreach (var field in fields)
                {
                    e.Node.ChildNodes.Add(ExtensionNodeTypes.FieldNode, field.Title, new Dictionary<object, object>
                    {
                        { typeof(IFieldNodeInfo), field }
                    });
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
            if (EnabledExtensionsOptionsPage.GetSetting<bool>(EnabledExtensionsOptions.ImportSiteColumnGroup, true))
            {
                if (DTEManager.ActiveSharePointProject != null)
                {
                    IMenuItem importSiteColumnsMenuItem = e.MenuItems.Add(CKSProperties.SiteColumnsGroupNodeTypeProvider_ImportSiteColumns);
                    importSiteColumnsMenuItem.Click += new EventHandler<MenuItemEventArgs>(importSiteColumnsMenuItem_Click);
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the importSiteColumnsMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.VisualStudio.SharePoint.MenuItemEventArgs"/> instance containing the event data.</param>
        void importSiteColumnsMenuItem_Click(object sender, MenuItemEventArgs e)
        {
            IExplorerNode siteColumnsGroupNode = e.Owner as IExplorerNode;
            if (siteColumnsGroupNode != null &&
                siteColumnsGroupNode.ChildNodes != null &&
                siteColumnsGroupNode.ChildNodes.Count() > 0)
            {
                foreach (IExplorerNode childNode in siteColumnsGroupNode.ChildNodes)
                {
                    FieldNodeExtension.ImportField(childNode);
                }
            }
        }
    }
}
