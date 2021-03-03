using Microsoft.VisualStudio.SharePoint;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace CKS.Dev2015.VisualStudio.SharePoint
{
    static class SPMetalUtilities
    {
        /// <summary>
        /// Runs the SP metal.
        /// </summary>
        /// <param name="siteUrl">The site URL.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="projectService">The project service.</param>
        internal static void RunSPMetal(string siteUrl, string arguments, string fileName, ISharePointProjectService projectService)
        {
            string tempFilePath = Path.Combine(Path.GetTempPath(), fileName);
            File.Delete(tempFilePath);

            System.Diagnostics.Process spmetal = new System.Diagnostics.Process();
            spmetal.StartInfo = new ProcessStartInfo
            {
                Arguments = String.Format("/web:{0} /language:csharp /code:{1} {2}", siteUrl, tempFilePath, arguments),
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                FileName = Path.Combine(projectService.SharePointInstallPath, @"bin\spmetal.exe")
            };
            projectService.Logger.ActivateOutputWindow();
            projectService.Logger.WriteLine("Running SPMetal...", LogCategory.Status);
            projectService.Logger.WriteLine(String.Format("--Starting SPMetal.exe with arguments: {0}", spmetal.StartInfo.Arguments), LogCategory.Verbose);

            spmetal.Start();
            string output = spmetal.StandardOutput.ReadToEnd();
            if (spmetal.ExitCode == 0)
            {
                OutputCode(tempFilePath, projectService.SharePointInstallPath);

                if (!String.IsNullOrEmpty(output))
                {
                    projectService.Logger.WriteLine(output, LogCategory.Message);
                }

                projectService.Logger.WriteLine("SPMetal successfuly executed", LogCategory.Status);
            }
            else
            {
                MessageBox.Show(String.Format("The following error has occured while executing SPMetal:{0}{1}", System.Environment.NewLine, output), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Outputs the code.
        /// </summary>
        /// <param name="tempFilePath">The temp file path.</param>
        /// <param name="sharePointInstallPath">The share point install path.</param>
        internal static void OutputCode(string tempFilePath, string sharePointInstallPath)
        {
            EnvDTE.Project activeProject = DTEManager.ActiveProject;

            if (activeProject != null)
            {
                try
                {
                    activeProject.ProjectItems.AddFromFileCopy(tempFilePath);
                    File.Delete(tempFilePath);

                    // add reference to Microsoft.SharePoint.Linq
                    VSLangProj.VSProject vsProject = (VSLangProj.VSProject)activeProject.Object;
                    vsProject.References.Add(Path.Combine(sharePointInstallPath, @"ISAPI\Microsoft.SharePoint.Linq.dll"));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                string code = File.ReadAllText(tempFilePath);
                File.Delete(tempFilePath);
                DTEManager.CreateNewTextFile(Path.GetFileName(tempFilePath), code);
            }
        }
    }
}
