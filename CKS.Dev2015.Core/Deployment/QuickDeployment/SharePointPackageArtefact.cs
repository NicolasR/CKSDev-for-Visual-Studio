using Microsoft.VisualStudio.SharePoint;
using Microsoft.VisualStudio.SharePoint.Packages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CKS.Dev2015.VisualStudio.SharePoint.Deployment.QuickDeployment
{
    /// <summary>
    /// SharePoint Package Artefact.
    /// </summary>
    public class SharePointPackageArtefact : QuickCopyableSharePointArtefact
    {
        #region Fields

        private ISharePointProject project = null;
        private IEnumerable<QuickCopyableSharePointArtefact> kids = null;
        private Dictionary<string, string> tokens = null;

        //TODO these should be used
        /*private static readonly string ProjectAssemblyFileNameToken = "$SharePoint.Project.AssemblyFileName$";
        private static readonly string ProjectAssemblyFileNameWithoutExtensionToken = "$SharePoint.Project.AssemblyFileNameWithoutExtension$";
        private static readonly string ProjectAssemblyFullNameToken = "$SharePoint.Project.AssemblyFullName$";
        private static readonly string ProjectAssemblyNameToken = "$SharePoint.Project.AssemblyName$";
        private static readonly string ProjectAssemblyPublicKeyBlobsToken = "$SharePoint.Project.AssemblyPublicKeyBlob$";
        private static readonly string ProjectAssemblyPublicKeyTokenToken = "$SharePoint.Project.AssemblyPublicKeyToken$";
        private static readonly string ProjectAssemblyVersionToken = "$SharePoint.Project.AssemblyVersion$";
        private static readonly string ProjectFileNameToken = "$SharePoint.Project.FileName$";
        private static readonly string ProjectFileNameWithoutExtensionToken = "$SharePoint.Project.FileNameWithoutExtension$";
        private static readonly string TypeAssemblyQualifiedNameTokenBase = "$SharePoint.Type.{0}.AssemblyQualifiedName$";
        private static readonly string TypeFullNameTokenBase = "$SharePoint.Type.{0}.FullName$";
        */
        #endregion

        #region Properties

        /// <summary>
        /// Gets the SharePoint project.
        /// </summary>
        public ISharePointProject Project
        {
            get
            {
                return this.project;
            }
        }

        /// <summary>
        /// Gets the base project path.
        /// </summary>
        public string BaseProjectPath
        {
            get
            {
                return "{ProjectRoot}\\";
            }
        }

        /// <summary>
        /// Gets the base package path.
        /// </summary>
        public string BasePackagePath
        {
            get
            {
                EnvDTE.Project dteProject = DTEManager.ProjectService.Convert<ISharePointProject, EnvDTE.Project>(this.Project);

                string sourcePathBase = this.BaseProjectPath;
                sourcePathBase = Path.Combine(sourcePathBase, "pkg\\");
                sourcePathBase = Path.Combine(sourcePathBase, dteProject.ConfigurationManager.ActiveConfiguration.ConfigurationName);
                sourcePathBase = Path.Combine(sourcePathBase, project.Package.Model.Name);
                return sourcePathBase;
            }
        }

        /// <summary>
        /// Gets the assembly path.
        /// </summary>
        public string AssemblyPath
        {
            get
            {
                EnvDTE.Project dteProject = DTEManager.ProjectService.Convert<ISharePointProject, EnvDTE.Project>(this.Project);
                // TODO: Remove this bug. There is no setting that allows
                // hardcoding of BIN folder. It is a project setting
                return Path.Combine("bin\\", dteProject.ConfigurationManager.ActiveConfiguration.ConfigurationName);
            }
        }

        /// <summary>
        /// Is packaged?
        /// </summary>
        /// <param name="service">The SharePoint project service.</param>
        /// <returns>Always true as a package is always packaged.</returns>
        public override bool IsPackaged(ISharePointProjectService service)
        {
            // A package is always packaged!
            return true;
        }

        /// <summary>
        /// Determines if this artefact is packaged as part of a specific project.
        /// </summary>
        /// <param name="project">The SharePoint project.</param>
        /// <returns>True if the artefact is packaged.</returns>
        public override bool IsPackaged(ISharePointProject project)
        {
            // A package is always packaged!
            return true;
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
            return new List<ISharePointProject>(new ISharePointProject[] { this.project });
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

                    // Process all features set to deploy in the package.
                    foreach (ISharePointProjectFeature feature in project.Package.Features)
                    {
                        children.Add(new SharePointProjectFeatureArtefact(feature));
                    }

                    // Process SPIs outside the feature - normally mapped folders.
                    foreach (ISharePointProjectItem spi in project.Package.ProjectItems)
                    {
                        children.Add(new SharePointProjectItemArtefact(spi));
                    }

                    kids = children;
                }
                return kids;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create a new instance of the SharePointPackageArtefact object.
        /// </summary>
        /// <param name="project">The SharePoint project.</param>
        public SharePointPackageArtefact(ISharePointProject project)
        {
            this.project = project;
        }



        #endregion




















        /// <summary>
        /// Gets the substitution tokens for this artefact.
        /// </summary>
        /// <value></value>
        /// <returns>The tokens dictionary.</returns>
        protected override Dictionary<string, string> Tokens
        {
            get
            {
                EnvDTE.Project dteProject = DTEManager.ProjectService.Convert<ISharePointProject, EnvDTE.Project>(this.Project);

                if (tokens == null)
                {
                    //TODO: make these use the static field non-magic strings
                    tokens = new Dictionary<string, string>();
                    tokens.Add("SharePoint.Project.FileName", Path.GetFileName(dteProject.FullName));
                    tokens.Add("SharePoint.Project.FileNameWithoutExtension", Path.GetFileNameWithoutExtension(dteProject.FullName));
                    tokens.Add("SharePoint.Package.Name", Path.GetFileNameWithoutExtension(project.Package.OutputPath));
                    tokens.Add("SharePoint.Package.FileName", Path.GetFileName(project.Package.Name));
                    tokens.Add("SharePoint.Package.FileNameWithoutExtension", Path.GetFileNameWithoutExtension(project.Package.Name));
                    tokens.Add("SharePoint.Package.Id", project.Package.Id.ToString());

                    AssemblyName assemblyName = AssemblyName.GetAssemblyName(project.OutputFullPath);
                    if (assemblyName == null)
                    {
                        project.ProjectService.Logger.ActivateOutputWindow();
                        project.ProjectService.Logger.WriteLine(String.Format("WARNING: Project {0} must be built at least once for all SharePoint tokens to be replaced correctly", dteProject.FullName),
                            LogCategory.Warning);
                    }

                    tokens.Add("SharePoint.Project.AssemblyFullName", assemblyName == null ? "PROJECTNOTBUILT" : assemblyName.FullName);
                    tokens.Add("SharePoint.Project.AssemblyFileName", Path.GetFileName(project.OutputFullPath));
                    tokens.Add("SharePoint.Project.AssemblyFileNameWithoutExtension", Path.GetFileNameWithoutExtension(project.OutputFullPath));
                    tokens.Add("SharePoint.Project.AssemblyPublicKeyToken", assemblyName == null ? "PROJECTNOTBUILT" : DeploymentUtilities.GetPublicKeyToken(assemblyName.FullName));

                    //Get the Guid based replaceable params
                    Dictionary<string, string> guidTokens = DeploymentUtilities.GetReplaceableGuidTokens(project, true);

                    if (guidTokens != null)
                    {
                        if (guidTokens.Count > 0)
                        {
                            tokens.AddRange(guidTokens);
                        }
                    }
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
            QuickCopy(requiresQuickPackage);
        }

        /// <summary>
        /// Quick copy this artefact in the context of the specific package, and the specific containing artefact only.
        /// </summary>
        /// <param name="packageProject">The project.</param>
        /// <param name="parentArtefact">The deployable SharePoint artefact.</param>
        /// <param name="requiresQuickPackage">Flag to indicate it requires a quick package.</param>
        public override void QuickCopy(SharePointPackageArtefact packageProject, QuickCopyableSharePointArtefact parentArtefact, bool requiresQuickPackage)
        {
            QuickCopy(requiresQuickPackage);
        }

        /// <summary>
        /// Quicks the copy.
        /// </summary>
        /// <param name="requiresQuickPackage">if set to <c>true</c> [requires quick package].</param>
        public void QuickCopy(bool requiresQuickPackage)
        {
            project.ProjectService.Logger.ActivateOutputWindow();
            project.ProjectService.Logger.WriteLine("------ Quick Copying Package: " + this.Project.Name + " ------", LogCategory.Status);

            foreach (QuickCopyableSharePointArtefact kid in this.ChildArtefacts)
            {
                kid.QuickCopy(this, this, requiresQuickPackage);
            }
        }

        /// <summary>
        /// Quicks the copy binaries.
        /// </summary>
        /// <param name="requiresQuickPackage">if set to <c>true</c> [requires quick package].</param>
        public void QuickCopyBinaries(bool requiresQuickPackage)
        {
            project.ProjectService.Logger.ActivateOutputWindow();
            project.ProjectService.Logger.WriteLine("------ Quick Copying Binaries: " + project.Name + " ------", LogCategory.Status);
            string currentProjectAssemblyName = Path.GetFileName(project.OutputFullPath);
            bool baseAssemblyCopied = false;
            if (project.IncludeAssemblyInPackage)
            {
                string packageBaseAssemblyPath = this.BasePackagePath;

                if (requiresQuickPackage)
                {
                    // Ensure the source file exists - it won't if the project has never been built.  In this case we
                    // let the CopyToGac() and CopyToBin() methods below handle the error.
                    if (File.Exists(project.OutputFullPath))
                    {
                        // Copy the binary from the source folder (e.g. bin/debug) to the appropriate place in the pkg folder.
                        DeploymentUtilities.CopyFileWithTokenReplacement(this.Project,
                            currentProjectAssemblyName,
                            Path.GetDirectoryName(project.OutputFullPath),
                            packageBaseAssemblyPath,
                            null
                        );
                    }
                }

                string sourceAssembly = Path.Combine(packageBaseAssemblyPath, currentProjectAssemblyName);
                if (project.AssemblyDeploymentTarget == AssemblyDeploymentTarget.GlobalAssemblyCache)
                {
                    DeploymentUtilities.CopyToGac(project, sourceAssembly);
                }
                else
                {
                    DeploymentUtilities.CopyToBin(project, sourceAssembly);
                }
                baseAssemblyCopied = true;
            }

            foreach (IAssembly assembly in project.Package.Model.Assemblies)
            {
                string packageBaseAssemblyPath = this.BasePackagePath;
                string assemblyName = assembly.Location;

                if (requiresQuickPackage)
                {
                    string originalAssemblyPath = null;
                    if (assembly is ICustomAssembly)
                    {
                        ICustomAssembly customAssembly = assembly as ICustomAssembly;
                        originalAssemblyPath = Path.GetDirectoryName(customAssembly.SourcePath);
                    }
                    else if (assembly is IProjectOutputAssembly)
                    {
                        IProjectOutputAssembly poAssembly = assembly as IProjectOutputAssembly;
                        string projectPath = poAssembly.ProjectPath;

                        // Happen if project is manually referenced in package
                        if (string.IsNullOrEmpty(projectPath))
                        {
                            if (currentProjectAssemblyName == poAssembly.Location)
                            {
                                if (baseAssemblyCopied)
                                {
                                    continue;
                                }

                                originalAssemblyPath = project.OutputFullPath;
                            }
                            else
                            {
                                project.ProjectService.Logger.ActivateOutputWindow();
                                project.ProjectService.Logger.WriteLine($"WARNING: can't find project path for {poAssembly.Location}", LogCategory.Warning);
                                continue;
                            }
                        }
                        else
                        {
                            string projPath = Path.GetDirectoryName(projectPath);
                            originalAssemblyPath = Path.Combine(projPath, this.AssemblyPath);
                        }
                    }

                    if (originalAssemblyPath != null)
                    {
                        string originalFullPath = Path.Combine(Path.GetDirectoryName(project.FullPath), originalAssemblyPath);
                        if (File.Exists(Path.Combine(originalFullPath, assemblyName)))
                        {
                            // Copy the binary from the source location (e.g. bin/debug) to the appropriate place in the pkg folder.
                            DeploymentUtilities.CopyFileWithTokenReplacement(this.Project,
                                assemblyName,
                                originalFullPath,
                                packageBaseAssemblyPath,
                                null
                            );
                        }
                    }
                }

                string sourceAssembly = Path.Combine(packageBaseAssemblyPath, assemblyName);
                if (assembly.DeploymentTarget == DeploymentTarget.GlobalAssemblyCache)
                {
                    DeploymentUtilities.CopyToGac(project, sourceAssembly);
                }
                else
                {
                    DeploymentUtilities.CopyToBin(project, sourceAssembly);
                }
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