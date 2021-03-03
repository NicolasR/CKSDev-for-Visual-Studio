using Microsoft.VisualStudio.SharePoint.Deployment;
using System;
using System.ComponentModel.Composition;
using System.Net;
using CKSProperties = CKS.Dev2015.Core.Properties.Resources;

namespace CKS.Dev2015.VisualStudio.SharePoint.Deployment.DeploymentSteps
{
    /// <summary>
    /// Warms up the Site by placing a request to it.
    /// </summary>
    [Export(typeof(IDeploymentStep))]
    [DeploymentStep(CustomDeploymentStepIds.WarmUpSite)]
    public class WarmUpSiteStep
        : IDeploymentStep
    {
        /// <summary>
        /// Initializes the deployment step.
        /// </summary>
        /// <param name="stepInfo">An object that contains information about the deployment step.</param>
        public void Initialize(IDeploymentStepInfo stepInfo)
        {
            stepInfo.Name = CKSProperties.WarmUpSiteStep_Name;
            stepInfo.StatusBarMessage = CKSProperties.WarmUpSiteStep_StatusBarMessage;
            stepInfo.Description = CKSProperties.WarmUpSiteStep_Description;
        }

        /// <summary>
        /// Determines whether the deployment step can be executed in the current context.
        /// </summary>
        /// <param name="context">An object that provides information you can use to determine the context in which the deployment step is executing.</param>
        /// <returns>
        /// true if the deployment step can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(IDeploymentContext context)
        {
            return true;
        }

        /// <summary>
        /// Executes the deployment step.
        /// </summary>
        /// <param name="context">An object that provides information you can use to determine the context in which the deployment step is executing.</param>
        public void Execute(IDeploymentContext context)
        {
            Uri url = context.Project.SiteUrl;
            WebRequest request = HttpWebRequest.Create(url);
            request.BeginGetResponse(
                a => request.EndGetResponse(a), null);
        }
    }
}
