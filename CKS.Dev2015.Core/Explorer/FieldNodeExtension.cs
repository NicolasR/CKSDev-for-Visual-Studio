using CKS.Dev2015.VisualStudio.SharePoint.Commands;
using CKS.Dev2015.VisualStudio.SharePoint.Commands.Info;
using CKS.Dev2015.VisualStudio.SharePoint.Environment.Options;
using Microsoft.VisualStudio.SharePoint;
using Microsoft.VisualStudio.SharePoint.Explorer;
using Microsoft.VisualStudio.SharePoint.Explorer.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows;
using System.Xml.Linq;
using CKSProperties = CKS.Dev2015.Core.Properties.Resources;

namespace CKS.Dev2015.VisualStudio.SharePoint.Explorer
{
    /// <summary>
    /// Extend the field node with functionality.
    /// </summary>
    [Export(typeof(IExplorerNodeTypeExtension))]
    //The node type to bind with
    [ExplorerNodeType(ExtensionNodeTypes.FieldNode)]
    internal class FieldNodeExtension : IExplorerNodeTypeExtension
    {
        /// <summary>
        /// Initializes the node extension.
        /// </summary>
        /// <param name="nodeType">The node type that is being extended.</param>
        public void Initialize(IExplorerNodeType nodeType)
        {
            nodeType.NodeMenuItemsRequested += NodeMenuItemsRequested;
            nodeType.NodePropertiesRequested += NodePropertiesRequested;
        }

        /// <summary>
        /// Nodes the properties requested.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ExplorerNodePropertiesRequestedEventArgs" /> instance containing the event data.</param>
        void NodePropertiesRequested(object sender, ExplorerNodePropertiesRequestedEventArgs e)
        {
            try
            {
                IFieldNodeInfo info = e.Node.Annotations.GetValue<IFieldNodeInfo>();
                if (info.ListId == Guid.Empty && String.IsNullOrEmpty(info.ContentTypeName))
                {
                    IDictionary<string, string> fieldProperties = e.Node.Context.SharePointConnection.ExecuteCommand<FieldNodeInfo, Dictionary<string, string>>(SiteColumnsSharePointCommandIds.GetProperties, info as FieldNodeInfo);
                    object propertySource = e.Node.Context.CreatePropertySourceObject(fieldProperties);
                    e.PropertySources.Add(propertySource);
                }
            }
            catch (Exception ex)
            {
                DTEManager.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
            }
        }

        /// <summary>
        /// Nodes the menu items requested.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ExplorerNodeMenuItemsRequestedEventArgs" /> instance containing the event data.</param>
        void NodeMenuItemsRequested(object sender, ExplorerNodeMenuItemsRequestedEventArgs e)
        {
            if (EnabledExtensionsOptionsPage.GetSetting<bool>(EnabledExtensionsOptions.FieldCopyID, true))
            {
                IMenuItem copyIdItem = e.MenuItems.Add(CKSProperties.ContentTypeFieldNodeExtension_CopyIdNodeName, 1);
                copyIdItem.Click += copyIdItem_Click;
            }

            if (EnabledExtensionsOptionsPage.GetSetting<bool>(EnabledExtensionsOptions.ImportField, true))
            {
                if (DTEManager.ActiveSharePointProject != null)
                {
                    IMenuItem importFieldMenuItem = e.MenuItems.Add(CKSProperties.FieldNodeExtension_ImportNodeName);
                    importFieldMenuItem.Click += importFieldMenuItem_Click;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the copyIdItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MenuItemEventArgs" /> instance containing the event data.</param>
        void copyIdItem_Click(object sender, MenuItemEventArgs e)
        {
            IExplorerNode owner = (IExplorerNode)e.Owner;
            IFieldNodeInfo annotation = owner.Annotations.GetValue<IFieldNodeInfo>();
            if (annotation.Id != null)
            {
                Clipboard.SetData(DataFormats.Text, annotation.Id.ToString("B"));
            }
        }

        /// <summary>
        /// Handles the Click event of the importFieldMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MenuItemEventArgs" /> instance containing the event data.</param>
        void importFieldMenuItem_Click(object sender, MenuItemEventArgs e)
        {
            IExplorerNode node = e.Owner as IExplorerNode;
            if (node != null)
            {
                ImportField(node);
            }
        }

        /// <summary>
        /// Imports the field.
        /// </summary>
        /// <param name="fieldNode">The field node.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void ImportField(IExplorerNode fieldNode)
        {
            if (fieldNode == null)
            {
                throw new ArgumentNullException("fieldNode");
            }

            Microsoft.VisualStudio.SharePoint.Explorer.Extensions.IFieldNodeInfo nodeInfo = fieldNode.Annotations.GetValue<Microsoft.VisualStudio.SharePoint.Explorer.Extensions.IFieldNodeInfo>();
            if (nodeInfo != null)
            {
                FieldNodeInfo fieldNodeInfo = new FieldNodeInfo
                {
                    ContentTypeName = nodeInfo.ContentTypeName,
                    Id = nodeInfo.Id,
                    IsHidden = nodeInfo.IsHidden,
                    ListId = nodeInfo.ListId,
                    Title = nodeInfo.Title
                };
                Dictionary<string, string> fieldProperties = null;

                if (String.IsNullOrEmpty(fieldNodeInfo.ContentTypeName) && fieldNodeInfo.ListId == Guid.Empty)
                {
                    fieldProperties = fieldNode.Context.SharePointConnection.ExecuteCommand<FieldNodeInfo, Dictionary<string, string>>(SiteColumnsSharePointCommandIds.GetProperties, fieldNodeInfo);
                }
                else
                {
                    fieldProperties = fieldNode.Context.SharePointConnection.ExecuteCommand<FieldNodeInfo, Dictionary<string, string>>(FieldSharePointCommandIds.GetProperties, fieldNodeInfo);
                }

                if (fieldProperties != null)
                {
                    XNamespace xn = XNamespace.Get("http://schemas.microsoft.com/sharepoint/");
                    XElement xElements = new XElement(xn + "Elements",
                        XElement.Parse(fieldProperties["SchemaXml"]));

                    EnvDTE.Project activeProject = DTEManager.ActiveProject;
                    if (activeProject != null)
                    {
                        ISharePointProjectService projectService = fieldNode.ServiceProvider.GetService(typeof(ISharePointProjectService)) as ISharePointProjectService;
                        ISharePointProject activeSharePointProject = projectService.Projects[activeProject.FullName];
                        if (activeSharePointProject != null)
                        {
                            ISharePointProjectItem fieldProjectItem = activeSharePointProject.ProjectItems.Add(fieldProperties["InternalName"], "Microsoft.VisualStudio.SharePoint.Field");
                            System.IO.File.WriteAllText(Path.Combine(fieldProjectItem.FullPath, "Elements.xml"), xElements.ToString().Replace("xmlns=\"\"", String.Empty));
                            ISharePointProjectItemFile elementsXml = fieldProjectItem.Files.AddFromFile("Elements.xml");
                            elementsXml.DeploymentType = DeploymentType.ElementManifest;
                            elementsXml.DeploymentPath = String.Format(@"{0}\", fieldProperties["InternalName"]);
                            fieldProjectItem.DefaultFile = elementsXml;
                        }
                    }
                }
            }
        }
    }
}
