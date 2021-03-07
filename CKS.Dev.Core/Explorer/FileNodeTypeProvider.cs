using CKS.Dev.VisualStudio.SharePoint.Commands;
using CKS.Dev.VisualStudio.SharePoint.Commands.Info;
using CKS.Dev.VisualStudio.SharePoint.Environment.Options;
using EnvDTE;
using Microsoft.VisualStudio.SharePoint;
using Microsoft.VisualStudio.SharePoint.Explorer;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Forms;
using CKSProperties = CKS.Dev.Core.Properties.Resources;

namespace CKS.Dev.VisualStudio.SharePoint.Explorer
{
    /// <summary>
    /// Represents an extension of SharePoint folder nodes in Server Explorer 
    /// </summary>
    // Enables Visual Studio to discover and load this extension.
    [Export(typeof(IExplorerNodeTypeProvider))]
    // Indicates that this class extends SharePoint file nodes in Server Explorer.
    [ExplorerNodeType(ExplorerNodeIds.FileNode)]
    internal class FileNodeTypeProvider : IExplorerNodeTypeProvider
    {
        #region Methods

        /// <summary>
        /// Initializes the new node type.
        /// </summary>
        /// <param name="typeDefinition">The definition of the new node type.</param>
        public virtual void InitializeType(IExplorerNodeTypeDefinition typeDefinition)
        {
            typeDefinition.DefaultIcon = CKSProperties.icgen.ToBitmap();
            typeDefinition.IsAlwaysLeaf = true;

            if (EnabledExtensionsOptionsPage.GetSetting<bool>(EnabledExtensionsOptions.FileProperties, true))
            {
                typeDefinition.NodePropertiesRequested += NodePropertiesRequested;
            }

            if (EnabledExtensionsOptionsPage.GetSetting<bool>(EnabledExtensionsOptions.FileOperations, true))
            {
                typeDefinition.NodeMenuItemsRequested += NodeMenuItemsRequested;
            }
        }

        /// <summary>
        /// Nodes the properties requested.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ExplorerNodePropertiesRequestedEventArgs" /> instance containing the event data.</param>
        protected virtual void NodePropertiesRequested(object sender, ExplorerNodePropertiesRequestedEventArgs e)
        {
            IExplorerNode fileNode = e.Node;
            FileNodeInfo file = fileNode.Annotations.GetValue<FileNodeInfo>();
            Dictionary<string, string> fileProperties = fileNode.Context.SharePointConnection.ExecuteCommand<FileNodeInfo, Dictionary<string, string>>(FileSharePointCommandIds.GetFilePropertiesCommand, file);
            object propertySource = fileNode.Context.CreatePropertySourceObject(fileProperties);
            e.PropertySources.Add(propertySource);
        }

        /// <summary>
        /// Nodes the menu items requested.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ExplorerNodeMenuItemsRequestedEventArgs" /> instance containing the event data.</param>
        protected virtual void NodeMenuItemsRequested(object sender, ExplorerNodeMenuItemsRequestedEventArgs e)
        {
            IExplorerNode fileNode = e.Node;
            FileNodeInfo file = fileNode.Annotations.GetValue<FileNodeInfo>();

            IMenuItem openFileMenuItem = e.MenuItems.Add(CKSProperties.FileNodeTypeProvider_OpenFile);
            openFileMenuItem.Click += OpenFileMenuItemClick;

            IMenuItem checkOutFileMenuItem = e.MenuItems.Add(CKSProperties.FileNodeTypeProvider_CheckOutFile);
            checkOutFileMenuItem.Click += CheckOutFileMenuItemClick;
            checkOutFileMenuItem.IsEnabled = file.IsCheckedOut == false;

            IMenuItem checkInFileMenuItem = e.MenuItems.Add(CKSProperties.FileNodeTypeProvider_CheckInFile);
            checkInFileMenuItem.Click += CheckInFileMenuItemClick;
            checkInFileMenuItem.IsEnabled = file.IsCheckedOut == true;

            IMenuItem discardCheckOutFileMenuItem = e.MenuItems.Add(CKSProperties.FileNodeTypeProvider_DiscardCheckOut);
            discardCheckOutFileMenuItem.Click += DiscardCheckOutFileMenuItemClick;
            discardCheckOutFileMenuItem.IsEnabled = file.IsCheckedOut == true;

            IMenuItem saveFileMenuItem = e.MenuItems.Add(CKSProperties.FileNodeTypeProvider_SaveFile);
            saveFileMenuItem.Click += SaveFileMenuItemClick;
        }

        /// <summary>
        /// Opens the file menu item click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MenuItemEventArgs" /> instance containing the event data.</param>
        protected virtual void OpenFileMenuItemClick(object sender, MenuItemEventArgs e)
        {
            IExplorerNode fileNode = e.Owner as IExplorerNode;
            var fileNodeInfo = fileNode.Annotations.GetValue<FileNodeInfo>();

            DTEManager.SetStatus(CKSProperties.FileUtilities_OpeningFile);

            string fileContents = fileNode.Context.SharePointConnection.ExecuteCommand<FileNodeInfo, string>(FileSharePointCommandIds.GetFileContentsCommand, fileNodeInfo);
            DTEManager.CreateNewTextFile(fileNodeInfo.Name, fileContents);

            DTEManager.SetStatus(CKSProperties.FileUtilities_FileSuccessfullyOpened);

        }

        /// <summary>
        /// Checks the out file menu item click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MenuItemEventArgs" /> instance containing the event data.</param>
        protected virtual void CheckOutFileMenuItemClick(object sender, MenuItemEventArgs e)
        {
            IExplorerNode parentNode = e.Owner as IExplorerNode;
            var fileNodeInfo = parentNode.Annotations.GetValue<FileNodeInfo>();

            DTEManager.SetStatus(CKSProperties.FileUtilities_CheckingOutFile);

            bool result = parentNode.Context.SharePointConnection.ExecuteCommand<FileNodeInfo, bool>(FileSharePointCommandIds.CheckOutFileCommand, fileNodeInfo);
            if (result)
            {
                parentNode.ParentNode.Refresh();
                DTEManager.SetStatus(CKSProperties.FileUtilities_FileSuccessfullyCheckedOut);
            }
            else
            {
                MessageBox.Show(CKSProperties.FileUtilities_FileCheckOutError, CKSProperties.FileUtilities_FileCheckOutErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        /// <summary>
        /// Checks the in file menu item click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MenuItemEventArgs" /> instance containing the event data.</param>
        protected virtual void CheckInFileMenuItemClick(object sender, MenuItemEventArgs e)
        {
            IExplorerNode parentNode = e.Owner as IExplorerNode;
            var fileNodeInfo = parentNode.Annotations.GetValue<FileNodeInfo>();

            DTEManager.SetStatus(CKSProperties.FileUtilities_CheckingInFile);

            bool result = parentNode.Context.SharePointConnection.ExecuteCommand<FileNodeInfo, bool>(FileSharePointCommandIds.CheckInFileCommand, fileNodeInfo);
            if (result)
            {
                parentNode.ParentNode.Refresh();
                DTEManager.SetStatus(CKSProperties.FileUtilities_FileSuccessfullyCheckedIn);
            }
            else
            {
                MessageBox.Show(CKSProperties.FileUtilities_FileCheckInError, CKSProperties.FileUtilities_FileCheckInErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Discards the check out file menu item click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MenuItemEventArgs" /> instance containing the event data.</param>
        protected virtual void DiscardCheckOutFileMenuItemClick(object sender, MenuItemEventArgs e)
        {
            IExplorerNode parentNode = e.Owner as IExplorerNode;
            var fileNodeInfo = parentNode.Annotations.GetValue<FileNodeInfo>();

            DTEManager.SetStatus(CKSProperties.FileUtilities_DiscardingFileCheckOut);

            bool result = parentNode.Context.SharePointConnection.ExecuteCommand<FileNodeInfo, bool>(FileSharePointCommandIds.DiscardCheckOutCommand, fileNodeInfo);
            if (result)
            {
                parentNode.ParentNode.Refresh();
                DTEManager.SetStatus(CKSProperties.FileUtilities_FileCheckOutSuccessfullyDiscarded);
            }
            else
            {
                MessageBox.Show(CKSProperties.FileUtilities_DiscardFileCheckOutError, CKSProperties.FileUtilities_DiscardFileCheckOutErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Saves the file menu item click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MenuItemEventArgs" /> instance containing the event data.</param>
        protected virtual void SaveFileMenuItemClick(object sender, MenuItemEventArgs e)
        {
            IExplorerNode parentNode = e.Owner as IExplorerNode;
            var fileNodeInfo = parentNode.Annotations.GetValue<FileNodeInfo>();

            DTEManager.SetStatus(CKSProperties.FileUtilities_SavingFile);

            Document file = DTEManager.DTE.ActiveDocument;
            TextSelection selection = file.Selection as TextSelection;
            selection.SelectAll();
            fileNodeInfo.Contents = selection.Text;
            selection.StartOfDocument();

            bool result = parentNode.Context.SharePointConnection.ExecuteCommand<FileNodeInfo, bool>(FileSharePointCommandIds.SaveFileCommand, fileNodeInfo);
            if (result)
            {
                DTEManager.SetStatus(CKSProperties.FileUtilities_FileSuccessfullySaved);
            }
            else
            {
                MessageBox.Show(CKSProperties.FileUtilities_FileSaveError, CKSProperties.FileUtilities_FileSaveErrorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        /// <summary>
        /// Creates the files nodes.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        public static void CreateFilesNodes(IExplorerNode parentNode)
        {
            DTEManager.SetStatus(CKSProperties.FileNodeTypeProvider_RetrievingFolders);
            FolderNodeInfo[] folders = parentNode.Context.SharePointConnection.ExecuteCommand<FolderNodeInfo, FolderNodeInfo[]>(FileSharePointCommandIds.GetFoldersCommand, parentNode.Annotations.GetValue<FolderNodeInfo>());
            DTEManager.SetStatus(CKSProperties.FileNodeTypeProvider_RetrievingFiles);
            FileNodeInfo[] files = parentNode.Context.SharePointConnection.ExecuteCommand<FolderNodeInfo, FileNodeInfo[]>(FileSharePointCommandIds.GetFilesCommand, parentNode.Annotations.GetValue<FolderNodeInfo>());

            if (folders != null)
            {
                foreach (FolderNodeInfo folder in folders)
                {
                    var annotations = new Dictionary<object, object>
                    {
                        { typeof(FolderNodeInfo), folder }
                    };

                    string nodeTypeId = ExplorerNodeIds.FolderNode;

                    IExplorerNode fileNode = parentNode.ChildNodes.Add(nodeTypeId, folder.Name, annotations);
                }
            }

            if (files != null)
            {
                foreach (FileNodeInfo file in files)
                {
                    var annotations = new Dictionary<object, object>
                    {
                        { typeof(FileNodeInfo), file }
                    };

                    string nodeTypeId = ExplorerNodeIds.FileNode;

                    IExplorerNode fileNode = parentNode.ChildNodes.Add(nodeTypeId, file.Name, annotations);
                    fileNode.DoubleClick += delegate (object sender, ExplorerNodeEventArgs e)
                    {
                        var fileNodeInfo = e.Node.Annotations.GetValue<FileNodeInfo>();

                        DTEManager.SetStatus(CKSProperties.FileUtilities_OpeningFile);

                        string fileContents = fileNode.Context.SharePointConnection.ExecuteCommand<FileNodeInfo, string>(FileSharePointCommandIds.GetFileContentsCommand, fileNodeInfo);
                        DTEManager.CreateNewTextFile(fileNodeInfo.Name, fileContents);

                        DTEManager.SetStatus(CKSProperties.FileUtilities_FileSuccessfullyOpened);

                    };
                    SetExplorerNodeIcon(file, fileNode);
                }
            }

            DTEManager.SetStatus(String.Empty);
        }

        /// <summary>
        /// Sets the explorer node icon.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="fileNode">The file node.</param>
        private static void SetExplorerNodeIcon(FileNodeInfo file, IExplorerNode fileNode)
        {
            string extension = Path.GetExtension(file.Name.ToLower()).TrimStart('.');

            if (file.IsCheckedOut)
            {
                switch (extension)
                {
                    case "css":
                        fileNode.Icon = CKSProperties.co_iccss.ToBitmap();
                        break;
                    case "gif":
                    case "jpg":
                    case "jpeg":
                        fileNode.Icon = CKSProperties.co_icjpg.ToBitmap();
                        break;
                    case "js":
                        fileNode.Icon = CKSProperties.co_icjs.ToBitmap();
                        break;
                    case "png":
                        fileNode.Icon = CKSProperties.co_icpng.ToBitmap();
                        break;
                    case "xsl":
                        fileNode.Icon = CKSProperties.co_icxsl.ToBitmap();
                        break;
                    case "xaml":
                        fileNode.Icon = CKSProperties.co_icxaml.ToBitmap();
                        break;
                    default:
                        fileNode.Icon = CKSProperties.co_icgen.ToBitmap();
                        break;
                }
            }
            else
            {
                switch (extension)
                {
                    case "css":
                        fileNode.Icon = CKSProperties.iccss.ToBitmap();
                        break;
                    case "gif":
                    case "jpg":
                    case "jpeg":
                        fileNode.Icon = CKSProperties.icjpg.ToBitmap();
                        break;
                    case "js":
                        fileNode.Icon = CKSProperties.icjs.ToBitmap();
                        break;
                    case "png":
                        fileNode.Icon = CKSProperties.icpng.ToBitmap();
                        break;
                    case "xsl":
                        fileNode.Icon = CKSProperties.icxsl.ToBitmap();
                        break;
                    case "xaml":
                        fileNode.Icon = CKSProperties.icxaml.ToBitmap();
                        break;
                    default:
                        fileNode.Icon = CKSProperties.icgen.ToBitmap();
                        break;
                }
            }
        }

        #endregion
    }
}
