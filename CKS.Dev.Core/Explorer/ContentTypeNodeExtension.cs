using CKS.Dev.VisualStudio.SharePoint.Commands;
using CKS.Dev.VisualStudio.SharePoint.Commands.Info;
using CKS.Dev.VisualStudio.SharePoint.Environment.Options;
using Microsoft.VisualStudio.SharePoint;
using Microsoft.VisualStudio.SharePoint.Explorer;
using Microsoft.VisualStudio.SharePoint.Explorer.Extensions;
using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using CKSProperties = CKS.Dev.Core.Properties.Resources;
namespace CKS.Dev.VisualStudio.SharePoint.Explorer
{
    /// <summary>
    /// Extend the site node for Content Type with functionality.
    /// </summary>
    [Export(typeof(IExplorerNodeTypeExtension))]
    //The node type to bind with
    [ExplorerNodeType(ExtensionNodeTypes.ContentTypeNode)]
    class ContentTypeNodeExtension : IExplorerNodeTypeExtension
    {
        #region Methods

        /// <summary>
        /// Initialise the node and register its events.
        /// </summary>
        /// <param name="nodeType">The node type.</param>
        public void Initialize(IExplorerNodeType nodeType)
        {
            nodeType.NodeMenuItemsRequested += NodeType_NodeMenuItemsRequested;
        }

        /// <summary>
        /// Create the child nodes and register their events.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The ExplorerNodeMenuItemsRequestedEventArgs.</param>
        void NodeType_NodeMenuItemsRequested(object sender, ExplorerNodeMenuItemsRequestedEventArgs e)
        {
            //Add the child nodes
            if (EnabledExtensionsOptionsPage.GetSetting<bool>(EnabledExtensionsOptions.ContentTypeCopyID, true))
            {
                IMenuItem copyIdItem = e.MenuItems.Add(CKSProperties.ContentTypeNodeExtension_CopyIdNodeName, 1);
                copyIdItem.Click += new EventHandler<MenuItemEventArgs>(CopyIdItem_Click);
            }

            if (EnabledExtensionsOptionsPage.GetSetting<bool>(EnabledExtensionsOptions.ImportContentType, true))
            {
                if (DTEManager.ActiveSharePointProject != null)
                {
                    IMenuItem importContentTypeItem = e.MenuItems.Add(CKSProperties.ContentTypeNodeExtension_ImportNodeName, 2);
                    importContentTypeItem.Click += new EventHandler<MenuItemEventArgs>(ImportContentTypeItem_Click);
                }
            }

            if (EnabledExtensionsOptionsPage.GetSetting<bool>(EnabledExtensionsOptions.CreatePageLayoutFromContentType, true))
            {
                IMenuItem createPageLayoutMenuItem = e.MenuItems.Add(CKSProperties.ContentTypeNodeExtension_CreatePublishingPageLayoutName);
                createPageLayoutMenuItem.Click += new EventHandler<Microsoft.VisualStudio.SharePoint.MenuItemEventArgs>(CreatePageLayoutContentTypeNodeExtension_Click);

                IContentTypeNodeInfo ctInfo = e.Node.Annotations.GetValue<IContentTypeNodeInfo>();
                createPageLayoutMenuItem.IsEnabled = e.Node.Context.SharePointConnection.ExecuteCommand<string, bool>(ContentTypeSharePointCommandIds.IsPublishingContentTypeCommand, ctInfo.Name);
            }
        }

        /// <summary>
        /// Handles the Click event of the CreatePageLayoutContentTypeNodeExtension control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.VisualStudio.SharePoint.MenuItemEventArgs" /> instance containing the event data.</param>
        void CreatePageLayoutContentTypeNodeExtension_Click(object sender, Microsoft.VisualStudio.SharePoint.MenuItemEventArgs e)
        {
            IExplorerNode ctNode = e.Owner as IExplorerNode;

            if (ctNode != null)
            {
                IContentTypeNodeInfo ctInfo = ctNode.Annotations.GetValue<IContentTypeNodeInfo>();

                string pageLayoutContents = ctNode.Context.SharePointConnection.ExecuteCommand<string, string>(ContentTypeSharePointCommandIds.CreatePageLayoutCommand, ctInfo.Name);
                DTEManager.CreateNewTextFile(SafeContentTypeName(ctInfo.Name) + ".aspx", pageLayoutContents);
            }
        }

        /// <summary>
        /// Safes the name of the content type.
        /// </summary>
        /// <param name="contentTypeName">Name of the content type.</param>
        /// <returns></returns>
        string SafeContentTypeName(string contentTypeName)
        {
            string safeName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(contentTypeName);
            safeName = new Regex(@"[^\w]").Replace(safeName, "");
            return safeName;
        }

        /// <summary>
        /// The copy id item click event copies the selected content type node id to the clipboard.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The MenuItemEventArgs.</param>
        void CopyIdItem_Click(object sender, MenuItemEventArgs e)
        {
            IExplorerNode owner = (IExplorerNode)e.Owner;
            IContentTypeNodeInfo annotation = owner.Annotations.GetValue<IContentTypeNodeInfo>();
            string id = owner.Context.SharePointConnection.ExecuteCommand<string, string>(ContentTypeSharePointCommandIds.GetContentTypeID, annotation.Name);
            if (!String.IsNullOrEmpty(id))
            {
                Clipboard.SetData(DataFormats.Text, id);
            }
        }

        /// <summary>
        /// The import item click event imports the selected content type into the currently selected project.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The MenuItemEventArgs.</param>
        void ImportContentTypeItem_Click(object sender, MenuItemEventArgs e)
        {
            //TODO: Would be useful to check if the selected content type is already part of the solution before import is allowed.... see the content type wizard for ideas
            IExplorerNode owner = (IExplorerNode)e.Owner;
            IContentTypeNodeInfo annotation = owner.Annotations.GetValue<IContentTypeNodeInfo>();
            if (owner.Context.SharePointConnection.ExecuteCommand<string, bool>(ContentTypeSharePointCommandIds.IsBuiltInContentType, annotation.Name))
            {
                if (MessageBox.Show(CKSProperties.ContentTypeNodeExtension_ImportConfirmationQuestion,
                    CKSProperties.ContentTypeNodeExtension_ImportConfirmationDialogTitle,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    ImportContentType(e);
                }
            }
            else
            {
                ImportContentType(e);
            }
        }

        /// <summary>
        /// Import the selected content type into the currently selected project.
        /// </summary>
        /// <param name="contentTypeNode">Selected Content Type node.</param>
        public static void ImportContentType(IExplorerNode contentTypeNode)
        {
            if (contentTypeNode == null)
            {
                throw new ArgumentNullException("contentTypeNode");
            }

            Cursor.Current = Cursors.WaitCursor;

            IContentTypeNodeInfo annotation = contentTypeNode.Annotations.GetValue<IContentTypeNodeInfo>();
            ContentTypeInfo contentTypeInfo = contentTypeNode.Context.SharePointConnection.ExecuteCommand<string, ContentTypeInfo>(ContentTypeSharePointCommandIds.GetContentTypeImportProperties, annotation.Name);

            ImportContentType(contentTypeInfo);
        }

        /// <summary>
        /// Imports the type of the content.
        /// </summary>
        /// <param name="contentTypeInfo">The content type info.</param>
        public static void ImportContentType(ContentTypeInfo contentTypeInfo)
        {
            if (contentTypeInfo != null)
            {
                //Create the encoding definition
                XDeclaration declaration = new XDeclaration("1.0", Encoding.UTF8.WebName, null);

                XNamespace sharepointToolsNamespace = @"http://schemas.microsoft.com/VisualStudio/2010/SharePointTools/SharePointProjectItemModel";
                XNamespace sharepointNamespace = @"http://schemas.microsoft.com/sharepoint/";

                XElement elementsFileContents = XElement.Parse(BuildElements(contentTypeInfo, declaration, sharepointNamespace));

                EnvDTE.Project activeProject = DTEManager.ActiveProject;

                if (activeProject != null)
                {
                    ISharePointProjectService projectService = DTEManager.ProjectService;
                    ISharePointProject activeSharePointProject = DTEManager.ActiveSharePointProject;

                    if (activeSharePointProject != null)
                    {
                        ISharePointProjectItem contentTypeProjectItem = activeSharePointProject.ProjectItems.Add(contentTypeInfo.Name, "Microsoft.VisualStudio.SharePoint.ContentType");
                        System.IO.File.WriteAllText(Path.Combine(contentTypeProjectItem.FullPath, "Elements.xml"), elementsFileContents.ToString().Replace("xmlns=\"\"", String.Empty));

                        ISharePointProjectItemFile elementsXml = contentTypeProjectItem.Files.AddFromFile("Elements.xml");
                        elementsXml.DeploymentType = DeploymentType.ElementManifest;
                        elementsXml.DeploymentPath = String.Format(@"{0}\", contentTypeInfo.Name);

                        contentTypeProjectItem.DefaultFile = elementsXml;
                    }
                }

            }
        }

        /// <summary>
        /// Import the selected content type into the currently selected project.
        /// </summary>
        /// <param name="e">The MenuItemEventArgs object.</param>
        void ImportContentType(MenuItemEventArgs e)
        {
            IExplorerNode owner = (IExplorerNode)e.Owner;
            if (owner != null)
            {
                ImportContentType(owner);
            }
        }

        /// <summary>
        /// Builds the elements.
        /// </summary>
        /// <param name="contentTypeInfo">The content type info.</param>
        /// <param name="declaration">The declaration.</param>
        /// <param name="sharepointNamespace">The sharepoint namespace.</param>
        /// <returns>The elements file.</returns>
        private static string BuildElements(ContentTypeInfo contentTypeInfo, XDeclaration declaration, XNamespace sharepointNamespace)
        {
            #region elements

            XElement elements = new XElement(sharepointNamespace + "Elements");

            elements.Add(BuildContentTypeElement(contentTypeInfo));

            string elementsAsString = elements.ToString().Replace(@" xmlns=""""", "");

            XDocument elementsDoc = new XDocument(declaration, elements);

            string elem = elementsDoc.ToString();

            string finalElements = elem.Replace(" xmlns=\"\"", "");

            #endregion

            return finalElements;
        }

        /// <summary>
        /// Builds the entire element.
        /// </summary>
        /// <param name="contentTypeInfo">The content type info.</param>
        /// <returns>The XElement representing the content type.</returns>
        static XElement BuildContentTypeElement(ContentTypeInfo contentTypeInfo)
        {
            XNamespace sharePointNamespace = XNamespace.Get("http://schemas.microsoft.com/sharepoint/");

            XElement contentType = new XElement("ContentType");

            if (!String.IsNullOrEmpty(contentTypeInfo.Id))
            {
                XAttribute id = new XAttribute("ID", contentTypeInfo.Id);
                contentType.Add(id);
            }

            if (!String.IsNullOrEmpty(contentTypeInfo.Name))
            {
                XAttribute name = new XAttribute("Name", contentTypeInfo.Name);
                contentType.Add(name);
            }

            if (!String.IsNullOrEmpty(contentTypeInfo.Description))
            {
                XAttribute description = new XAttribute("Description", contentTypeInfo.Description);
                contentType.Add(description);
            }

            if (!String.IsNullOrEmpty(contentTypeInfo.Group))
            {
                XAttribute group = new XAttribute("Group", contentTypeInfo.Group);
                contentType.Add(group);
            }

            if (contentTypeInfo.Inherits != null)
            {
                XAttribute inherits = new XAttribute("Inherits", contentTypeInfo.Inherits);
                contentType.Add(inherits);
            }

            if (contentTypeInfo.Hidden != null)
            {
                XAttribute hidden = new XAttribute("Hidden", contentTypeInfo.Hidden);
                contentType.Add(hidden);
            }

            if (contentTypeInfo.ReadOnly != null)
            {
                XAttribute readOnly = new XAttribute("ReadOnly", contentTypeInfo.ReadOnly);
                contentType.Add(readOnly);
            }

            if (contentTypeInfo.Sealed != null)
            {
                XAttribute sealedValue = new XAttribute("Sealed", contentTypeInfo.Sealed);
                contentType.Add(sealedValue);
            }

            //Add the field refs
            if (contentTypeInfo.FieldRefs != null && contentTypeInfo.FieldRefs.Length > 0)
            {
                XElement fieldRefs = new XElement("FieldRefs");

                foreach (var item in contentTypeInfo.FieldRefs)
                {
                    fieldRefs.Add(new XElement("FieldRef",
                        new XAttribute("ID", item.Id),
                        new XAttribute("Name", item.Name),
                        new XAttribute("DisplayName", item.DisplayName)));
                }

                contentType.Add(fieldRefs);
            }

            //Add the document template
            if (!String.IsNullOrEmpty(contentTypeInfo.DocumentTemplate))
            {
                contentType.Add(new XElement(sharePointNamespace + "DocumentTemplate", new XAttribute("TargetName", contentTypeInfo.DocumentTemplate)));
            }

            if (contentTypeInfo.XmlDocuments != null && contentTypeInfo.XmlDocuments.Length > 0)
            {
                XElement xmlDocuments = new XElement(sharePointNamespace + "XmlDocuments");

                foreach (string xmlDocument in contentTypeInfo.XmlDocuments)
                {
                    XElement xDocument = XElement.Parse(xmlDocument);
                    string ns = "http://schemas.microsoft.com/sharepoint/v3/contenttype/forms";
                    XAttribute xmlns = xDocument.Attribute("xmlns");
                    if (xmlns != null)
                    {
                        ns = xmlns.Value;
                    }

                    xmlDocuments.Add(new XElement(sharePointNamespace + "XmlDocument",
                        new XAttribute("NamespaceURI", ns),
                        xDocument));
                }

                contentType.Add(xmlDocuments);
            }

            return contentType;
        }

        #endregion
    }
}
