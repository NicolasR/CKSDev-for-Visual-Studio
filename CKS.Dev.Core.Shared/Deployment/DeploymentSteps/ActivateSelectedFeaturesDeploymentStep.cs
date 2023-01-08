using CKS.Dev.VisualStudio.SharePoint.Commands;
using CKS.Dev.VisualStudio.SharePoint.Commands.Info;
using CKS.Dev.VisualStudio.SharePoint.Environment;
using Microsoft.VisualStudio.SharePoint;
using Microsoft.VisualStudio.SharePoint.Deployment;
using Microsoft.VisualStudio.SharePoint.Packages;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using CKSProperties = CKS.Dev.Core.Properties.Resources;

namespace CKS.Dev.VisualStudio.SharePoint.Deployment.DeploymentSteps
{
    /// <summary>
    /// Activate Selected Features Deployment Step
    /// </summary>
    [Export(typeof(IDeploymentStep))]
    [DeploymentStep(CustomDeploymentStepIds.ActivateSelectedFeaturesDeploymentStep)]
    public class ActivateSelectedFeaturesDeploymentStep : IDeploymentStep
    {
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
            IPackage package = context.Project.Package.Model;
            IEnumerable<ISharePointProjectFeature> features = null;
            List<string> selectedFeatureIds = ProjectUtilities.GetValueFromCurrentProject(context.Project, ActivateSelectedFeaturesProjectExtension.ProjectPropertyName);

            if (selectedFeatureIds != null && selectedFeatureIds.Count > 0)
            {
                List<ISharePointProjectMemberReference> featureRefs = new List<ISharePointProjectMemberReference>(selectedFeatureIds.Count);
                foreach (string featureId in selectedFeatureIds)
                {
                    ISharePointProjectMemberReference featureRef = (from ISharePointProjectMemberReference feature
                                                                    in package.Features
                                                                    where feature.ItemId.ToString().Equals(featureId, StringComparison.InvariantCultureIgnoreCase)
                                                                    select feature).FirstOrDefault();

                    if (featureRef != null)
                    {
                        featureRefs.Add(featureRef);
                    }
                }

                if (featureRefs != null && featureRefs.Count() > 0)
                {
                    features = ProjectUtilities.GetFeaturesFromFeatureRefs(context.Project, featureRefs);
                }
            }

            if (features != null && features.Count() > 0)
            {
                context.Project.SharePointConnection.ExecuteCommand<FeatureActivationInfo>(DeploymentSharePointCommandIds.ActivateFeatures, CreateFeatureActivationInfo(features, context.Project));
            }
            else
            {
                context.Logger.WriteLine("No features selected for activation.", LogCategory.Status);
            }
        }

        /// <summary>
        /// Initializes the deployment step.
        /// </summary>
        /// <param name="stepInfo">An object that contains information about the deployment step.</param>
        public void Initialize(IDeploymentStepInfo stepInfo)
        {
            stepInfo.Name = CKSProperties.ActivateSelectedFeaturesDeploymentStep_Name;
            stepInfo.StatusBarMessage = CKSProperties.ActivateSelectedFeaturesDeploymentStep_StatusBarMessage;
            stepInfo.Description = CKSProperties.ActivateSelectedFeaturesDeploymentStep_Description;
        }

        /// <summary>
        /// Creates the feature activation info.
        /// </summary>
        /// <param name="features">The features.</param>
        /// <param name="project">The project.</param>
        /// <returns></returns>
        private static FeatureActivationInfo CreateFeatureActivationInfo(IEnumerable<ISharePointProjectFeature> features,
            ISharePointProject project)
        {
            FeatureActivationInfo info = new FeatureActivationInfo
            {
                Features = (from ISharePointProjectFeature feature
                           in features
                            select new DeploymentFeatureInfo
                            {
                                FeatureID = feature.Model.FeatureId,
                                Name = feature.Model.Title,
                                Scope = (DeploymentFeatureScope)((int)feature.Model.Scope)
                            }).ToArray(),

                IsSandboxedSolution = project.IsSandboxedSolution
            };

            return info;
        }
    }
}

