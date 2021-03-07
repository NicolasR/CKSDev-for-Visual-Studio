using Microsoft.VisualStudio.SharePoint;
using System;
using System.Collections.Generic;
using System.IO;

namespace CKS.Dev.VisualStudio.SharePoint.Deployment.QuickDeployment
{
    /// <summary>
    /// Project item file that can be copied to the SharePoint root.
    /// </summary>
    public class SharePointProjectItemFileArtefact : QuickCopyableSharePointArtefact
    {
        private ISharePointProjectItemFile file = null;

        /// <summary>
        /// Create a new instance of the SharePointProjectItemFileArtefact object.
        /// </summary>
        /// <param name="file">The project file.</param>
        public SharePointProjectItemFileArtefact(ISharePointProjectItemFile file)
        {
            this.file = file;
        }

        /// <summary>
        /// Gets the child artefacts.
        /// </summary>
        public override IEnumerable<QuickCopyableSharePointArtefact> ChildArtefacts
        {
            get
            {
                return new List<QuickCopyableSharePointArtefact>();
            }
        }

        /// <summary>
        /// Determines if this artefact is packaged anywhere in the solution.
        /// </summary>
        /// <param name="service">The SharePoint service.</param>
        /// <returns>True if the artefact is packaged.</returns>
        public override bool IsPackaged(ISharePointProjectService service)
        {
            return file.ProjectItem.IsPartOfAnyProjectPackage(service);
        }

        /// <summary>
        /// Determines if this artefact is packaged as part of a specific project.
        /// </summary>
        /// <param name="project">The SharePoint project.</param>
        /// <returns>True if the artefact is packaged.</returns>
        public override bool IsPackaged(ISharePointProject project)
        {
            return file.ProjectItem.IsPartOfProjectPackage(project);
        }

        /// <summary>
        /// Gets all projects in the solution where this artefact is packaged.
        /// </summary>
        /// <param name="service">The SharePoint service.</param>
        /// <returns>An enumerable of the SharePoint projects.</returns>
        public override IEnumerable<ISharePointProject> GetPackagedProjects(ISharePointProjectService service)
        {
            return file.ProjectItem.GetProjectsWhereInPackage(service);
        }

        /// <summary>
        /// Gets the substitution tokens for this artefact.
        /// </summary>
        /// <returns>The tokens dictionary.</returns>
        protected override Dictionary<string, string> Tokens
        {
            get
            {
                return new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Quick copy this artefact in the context of the specific package, but wherever this artefact is contained in that package.
        /// </summary>
        /// <param name="packageProject"></param>
        /// <param name="requiresQuickPackage"></param>
        public override void QuickCopy(SharePointPackageArtefact packageProject, bool requiresQuickPackage)
        {
            // We are directly deploying just this file and have not been called by our parent.
            // A file will always be in one SPI, but that SPI may be included in multiple features, or be against the project directly.
            if (file.ProjectItem.IsDirectPartOfProjectPackage(packageProject.Project))
            {
                // Pass in the package itself as the parent.
                this.QuickCopy(packageProject, packageProject, requiresQuickPackage);
            }
            else
            {
                // A file will always be in one SPI, but that SPI may be included in multiple features in the project in context.
                foreach (ISharePointProjectFeature feature in file.ProjectItem.GetFeaturesWhereInPackage(packageProject.Project))
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
                SharePointProjectFeatureArtefact feature = parentArtefact as SharePointProjectFeatureArtefact;
                this.QuickCopy(packageProject, feature, requiresQuickPackage);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Quick copy this artefact in the context of the specific package, and the specific containing artefact only.
        /// </summary>
        /// <param name="packageProject">The project.</param>
        /// <param name="parentFeature">The deployable SharePoint artefact.</param>
        /// <param name="requiresQuickPackage">Flag to indicate it requires a quick package.</param>
        public void QuickCopy(SharePointPackageArtefact packageProject, SharePointProjectFeatureArtefact parentFeature, bool requiresQuickPackage)
        {
            if (file.DeploymentType != DeploymentType.NoDeployment)
            {
                // Determine the folder name of the parent feature (if applicable).
                string featureFolderName = parentFeature == null ? "" : parentFeature.FeatureFolderName;

                // The default destination path is given to us by the tooling (though includes tokens).
                string destinationPathHiveRelative = String.Empty;
                if (String.IsNullOrEmpty(file.DeploymentPath))
                {
                    destinationPathHiveRelative = file.DeploymentRoot;
                }
                else
                {
                    destinationPathHiveRelative = Path.Combine(file.DeploymentRoot, file.DeploymentPath);
                }

                // The source path of the packageable file begins with the base package path of the project.
                string sourcePackagePathProjectRelative = packageProject.BasePackagePath;

                // The remainder of the package path depends on the type of file.
                if (file.DeploymentType == DeploymentType.ElementFile || file.DeploymentType == DeploymentType.ElementManifest)
                {
                    // Source path within pkg is {FeatureName} + the item's relative path.
                    sourcePackagePathProjectRelative = Path.Combine(sourcePackagePathProjectRelative, featureFolderName);
                    sourcePackagePathProjectRelative = Path.Combine(sourcePackagePathProjectRelative, Path.GetDirectoryName(file.RelativePath));
                }
                if (file.DeploymentType == DeploymentType.AppGlobalResource || file.DeploymentType == DeploymentType.ApplicationResource)
                {
                    sourcePackagePathProjectRelative = Path.Combine(sourcePackagePathProjectRelative, Path.GetDirectoryName(file.RelativePath));
                }
                else if (file.DeploymentType == DeploymentType.RootFile || file.DeploymentType == DeploymentType.TemplateFile)
                {
                    // For both template and root files, these are stored relative to the pkg folder with a
                    // path matching file.DeploymentPath.
                    sourcePackagePathProjectRelative = Path.Combine(sourcePackagePathProjectRelative, Path.GetDirectoryName(file.DeploymentPath));
                }
                else
                {
                    // Unhandled file type.  Just show a message for now.
                    // TODO: check all the file types we should spuport and test.
                    packageProject.Project.ProjectService.Logger.ActivateOutputWindow();
                    packageProject.Project.ProjectService.Logger.WriteLine(string.Format("Unhandled File Type - {0} at {1} - please notify the CKSDEV team", file.DeploymentType, file.FullPath), LogCategory.Status);
                }

                // Make some final substitutions on the paths as necessary.
                sourcePackagePathProjectRelative = sourcePackagePathProjectRelative.Replace("{FeatureName}", featureFolderName);
                destinationPathHiveRelative = destinationPathHiveRelative.Replace("{FeatureName}", featureFolderName);

                // First package (if appropriate), then quick copy the file.
                if (requiresQuickPackage)
                {
                    // The actual project file path is also given to us by the tooling.
                    string originalFileProjectRelative = Path.GetDirectoryName(file.FullPath);

                    Dictionary<string, string> allTokens = null;
                    if (DeploymentUtilities.IsTokenReplacementFile(file.Project, file.Name))
                    {
                        // Tokens consist of those from this package, SPI, and (optionally) the feature. 
                        allTokens = new Dictionary<string, string>();
                        allTokens.AddRange(packageProject.GetReplacementTokens());
                        allTokens.AddRange(new SharePointProjectItemArtefact(file.ProjectItem).GetReplacementTokens());
                        if (parentFeature != null)
                        {
                            allTokens.AddRange(parentFeature.GetReplacementTokens());
                        }
                    }

                    DeploymentUtilities.CopyFileWithTokenReplacement(packageProject.Project, file.Name, originalFileProjectRelative, sourcePackagePathProjectRelative, allTokens);
                }

                DeploymentUtilities.CopyFile(packageProject.Project, file.Name, sourcePackagePathProjectRelative, destinationPathHiveRelative);
            }
        }

        //TODO: check that all these use cases are catered for during the logic above for file deployment.
        ///// <summary>
        ///// Gets the root paths.
        ///// </summary>
        ///// <param name="context">The context.</param>
        ///// <param name="file">The file.</param>
        ///// <returns></returns>
        //IEnumerable<string> GetRootPaths(IDeploymentContext context, ISharePointProjectItemFile file)
        //{
        //    List<string> rootPaths = new List<string>();
        //    switch (file.DeploymentType)
        //    {
        //        case DeploymentType.RootFile:
        //            rootPaths.Add(context.Project.ProjectService.SharePointInstallPath);
        //            break;
        //        case DeploymentType.TemplateFile:
        //            rootPaths.Add(Path.Combine(context.Project.ProjectService.SharePointInstallPath, "TEMPLATE"));
        //            break;
        //        case DeploymentType.ElementManifest:
        //        case DeploymentType.ElementFile:
        //        case DeploymentType.Resource:
        //            rootPaths.Add(Path.Combine(context.Project.ProjectService.SharePointInstallPath, @"TEMPLATE\FEATURES"));
        //            break;
        //        case DeploymentType.ApplicationResource:
        //            foreach (string binPath in GetBinPaths(context))
        //            {
        //                rootPaths.Add(Path.Combine(binPath, "resources"));
        //            }
        //            break;
        //        case DeploymentType.AppGlobalResource:
        //            foreach (string binPath in GetBinPaths(context))
        //            {
        //                rootPaths.Add(Path.Combine(binPath, "App_GlobalResources"));
        //            }
        //            break;
        //        case DeploymentType.ClassResource:
        //            if (file.Project.IncludeAssemblyInPackage)
        //            {
        //                if (file.Project.AssemblyDeploymentTarget == AssemblyDeploymentTarget.WebApplication)
        //                {
        //                    foreach (string binPath in GetBinPaths(context))
        //                    {
        //                        rootPaths.Add(Path.Combine(binPath, "wpresources"));
        //                    }
        //                }
        //                else
        //                {
        //                    string installPath = context.Project.ProjectService.SharePointInstallPath.TrimEnd('\\');
        //                    string parentFolder = Path.GetDirectoryName(installPath);
        //                    rootPaths.Add(Path.Combine(parentFolder, "wpresources"));
        //                }
        //            }
        //            break;
        //    }
        //    return rootPaths;
        //}

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