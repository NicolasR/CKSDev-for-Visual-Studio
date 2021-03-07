using CKS.Dev.VisualStudio.SharePoint.Environment.Options;
using Microsoft.VisualStudio.SharePoint;
using Microsoft.VisualStudio.SharePoint.Explorer;
using System;
using System.ComponentModel.Composition;

namespace CKS.Dev.VisualStudio.SharePoint.Explorer
{
    /// <summary>
    /// Entity classes site node extension.
    /// </summary>
    [Export(typeof(IExplorerNodeTypeExtension))]
    [ExplorerNodeType(ExplorerNodeTypes.SiteNode)]
    public class EntityClassesSiteNodeExtension : IExplorerNodeTypeExtension
    {
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
            if (EnabledExtensionsOptionsPage.GetSetting<bool>(EnabledExtensionsOptions.SiteGenerateEntityClasses, true))
            {
                e.MenuItems.Add("Generate entity classes").Click += new EventHandler<Microsoft.VisualStudio.SharePoint.MenuItemEventArgs>(EntityClassesSiteNodeExtension_Click);
            }
        }

        /// <summary>
        /// Handles the Click event of the EntityClassesSiteNodeExtension control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.VisualStudio.SharePoint.MenuItemEventArgs"/> instance containing the event data.</param>
        void EntityClassesSiteNodeExtension_Click(object sender, MenuItemEventArgs e)
        {
            IExplorerNode siteNode = e.Owner as IExplorerNode;
            IExplorerSiteNodeInfo siteInfo = siteNode.Annotations.GetValue<IExplorerSiteNodeInfo>();
            ISharePointProjectService projectService = (ISharePointProjectService)siteNode.ServiceProvider.GetService(typeof(ISharePointProjectService));
            if (projectService != null)
            {
                SPMetalUtilities.RunSPMetal(siteInfo.Url.ToString(), String.Empty, ProjectUtilities.GetSafeFileName(siteInfo.Title) + ".cs", projectService);
            }
        }
    }
}
