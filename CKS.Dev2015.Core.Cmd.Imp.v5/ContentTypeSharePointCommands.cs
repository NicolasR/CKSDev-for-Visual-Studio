using CKS.Dev2015.Core.Cmd.Imp.v5.Properties;
using CKS.Dev2015.VisualStudio.SharePoint.Commands.Extensions;
using CKS.Dev2015.VisualStudio.SharePoint.Commands.Info;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Publishing;
using Microsoft.SharePoint.Publishing.Fields;
using Microsoft.SharePoint.Publishing.WebControls;
using Microsoft.SharePoint.WebControls;
using Microsoft.VisualStudio.SharePoint.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace CKS.Dev2015.VisualStudio.SharePoint.Commands
{
    /// <summary>
    /// The content type commands.
    /// </summary>
    class ContentTypeSharePointCommands
    {
        #region Methods

        /// <summary>
        /// Get the content type Id for the supplied name
        /// </summary>
        /// <param name="context">The ISharePointCommandContext.</param>
        /// <param name="name">The content type name.</param>
        /// <returns>The Id of the content type.</returns>
        [SharePointCommand(ContentTypeSharePointCommandIds.GetContentTypeID)]
        private static string GetContentTypeID(ISharePointCommandContext context, string name)
        {
            SPContentType type = context.Web.AvailableContentTypes[name];
            if (type == null)
            {
                context.Logger.WriteLine(String.Format(Resources.ContentTypeSharePointCommands_GetContentTypeIDException, name), LogCategory.Error);
                return String.Empty;
            }
            return type.Id.ToString();
        }

        /// <summary>
        /// Checks whether the content type is one of the built in ones.
        /// </summary>
        /// <param name="context">The ISharePointCommandContext.</param>
        /// <param name="contentTypeName">The name of the content type.</param>
        /// <returns>True if the content type is a built in one.</returns>
        [SharePointCommand(ContentTypeSharePointCommandIds.IsBuiltInContentType)]
        private static bool IsBuiltInContentType(ISharePointCommandContext context, string contentTypeName)
        {
            SPContentType contentType = context.Web.AvailableContentTypes[contentTypeName];

            if (contentType.Id == SPBuiltInContentTypeId.AdminTask)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.Announcement)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.BasicPage)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.BlogComment)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.BlogPost)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.CallTracking)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.Contact)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.Discussion)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.Document)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.DocumentSet)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.DocumentWorkflowItem)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.DomainGroup)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.DublinCoreName)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.Event)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.FarEastContact)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.Folder)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.GbwCirculationCTName)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.GbwOfficialNoticeCTName)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.HealthReport)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.HealthRuleDefinition)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.Holiday)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.IMEDictionaryItem)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.Issue)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.Item)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.Link)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.LinkToDocument)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.MasterPage)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.Message)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.ODCDocument)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.Person)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.Picture)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.Resource)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.ResourceGroup)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.ResourceReservation)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.RootOfList)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.Schedule)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.ScheduleAndResourceReservation)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.SharePointGroup)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.SummaryTask)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.System)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.Task)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.Timecard)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.UDCDocument)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.UntypedDocument)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.WebPartPage)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.WhatsNew)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.Whereabouts)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.WikiDocument)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.WorkflowHistory)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.WorkflowTask)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.XMLDocument)
            {
                return true;
            }
            if (contentType.Id == SPBuiltInContentTypeId.XSLStyle)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get the import properties for the supplied content type.
        /// </summary>
        /// <param name="context">The ISharePointCommandContext.</param>
        /// <param name="contentTypeName">The name of the content type.</param>
        /// <returns>The import properties.</returns>
        [SharePointCommand(ContentTypeSharePointCommandIds.GetContentTypeImportProperties)]
        private static ContentTypeInfo GetContentTypeImportProperties(ISharePointCommandContext context,
            string contentTypeName)
        {
            SPContentType contentType = context.Web.AvailableContentTypes[contentTypeName];
            if (contentType == null)
            {
                context.Logger.WriteLine(String.Format(Resources.ContentTypeSharePointCommands_GetContentTypePropertiesException, contentTypeName), LogCategory.Error);
                return null;
            }

            ContentTypeInfo info = new ContentTypeInfo();

            if (contentType != null)
            {
                info.Id = contentType.Id.ToString();
                info.Name = contentType.Name;
                info.Description = contentType.Description;
                info.Group = contentType.Group;
                info.Sealed = contentType.Sealed;
                info.Hidden = contentType.Hidden;
                info.ReadOnly = contentType.ReadOnly;

                SPContentType parentContentType = contentType.Parent;

                List<ContentTypeInfo.FieldRef> fields = new List<ContentTypeInfo.FieldRef>();
                foreach (SPField field in contentType.Fields)
                {
                    // check if the field is in the parent content type, skip it...
                    if (parentContentType.Fields.ContainsField(field.StaticName))
                    {
                        continue;
                    }

                    ContentTypeInfo.FieldRef fieldRef = new ContentTypeInfo.FieldRef();
                    fieldRef.Id = field.Id.ToString();
                    fieldRef.Name = field.StaticName;
                    fieldRef.DisplayName = field.Title;

                    fields.Add(fieldRef);
                }

                if (fields.Count > 0)
                {
                    info.FieldRefs = fields.ToArray();
                }

                if (!String.IsNullOrEmpty(contentType.DocumentTemplate))
                {
                    info.DocumentTemplate = contentType.DocumentTemplate;
                }

                if (contentType.XmlDocuments != null && contentType.XmlDocuments.Count > 0)
                {
                    List<string> xmlDocs = new List<string>();
                    foreach (string doc in contentType.XmlDocuments)
                    {
                        xmlDocs.Add(doc);
                    }
                    info.XmlDocuments = xmlDocs.ToArray();
                }
            }

            return info;
        }

        /// <summary>
        /// Gets the content type groups.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The content type groups.</returns>
        [SharePointCommand(ContentTypeSharePointCommandIds.GetContentTypeGroups)]
        private static string[] GetContentTypeGroups(ISharePointCommandContext context)
        {
            SPContentTypeCollection contentTypes = context.Web.AvailableContentTypes;
            IEnumerable<string> allContentTypeGroups = (from SPContentType contentType
                                                        in contentTypes
                                                        select contentType.Group);

            string[] contentTypeGroups = allContentTypeGroups.Distinct().ToArray();

            return contentTypeGroups;
        }

        /// <summary>
        /// Gets the content types from group.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <returns>The content type node infos for the group.</returns>
        [SharePointCommand(ContentTypeSharePointCommandIds.GetContentTypesFromGroup)]
        private static ContentTypeNodeInfo[] GetContentTypesFromGroup(ISharePointCommandContext context, string groupName)
        {
            SPContentTypeCollection contentTypes = context.Web.AvailableContentTypes;
            ContentTypeNodeInfo[] contentTypesFromGroup = (from SPContentType contentType
                                                           in contentTypes
                                                           where contentType.Group == groupName
                                                           select new ContentTypeNodeInfo { Name = contentType.Name }).ToArray();

            return contentTypesFromGroup;
        }

        /// <summary>
        /// Creates the page layout.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="contentTypeName">Name of the content type.</param>
        /// <returns></returns>
        [SharePointCommand(ContentTypeSharePointCommandIds.CreatePageLayoutCommand)]
        private static string CreatePageLayout(ISharePointCommandContext context, string contentTypeName)
        {
            string pageLayout = String.Empty;

            try
            {
                SPContentType contentType = context.Web.ContentTypes[contentTypeName];

                Dictionary<string, TagInfo> assemblies = new Dictionary<string, TagInfo>();
                List<FieldInfo> fields = new List<FieldInfo>(contentType.Fields.Count);
                int tagCount = 0; // counter for custom tags

                foreach (SPField field in contentType.Fields)
                {
                    if (field.Hidden)
                    {
                        continue;
                    }

                    Type t = field.GetFieldRenderingControlType();
                    TagInfo tag = null;
                    if (assemblies.ContainsKey(t.Namespace))
                    {
                        tag = assemblies[t.Namespace];
                    }
                    else
                    {
                        if (BuiltInTags.ContainsKey(t.Namespace))
                        {
                            tag = BuiltInTags[t.Namespace];
                        }
                        else
                        {
                            tag = new TagInfo("CustomTag_" + tagCount++, t.Assembly.FullName, t.Namespace);
                        }
                        assemblies.Add(t.Namespace, tag);
                    }

                    fields.Add(new FieldInfo(field.InternalName, field.Title, field.Description, String.Format("<{{0}}:{0} FieldName=\"{1}\" InputFieldLabel=\"{2}\" runat=\"server\"></{{0}}:{0}>", t.Name, field.InternalName, field.Title), tag, field));
                }

                pageLayout = WritePageLayoutContents(assemblies, fields);
            }
            catch { }

            return pageLayout;
        }

        /// <summary>
        /// Determines whether [is publishing content type] [the specified context].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="contentTypeName">Name of the content type.</param>
        /// <returns>
        ///   <c>true</c> if [is publishing content type] [the specified context]; otherwise, <c>false</c>.
        /// </returns>
        [SharePointCommand(ContentTypeSharePointCommandIds.IsPublishingContentTypeCommand)]
        private static bool IsPublishingContentType(ISharePointCommandContext context, string contentTypeName)
        {
            bool isPublishingContentType = false;

            SPContentType contentType = context.Web.ContentTypes[contentTypeName];
            isPublishingContentType = contentType.Id.IsChildOf(ContentTypeId.Page);

            return isPublishingContentType;
        }

        #region Helper methods

        /// <summary>
        /// The sp namespace
        /// </summary>
        internal static XNamespace sp = XNamespace.Get("http://schemas.microsoft.com/sharepoint/");

        /// <summary>
        /// The tag prefix mappings
        /// </summary>
        internal static readonly Dictionary<String, String> TagPrefixMappings = new Dictionary<string, string>
        {
            { "Microsoft.SharePoint.WebControls", "SharePointWebControls" },
            { "Microsoft.SharePoint.WebPartPages", "WebPartPages" },
            { "Microsoft.SharePoint.Publishing.WebControls", "PublishingWebControls" },
            { "Microsoft.SharePoint.Publishing.Navigation", "PublishingNavigation" }
        };

        /// <summary>
        /// The built in tags
        /// </summary>
        internal static readonly Dictionary<string, TagInfo> BuiltInTags = new Dictionary<string, TagInfo>
        {
            { typeof(FormField).Namespace, new TagInfo("wss", typeof(FormField).Assembly.FullName, typeof(FormField).Namespace) },
            { typeof(BaseRichFieldType).Namespace, new TagInfo("cmsf", typeof(BaseRichFieldType).Assembly.FullName, typeof(BaseRichFieldType).Namespace) },
            { typeof(BaseRichField).Namespace, new TagInfo("cmsc", typeof(BaseRichField).Assembly.FullName, typeof(BaseRichField).Namespace) }
        };

        /// <summary>
        /// The custom tag prefix mappings
        /// </summary>
        internal static Dictionary<String, String> CustomTagPrefixMappings = new Dictionary<String, String>();

        /// <summary>
        /// Contains a list of Site Columns which values will be excluded of the export.
        /// This is either because the value of the Site Column is read-only or because it cannot be set for another reason
        /// </summary>
        private static readonly List<Guid> skipFields = new List<Guid>()
        {
          SPBuiltInFieldId.FileLeafRef,
          SPBuiltInFieldId.Title
        };

        /// <summary>
        /// Writes the page layout contents.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        internal static string WritePageLayoutContents(Dictionary<string, TagInfo> assemblies, List<FieldInfo> fields)
        {
            StringBuilder pageLayout = new StringBuilder();

            pageLayout.AppendLine("<%@ Page language=\"C#\" Inherits=\"Microsoft.SharePoint.Publishing.PublishingLayoutPage,Microsoft.SharePoint.Publishing,Version=14.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c\" %>");

            // assembly references
            foreach (KeyValuePair<string, TagInfo> assembly in assemblies)
            {
                pageLayout.AppendLine(assembly.Value.ToString());
            }

            // PlaceHolderAdditionalPageHead
            pageLayout.AppendLine("<asp:Content ContentPlaceholderID=\"PlaceHolderAdditionalPageHead\" runat=\"server\">");
            pageLayout.AppendLine("  <SharePointWebControls:CssRegistration name=\"<% $SPUrl:~sitecollection/Style Library/~language/Core Styles/page-layouts-21.css %>\" runat=\"server\"/>");
            pageLayout.AppendLine("  <PublishingWebControls:EditModePanel runat=\"server\">");
            pageLayout.AppendLine("    <!-- Styles for edit mode only-->");
            pageLayout.AppendLine("    <SharePointWebControls:CssRegistration name=\"<% $SPUrl:~sitecollection/Style Library/~language/Core Styles/edit-mode-21.css %>\"");
            pageLayout.AppendLine("      After=\"<% $SPUrl:~sitecollection/Style Library/~language/Core Styles/page-layouts-21.css %>\" runat=\"server\"/>");
            pageLayout.AppendLine("    </PublishingWebControls:EditModePanel>");
            pageLayout.AppendLine("  <SharePointWebControls:CssRegistration name=\"<% $SPUrl:~sitecollection/Style Library/~language/Core Styles/rca.css %>\" runat=\"server\"/>");
            pageLayout.AppendLine("  <SharePointWebControls:FieldValue id=\"PageStylesField\" FieldName=\"HeaderStyleDefinitions\" runat=\"server\"/>");
            pageLayout.AppendLine("</asp:Content>");

            // put title in the PlaceHolderPageTitle
            pageLayout.AppendLine("<asp:Content ContentPlaceholderID=\"PlaceHolderPageTitle\" runat=\"server\">");
            pageLayout.AppendLine("  <SharePointWebControls:FieldValue id=\"PageTitle\" FieldName=\"Title\" runat=\"server\"/>");
            pageLayout.AppendLine("</asp:Content>");

            // put title in the PlaceHolderPageTitleInTitleArea
            pageLayout.AppendLine("<asp:Content ContentPlaceholderID=\"PlaceHolderPageTitleInTitleArea\" runat=\"server\">");
            pageLayout.AppendLine("  <SharePointWebControls:FieldValue FieldName=\"Title\" runat=\"server\"/>");
            pageLayout.AppendLine("</asp:Content>");

            // Breadcrumbs
            pageLayout.AppendLine("<asp:Content ContentPlaceHolderId=\"PlaceHolderTitleBreadcrumb\" runat=\"server\">");
            pageLayout.AppendLine("  <SharePointWebControls:ListSiteMapPath runat=\"server\" SiteMapProviders=\"CurrentNavigation\" RenderCurrentNodeAsLink=\"false\" PathSeparator=\"\" NodeStyle-CssClass=\"s4-breadcrumbNode\" CurrentNodeStyle-CssClass=\"s4-breadcrumbCurrentNode\" RootNodeStyle-CssClass=\"s4-breadcrumbRootNode\" NodeImageOffsetX=0 NodeImageOffsetY=321 NodeImageWidth=16 NodeImageHeight=16 NodeImageUrl=\"/_layouts/images/fgimg.png\" HideInteriorRootNodes=\"true\" SkipLinkText=\"\" />");
            pageLayout.AppendLine("</asp:Content>");

            // Main content
            pageLayout.AppendLine();
            pageLayout.AppendLine("<asp:Content ContentPlaceHolderId=\"PlaceHolderMain\" runat=\"server\">");

            // view fields
            foreach (FieldInfo field in fields)
            {
                pageLayout.AppendFormat("  <SharePointWebControls:FieldValue FieldName=\"{0}\" runat=\"server\"/>" + Environment.NewLine, field.Name);
            }

            // edit panel
            pageLayout.AppendLine();
            pageLayout.AppendLine("  <PublishingWebControls:EditModePanel runat=\"server\" id=\"editmodepanel\" CssClass=\"editPanel\">");

            // edit fields
            foreach (FieldInfo field in fields)
            {
                if (!field.Field.ReadOnlyField)
                {
                    pageLayout.AppendFormat("    " + field.Template + Environment.NewLine, field.Tag.TagName);
                }
            }

            pageLayout.AppendLine("  </PublishingWebControls:EditModePanel>");

            pageLayout.AppendLine("</asp:Content>"); // end Main content

            return pageLayout.ToString();
        }

        /// <summary>
        /// Safes the name of the content type.
        /// </summary>
        /// <param name="contentTypeName">Name of the content type.</param>
        /// <returns></returns>
        internal static string SafeContentTypeName(string contentTypeName)
        {
            string safeName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(contentTypeName);
            safeName = new Regex(@"[^\w]").Replace(safeName, "");
            return safeName;
        }

        #endregion

        #endregion
    }
}
