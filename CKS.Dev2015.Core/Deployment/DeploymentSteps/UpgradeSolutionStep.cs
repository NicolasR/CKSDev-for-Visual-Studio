using CKS.Dev2015.VisualStudio.SharePoint.Commands;
using CKS.Dev2015.VisualStudio.SharePoint.Commands.Info;
using Microsoft.VisualStudio.SharePoint;
using Microsoft.VisualStudio.SharePoint.Deployment;
using System;
using System.ComponentModel.Composition;
using CKSProperties = CKS.Dev2015.Core.Properties.Resources;

namespace CKS.Dev2015.VisualStudio.SharePoint.Deployment.DeploymentSteps
{
    /// <summary>
    /// Upgrade the solution deployment step.
    /// </summary>
    [Export(typeof(IDeploymentStep))]
    [DeploymentStep(CustomDeploymentStepIds.UpgradeSolution)]
    public class UpgradeSolutionStep
        : IDeploymentStep
    {
        /// <summary>
        /// Initializes the deployment step.
        /// </summary>
        /// <param name="stepInfo">An object that contains information about the deployment step.</param>
        public void Initialize(IDeploymentStepInfo stepInfo)
        {
            stepInfo.Name = CKSProperties.UpgradeSolutionStep_Name;
            stepInfo.StatusBarMessage = CKSProperties.UpgradeSolutionStep_StatusBarMessage;
            stepInfo.Description = CKSProperties.UpgradeSolutionStep_Description;
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
            string solutionName;
            string solutionFullPath;

            // SharePoint returns all the installed solutions names in lower case.
            solutionName = (context.Project.Package.Model.Name + ".wsp").ToLower();
            solutionFullPath = context.Project.Package.OutputPath;
            bool solutionExists = context.Project.SharePointConnection.ExecuteCommand<string, bool>(
                DeploymentSharePointCommandIds.IsSolutionDeployed, solutionName);

            // Throw exceptions in error cases because deployment cannot proceed.
            if (context.Project.IsSandboxedSolution)
            {
                string sandboxMessage = "Cannot upgrade the solution. The upgrade deployment configuration " +
                    "does not support Sandboxed solutions.";
                context.Logger.WriteLine(sandboxMessage, LogCategory.Error);
                throw new InvalidOperationException(sandboxMessage);
            }
            else if (!solutionExists)
            {
                string notDeployedMessage = string.Format("Cannot upgrade the solution. The IsSolutionDeployed " +
                    "command cannot find the following solution: {0}.", solutionName);
                context.Logger.WriteLine(notDeployedMessage, LogCategory.Error);
                throw new InvalidOperationException(notDeployedMessage);
            }

            // Execute step and continue with deployment.
            return true;
        }

        /// <summary>
        /// Executes the deployment step.
        /// </summary>
        /// <param name="context">An object that provides information you can use to determine the context in which the deployment step is executing.</param>
        public void Execute(IDeploymentContext context)
        {
            context.Project.SharePointConnection.ExecuteCommand<SolutionInfo>(
                DeploymentSharePointCommandIds.UpgradeSolution,
                new SolutionInfo()
                {
                    IsSandboxedSolution = context.Project.IsSandboxedSolution,
                    LocalPath = context.Project.Package.OutputPath,
                    Name = context.Project.Package.Model.Name + ".wsp"
                });
        }
    }
}
