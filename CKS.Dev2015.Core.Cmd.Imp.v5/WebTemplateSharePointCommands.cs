using CKS.Dev2015.Core.Cmd.Imp.v5.Properties;
using CKS.Dev2015.VisualStudio.SharePoint.Commands.Info;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.VisualStudio.SharePoint.Commands;
using System;
using System.Collections.Generic;

namespace CKS.Dev2015.VisualStudio.SharePoint.Commands
{
    class WebTemplateSharePointCommands
    {
        #region Methods

        /// <summary>
        /// Gets data for each Web template categories on the SharePoint site, and returns an array of 
        /// serializable objects that contain the data.
        /// </summary>
        /// <param name="context">The command context</param>
        /// <returns>The categories</returns>
        [SharePointCommand(WebTemplateCollectionSharePointCommandIds.GetWebTemplateCategories)]
        private static string[] GetWebTemplateCategories(ISharePointCommandContext context)
        {
            context.Logger.WriteLine(Resources.WebTemplateSharePointCommands_TryingToRetrieveAvailableCategories, LogCategory.Status);

            List<string> categories = new List<string>();

            try
            {
                SPWebTemplateCollection webTemplates = context.Web.GetAvailableWebTemplates((uint)context.Web.Locale.LCID, true);

                foreach (SPWebTemplate item in webTemplates)
                {
                    if (!categories.Contains(item.DisplayCategory))
                    {
                        categories.Add(item.DisplayCategory);
                    }
                }
            }
            catch (Exception ex)
            {
                context.Logger.WriteLine(String.Format(Resources.WebTemplateSharePointCommands_CategoriesRetrievingException,
                          ex.Message,
                          Environment.NewLine,
                          ex.StackTrace), LogCategory.Error);
            }

            return categories.ToArray();
        }

        /// <summary>
        /// Gets data for each Web templates on the SharePoint site, and returns an array of 
        /// serializable objects that contain the data.
        /// </summary>
        /// <param name="context">The command context</param>
        /// <param name="category">The category</param>
        /// <returns>The web template infos</returns>
        [SharePointCommand(WebTemplateCollectionSharePointCommandIds.GetAvailableWebTemplatesByCategory)]
        private static WebTemplateInfo[] GetAvailableWebTemplatesByCategory(ISharePointCommandContext context, string category)
        {
            context.Logger.WriteLine(Resources.WebTemplateSharePointCommands_TryingToRetrieveAvailableWebTemplates, LogCategory.Status);

            List<WebTemplateInfo> webTemplateInfos = new List<WebTemplateInfo>();

            try
            {
                SPWebTemplateCollection webTemplates = context.Web.GetAvailableWebTemplates((uint)context.Web.Locale.LCID, true);

                foreach (SPWebTemplate item in webTemplates)
                {
                    if (item.DisplayCategory == category)
                    {
                        WebTemplateInfo info = new WebTemplateInfo();

                        info.Id = item.ID;
                        info.ImageUrl = item.ImageUrl;
                        info.Name = item.Name;
                        info.Description = item.Description;
                        info.DisplayCategory = item.DisplayCategory;
                        info.IsCustomTemplate = item.IsCustomTemplate;
                        info.IsHidden = item.IsHidden;
                        info.IsRootWebOnly = item.IsRootWebOnly;
                        info.IsSubWebOnly = item.IsSubWebOnly;
                        info.Lcid = item.Lcid;
                        info.Title = item.Title;
                        info.SetupPath = typeof(SPWebTemplate).InvokeMember("SetupPath", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetProperty, null, item, null) as string;

                        webTemplateInfos.Add(info);
                    }
                }
            }
            catch (Exception ex)
            {
                context.Logger.WriteLine(String.Format(Resources.WebTemplateSharePointCommands_RetrievingException,
                          ex.Message,
                          Environment.NewLine,
                          ex.StackTrace), LogCategory.Error);
            }

            return webTemplateInfos.ToArray();
        }

        /// <summary>
        /// Gets additional property data for a specific web template.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="nodeInfo">The node info</param>
        /// <returns>The properties</returns>
        [SharePointCommand(WebTemplateSharePointCommandIds.GetWebTemplateProperties)]
        private static Dictionary<string, string> GetWebTemplateProperties(ISharePointCommandContext context,
            WebTemplateInfo nodeInfo)
        {
            SPWebTemplate template = context.Web.GetAvailableWebTemplates((uint)context.Web.Locale.LCID, true)[nodeInfo.Name];

            return SharePointCommandServices.GetProperties(template);
        }

        /// <summary>
        /// Gets the web templates.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The web template infos</returns>
        [SharePointCommand(WebTemplateCollectionSharePointCommandIds.GetWebTemplates)]
        private static WebTemplateInfo[] GetWebTemplates(ISharePointCommandContext context)
        {
            context.Logger.WriteLine(Resources.WebTemplateSharePointCommands_TryingToRetrieveAvailableWebTemplates, LogCategory.Status);

            List<WebTemplateInfo> webTemplateInfos = new List<WebTemplateInfo>();

            try
            {
                SPSite caSite = null;
                SPWeb rootWeb = null;

                if (context.Site == null)
                {
                    //Do this as from item templates the site is always null
                    SPAdministrationWebApplication caWebApp = SPAdministrationWebApplication.Local;
                    caSite = caWebApp.Sites[0];
                    rootWeb = caSite.RootWeb;
                }
                else
                {
                    caSite = context.Site;
                    rootWeb = context.Web;
                }

                SPWebTemplateCollection webTemplates = caSite.GetWebTemplates((uint)rootWeb.Locale.LCID);

                foreach (SPWebTemplate item in webTemplates)
                {
                    //Check the temaplate is a site defintion and has a display category
                    if (!String.IsNullOrEmpty(item.DisplayCategory) && !item.IsCustomTemplate)
                    {
                        WebTemplateInfo info = new WebTemplateInfo();

                        info.Id = item.ID;
                        info.ImageUrl = item.ImageUrl;
                        info.Name = item.Name;
                        info.Description = item.Description;
                        info.DisplayCategory = item.DisplayCategory;
                        info.IsCustomTemplate = item.IsCustomTemplate;
                        info.IsHidden = item.IsHidden;
                        info.IsRootWebOnly = item.IsRootWebOnly;
                        info.IsSubWebOnly = item.IsSubWebOnly;
                        info.Lcid = item.Lcid;
                        info.Title = item.Title;
                        info.SetupPath = typeof(SPWebTemplate).InvokeMember("SetupPath", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetProperty, null, item, null) as string;

                        webTemplateInfos.Add(info);
                    }
                }
            }
            catch (Exception ex)
            {
                context.Logger.WriteLine(String.Format(Resources.WebTemplateSharePointCommands_RetrievingException,
                          ex.Message,
                          Environment.NewLine,
                          ex.StackTrace), LogCategory.Error);
            }

            return webTemplateInfos.ToArray();
        }

        #endregion
    }
}
