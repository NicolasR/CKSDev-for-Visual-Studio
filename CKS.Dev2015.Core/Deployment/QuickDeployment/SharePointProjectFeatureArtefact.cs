using Microsoft.VisualStudio.SharePoint;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CKS.Dev2015.VisualStudio.SharePoint.Deployment.QuickDeployment
{
    /// <summary>
    /// SharePoint project feature artefact.
    /// </summary>
    public class SharePointProjectFeatureArtefact : QuickCopyableSharePointArtefact
    {
        private ISharePointProjectFeature feature = null;
        private IEnumerable<QuickCopyableSharePointArtefact> kids = null;
        private Dictionary<string, string> tokens = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharePointProjectFeatureArtefact"/> class.
        /// </summary>
        /// <param name="feature">The feature.</param>
        public SharePointProjectFeatureArtefact(ISharePointProjectFeature feature)
        {
            this.feature = feature;
        }

        /// <summary>
        /// Gets the name of the feature folder.
        /// </summary>
        /// <value>The name of the feature folder.</value>
        public string FeatureFolderName
        {
            get
            {
                // Even though a feature can be included in multiple projects/packages, the tokenized feature name is 
                // always replaced with the VS project the feature is actually contained in.
                string featureFolderName = feature.Model.DeploymentPath;
                featureFolderName = featureFolderName.Replace("$SharePoint.Project.FileNameWithoutExtension$", feature.Project.Name);
                featureFolderName = featureFolderName.Replace("$SharePoint.Feature.FileNameWithoutExtension$", DeploymentUtilities.GetLastFolderName(feature.FullPath));
                return featureFolderName;
            }
        }

        /// <summary>
        /// Gets all the child artefacts of this artefact.
        /// </summary>
        /// <value></value>
        public override IEnumerable<QuickCopyableSharePointArtefact> ChildArtefacts
        {
            get
            {
                if (kids == null)
                {
                    List<QuickCopyableSharePointArtefact> children = new List<QuickCopyableSharePointArtefact>();

                    // Get SPIs in this feature - will only include those that are set to be packaged.
                    foreach (ISharePointProjectItem spi in feature.ProjectItems)
                    {
                        children.Add(new SharePointProjectItemArtefact(spi));
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
            return feature.IsPartOfAnyProjectPackage(service);
        }

        /// <summary>
        /// Determines if this artefact is packaged as part of a specific project.
        /// </summary>
        /// <param name="project">The SharePoint project.</param>
        /// <returns>True if the artefact is packaged.</returns>
        public override bool IsPackaged(ISharePointProject project)
        {
            return feature.IsPartOfProjectPackage(project);
        }

        /// <summary>
        /// Gets all projects in the solution where this artefact is packaged.
        /// </summary>
        /// <param name="service">The SharePoint service.</param>
        /// <returns>
        /// An enumerable of the SharePoint projects.
        /// </returns>
        public override IEnumerable<ISharePointProject> GetPackagedProjects(ISharePointProjectService service)
        {
            return feature.GetProjectsWhereInPackage(service);
        }

        /// <summary>
        /// Gets the substitution tokens for this artefact.
        /// </summary>
        /// <value></value>
        /// <returns>The tokens dictionary.</returns>
        protected override Dictionary<string, string> Tokens
        {
            get
            {
                if (tokens == null)
                {
                    tokens = new Dictionary<string, string>();
                    tokens.Add("SharePoint.Feature.FileName", Path.GetFileName(feature.FeatureFile.Name));
                    tokens.Add("SharePoint.Feature.FileNameWithoutExtension", Path.GetFileNameWithoutExtension(feature.FeatureFile.Name));
                    tokens.Add("SharePoint.Feature.DeploymentPath", this.FeatureFolderName);
                    tokens.Add("SharePoint.Feature.Id", feature.Id.ToString());
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
            // We are directly deploying just this feature and have not been called by our parent.
            if (feature.IsPartOfProjectPackage(packageProject.Project))
            {
                // Pass in the package itself as the parent.
                this.QuickCopy(packageProject, packageProject, requiresQuickPackage);
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
            else if (parentArtefact is SharePointPackageArtefact)
            {
                string sourcePathBase = packageProject.BasePackagePath;
                string featureFolderName = this.FeatureFolderName;

                feature.Project.ProjectService.Logger.ActivateOutputWindow();
                feature.Project.ProjectService.Logger.WriteLine("------ Quick Copying Feature: " + this.FeatureFolderName + " ------", LogCategory.Status);

                // Feature.xml must first be Quick Copied.
                if (requiresQuickPackage)
                {
                    // Tokens consist of those from this package, and feature.
                    Dictionary<string, string> allTokens = new Dictionary<string, string>();
                    allTokens.AddRange(packageProject.GetReplacementTokens());
                    allTokens.AddRange(this.Tokens);

                    // TODO: Merge feature.feature.
                    feature.Project.ProjectService.Logger.ActivateOutputWindow();
                    feature.Project.ProjectService.Logger.WriteLine("WARNING: Quick packaging of changes to Feature.feature is not yet supported.  The last packaged version of the file will be copied.", LogCategory.Warning);

                    //QuickDeploymentUtilities.CopyFileWithTokenReplacement(packageProject.Project, file.Name, originalFileProjectRelative, sourcePackagePathProjectRelative, allTokens);
                }

                DeploymentUtilities.CopyFile(packageProject.Project, "Feature.xml", Path.Combine(sourcePathBase, featureFolderName), "{SharePointRoot}\\Template\\Features\\" + featureFolderName);

                // Process items in features.  Note that we are only processing items that have been set to be packaged.
                foreach (SharePointProjectItemArtefact spi in this.ChildArtefacts.Select(ca => ca as SharePointProjectItemArtefact))
                {
                    spi.QuickCopy(packageProject, this, requiresQuickPackage);
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
        /// <returns>A dictionary containing the tokens.</returns>
        public override Dictionary<string, string> GetReplacementTokens()
        {
            //TODO: revise this token stuff
            return this.Tokens;
        }
    }
}