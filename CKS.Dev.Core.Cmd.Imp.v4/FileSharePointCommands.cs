using CKS.Dev.Core.Cmd.Imp.v4.Properties;
using CKS.Dev.VisualStudio.SharePoint.Commands.Common;
using CKS.Dev.VisualStudio.SharePoint.Commands.Common.ExtensionMethods;
using CKS.Dev.VisualStudio.SharePoint.Commands.Info;
using Microsoft.SharePoint;
using Microsoft.VisualStudio.SharePoint.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace CKS.Dev.VisualStudio.SharePoint.Commands
{
    internal static class FileSharePointCommands
    {
        [SharePointCommand(FileSharePointCommandIds.GetFileContentsCommand)]
        private static string GetFileContents(ISharePointCommandContext context, FileNodeInfo fileNodeInfo)
        {
            string fileContents = null;
            try
            {
                SPFile f = context.Web.GetFile(fileNodeInfo.ServerRelativeUrl);
                fileContents = Encoding.UTF8.GetString(f.OpenBinary());
            }
            catch (Exception ex)
            {
                context.Logger.WriteLine(String.Format(Resources.FileSharePointCommands_GetContentsException,
                    fileNodeInfo.ServerRelativeUrl,
                    ex.Message,
                    Environment.NewLine,
                    ex.StackTrace), LogCategory.Error);
            }

            return fileContents;
        }

        [SharePointCommand(FileSharePointCommandIds.CheckOutFileCommand)]
        private static bool CheckOutFile(ISharePointCommandContext context, FileNodeInfo fileNodeInfo)
        {
            bool result = false;

            try
            {
                SPFile f = context.Web.GetFile(fileNodeInfo.ServerRelativeUrl);
                f.CheckOut();
                result = true;
            }
            catch (Exception ex)
            {
                context.Logger.WriteLine(String.Format(Resources.FileSharePointCommands_CheckOutException,
                    fileNodeInfo.ServerRelativeUrl,
                    ex.Message,
                    Environment.NewLine,
                    ex.StackTrace), LogCategory.Error);
            }

            return result;
        }

        [SharePointCommand(FileSharePointCommandIds.DiscardCheckOutCommand)]
        private static bool DiscardCheckOut(ISharePointCommandContext context, FileNodeInfo fileNodeInfo)
        {
            bool result = false;

            try
            {
                SPFile f = context.Web.GetFile(fileNodeInfo.ServerRelativeUrl);
                f.UndoCheckOut();
                result = true;
            }
            catch (Exception ex)
            {
                context.Logger.WriteLine(String.Format(Resources.FileSharePointCommands_DiscardCheckOutException,
                    fileNodeInfo.ServerRelativeUrl,
                    ex.Message,
                    Environment.NewLine,
                    ex.StackTrace), LogCategory.Error);
            }

            return result;
        }

        [SharePointCommand(FileSharePointCommandIds.CheckInFileCommand)]
        private static bool CheckInFile(ISharePointCommandContext context, FileNodeInfo fileNodeInfo)
        {
            bool result = false;

            try
            {
                SPFile f = context.Web.GetFile(fileNodeInfo.ServerRelativeUrl);
                f.CheckIn(String.Empty);
                result = true;
            }
            catch (Exception ex)
            {
                context.Logger.WriteLine(String.Format(Resources.FileSharePointCommands_CheckInException,
                          fileNodeInfo.ServerRelativeUrl,
                          ex.Message,
                          Environment.NewLine,
                          ex.StackTrace), LogCategory.Error);
            }

            return result;
        }

        [SharePointCommand(FileSharePointCommandIds.SaveFileCommand)]
        private static bool SaveFile(ISharePointCommandContext context, FileNodeInfo fileNodeInfo)
        {
            bool result = false;

            try
            {
                SPFile f = context.Web.GetFile(fileNodeInfo.ServerRelativeUrl);
                f.SaveBinary(Encoding.UTF8.GetBytes(fileNodeInfo.Contents));
                result = true;
            }
            catch (Exception ex)
            {
                context.Logger.WriteLine(String.Format(Resources.FileSharePointCommands_SaveException,
                          fileNodeInfo.ServerRelativeUrl,
                          ex.Message,
                          Environment.NewLine,
                          ex.StackTrace), LogCategory.Error);
            }

            return result;
        }

        [SharePointCommand(FileSharePointCommandIds.GetFilesCommand)]
        private static FileNodeInfo[] GetFiles(ISharePointCommandContext context, FolderNodeInfo folderNodeInfo)
        {
            List<FileNodeInfo> nodeInfos = new List<FileNodeInfo>();
            try
            {
                SPList styleLibrary = context.Web.GetList(Utilities.CombineUrl(context.Web.ServerRelativeUrl, "Style%20Library"));
                SPFolder folder = styleLibrary.RootFolder;
                if (folderNodeInfo != null)
                {
                    folder = context.Web.GetFolder(folderNodeInfo.Url);
                }

                SPFileCollection files = folder.Files;
                if (files != null)
                {
                    nodeInfos = files.ToFileNodeInfo();
                }
            }
            catch (Exception ex)
            {
                context.Logger.WriteLine(String.Format(Resources.FileSharePointCommands_RetrieveFilesException,
                          ex.Message,
                          Environment.NewLine,
                          ex.StackTrace), LogCategory.Error);
            }

            return nodeInfos.ToArray();
        }

        [SharePointCommand(FileSharePointCommandIds.GetFoldersCommand)]
        private static FolderNodeInfo[] GetFolders(ISharePointCommandContext context, FolderNodeInfo folderNodeInfo)
        {
            List<FolderNodeInfo> nodeInfos = new List<FolderNodeInfo>();
            try
            {
                SPList styleLibrary = context.Web.GetList(Utilities.CombineUrl(context.Web.ServerRelativeUrl, "Style%20Library"));
                SPFolder folder = styleLibrary.RootFolder;
                if (folderNodeInfo != null)
                {
                    folder = context.Web.GetFolder(folderNodeInfo.Url);
                }

                SPFolderCollection subfolders = folder.SubFolders;
                if (subfolders != null)
                {
                    nodeInfos = subfolders.ToFolderNodeInfo();
                }
            }
            catch (Exception ex)
            {
                context.Logger.WriteLine(String.Format(Resources.FileSharePointCommands_RetrieveFoldersException,
                          ex.Message,
                          Environment.NewLine,
                          ex.StackTrace), LogCategory.Error);
            }

            return nodeInfos.ToArray();
        }

        [SharePointCommand(FileSharePointCommandIds.GetFilePropertiesCommand)]
        private static Dictionary<string, string> GetFileProperties(ISharePointCommandContext context, FileNodeInfo fileNodeInfo)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();

            try
            {
                SPFile file = context.Web.GetFile(fileNodeInfo.ServerRelativeUrl);
                if (file != null)
                {
                    properties = SharePointCommandServices.GetProperties(file.Item);
                }
            }
            catch { }

            return properties;
        }
    }
}
