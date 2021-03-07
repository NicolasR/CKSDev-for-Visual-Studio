using Microsoft.VisualStudio.SharePoint;
using System.Collections.Generic;

namespace CKS.Dev.VisualStudio.SharePoint.Deployment.QuickDeployment
{
    /// <summary>
    /// Object that can be copied from the solution to the SharePoint root.
    /// </summary>
    public abstract class QuickCopyableSharePointArtefact
    {
        /// <summary>
        /// Determines if this artefact is packaged anywhere in the solution.
        /// </summary>
        /// <param name="service">The SharePoint service.</param>
        /// <returns>True if the artefact is packaged.</returns>
        public abstract bool IsPackaged(ISharePointProjectService service);

        /// <summary>
        /// Determines if this artefact is packaged as part of a specific project.
        /// </summary>
        /// <param name="project">The SharePoint project.</param>
        /// <returns>True if the artefact is packaged.</returns>
        public abstract bool IsPackaged(ISharePointProject project);

        /// <summary>
        /// Gets all projects in the solution where this artefact is packaged.
        /// </summary>
        /// <param name="service">The SharePoint service.</param>
        /// <returns>An enumerable of the SharePoint projects.</returns>
        public abstract IEnumerable<ISharePointProject> GetPackagedProjects(ISharePointProjectService service);

        /// <summary>
        /// Gets all the child artefacts of this artefact.
        /// </summary>
        public abstract IEnumerable<QuickCopyableSharePointArtefact> ChildArtefacts
        {
            get;
        }

        /// <summary>
        /// Gets the substitution tokens for this artefact.
        /// </summary>
        /// <returns>The tokens dictionary.</returns>
        protected abstract Dictionary<string, string> Tokens
        {
            get;
        }

        /// <summary>
        /// Quick copy this artefact in the context of the specific package, and the specific containing artefact only.
        /// </summary>
        /// <param name="packageProject">The project.</param>
        /// <param name="parentArtefact">The deployable SharePoint artefact.</param>
        /// <param name="requiresQuickPackage">Flag to indicate it requires a quick package.</param>
        public abstract void QuickCopy(SharePointPackageArtefact packageProject, QuickCopyableSharePointArtefact parentArtefact, bool requiresQuickPackage);

        /// <summary>
        /// Quick copy this artefact in the context of the specific package, but wherever this artefact is contained in that package.
        /// </summary>
        /// <param name="packageProject"></param>
        /// <param name="requiresQuickPackage"></param>
        public abstract void QuickCopy(SharePointPackageArtefact packageProject, bool requiresQuickPackage);

        /// <summary>
        /// Quick copy this artefact and all children via all projects and artefacts in the project it is packaged in.
        /// </summary>
        /// <param name="service">The SharePoint service.</param>
        /// <param name="requiresQuickPackage">Flag to indicate if this requires quick package.</param>
        public void QuickCopy(ISharePointProjectService service, bool requiresQuickPackage)
        {
            IEnumerable<ISharePointProject> projects = this.GetPackagedProjects(service);
            foreach (ISharePointProject project in projects)
            {
                this.QuickCopy(new SharePointPackageArtefact(project), requiresQuickPackage);
            }
        }

        /// <summary>
        /// Gets the replacement tokens.
        /// </summary>
        /// <returns>A dictionary containing the tokens.</returns>
        public abstract Dictionary<string, string> GetReplacementTokens();
    }
}