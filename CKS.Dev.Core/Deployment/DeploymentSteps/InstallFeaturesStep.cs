using CKS.Dev.VisualStudio.SharePoint.Commands;
using Microsoft.VisualStudio.SharePoint;
using Microsoft.VisualStudio.SharePoint.Deployment;
using System.ComponentModel.Composition;
using System.IO;
using CKSProperties = CKS.Dev.Core.Properties.Resources;

namespace CKS.Dev.VisualStudio.SharePoint.Deployment.DeploymentSteps
{
    /// <summary>
    /// Install features deployment step.
    /// </summary>
    [Export(typeof(IDeploymentStep))]
    [DeploymentStep(CustomDeploymentStepIds.InstallFeatures)]
    public class InstallFeaturesStep
        : IDeploymentStep
    {
        /// <summary>
        /// Initializes the deployment step.
        /// </summary>
        /// <param name="stepInfo">An object that contains information about the deployment step.</param>
        public void Initialize(IDeploymentStepInfo stepInfo)
        {
            stepInfo.Name = CKSProperties.InstallFeaturesStep_Name;
            stepInfo.StatusBarMessage = CKSProperties.InstallFeaturesStep_StatusBarMessage;
            stepInfo.Description = CKSProperties.InstallFeaturesStep_Description;
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
            bool canExecute = context.Project.IsSandboxedSolution == false;
            if (canExecute == false)
            {
                context.Logger.WriteLine("Skipping step because the project is configured to deploy into the solution sandbox.", LogCategory.Status);
            }
            return canExecute;
        }

        /// <summary>
        /// Executes the deployment step.
        /// </summary>
        /// <param name="context">An object that provides information you can use to determine the context in which the deployment step is executing.</param>
        public void Execute(IDeploymentContext context)
        {
            foreach (ISharePointProjectFeature feature in context.Project.Package.Features)
            {
                string relativePath = Path.Combine(feature.UnTokenize(feature.Model.DeploymentPath), "Feature.xml");
                context.Project.SharePointConnection.ExecuteCommand<string>(DeploymentSharePointCommandIds.InstallFeature, relativePath);
            }
        }
    }
}