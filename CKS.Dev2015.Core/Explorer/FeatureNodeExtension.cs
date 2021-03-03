using CKS.Dev2015.VisualStudio.SharePoint.Commands;
using CKS.Dev2015.VisualStudio.SharePoint.Commands.Info;
using CKS.Dev2015.VisualStudio.SharePoint.Environment.Options;
using Microsoft.VisualStudio.SharePoint;
using Microsoft.VisualStudio.SharePoint.Explorer;
using Microsoft.VisualStudio.SharePoint.Explorer.Extensions;
using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using CKSProperties = CKS.Dev2015.Core.Properties.Resources;
using Scope = CKS.Dev2015.VisualStudio.SharePoint.Commands.Info.FeatureScope;

namespace CKS.Dev2015.VisualStudio.SharePoint.Explorer
{
    /// <summary>
    /// Extend the site node for a Feature with functionality.
    /// </summary>
    [Export(typeof(IExplorerNodeTypeExtension))]
    //The node type to bind with
    [ExplorerNodeType(ExtensionNodeTypes.FeatureNode)]
    public class FeatureNodeExtension : IExplorerNodeTypeExtension
    {
        #region Methods

        /// <summary>
        /// Initialise the node and register its events.
        /// </summary>
        /// <param name="nodeType">The node type.</param>
        public void Initialize(IExplorerNodeType nodeType)
        {
            nodeType.NodeChildrenRequested += NodeType_NodeChildrenRequested;
            nodeType.NodeMenuItemsRequested += NodeType_NodeMenuItemsRequested;
        }

        /// <summary>
        /// Create the child nodes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The ExplorerNodeEventArgs.</param>
        void NodeType_NodeChildrenRequested(object sender, ExplorerNodeEventArgs e)
        {
            if (EnabledExtensionsOptionsPage.GetSetting<bool>(EnabledExtensionsOptions.ViewFeatureDependencies, true))
            {
                e.Node.ChildNodes.AddFolder(CKSProperties.FeatureNodeExtension_DependenciesFolderNodeName,
                CKSProperties.FolderNode.ToBitmap(),
                new Action<IExplorerNode>(FeatureDependencyNodeTypeProvider.CreateFeatureDependencyNodes));
            }

            if (EnabledExtensionsOptionsPage.GetSetting<bool>(EnabledExtensionsOptions.ViewFeatureElements, true))
            {
                e.Node.ChildNodes.AddFolder(CKSProperties.FeatureNodeExtension_ElementsFolderNodeName,
                CKSProperties.FolderNode.ToBitmap(),
                new Action<IExplorerNode>(FeatureElementNodeTypeProvider.CreateFeatureElementNodes));
            }
        }

        /// <summary>
        /// Create the child nodes and register their events.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The ExplorerNodeMenuItemsRequestedEventArgs.</param>
        void NodeType_NodeMenuItemsRequested(object sender, ExplorerNodeMenuItemsRequestedEventArgs e)
        {
            IFeatureNodeInfo info = e.Node.Annotations.GetValue<IFeatureNodeInfo>();
            FeatureInfo featureDetails = new FeatureInfo()
            {
                FeatureID = info.Id,
                Scope = (Scope)info.Scope
            };

            if (EnabledExtensionsOptionsPage.GetSetting<bool>(EnabledExtensionsOptions.ActivateDeactivateFeature, true))
            {
                AddToggleFeatureMenuItem(e.Node, e.MenuItems, featureDetails);
            }

            if (EnabledExtensionsOptionsPage.GetSetting<bool>(EnabledExtensionsOptions.FeatureCopyID, true))
            {
                AddCopyIDMenuItem(e.Node, e.MenuItems, info);
            }
        }

        /// <summary>
        /// Add the copy Id menu item.
        /// </summary>
        /// <param name="parent">The parent node.</param>
        /// <param name="items">The menu item collection.</param>
        /// <param name="nodeInfo">The feature info.</param>
        void AddCopyIDMenuItem(IExplorerNode parent, IMenuItemCollection items, IFeatureNodeInfo nodeInfo)
        {
            IMenuItem copyIdItem = items.Add(CKSProperties.FeatureNodeExtension_CopyIdNodeName, 1);
            copyIdItem.Click += new EventHandler<MenuItemEventArgs>(CopyIdItem_Click);
        }

        /// <summary>
        /// The copy id item click event copies the selected feature node id to the clipboard.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The MenuItemEventArgs.</param>
        void CopyIdItem_Click(object sender, MenuItemEventArgs e)
        {
            IExplorerNode owner = (IExplorerNode)e.Owner;
            IFeatureNodeInfo annotation = owner.Annotations.GetValue<IFeatureNodeInfo>();
            if (annotation.Id != null)
            {
                Clipboard.SetData(DataFormats.Text, annotation.Id.ToString("D"));
            }
        }

        /// <summary>
        /// Add the toggle feature menu item.
        /// </summary>
        /// <param name="parent">The parent node.</param>
        /// <param name="items">The menu item collection.</param>
        /// <param name="featureDetails">The feature info.</param>
        void AddToggleFeatureMenuItem(IExplorerNode parent, IMenuItemCollection items, FeatureInfo featureDetails)
        {
            bool isEnabled = parent.Context.SharePointConnection.ExecuteCommand<FeatureInfo, bool>(FeatureSharePointCommandIds.IsFeatureEnabled, featureDetails);
            if (isEnabled == false)
            {
                IMenuItem item = items.Add(
                    String.Format(CKSProperties.FeatureNodeExtension_ActivateNodeName, featureDetails.Scope));
                item.Click +=
                    delegate
                    {
                        try
                        {
                            Cursor.Current = Cursors.WaitCursor;
                            parent.Context.SharePointConnection.ExecuteCommand<FeatureInfo>(
                                FeatureSharePointCommandIds.EnableFeature, featureDetails);
                        }
                        finally
                        {
                            Cursor.Current = Cursors.Default;
                        }
                    };
            }
            else
            {
                IMenuItem item = items.Add(
                    String.Format(CKSProperties.FeatureNodeExtension_DeactivateNodeName, featureDetails.Scope));
                item.Click +=
                    delegate
                    {
                        try
                        {
                            Cursor.Current = Cursors.WaitCursor;
                            parent.Context.SharePointConnection.ExecuteCommand<FeatureInfo>(
                                FeatureSharePointCommandIds.DisableFeature, featureDetails);
                        }
                        finally
                        {
                            Cursor.Current = Cursors.Default;
                        }
                    };
            }
        }

        #endregion
    }
}
