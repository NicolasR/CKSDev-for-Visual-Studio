using Microsoft.VisualStudio.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CKS.Dev.VisualStudio.SharePoint.Deployment.QuickDeployment
{
    /// <summary>
    /// Project item that can be copied to the SharePoint root.
    /// </summary>
    public class SharePointProjectItemArtefact : QuickCopyableSharePointArtefact
    {
        private ISharePointProjectItem item = null;
        private IEnumerable<QuickCopyableSharePointArtefact> kids = null;
        private Dictionary<string, string> tokens = null;

        /// <summary>
        /// Create a new instance of the SharePointProjectItemArtefact object.
        /// </summary>
        /// <param name="item"></param>
        public SharePointProjectItemArtefact(ISharePointProjectItem item)
        {
            this.item = item;
        }

        /// <summary>
        /// Gets all the child artefacts of this artefact.
        /// </summary>
        public override IEnumerable<QuickCopyableSharePointArtefact> ChildArtefacts
        {
            get
            {
                if (kids == null)
                {
                    List<QuickCopyableSharePointArtefact> children = new List<QuickCopyableSharePointArtefact>();

                    foreach (ISharePointProjectItemFile spiFile in item.Files.Where(file => file.DeploymentType != DeploymentType.NoDeployment))
                    {
                        children.Add(new SharePointProjectItemFileArtefact(spiFile));
                    }

                    kids = children;
                }
                return kids;
            }
        }

        /// <summary>
        /// Determines if this artefact is packaged anywhere in the solution.
        /// </summary>
        /// <param name="service">The SharePoint service.</param>
        /// <returns>True if the artefact is packaged.</returns>
        public override bool IsPackaged(ISharePointProjectService service)
        {
            return item.IsPartOfAnyProjectPackage(service);
        }

        /// <summary>
        /// Determines if this artefact is packaged as part of a specific project.
        /// </summary>
        /// <param name="project">The SharePoint project.</param>
        /// <returns>True if the artefact is packaged.</returns>
        public override bool IsPackaged(ISharePointProject project)
        {
            return item.IsPartOfProjectPackage(project);
        }

        /// <summary>
        /// Gets all projects in the solution where this artefact is packaged.
        /// </summary>
        /// <param name="service">The SharePoint service.</param>
        /// <returns>An enumerable of the SharePoint projects.</returns>
        public override IEnumerable<ISharePointProject> GetPackagedProjects(ISharePointProjectService service)
        {
            return item.GetProjectsWhereInPackage(service);
        }

        /// <summary>
        /// Gets the substitution tokens for this artefact.
        /// </summary>
        /// <returns>The tokens dictionary.</returns>
        protected override Dictionary<string, string> Tokens
        {
            get
            {
                if (tokens == null)
                {
                    tokens = new Dictionary<string, string>();
                    tokens.Add("SharePoint.ProjectItem.Name", item.Name);

                    //Team Note:
                    //The tokens for the Guids have been processed at Project level as the 
                    //reflection needed is too expensive to do over and over per class.
                }

                return tokens;
            }
        }

        /// <summary>
        /// Quick copy this artefact in the context of the specific package, but wherever this artefact is contained in that package.
        /// </summary>
        /// <param name="packageProject"></param>
        /// <param name="requiresQuickPackage"></param>
        public override void QuickCopy(SharePointPackageArtefact packageProject, bool requiresQuickPackage)
        {
            // We are directly deploying just this SPI and have not been called by our parent.
            // An SPI may be included in multiple features, or be against the project directly.
            if (item.IsDirectPartOfProjectPackage(packageProject.Project))
            {
                // Pass in the package itself as the parent.
                this.QuickCopy(packageProject, packageProject, requiresQuickPackage);
            }
            else
            {
                // The SPI may be included in multiple features in the project in context.
                foreach (ISharePointProjectFeature feature in item.GetFeaturesWhereInPackage(packageProject.Project))
                {
                    QuickCopyableSharePointArtefact parent = new SharePointProjectFeatureArtefact(feature);
                    this.QuickCopy(packageProject, parent, requiresQuickPackage);
                }
            }
        }

        /// <summary>
        /// Quick copy this artefact in the context of the specific package, and the specific containing artefact only.
        /// </summary>
        /// <param name="packageProject">The project.</param>
        /// <param name="parentArtefact">The deployable SharePoint artefact.</param>
        /// <param name="requiresQuickPackage">Flag to indicate it requires a quick package.</param>
        public override void QuickCopy(SharePointPackageArtefact packageProject, QuickCopyableSharePointArtefact parentArtefact, bool requiresQuickPackage)
        {
            if (parentArtefact == null)
            {
                throw new NotSupportedException();
            }
            else if (parentArtefact is SharePointProjectFeatureArtefact || parentArtefact is SharePointPackageArtefact)
            {
                packageProject.Project.ProjectService.Logger.ActivateOutputWindow();
                packageProject.Project.ProjectService.Logger.WriteLine("------ Quick Copying SPI in " + (parentArtefact is SharePointPackageArtefact ? "Package" : "Feature") + ": " + item.Name + " ------", LogCategory.Status);

                foreach (SharePointProjectItemFileArtefact spiFile in this.ChildArtefacts.Select(ca => ca as SharePointProjectItemFileArtefact))
                {
                    spiFile.QuickCopy(packageProject, parentArtefact, requiresQuickPackage);
                }
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets the replacement tokens.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override Dictionary<string, string> GetReplacementTokens()
        {
            //TODO: revise this token stuff
            return this.Tokens;
        }
    }
}