using EnvDTE;
using EnvDTE80;
using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio.SharePoint;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using VSLangProj;
using Eval = Microsoft.Build.Evaluation;

namespace CKS.Dev.VisualStudio.SharePoint
{
    /// <summary>
    /// Project utilities.
    /// </summary>
    static class ProjectUtilities
    {
        #region Methods

        /// <summary>
        /// Gets the type of the items of.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="projectItemTypeId">The project item type id.</param>
        /// <returns></returns>
        public static IEnumerable<ISharePointProjectItem> GetItemsOfType(ISharePointProject project,
            string projectItemTypeId)
        {
            return project.ProjectItems.Where(
                i => i.ProjectItemType.Id == projectItemTypeId);
        }

        /// <summary>
        /// Gets the name of the assembly.
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyName(ISharePointProject project)
        {
            string outputFullPath = project.OutputFullPath;
            if (File.Exists(outputFullPath))
            {
                return AssemblyName.GetAssemblyName(outputFullPath).FullName;
            }
            return Path.GetFileNameWithoutExtension(outputFullPath);
        }

        /// <summary>
        /// Adds the reference.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="reference">The reference.</param>
        /// <returns>True if the reference got added, false if not</returns>
        public static bool AddReference(EnvDTE.Project project, string reference)
        {
            VSProject proj = project.Object as VSProject;
            System.Diagnostics.Debug.Assert(proj != null); // This project is not a VSProject
            if (proj == null)
            {
                return false;
            }

            try
            {
                proj.References.Add(reference);
            }
            catch (Exception ex)
            {
                string message = String.Format("Could not add {0}. \n Exception: {1}", reference, ex.Message);
                System.Diagnostics.Trace.WriteLine(message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Adds the allow partially trusted callers attribute to the AssemblyInfo.
        /// </summary>
        /// <param name="dteProject">The DTE project.</param>
        public static void AddAllowPartiallyTrustedCallersAttribute(EnvDTE.Project dteProject)
        {
            EnvDTE.ProjectItem item = DTEManager.FindItemByName(dteProject.ProjectItems, "assemblyinfo.cs", true);

            bool contains = false;

            FileCodeModel2 model = (FileCodeModel2)item.FileCodeModel;
            foreach (CodeElement codeElement in model.CodeElements)
            {
                if (ExamineCodeElement(codeElement, "allowpartiallytrustedcallers", 3))
                {
                    contains = true;
                    break;
                }
            }

            //The attribute was not found so make sure it gets added.
            if (!contains)
            {
                model.AddAttribute("AllowPartiallyTrustedCallers", null);
            }
        }

        /// <summary>
        /// Examines the code element.
        /// </summary>
        /// <param name="codeElement">The code element.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="tabs">The tabs.</param>
        /// <returns></returns>
        private static bool ExamineCodeElement(CodeElement codeElement, string elementName, int tabs)
        {
            tabs++;
            try
            {
                Console.WriteLine(new string('\t', tabs) + "{0} {1}",
                    codeElement.Name, codeElement.Kind.ToString());

                // if this is a namespace, add a class to it.
                if (codeElement.Kind == vsCMElement.vsCMElementAttribute)
                {
                    if (codeElement.Name.ToLower() == elementName)
                    {
                        return true;
                    }
                }

                foreach (CodeElement childElement in codeElement.Children)
                {
                    if (ExamineCodeElement(childElement, elementName, tabs))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                Console.WriteLine(new string('\t', tabs) + "codeElement without name: {0}", codeElement.Kind.ToString());
            }
            return false;
        }


        /// <summary>
        /// Gets the HKLM registry key value.
        /// </summary>
        /// <param name="subKeyName">Name of the sub key.</param>
        /// <param name="valueName">Name of the value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        private static string GetHKLMRegistryKeyValue(string subKeyName,
            string valueName,
            string defaultValue)
        {
            string value64 = string.Empty;
            string value32 = string.Empty;
            RegistryKey localKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64);
            localKey = localKey.OpenSubKey(subKeyName);
            if (localKey != null)
            {
                value64 = localKey.GetValue(valueName).ToString();
            }

            RegistryKey localKey32 = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry32);
            localKey32 = localKey32.OpenSubKey(subKeyName);
            if (localKey32 != null)
            {
                value32 = localKey32.GetValue(valueName).ToString();
            }

            if (!String.IsNullOrEmpty(value64))
            {
                return value64;
            }
            else if (!String.IsNullOrEmpty(value32))
            {
                return value32;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Gets the office 14 server install root.
        /// </summary>
        /// <returns></returns>
        public static string GetSharePoint14DesignerInstallRoot()
        {
            return GetHKLMRegistryKeyValue(@"SOFTWARE\Microsoft\Office\14.0\SharePoint Designer\InstallRoot",
                "Path",
                @"C:\Program Files\Microsoft Office\Office14\");
        }

        /// <summary>
        /// Gets the office 15 server install root.
        /// </summary>
        /// <returns></returns>
        public static string GetSharePoint15DesignerInstallRoot()
        {
            //TODO: check this path
            return GetHKLMRegistryKeyValue(@"SOFTWARE\Microsoft\Office\15.0\SharePoint Designer\InstallRoot",
                "Path",
                @"C:\Program Files\Microsoft Office\Office15\");
        }

        /// <summary>
        /// Whiches the share point version is project deploying to.
        /// </summary>
        /// <returns></returns>
        public static SharePointVersion WhichSharePointVersionIsProjectDeployingTo()
        {
            return WhichSharePointVersionIsProjectDeployingTo(DTEManager.ActiveSharePointProject);
        }

        /// <summary>
        /// Whiches the share point version is project deploying to.
        /// </summary>
        /// <returns></returns>
        public static SharePointVersion WhichSharePointVersionIsProjectDeployingTo(ISharePointProject spProject)
        {
            //try
            //{
            //    if (spProject != null)
            //    {
            //        string targetVersion = spProject.TargetOfficeVersion;

            //        if (String.IsNullOrWhiteSpace(targetVersion))
            //        {
            //            if (targetVersion.Contains("14"))
            //            {
            //                return SharePointVersion.SP2010;
            //            }
            //            else if (targetVersion.Contains("15"))
            //            {
            //                return SharePointVersion.SP2013;
            //            }
            //        }
            //        return SharePointVersion.SP2010;
            //    }

            //    return SharePointVersion.SP2010;
            //}
            //catch
            //{
            string sharePointInstallPath = spProject.ProjectService.SharePointInstallPath;

            if (String.IsNullOrWhiteSpace(sharePointInstallPath))
            {
                if (sharePointInstallPath.Contains("/14/"))
                {
                    return SharePointVersion.SP2010;
                }
                else if (sharePointInstallPath.Contains("/15/"))
                {
                    return SharePointVersion.SP2013;
                }

            }
            return SharePointVersion.SP2010;
            //}
        }

        //TODO: look at http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.sharepoint.isharepointproject.projectmode.aspx for the way to know which proj type we're in



        /// <summary>
        /// Gets the name of the safe file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static string GetSafeFileName(string fileName)
        {
            string safeName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(fileName);
            safeName = new Regex(@"[^\w]").Replace(safeName, "");
            return safeName;
        }

        /// <summary>
        /// Gets the share point projects.
        /// </summary>
        /// <param name="isSolutionContext">if set to <c>true</c> [is solution context].</param>
        /// <param name="requireFarm">if set to <c>true</c> [require farm].</param>
        /// <returns></returns>
        public static ISharePointProject[] GetSharePointProjects(bool isSolutionContext, bool requireFarm)
        {
            List<EnvDTE.Project> candidateProjects = new List<EnvDTE.Project>();

            if (isSolutionContext)
            {
                foreach (EnvDTE.Project project in DTEManager.DTE.Solution.Projects)
                {
                    candidateProjects.Add(project);
                }
            }
            else
            {
                foreach (SelectedItem item in DTEManager.DTE.SelectedItems)
                {
                    if (item.Project != null)
                    {
                        candidateProjects.Add(item.Project);
                    }
                }
            }

            List<ISharePointProject> spProjects = new List<ISharePointProject>();

            foreach (EnvDTE.Project project in candidateProjects)
            {
                ISharePointProject spProject = DTEManager.ProjectService.Convert<EnvDTE.Project, ISharePointProject>(project);
                if (spProject != null)
                {
                    if ((!spProject.IsSandboxedSolution) || (!requireFarm))
                    {
                        spProjects.Add(spProject);
                    }
                }
            }

            // no SP project selected, check startup projects
            if (spProjects.Count == 0)
            {
                SolutionBuild solutionBuild = DTEManager.DTE.Solution.SolutionBuild;
                if (solutionBuild != null)
                {
                    Array startupProj = solutionBuild.StartupProjects as Array;
                    if (startupProj != null)
                    {
                        foreach (String projectName in startupProj)
                        {
                            EnvDTE.Project project = DTEManager.GetProjectByName(projectName);
                            if (project != null)
                            {
                                ISharePointProject spProject = DTEManager.ProjectService.Convert<EnvDTE.Project, ISharePointProject>(project);
                                if (spProject != null)
                                {
                                    if ((!spProject.IsSandboxedSolution) || (!requireFarm))
                                    {
                                        spProjects.Add(spProject);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return spProjects.ToArray();
        }

        /// <summary>
        /// Adds a file extension to the TokenReplacementFileExtensions project property
        /// </summary>
        /// <param name="project">The project file to be altered</param>
        /// <param name="extension">The file extension to be added excluding and leading period (ie. svc) </param>
        public static void AddTokenReplacementFileExtension(EnvDTE.Project project, string extension)
        {
            Eval.Project prj = Eval.ProjectCollection.GlobalProjectCollection.GetLoadedProjects(project.FullName).FirstOrDefault();

            if (prj == null)
            {
                prj = new Eval.Project(project.FullName);
            }

            Eval.ProjectProperty prop = prj.GetProperty("TokenReplacementFileExtensions");

            string val;

            if (prop != null)
            {
                List<string> elements = new List<string>(prop.EvaluatedValue.Split(';'));

                List<string> distinctElements = elements.Distinct().ToList();
                distinctElements.RemoveAll(p => p == String.Empty);

                if (!distinctElements.Contains(extension))
                {
                    distinctElements.Add(extension);
                }

                StringBuilder sb = new StringBuilder("$(TokenReplacementFileExtensions);");

                foreach (string item in distinctElements)
                {
                    sb.Append(item + ";");
                }

                val = sb.ToString();
            }
            else
            {
                val = "$(TokenReplacementFileExtensions);" + extension + ";";
            }

            prop = prj.SetProperty("TokenReplacementFileExtensions", val);
        }

        /// <summary>
        /// Gets the token replacement file extension.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns></returns>
        public static List<string> GetTokenReplacementFileExtension(EnvDTE.Project project)
        {
            Eval.Project prj = Eval.ProjectCollection.GlobalProjectCollection.GetLoadedProjects(project.FullName).FirstOrDefault();

            if (prj == null)
            {
                prj = new Eval.Project(project.FullName);
            }

            Eval.ProjectProperty prop = prj.GetProperty("TokenReplacementFileExtensions");

            if (prop != null)
            {
                List<string> elements = new List<string>(prop.EvaluatedValue.Split(';'));

                List<string> distinctElements = elements.Distinct().ToList();
                distinctElements.RemoveAll(p => p == String.Empty);

                return distinctElements;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Determines whether [is loaded share point project selected] [the specified DTE].
        /// </summary>
        /// <param name="dte">The DTE.</param>
        /// <param name="requireFarm">if set to <c>true</c> [require farm].</param>
        /// <returns>
        /// 	<c>true</c> if [is loaded share point project selected] [the specified DTE]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsLoadedSharePointProjectSelected(EnvDTE.DTE dte, bool requireFarm)
        {
            // We must loop to check a project is not only selecetd but loaded.
            foreach (SelectedItem item in dte.SelectedItems)
            {
                // If selected item is a project, and it is loaded, and it is a SharePoint project, then at least one project is selected.
                if ((item.Project != null) && (item.Project.ConfigurationManager != null) && IsSharePointProject(item.Project, requireFarm))
                {
                    return true;
                }
            }
            // no SP project selected, check startup projects
            SolutionBuild solutionBuild = dte.Solution.SolutionBuild;
            if (solutionBuild != null)
            {
                Array startupProj = solutionBuild.StartupProjects as Array;
                if (startupProj != null)
                {
                    foreach (String projectName in startupProj)
                    {
                        EnvDTE.Project project = DTEManager.GetProjectByName(projectName);
                        if ((project != null) && (project.ConfigurationManager != null) && IsSharePointProject(project, requireFarm))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Detects if a project is a SharePoint project based on a list of project type GUIDs.
        /// </summary>
        public static bool IsSharePointProject(EnvDTE.Project project, bool requireFarm)
        {
            //TODO: move this into the project utilities

            try
            {
                ISharePointProjectService projectService = DTEManager.ProjectService;
                if (projectService != null)
                {
                    // Convert the DTE project into a SharePoint project. If the conversion fails, this is not a SP project.
                    ISharePointProject p = projectService.Convert<EnvDTE.Project, Microsoft.VisualStudio.SharePoint.ISharePointProject>(project);
                    if (p != null && requireFarm)
                    {
                        return !p.IsSandboxedSolution;
                    }
                    else
                    {
                        return p != null;
                    }
                }
            }
            catch
            {
                // Must be VERY careful not to throw exceptions in here, since this is called on every click of the context menu.
            }

            return false;
        }

        /// <summary>
        /// Gets the features from feature refs.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="features">The features.</param>
        /// <returns></returns>
        public static IEnumerable<ISharePointProjectFeature> GetFeaturesFromFeatureRefs(ISharePointProject project,
            IEnumerable<ISharePointProjectMemberReference> features)
        {
            string currentProjectPath = Path.GetDirectoryName(project.FullPath);

            List<ISharePointProjectFeature> featuresFromPackage = new List<ISharePointProjectFeature>(features.Count());
            foreach (ISharePointProjectMemberReference featureRef in features)
            {
                string featureProjectPath = String.IsNullOrEmpty(featureRef.ProjectPath) ? project.FullPath : Path.Combine(currentProjectPath, featureRef.ProjectPath);
                featureProjectPath = new DirectoryInfo(featureProjectPath).FullName; // required to get rid of ..\..\ in the project path
                ISharePointProjectFeature feature = GetFeature(project, featureProjectPath, featureRef.ItemId);

                if (feature != null)
                {
                    featuresFromPackage.Add(feature);
                }
            }

            return featuresFromPackage;
        }

        /// <summary>
        /// Gets the feature.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="projectPath">The project path.</param>
        /// <param name="itemId">The item id.</param>
        /// <returns></returns>
        public static ISharePointProjectFeature GetFeature(ISharePointProject project,
            string projectPath,
            Guid itemId)
        {
            ISharePointProjectFeature feature = null;

            ISharePointProject featureProject = project.ProjectService.Projects[projectPath];
            if (featureProject != null)
            {
                feature = (from ISharePointProjectFeature f
                           in featureProject.Features
                           where f.Id == itemId
                           select f).FirstOrDefault();
            }

            return feature;
        }

        /// <summary>
        /// Gets the value from current project.
        /// </summary>
        /// <param name="sharePointProject">The share point project.</param>
        /// <param name="projectPropertyName">Name of the project property.</param>
        /// <returns></returns>
        public static List<string> GetValueFromCurrentProject(ISharePointProject sharePointProject,
            string projectPropertyName)
        {
            List<string> value = null;

            Microsoft.Build.Evaluation.Project project = GetCurrentProject(sharePointProject.FullPath);
            if (project != null)
            {
                string rawValue = project.GetPropertyValue(projectPropertyName);
                if (!String.IsNullOrEmpty(rawValue))
                {
                    value = rawValue.Split(';').ToList();
                }
            }

            return value;
        }

        /// <summary>
        /// Stores the value in current project.
        /// </summary>
        /// <param name="selectedFeaturesIds">The selected features ids.</param>
        /// <param name="sharePointProject">The share point project.</param>
        /// <param name="projectPropertyName">Name of the project property.</param>
        public static void StoreValueInCurrentProject(List<string> selectedFeaturesIds,
            ISharePointProject sharePointProject,
            string projectPropertyName)
        {
            string value = String.Empty;

            if (selectedFeaturesIds != null && selectedFeaturesIds.Count > 0)
            {
                value = String.Join(";", selectedFeaturesIds.ToArray());
            }

            Microsoft.Build.Evaluation.Project project = GetCurrentProject(sharePointProject.FullPath);
            if (project != null)
            {
                project.SetProperty(projectPropertyName, value as string);
            }
        }

        /// <summary>
        /// Gets the current project.
        /// </summary>
        /// <param name="projectFilePath">The project file path.</param>
        /// <returns></returns>
        public static Microsoft.Build.Evaluation.Project GetCurrentProject(string projectFilePath)
        {
            return ProjectCollection.GlobalProjectCollection.GetLoadedProjects(projectFilePath).FirstOrDefault();
        }

        #endregion
    }
}
