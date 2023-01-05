using CKS.Dev.VisualStudio.SharePoint.Deployment.QuickDeployment;
using Microsoft.VisualStudio.SharePoint;
using Microsoft.VisualStudio.SharePoint.Deployment;
using System;
using System.ComponentModel.Composition;
using CKSProperties = CKS.Dev.Core.Properties.Resources;

namespace CKS.Dev.VisualStudio.SharePoint.Deployment.DeploymentSteps
{
    /// <summary>
    /// Defines a new deployment step that can be used to "Copy to 14" on a SharePoint site.
    /// </summary>   
    // Enables Visual Studio to discover and load this deployment step.
    [Export(typeof(IDeploymentStep))]
    // Specifies the ID for this new deployment step.
    [DeploymentStep(CustomDeploymentStepIds.CopyToSharePointRoot)]
    internal class CopyToSharePointRootStep
        : IDeploymentStep
    {
        /// <summary>
        /// Initializes the deployment step.
        /// </summary>
        /// <param name="stepInfo">An object that contains information about the deployment step.</param>
        public void Initialize(IDeploymentStepInfo stepInfo)
        {
            stepInfo.Name = CKSProperties.CopyToSharePointRootStep_Name;
            stepInfo.StatusBarMessage = CKSProperties.CopyToSharePointRootStep_StatusBarMessage;
            stepInfo.Description = CKSProperties.CopyToSharePointRootStep_Description;
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
            if (context.IsRetracting)
            {
                string sandboxMessage = "Copy to SharePoint Root cannot Retract.";
                context.Logger.WriteLine(sandboxMessage, LogCategory.Error);
                throw new InvalidOperationException(sandboxMessage);
            }

            if (context.Project.IsSandboxedSolution)
            {
                string sandboxMessage = "Copy to SharePoint Root does not support Sandboxed Solutions.";
                context.Logger.WriteLine(sandboxMessage, LogCategory.Error);
                throw new InvalidOperationException(sandboxMessage);
            }

            return true;
        }

        /// <summary>
        /// Executes the deployment step.
        /// </summary>
        /// <param name="context">An object that provides information you can use to determine the context in which the deployment step is executing.</param>
        public void Execute(IDeploymentContext context)
        {
            if (context.IsDeploying)
            {
                SharePointPackageArtefact item = new SharePointPackageArtefact(context.Project);
                item.QuickCopy(item, false);
            }
        }
    }
}
