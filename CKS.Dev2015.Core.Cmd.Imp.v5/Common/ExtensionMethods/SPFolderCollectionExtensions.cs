using CKS.Dev2015.VisualStudio.SharePoint.Commands.Info;
using Microsoft.SharePoint;
using System.Collections.Generic;

namespace CKS.Dev2015.VisualStudio.SharePoint.Commands.Common.ExtensionMethods

{
    internal static class SPFolderCollectionExtensions
    {
        internal static List<FolderNodeInfo> ToFolderNodeInfo(this SPFolderCollection folders)
        {
            List<FolderNodeInfo> nodeInfos = new List<FolderNodeInfo>();

            foreach (SPFolder folder in folders)
            {
                FolderNodeInfo nodeInfo = new FolderNodeInfo
                {
                    Name = folder.Name,
                    Url = folder.Url
                };
                nodeInfos.Add(nodeInfo);
            }

            return nodeInfos;
        }
    }
}
