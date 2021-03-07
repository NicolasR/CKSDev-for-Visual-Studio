using CKS.Dev.Core.Cmd.Imp.v5.Properties;
using CKS.Dev.VisualStudio.SharePoint.Commands.Common.ExtensionMethods;
using CKS.Dev.VisualStudio.SharePoint.Commands.Info;
using Microsoft.SharePoint;
using Microsoft.VisualStudio.SharePoint.Commands;
using System;
using System.Collections.Generic;

namespace CKS.Dev.VisualStudio.SharePoint.Commands
{
    class MasterPageGallerySharePointCommands
    {
        [SharePointCommand(MasterPageGallerySharePointCommandIds.GetMasterPagesAndPageLayoutsCommand)]
        private static FileNodeInfo[] GetMasterPagesAndPageLayouts(ISharePointCommandContext context)
        {
            List<FileNodeInfo> nodeInfos = new List<FileNodeInfo>();
            try
            {
                context.Logger.WriteLine(Resources.MasterPageGallerySharePointCommands_TryingToRetrieveAvailableMasterPagesAndPageLayouts, LogCategory.Status);

                SPListItemCollection masterPagesAndPageLayouts = context.Web.GetCatalog(SPListTemplateType.MasterPageCatalog).GetItems(
                    new SPQuery
                    {
                        ViewXml = "<View />"
                    }
                );
                nodeInfos = masterPagesAndPageLayouts.ToFileNodeInfo();

                context.Logger.WriteLine(Resources.MasterPageGallerySharePointCommands_MasterPagesAndPageLayoutsSuccessfullyRetrieved, LogCategory.Status);
            }
            catch (Exception ex)
            {
                context.Logger.WriteLine(String.Format(Resources.MasterPageGallerySharePointCommands_RetrievingException,
                          ex.Message,
                          Environment.NewLine,
                          ex.StackTrace), LogCategory.Error);
            }

            return nodeInfos.ToArray();
        }

        [SharePointCommand(MasterPageGallerySharePointCommandIds.GetMasterPagesOrPageLayoutPropertiesCommand)]
        private static Dictionary<string, string> GetMasterPageOrPageLayoutProperties(ISharePointCommandContext context, FileNodeInfo fileNodeInfo)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();

            try
            {
                SPList masterPageGallery = context.Web.GetCatalog(SPListTemplateType.MasterPageCatalog);
                SPListItem masterPageOrPageLayout = masterPageGallery.Items[fileNodeInfo.UniqueId];

                properties = SharePointCommandServices.GetProperties(masterPageOrPageLayout);
            }
            catch { }

            return properties;
        }
    }
}
