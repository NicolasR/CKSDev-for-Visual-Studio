using CKS.Dev.VisualStudio.SharePoint.Environment;
using Microsoft.VisualStudio.SharePoint;
using Microsoft.VisualStudio.SharePoint.Deployment;
using System.ComponentModel.Composition;
using CKSProperties = CKS.Dev.Core.Properties.Resources;
namespace CKS.Dev.VisualStudio.SharePoint.Deployment.DeploymentSteps
{
    /// <summary>
    /// Reset timer service deployment step.
    /// </summary>
    [Export(typeof(IDeploymentStep))]
    [DeploymentStep(CustomDeploymentStepIds.RestartOWSTimerService)]
    public class RestartOWSTimerServiceStep
        : IDeploymentStep
    {
        /// <summary>
        /// Initializes the deployment step.
        /// </summary>
        /// <param name="stepInfo">An object that contains information about the deployment step.</param>
        public void Initialize(IDeploymentStepInfo stepInfo)
        {
            stepInfo.Name = CKSProperties.RestartOWSTimerServiceStep_Name;
            stepInfo.StatusBarMessage = CKSProperties.RestartOWSTimerServiceStep_StatusBarMessage;
            stepInfo.Description = CKSProperties.RestartOWSTimerServiceStep_Description;
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
            bool? _canExecute = null;

            if (_canExecute == null)
            {
                _canExecute = new ProcessUtilities(context.Project.ProjectService.Convert<ISharePointProject, EnvDTE.Project>(context.Project).DTE).IsProcessAvailableByName(ProcessConstants.OWSTimerProcessName);
            }
            if (_canExecute == false)
            {
                context.Logger.WriteLine("Skipping step because the timer service is not installed on the local machine.", LogCategory.Status);
            }
            return _canExecute.Value;
        }

        /// <summary>
        /// Executes the deployment step.
        /// </summary>
        /// <param name="context">An object that provides information you can use to determine the context in which the deployment step is executing.</param>
        public void Execute(IDeploymentContext context)
        {
            new ProcessUtilities(context.Project.ProjectService.Convert<ISharePointProject, EnvDTE.Project>(context.Project).DTE).RestartProcess(ProcessConstants.OWSTimerProcessName, 60);
        }
    }
}
