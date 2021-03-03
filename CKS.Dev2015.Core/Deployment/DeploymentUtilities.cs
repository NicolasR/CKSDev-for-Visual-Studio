using CKS.Dev2015.VisualStudio.SharePoint.Commands;
using CKS.Dev2015.VisualStudio.SharePoint.Deployment.QuickDeployment;
using EnvDTE;
using Microsoft.VisualStudio.SharePoint;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CKS.Dev2015.VisualStudio.SharePoint.Deployment
{
    class DeploymentUtilities
    {
        private static readonly string[] DefaultTokenizedFiles = new string[] { ".xml", ".ascx", ".aspx", ".webpart", ".dwp", ".ashx" };

        /// <summary>
        /// Returns the last part of a path representing a folder, whether or not the path has
        /// a trailing directory separator.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        public static string GetLastFolderName(string fullPath)
        {
            string path = fullPath + "";
            if (fullPath.EndsWith(Path.DirectorySeparatorChar + ""))
            {
                path = path.Remove(path.Length - 1);
            }
            return Path.GetFileName(path);
        }

        /// <summary>
        /// Checks a file for the ReadOnly attribute, and removes it if it exists.
        /// </summary>
        /// <param name="fullPath"></param>
        public static void EnsureFileIsNotReadOnly(string fullPath)
        {
            FileAttributes attributes = File.GetAttributes(fullPath);
            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                attributes ^= FileAttributes.ReadOnly;
                File.SetAttributes(fullPath, attributes);
            }
        }

        /// <summary>
        /// Returns all SharePoint artefacts for a given ProjectItem.  Multiple items may be returned, for example, if the
        /// selection is a folder.  Note that this list is not pruned in the event of multiple selections from the same sub-tree.
        /// </summary>
        /// <param name="projectService"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static List<QuickCopyableSharePointArtefact> ResolveProjectItemToArtefacts(ISharePointProjectService projectService, ProjectItem item)
        {
            // Ensure we have a SharePoint service.
            if (projectService == null)
            {
                return null;
            }

            // Prepare list for return.
            List<QuickCopyableSharePointArtefact> artefacts = new List<QuickCopyableSharePointArtefact>();

            // See if this item is a SPI file.
            try
            {
                ISharePointProjectItemFile spFile = projectService.Convert<ProjectItem, ISharePointProjectItemFile>(item);
                if (spFile != null)
                {
                    if (spFile.DeploymentType != DeploymentType.NoDeployment)
                    {
                        artefacts.Add(new SharePointProjectItemFileArtefact(spFile));
                        return artefacts;
                    }
                }
            }
            catch { }

            // See if this item is an SPI.
            try
            {
                ISharePointProjectItem spItem = projectService.Convert<ProjectItem, ISharePointProjectItem>(item);
                if (spItem != null)
                {
                    artefacts.Add(new SharePointProjectItemArtefact(spItem));
                    return artefacts;
                }
            }
            catch { }

            // See if this item is a Feature.
            try
            {
                ISharePointProjectFeature spFeature = projectService.Convert<ProjectItem, ISharePointProjectFeature>(item);
                if (spFeature != null)
                {
                    artefacts.Add(new SharePointProjectFeatureArtefact(spFeature));
                    return artefacts;
                }
            }
            catch { }

            // See if we have a Folder, and recursively find SharePoint items.
            try
            {
                if (item.ProjectItems.Count > 0)
                {
                    for (int i = 1; i <= item.ProjectItems.Count; i++)
                    {
                        ProjectItem childItem = item.ProjectItems.Item(i);
                        artefacts.AddRange(ResolveProjectItemToArtefacts(projectService, childItem));
                    }
                }

            }
            catch { }

            // TODO: Also, these items are potential types for conversion.
            // ISharePointProjectFeatureResourceFile
            // ISharePointProjectPackage

            return artefacts;
        }


        /// <summary>
        /// Substitutes the root tokens.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        internal static string SubstituteRootTokens(ISharePointProject project, string name)
        {
            name = name.Replace("{ProjectRoot}", Path.GetDirectoryName(project.FullPath)).Replace("\\\\", "\\");
            name = name.Replace("{SharePointRoot}", project.ProjectService.SharePointInstallPath).Replace("\\\\", "\\");
            // for performance reasons we don't want to execute command unless we really have to replace the token
            if (name.Contains("{WebApplicationRoot}"))
            {
                String webApplicationRoot = project.SharePointConnection.ExecuteCommand<String>("Microsoft.VisualStudio.SharePoint.Commands.GetWebApplicationLocalPath");
                name = name.Replace("{WebApplicationRoot}", webApplicationRoot).Replace("\\\\", "\\");
            }
            return name;
        }

        /// <summary>
        /// Copies the file.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        public static void CopyFile(ISharePointProject project, string fileName, string source, string target)
        {
            // Log the copy using the project and hive-relative formats.
            source = Path.Combine(source, fileName);
            target = Path.Combine(target, fileName);

            project.ProjectService.Logger.ActivateOutputWindow();
            project.ProjectService.Logger.WriteLine(string.Format("{0} -> {1}", source, target), LogCategory.Status);

            // Now replace the project and hive tokens ready for the actual copy.
            string fullSource = SubstituteRootTokens(project, source);
            string fullTarget = SubstituteRootTokens(project, target);

            // Now create the directory and copy the files.
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullTarget));

                if (File.Exists(fullTarget))
                {
                    //Remove any read only attribute.
                    EnsureFileIsNotReadOnly(fullTarget);

                    File.Delete(fullTarget);
                }

                File.Copy(fullSource, fullTarget, true);
                EnsureFileIsNotReadOnly(fullTarget);
            }
            catch (Exception ex)
            {
                project.ProjectService.Logger.ActivateOutputWindow();
                project.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
            }
        }

        /// <summary>
        /// Copies the file with token replacement.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="tokens">The tokens.</param>
        public static void CopyFileWithTokenReplacement(ISharePointProject project,
            string fileName,
            string source,
            string target,
            Dictionary<string, string> tokens)
        {
            // Log the copy using the project and hive-relative formats.
            source = Path.Combine(source, fileName);
            target = Path.Combine(target, fileName);

            // Now replace the project and hive tokens ready for the actual copy.
            string fullSource = SubstituteRootTokens(project, source);
            string fullTarget = SubstituteRootTokens(project, target);

            // Now create the directory and copy the files.
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullTarget));

                if (File.Exists(fullTarget))
                {
                    //Remove any read only attribute.
                    EnsureFileIsNotReadOnly(fullTarget);

                    File.Delete(fullTarget);
                }

                if (tokens == null)
                {
                    File.Copy(fullSource, fullTarget, true);
                    EnsureFileIsNotReadOnly(fullTarget);
                }
                else
                {
                    StreamReader reader = new StreamReader(fullSource);
                    StreamWriter writer = File.CreateText(fullTarget);

                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();

                        // Substitute tokens.
                        foreach (string token in tokens.Keys)
                        {
                            //TODO: put logic into here which checks for $ as well
                            line = line.Replace("$" + token + "$", tokens[token]);
                        }

                        writer.WriteLine(line);
                    }

                    reader.Close();
                    writer.Close();

                    EnsureFileIsNotReadOnly(fullTarget);
                }
            }
            catch (Exception ex)
            {
                project.ProjectService.Logger.ActivateOutputWindow();
                project.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
            }
        }

        /// <summary>
        /// Copies to gac.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="fileName">Name of the file.</param>
        public static void CopyToGac(ISharePointProject project, string fileName)
        {
            try
            {
                project.ProjectService.Logger.ActivateOutputWindow();
                project.ProjectService.Logger.WriteLine(fileName + " -> GAC", LogCategory.Status);


                AssemblyCache.InstallAssembly(SubstituteRootTokens(project, fileName), null, AssemblyCache.AssemblyCommitFlags.Force);
                //new ProcessUtilities(DTEManager.DTE).CopyToGAC(DTEManager.ActiveSharePointProject, DeploymentUtilities.SubstituteRootTokens(DTEManager.ActiveSharePointProject, fileName));

            }
            catch (Exception ex)
            {
                project.ProjectService.Logger.ActivateOutputWindow();
                project.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
            }
        }

        /// <summary>
        /// Copies to bin.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="fileName">Name of the file.</param>
        public static void CopyToBin(ISharePointProject project, string fileName)
        {
            try
            {
                List<string> appPaths = new List<string>();

                appPaths = DeploymentUtilities.GetWebApplicationPhysicalPaths(project.ProjectService, project);

                if (appPaths.Count() <= 0)
                {
                    appPaths.Add(DeploymentUtilities.GetWebApplicationDefaultPhysicalPath(project.ProjectService, project.SiteUrl.ToString()));
                }

                if (appPaths.Count() <= 0)
                {
                    throw new Exception(string.Format("Unable to retrieve Web Application bin path for URL: {0}", project.SiteUrl.ToString()));
                }
                else
                {
                    foreach (string destinationPath in appPaths)
                    {
                        string fullSource = DeploymentUtilities.SubstituteRootTokens(project, fileName);
                        string fullDestination = DeploymentUtilities.SubstituteRootTokens(project, Path.Combine(Path.Combine(destinationPath, "bin"), Path.GetFileName(fileName)));

                        project.ProjectService.Logger.ActivateOutputWindow();
                        project.ProjectService.Logger.WriteLine(fileName + " -> " + fullDestination, LogCategory.Status);

                        // Copy the file, letting exceptions be thrown and caught below (e.g. don't check source path).
                        EnsureFileIsNotReadOnly(fullDestination);

                        File.Copy(fullSource, fullDestination, true);

                        EnsureFileIsNotReadOnly(fullDestination);
                    }
                }

            }
            catch (Exception ex)
            {
                project.ProjectService.Logger.ActivateOutputWindow();
                project.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
            }
        }

        /// <summary>
        /// Gets the assembly.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static Assembly GetAssembly(string file)
        {
            if (File.Exists(file))
            {
                return Assembly.LoadFrom(file);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get the public key token.
        /// </summary>
        /// <param name="fullAssemblyName">The full assembly name.</param>
        /// <returns>The public key token.</returns>
        public static string GetPublicKeyToken(string fullAssemblyName)
        {
            string token = String.Empty;
            if (fullAssemblyName.Contains("PublicKeyToken="))
            {
                token = fullAssemblyName.Substring(fullAssemblyName.IndexOf("PublicKeyToken=") + 15);
                if (token == "null")
                {
                    token = String.Empty;
                }
            }
            return token;
        }

        /// <summary>
        /// Convert a byte array to string.
        /// </summary>
        /// <param name="item">The byte array.</param>
        /// <returns>As a string.</returns>
        public static string ByteArrayToString(byte[] item)
        {
            return new System.Text.UTF8Encoding().GetString(item);
        }

        /// <summary>
        /// Is the token replacement file.
        /// </summary>
        /// <param name="project">The SharePoint project to get the token replacement values for.</param>
        /// <param name="fileName">The file name.</param>
        /// <returns>True if its valid for token replacment.</returns>
        public static bool IsTokenReplacementFile(ISharePointProject project,
            string fileName)
        {
            List<string> fileExtensionsForTokenisation = ProjectUtilities.GetTokenReplacementFileExtension(project.ProjectService.Convert<ISharePointProject, EnvDTE.Project>(project));

            string ext = Path.GetExtension(fileName).ToLower();
            ext = ext.Replace(".", "");

            return fileExtensionsForTokenisation.Contains(ext);
        }

        /// <summary>
        /// Get the web application physical path.
        /// </summary>
        /// <param name="service">The SharePoint service.</param>
        /// <param name="url">The URL.</param>
        /// <returns>
        /// The physical path.
        /// </returns>
        public static string GetWebApplicationDefaultPhysicalPath(ISharePointProjectService service, string url)
        {
            return service.SharePointConnection.ExecuteCommand<string, string>(DeploymentSharePointCommandIds.GetWebApplicationDefaultPhysicalPath, url);
        }

        /// <summary>
        /// Gets the web application physical paths.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="project">The project.</param>
        /// <returns>The web application physical paths.</returns>
        public static List<string> GetWebApplicationPhysicalPaths(ISharePointProjectService service,
            ISharePointProject project)
        {
            string[] binPaths = project.SharePointConnection.ExecuteCommand<string[]>(DeploymentSharePointCommandIds.GetWebApplicationPhysicalPaths);
            return new List<string>(binPaths);
        }

        /// <summary>
        /// Gets the replaceable GUID tokens.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="withMarshalByrefObject">if set to <c>true</c> [with marshal byref object].</param>
        /// <returns>A dictionary of replaceable tokens.</returns>
        public static Dictionary<string, string> GetReplaceableGuidTokens(ISharePointProject project,
            bool withMarshalByrefObject)
        {
            //TODO: put this back maybe once the quick deploy is working
            //    AppDomain tempAppDomain = null;
            //    Dictionary<string, string> tokens = null;

            //    try
            //    {
            //        if (File.Exists(project.OutputFullPath))
            //        {
            //            tempAppDomain = AppDomain.CreateDomain((project.Name.Replace(" ", "")) + "TempAppDomain" + DateTime.Now.Ticks.ToString());
            //            project.ProjectService.Logger.WriteLine("Creating Appdomain " + tempAppDomain.FriendlyName, LogCategory.Message);

            //            project.ProjectService.Logger.WriteLine("Creating AssemblyInspector in Appdomain " + tempAppDomain.FriendlyName, LogCategory.Message);

            //            string assemblyInspectorClassName = string.Empty;

            //            if (withMarshalByrefObject)
            //            {
            //                assemblyInspectorClassName = "AssemblyInspectorWithMarshalByRefObject";
            //            }
            //            else
            //            {
            //                throw new NotImplementedException("The non marshal by will not work for CKSDev applications!");
            //                //assemblyInspectorClassName = "AssemblyInspector";
            //            }

            //            AssemblyName name = new AssemblyName(Assembly.GetExecutingAssembly().FullName);

            //            FileInfo fi = new FileInfo(project.OutputFullPath);
            //            if (fi.LastWriteTime.Ticks < DateTime.Now.AddMinutes(-5).Ticks)
            //            {
            //                project.ProjectService.Logger.ActivateOutputWindow();
            //                project.ProjectService.Logger.WriteLine("Quick Deploy: Project Guid replaceable parameters might be out of date, the assembly is older than 5 minutes.", LogCategory.Warning);
            //            }

            //            //Unpack an instance within the temp domain
            //            object anObject = tempAppDomain.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().Location,
            //                string.Format("CKS.Dev2015.VisualStudio.SharePoint.Deployment.QuickDeployment.{0}", assemblyInspectorClassName));

            //            IAssemblyInspector assemblyInspector = anObject as IAssemblyInspector;

            //            AssemblyInspectorResult result = assemblyInspector.GetReplaceableGuidTokens(project.OutputFullPath);

            //            if (result != null)
            //            {
            //                if (result.Tokens != null)
            //                {
            //                    tokens = result.Tokens;
            //                }
            //                if (result.Messages != null)
            //                {
            //                    if (result.Messages.Count > 0)
            //                    {
            //                        foreach (string msg in result.Messages)
            //                        {
            //                            project.ProjectService.Logger.WriteLine(msg, LogCategory.Message);
            //                        }
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                project.ProjectService.Logger.WriteLine("The type processing yeilded no result for: " + project.OutputFullPath + ", build the project and try again", LogCategory.Warning);
            //            }
            //        }
            //        else
            //        {
            //            project.ProjectService.Logger.WriteLine("The output assembly at: " + project.OutputFullPath + " does not exist, build the project and try again", LogCategory.Warning);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        project.ProjectService.Logger.ActivateOutputWindow();
            //        project.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
            //    }
            //    finally
            //    {
            //        if (tempAppDomain != null)
            //        {
            //            project.ProjectService.Logger.WriteLine("Unloading Appdomain " + tempAppDomain.FriendlyName, LogCategory.Message);
            //            AppDomain.Unload(tempAppDomain);
            //            tempAppDomain = null;
            //        }
            //    }

            //    return tokens;
            return null;
        }
    }
}
