using CKS.Dev.VisualStudio.SharePoint.Commands.Info;
using System;

namespace CKS.Dev.VisualStudio.SharePoint.Deployment.DeploymentSteps
{
    [Serializable]
    internal class FeatureActivationInfo
    {
        /// <summary>
        /// Gets or sets the features.
        /// </summary>
        /// <value>
        /// The features.
        /// </value>
        public DeploymentFeatureInfo[] Features { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is sandboxed solution.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is sandboxed solution; otherwise, <c>false</c>.
        /// </value>
        public bool IsSandboxedSolution { get; set; }
    }
}
