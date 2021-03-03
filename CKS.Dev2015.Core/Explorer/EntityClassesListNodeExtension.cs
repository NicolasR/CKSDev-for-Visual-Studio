using CKS.Dev2015.VisualStudio.SharePoint.Environment.Options;
using Microsoft.VisualStudio.SharePoint;
using Microsoft.VisualStudio.SharePoint.Explorer;
using Microsoft.VisualStudio.SharePoint.Explorer.Extensions;
using System;
using System.ComponentModel.Composition;
using System.IO;
using CKSProperties = CKS.Dev2015.Core.Properties.Resources;

namespace CKS.Dev2015.VisualStudio.SharePoint.Explorer
{
    /// <summary>
    /// Entity classes list node extension.
    /// </summary>
    [Export(typeof(IExplorerNodeTypeExtension))]
    [ExplorerNodeType(ExtensionNodeTypes.ListNode)]
    public class EntityClassesListNodeExtension : IExplorerNodeTypeExtension
    {
        #region Methods

        /// <summary>
        /// Initializes the node extension.
        /// </summary>
        /// <param name="nodeType">The node type that is being extended.</param>
        public void Initialize(IExplorerNodeType nodeType)
        {
            nodeType.NodeMenuItemsRequested += new EventHandler<ExplorerNodeMenuItemsRequestedEventArgs>(nodeType_NodeMenuItemsRequested);
        }

        /// <summary>
        /// Handles the NodeMenuItemsRequested event of the nodeType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.VisualStudio.SharePoint.Explorer.ExplorerNodeMenuItemsRequestedEventArgs"/> instance containing the event data.</param>
        void nodeType_NodeMenuItemsRequested(object sender, ExplorerNodeMenuItemsRequestedEventArgs e)
        {
            if (EnabledExtensionsOptionsPage.GetSetting<bool>(EnabledExtensionsOptions.ListGenerateEntityClasses, true))
            {
                e.MenuItems.Add("Generate entity classes").Click += new EventHandler<MenuItemEventArgs>(SPMetalListNodeExtension_Click);
            }
        }

        /// <summary>
        /// Handles the Click event of the SPMetalListNodeExtension control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.VisualStudio.SharePoint.MenuItemEventArgs"/> instance containing the event data.</param>
        void SPMetalListNodeExtension_Click(object sender, MenuItemEventArgs e)
        {
            IExplorerNode listNode = e.Owner as IExplorerNode;
            IListNodeInfo listInfo = listNode.Annotations.GetValue<IListNodeInfo>();
            ISharePointProjectService projectService = (ISharePointProjectService)listNode.ServiceProvider.GetService(typeof(ISharePointProjectService));
            if (projectService != null)
            {
                string tempParamsFilePath = Path.GetTempFileName();
                File.WriteAllText(tempParamsFilePath, String.Format(CKSProperties.SPMetalListParametersXml, listInfo.Title));

                SPMetalUtilities.RunSPMetal(listNode.Context.SiteUrl.ToString(), String.Format(" /parameters:{0}", tempParamsFilePath), ProjectUtilities.GetSafeFileName(listInfo.Title) + ".cs", projectService);
                File.Delete(tempParamsFilePath);
            }
        }

        #endregion
    }
}

