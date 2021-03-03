using CKS.Dev2015.VisualStudio.SharePoint.Commands.Info;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.VisualStudio.SharePoint.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CKS.Dev2015.VisualStudio.SharePoint.Commands
{
    class DeploymentSharePointCommands
    {
        /// <summary>
        /// Determines whether the specified solution has been deployed to the local SharePoint server.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="solutionName">Name of the solution.</param>
        /// <returns>
        /// 	<c>true</c> if solution is deployed otherwise, <c>false</c>.
        /// </returns>
        [SharePointCommand(DeploymentSharePointCommandIds.IsSolutionDeployed)]
        private bool IsSolutionDeployed(ISharePointCommandContext context, string solutionName)
        {
            SPSolution solution = SPFarm.Local.Solutions[solutionName];
            return solution != null;
        }

        /// <summary>
        /// Upgrades the solution.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="solutionInfo">The solution info.</param>
        [SharePointCommand(DeploymentSharePointCommandIds.UpgradeSolution)]
        public static void UpgradeSolution(ISharePointCommandContext context, SolutionInfo solutionInfo)
        {
            SPSolution solution = SPFarm.Local.Solutions[solutionInfo.Name];

            //Do not use this version as it causes SP to return success prior to timer execution so VS falselt notifies the user it is completed. https://cksdev.codeplex.com/workitem/9027
            //solution.Upgrade(solutionInfo.LocalPath, DateTime.Now);

            solution.Upgrade(solutionInfo.LocalPath);
        }

        /// <summary>
        /// Retrieves the application pool name of a given url.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        [SharePointCommand(DeploymentSharePointCommandIds.GetApplicationPoolName)]
        private string GetApplicationPoolName(ISharePointCommandContext context, string url)
        {
            string name = null;
            try
            {
                using (SPSite site = new SPSite(url))
                {
                    name = site.WebApplication.ApplicationPool.Name;
                }
            }
            catch
            {
            }

            return name;
        }

        /// <summary>
        /// Retrieves a unique list of all application pools used by SharePoint.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        [SharePointCommand(DeploymentSharePointCommandIds.GetAllApplicationPoolNames)]
        private string[] GetAllApplicationPoolNames(ISharePointCommandContext context)
        {
            List<string> names = new List<string>();

            foreach (SPWebApplication app in SPWebService.ContentService.WebApplications.Union(SPWebService.AdministrationService.WebApplications))
            {
                if (!names.Contains(app.ApplicationPool.Name))
                {
                    names.Add(app.ApplicationPool.Name);
                }
            }

            return names.ToArray();
        }

        // Retrieves the physical path of a given web application.
        [SharePointCommand(DeploymentSharePointCommandIds.GetWebApplicationDefaultPhysicalPath)]
        private string GetWebApplicationDefaultPhysicalPath(ISharePointCommandContext context, string url)
        {
            string name = null;
            try
            {
                using (SPSite site = new SPSite(url))
                {
                    SPWebApplication app = site.WebApplication;

                    if (app.IisSettings.ContainsKey(SPUrlZone.Default))
                    {
                        name = app.IisSettings[SPUrlZone.Default].Path.FullName;
                    }
                    else
                    {
                        foreach (SPUrlZone zone in app.IisSettings.Keys)
                        {
                            name = app.IisSettings[zone].Path.FullName;
                            break;
                        }
                    }
                }
            }
            catch
            {
            }

            return name;
        }

        /// <summary>
        /// Gets the web application physical paths from IIS for each zone.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>An array of full file paths to each IIS site.</returns>
        [SharePointCommand(DeploymentSharePointCommandIds.GetWebApplicationPhysicalPaths)]
        public static string[] GetWebApplicationPhysicalPaths(ISharePointCommandContext context)
        {
            List<string> folders = new List<string>();

            SPWebApplication webApp = context.Site.WebApplication;
            foreach (KeyValuePair<SPUrlZone, SPIisSettings> iisSettingsByZone in webApp.IisSettings)
            {
                folders.Add(iisSettingsByZone.Value.Path.FullName);
            }
            return folders.ToArray();
        }

        [SharePointCommand(DeploymentSharePointCommandIds.CanCreateSite)]
        public static bool CanCreateSite(ISharePointCommandContext context)
        {
            return true;
        }

        [SharePointCommand(DeploymentSharePointCommandIds.RecreateSite)]
        public static void RecreateSite(ISharePointCommandContext context, string defaultTemplate)
        {
            SPSite siteCollection = context.Site;
            SPWebApplication webApp = siteCollection.WebApplication;
            int configuration = siteCollection.RootWeb.Configuration;
            string template = siteCollection.RootWeb.WebTemplate;
            string url = siteCollection.ServerRelativeUrl;
            string ownerLogin = siteCollection.Owner.LoginName;
            string ownerEmail = siteCollection.Owner.Email;
            string ownerName = siteCollection.Owner.Name;
            uint lcid = siteCollection.RootWeb.Language;
            string title = siteCollection.RootWeb.Title;
            siteCollection.Delete();
            webApp.Sites.Add(url, title, null, lcid, defaultTemplate ?? String.Format("{0}#{1}", template, configuration), ownerLogin, ownerName, ownerEmail);
        }

        /// <summary>
        /// Copies the content of the app bin.
        /// </summary>
        /// <param name="context">The context.</param>
        [SharePointCommand(DeploymentSharePointCommandIds.CopyAppBinContent)]
        public static void CopyAppBinContent(ISharePointCommandContext context)
        {
            SPServiceInstance localContent = SPWebServiceInstance.LocalContent;
            if ((localContent != null) && (localContent.Status == SPObjectStatus.Online))
            {
                SPWebService.ContentService.ApplyApplicationContentToLocalServer();
            }

            localContent = SPWebServiceInstance.LocalAdministration;
            if ((localContent != null) && (localContent.Status == SPObjectStatus.Online))
            {
                SPWebService.AdministrationService.ApplyApplicationContentToLocalServer();
            }
        }

        /// <summary>
        /// Installs the feature.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="featurePath">The feature path.</param>
        [SharePointCommand(DeploymentSharePointCommandIds.InstallFeature)]
        public static void InstallFeature(ISharePointCommandContext context, string featurePath)
        {
            SPFarm.Local.FeatureDefinitions.Add(featurePath, Guid.Empty, true);
        }

        /// <summary>
        /// Activates the features.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="activationInfo">The activation info.</param>
        [SharePointCommand(DeploymentSharePointCommandIds.ActivateFeatures)]
        private static void ActivateFeatures(ISharePointCommandContext context, FeatureActivationInfo activationInfo)
        {
            SPFeatureDefinitionScope scope = activationInfo.IsSandboxedSolution ? SPFeatureDefinitionScope.Site : SPFeatureDefinitionScope.Farm;

            foreach (DeploymentFeatureInfo featureInfo in activationInfo.Features)
            {
                context.Logger.WriteLine(String.Format("Activating Feature {0}...", featureInfo.Name), LogCategory.Status);

                if (featureInfo.Scope == DeploymentFeatureScope.Web)
                {
                    context.Web.Features.Add(featureInfo.FeatureID, false, scope);
                }
                else if (featureInfo.Scope == DeploymentFeatureScope.Site)
                {
                    context.Site.Features.Add(featureInfo.FeatureID, false, scope);
                }
                else if (featureInfo.Scope == DeploymentFeatureScope.WebApplication)
                {
                    context.Site.WebApplication.Features.Add(featureInfo.FeatureID, false, scope);
                }
            }
        }
    }
}
