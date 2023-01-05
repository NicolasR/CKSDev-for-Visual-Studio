using CKS.Dev.VisualStudio.SharePoint.Commands.Info;
using Microsoft.VisualStudio.SharePoint.Commands;
using System;
using System.Collections.Generic;

namespace CKS.Dev.VisualStudio.SharePoint.Commands
{
    class FieldSharePointCommands
    {
        [SharePointCommand(FieldSharePointCommandIds.GetProperties)]
        public static Dictionary<string, string> GetProperties(ISharePointCommandContext context, FieldNodeInfo field)
        {
            Dictionary<string, string> properties = null;

            if (field.ListId == Guid.Empty)
            {
                properties = SharePointCommandServices.GetProperties(context.Web.AvailableContentTypes[field.ContentTypeName].Fields[field.Id]);
            }
            else
            {
                properties = SharePointCommandServices.GetProperties(context.Web.Lists[field.ListId].Fields[field.Id]);
            }

            return properties;
        }
    }
}
