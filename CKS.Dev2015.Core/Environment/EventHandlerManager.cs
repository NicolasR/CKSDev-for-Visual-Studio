using CKS.Dev2015.VisualStudio.SharePoint.Deployment;
using CKS.Dev2015.VisualStudio.SharePoint.Deployment.ProjectProperties;
using CKS.Dev2015.VisualStudio.SharePoint.Deployment.QuickDeployment;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.SharePoint;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using CKSProperties = CKS.Dev2015.Core.Properties.Resources;

namespace CKS.Dev2015.VisualStudio.SharePoint.Environment
{
    /// <summary>
    /// Manager class for registering and responding to various Visual Studio buttons and events.
    /// </summary>
    public class EventHandlerManager
    {
        #region Constants

        /// <summary>
        /// Teh vsProject Item Kind Physical Folder ID
        /// </summary>
        public const string vsProjectItemKindPhysicalFolder = "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}";

        #endregion

        #region Fields

        /// <summary>
        /// Cache MenuCommandService locally for re-use
        /// </summary>
        private OleMenuCommandService mcs = null;

        // Cache DTE objects locally to workaround COM disposing them before they can be called back.        
        private DTE2 m_ApplicationObject2;
        private EnvDTE.DocumentEvents m_DocumentEvents;
        private EnvDTE.BuildEvents m_BuildEvents;
        private _dispDocumentEvents_DocumentSavedEventHandler m_DocumentSavedEventHandler;
        private _dispBuildEvents_OnBuildBeginEventHandler m_BuildBeginEventHandler;
        private _dispBuildEvents_OnBuildDoneEventHandler m_BuildDoneEventHandler;
        private _dispBuildEvents_OnBuildProjConfigDoneEventHandler m_BuildProjConfigDoneHandler;

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets or sets the CKSDEV package.
        /// </summary>
        /// <value>
        /// The CKSDEV package.
        /// </value>
        private ICKSDevVSPackage CKSDEVPackage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the share point project service.
        /// </summary>
        /// <value>
        /// The share point project service.
        /// </value>
        internal ISharePointProjectService CKSDEVPackageSharePointProjectService
        {
            get
            {
                return CKSDEVPackage.GetServiceInternal(typeof(ISharePointProjectService)) as ISharePointProjectService;
            }
        }

        /// <summary>
        /// Gets the DTE.
        /// </summary>
        /// <value>
        /// The DTE.
        /// </value>
        EnvDTE.DTE DTE
        {
            get
            {
                return DTEManager.DTE;
            }
        }

        /// <summary>
        /// Gets the menu command service.
        /// </summary>
        /// <value>
        /// The menu command service.
        /// </value>
        private OleMenuCommandService MenuCommandService
        {
            get
            {
                if (this.mcs == null)
                {
                    this.mcs = this.CKSDEVPackage.GetServiceInternal(typeof(IMenuCommandService)) as OleMenuCommandService;
                }
                return this.mcs;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is share point installed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is share point installed; otherwise, <c>false</c>.
        /// </value>
        private bool IsSharePointInstalled
        {
            get;
            set;
        }

        #endregion

        #region Construction and Handler Registration

        /// <summary>
        /// Creates a new EventHandlerManager with a reference back to the SharePoint CKSDEVPackage.
        /// </summary>
        /// <param name="package"></param>
        public EventHandlerManager(ICKSDevVSPackage package)
        {
            this.CKSDEVPackage = package;
            this.IsSharePointInstalled = CKSDEVPackageSharePointProjectService.IsSharePointInstalled;
        }

        #region Register Handlers

        /// <summary>
        /// Registers the handlers.
        /// </summary>
        public void RegisterHandlers()
        {
            AddPackageHandlers();

            AddQuickDeployMenuHandlers();

            AddCopyToRootHandlers();

            AddCopyBinaryHandlers();

            AddCopyBothHandlers();

            AddRecycleAppPoolHandlers();

            AddRecycleAllAppPoolsHandlers();

            AddRestartIISHandlers();

            AddRestartUserCodeProcessHandlers();

            AddRestartTimerHandlers();

            AddAttachToProcessHandlers();

            AddCopyToRootForFileLevelHandlers();

            AddOpenDeploymentLocationHandlers();

            AddHelpMenuHandlers();

        }

        /// <summary>
        /// Adds the package handlers.
        /// </summary>
        private void AddPackageHandlers()
        {
            // Package All SharePoint Packages.
            AddHandler(PkgCmdIDList.cmdidPackageAllSharePointProjects, new EventHandler(this.PackageAllSharePointProjects_Click), new EventHandler(PackageAllSharePointProjects_BeforeQueryStatus));
        }

        /// <summary>
        /// Adds the quick deploy menu handlers.
        /// </summary>
        private void AddQuickDeployMenuHandlers()
        {
            // Query status handlers for overall Quick Deploy menu.
            AddHandler(PkgCmdIDList.cmdidMnuQuickDeployCtx, null, new EventHandler(QuickDeployContext_BeforeQueryStatus));
            AddHandler(PkgCmdIDList.cmdidMnuQuickDeploySel, null, new EventHandler(QuickDeploySelection_BeforeQueryStatus));
            AddHandler(PkgCmdIDList.cmdidMnuQuickDeploySln, null, new EventHandler(QuickDeploySolution_BeforeQueryStatus));
        }

        /// <summary>
        /// Adds the copy to root handlers.
        /// </summary>
        private void AddCopyToRootHandlers()
        {
            // Copy to Root project, selection (multiple projects) and solution (all projects).
            AddHandler(PkgCmdIDList.cmdidCopySharePointRootCtx, new EventHandler(this.CopySharePointRootContext_Click), new EventHandler(StandardContextDisableForSandboxed_BeforeQueryStatus));
            AddHandler(PkgCmdIDList.cmdidCopySharePointRootSel, new EventHandler(this.CopySharePointRootSelection_Click), new EventHandler(StandardContextDisableForSandboxed_BeforeQueryStatus));
            AddHandler(PkgCmdIDList.cmdidCopySharePointRootSln, new EventHandler(this.CopySharePointRootSolution_Click), new EventHandler(StandardContextDisableForSandboxed_BeforeQueryStatus));
        }

        /// <summary>
        /// Adds the copy binary handlers.
        /// </summary>
        private void AddCopyBinaryHandlers()
        {
            // Copy binary, selection (multiple projects) and solution (all projects).
            AddHandler(PkgCmdIDList.cmdidCopyBinaryCtx, new EventHandler(this.CopyBinaryContext_Click), new EventHandler(StandardContextDisableForSandboxed_BeforeQueryStatus));
            AddHandler(PkgCmdIDList.cmdidCopyBinarySel, new EventHandler(this.CopyBinarySelection_Click), new EventHandler(StandardSelectionDisableForSandboxed_BeforeQueryStatus));
            AddHandler(PkgCmdIDList.cmdidCopyBinarySln, new EventHandler(this.CopyBinarySolution_Click), new EventHandler(StandardSolutionDisableForSandboxed_BeforeQueryStatus));
        }

        /// <summary>
        /// Adds the copy both handlers.
        /// </summary>
        private void AddCopyBothHandlers()
        {
            // Copy both on project, selection (multiple projects) and solution (all projects).
            AddHandler(PkgCmdIDList.cmdidCopyBothCtx, new EventHandler(this.CopyBothContext_Click), new EventHandler(StandardContextDisableForSandboxed_BeforeQueryStatus));
            AddHandler(PkgCmdIDList.cmdidCopyBothSel, new EventHandler(this.CopyBothSelection_Click), new EventHandler(StandardSelectionDisableForSandboxed_BeforeQueryStatus));
            AddHandler(PkgCmdIDList.cmdidCopyBothSln, new EventHandler(this.CopyBothSolution_Click), new EventHandler(StandardSolutionDisableForSandboxed_BeforeQueryStatus));
        }

        /// <summary>
        /// Adds the recycle app pool handlers.
        /// </summary>
        private void AddRecycleAppPoolHandlers()
        {
            // Recycle app pool on project, selection (multiple projects) and solution (all projects).
            AddHandler(PkgCmdIDList.cmdidRecycleAppPoolCtx, new EventHandler(this.RecycleAppPoolContext_Click), new EventHandler(StandardContextFarmOrSandboxed_BeforeQueryStatus));
            AddHandler(PkgCmdIDList.cmdidRecycleAppPoolSel, new EventHandler(this.RecycleAppPoolSelection_Click), new EventHandler(StandardSelectionFarmOrSandboxed_BeforeQueryStatus));
            AddHandler(PkgCmdIDList.cmdidRecycleAppPoolSln, new EventHandler(this.RecycleAppPoolSolution_Click), new EventHandler(StandardSolutionFarmOrSandboxed_BeforeQueryStatus));
        }

        /// <summary>
        /// Adds the recycle all app pools handlers.
        /// </summary>
        private void AddRecycleAllAppPoolsHandlers()
        {
            // Recycle All Application Pools.
            AddHandler(PkgCmdIDList.cmdidRecycleAllAppPools, new EventHandler(this.RecycleAllAppPools_Click), new EventHandler(StandardRequireSharePointInstalled_BeforeQueryStatus));
        }

        /// <summary>
        /// Adds the restart IIS handlers.
        /// </summary>
        private void AddRestartIISHandlers()
        {
            // Restart IIS.
            AddHandler(PkgCmdIDList.cmdidRestartIIS, new EventHandler(this.RestartIIS_Click), new EventHandler(StandardRequireSharePointInstalled_BeforeQueryStatus));
        }

        /// <summary>
        /// Adds the restart user code process handlers.
        /// </summary>
        private void AddRestartUserCodeProcessHandlers()
        {
            // Restart User Code Process.
            AddHandler(PkgCmdIDList.cmdidRestartUserCodeProcess, new EventHandler(this.RestartUserCodeProcess_Click), new EventHandler(RestartUserCodeProcess_BeforeQueryStatus));
        }

        /// <summary>
        /// Adds the restart timer handlers.
        /// </summary>
        private void AddRestartTimerHandlers()
        {
            // Restart OWS Timer Process.
            AddHandler(PkgCmdIDList.cmdidRestartOWSTimerProcess, new EventHandler(this.RestartOWSTimerProcess_Click), new EventHandler(RestartOWSTimerProcess_BeforeQueryStatus));
        }

        /// <summary>
        /// Adds the attach to process handlers.
        /// </summary>
        private void AddAttachToProcessHandlers()
        {
            // Attach to processes.
            AddHandler(PkgCmdIDList.cmdidAttachToAllSharePointProcess, new EventHandler(this.AttachToAllSharePointProcesses_Click), new EventHandler(AttachToAllSharePointProcesses_BeforeQueryStatus));
            AddHandler(PkgCmdIDList.cmdidAttachToIISProcesses, new EventHandler(this.AttachToIISProcesses_Click), new EventHandler(AttachToIISProcesses_BeforeQueryStatus));
            AddHandler(PkgCmdIDList.cmdidAttachToOWSTimerProcess, new EventHandler(this.AttachToOWSTimerProcess_Click), new EventHandler(AttachToOWSTimerProcess_BeforeQueryStatus));
            AddHandler(PkgCmdIDList.cmdidAttachToUserCodeProcess, new EventHandler(this.AttachToUserCodeProcess_Click), new EventHandler(AttachToUserCodeProcess_BeforeQueryStatus));
            AddHandler(PkgCmdIDList.cmdidAttachToVSSPHost5Process, new EventHandler(this.AttachToVSSPHost5Process_Click), new EventHandler(AttachToVSSPHost5Process_BeforeQueryStatus));
        }

        /// <summary>
        /// Adds the copy to root for file level handlers.
        /// </summary>
        private void AddCopyToRootForFileLevelHandlers()
        {
            // Copy to SharePoint Root at file/folder level.
            AddHandler(PkgCmdIDList.cmdidCopySharePointRootFld, new EventHandler(this.CopySharePointRootFolder_Click), new EventHandler(CopySharePointRootFolder_BeforeQueryStatus));
            AddHandler(PkgCmdIDList.cmdidCopySharePointRootFle, new EventHandler(this.CopySharePointRootFile_Click), new EventHandler(CopySharePointRootFile_BeforeQueryStatus));
        }

        /// <summary>
        /// Adds the open deployment location handlers.
        /// </summary>
        private void AddOpenDeploymentLocationHandlers()
        {
            // Open the deployment location folder
            AddHandler(PkgCmdIDList.cmdidOpenDeplFld, new EventHandler(this.OpenDeploymentLocationFolder_Click), new EventHandler(OpenDeploymentLocationFolder_BeforeQueryStatus));
        }

        /// <summary>
        /// Adds the help menu handlers.
        /// </summary>
        private void AddHelpMenuHandlers()
        {
            //Help Menus
            AddHandler(PkgCmdIDList.cmdidCodePlexHome, new EventHandler(this.CodePlexHome_Click), new EventHandler(CodePlexHome_BeforeQueryStatus));
            AddHandler(PkgCmdIDList.cmdidCodePlexDocumentation, new EventHandler(this.CodePlexDocumentation_Click), new EventHandler(CodePlexDocumentation_BeforeQueryStatus));
            AddHandler(PkgCmdIDList.cmdidCodePlexNewFeature, new EventHandler(this.CodePlexNewFeature_Click), new EventHandler(CodePlexNewFeature_BeforeQueryStatus));
            AddHandler(PkgCmdIDList.cmdidCodePlexNewIssue, new EventHandler(this.CodePlexNewIssue_Click), new EventHandler(CodePlexNewIssue_BeforeQueryStatus));
        }

        #endregion


        /// <summary>
        /// Registers the DTE dependent handlers.
        /// </summary>
        /// <param name="dte2">The dte2.</param>
        /// <param name="events">The events.</param>
        public void RegisterDteDependentHandlers(DTE2 dte2, Events2 events)
        {
            m_ApplicationObject2 = dte2;

            m_DocumentEvents = (DocumentEvents)events.DocumentEvents;
            m_DocumentSavedEventHandler = new _dispDocumentEvents_DocumentSavedEventHandler(DocEvents_DocumentSaved);
            m_DocumentEvents.DocumentSaved += m_DocumentSavedEventHandler;

            m_BuildEvents = (BuildEvents)events.BuildEvents;
            m_BuildBeginEventHandler = new _dispBuildEvents_OnBuildBeginEventHandler(BuildEvents_OnBuildBegin);
            m_BuildEvents.OnBuildBegin += m_BuildBeginEventHandler;
            m_BuildDoneEventHandler = new _dispBuildEvents_OnBuildDoneEventHandler(BuildEvents_OnBuildDone);
            m_BuildEvents.OnBuildDone += m_BuildDoneEventHandler;
            m_BuildProjConfigDoneHandler = new _dispBuildEvents_OnBuildProjConfigDoneEventHandler(BuildEvents_OnBuildProjConfigDone);
            m_BuildEvents.OnBuildProjConfigDone += m_BuildProjConfigDoneHandler;
        }

        /// <summary>
        /// Adds the handler.
        /// </summary>
        /// <param name="commandID">The command ID.</param>
        /// <param name="commandHandler">The command handler.</param>
        /// <param name="queryHandler">The query handler.</param>
        /// <returns></returns>
        private OleMenuCommand AddHandler(uint commandID,
            EventHandler commandHandler,
            EventHandler queryHandler)
        {
            CommandID cmdID = new CommandID(GuidList.guidCKSDEVCmdSet, (int)commandID);
            OleMenuCommand cmdOmc = new OleMenuCommand(commandHandler, cmdID);
            if (queryHandler != null)
            {
                cmdOmc.BeforeQueryStatus += queryHandler;
            }

            this.MenuCommandService.AddCommand(cmdOmc);
            return cmdOmc;
        }

        //TODO: see if this ever gets called
        //private OleMenuCommand FindCommand(uint commandID)
        //{
        //    CommandID cmdID = new CommandID(GuidList.guidCKSDEVCmdSet, (int)commandID);
        //    return this.MenuCommandService.FindCommand(cmdID) as OleMenuCommand;
        //}

        #endregion

        #region Package All SharePoint Projects

        /// <summary>
        /// Handles the BeforeQueryStatus event of the PackageAllSharePointProjects control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void PackageAllSharePointProjects_BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand cmd = sender as OleMenuCommand;
            if (null != cmd)
            {
                EnvDTE.DTE dte = this.DTE;

                cmd.Visible = cmd.Supported;
                cmd.Enabled = IsSharePointInstalled && IsNotBuilding(dte);

                if (cmd.Enabled)
                {
                    // Only show the command if SharePoint projects exist.
                    try
                    {
                        if (DTEManager.SharePointProjects.Count > 0)
                        {
                            cmd.Enabled = true;
                        }
                        else
                        {
                            cmd.Enabled = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        DTEManager.ProjectService.Logger.ActivateOutputWindow();
                        DTEManager.ProjectService.Logger.WriteLine(ex.ToString(), LogCategory.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the PackageAllSharePointProjects menu.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void PackageAllSharePointProjects_Click(object sender, EventArgs arguments)
        {
            try
            {
                DTEManager.ProjectService.Logger.ActivateOutputWindow();
                foreach (ISharePointProject spProject in DTEManager.SharePointProjects)
                {
                    DTEManager.ProjectService.Logger.WriteLine("Packaging: " + spProject.Name, LogCategory.Status);
                    spProject.Package.BuildPackage();
                }
            }
            catch (Exception ex)
            {
                DTEManager.ProjectService.Logger.ActivateOutputWindow();
                DTEManager.ProjectService.Logger.WriteLine(ex.ToString(), LogCategory.Error);
            }
        }

        #endregion

        #region Standard Command Handlers

        /// <summary>
        /// Handles the BeforeQueryStatus event of the StandardRequireSharePointInstalled control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void StandardRequireSharePointInstalled_BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand cmd = sender as OleMenuCommand;
            if (null != cmd)
            {
                cmd.Visible = cmd.Supported;
                cmd.Enabled = cmd.Visible && IsSharePointInstalled;
            }
        }

        /// <summary>
        /// Handles the BeforeQueryStatus event of the StandardContextDisableForSandboxed control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void StandardContextDisableForSandboxed_BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand cmd = sender as OleMenuCommand;
            if (null != cmd)
            {
                EnvDTE.DTE dte = this.DTE;
                cmd.Visible = cmd.Supported && IsSharePointInstalled && ProjectUtilities.IsLoadedSharePointProjectSelected(dte, false);
                cmd.Enabled = cmd.Visible && IsNotBuilding(dte) && ProjectUtilities.IsLoadedSharePointProjectSelected(dte, true);
            }
        }

        /// <summary>
        /// Handles the BeforeQueryStatus event of the StandardSolutionDisableForSandboxed control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void StandardSolutionDisableForSandboxed_BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand cmd = sender as OleMenuCommand;
            if (null != cmd)
            {
                EnvDTE.DTE dte = this.DTE;
                cmd.Visible = cmd.Supported && IsSharePointInstalled && DoLoadedSharePointProjectsExist(dte, false);
                cmd.Enabled = cmd.Visible && IsNotBuilding(dte) && DoLoadedSharePointProjectsExist(dte, true);
            }
        }

        /// <summary>
        /// Handles the BeforeQueryStatus event of the StandardSelectionDisableForSandboxed control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void StandardSelectionDisableForSandboxed_BeforeQueryStatus(object sender, EventArgs e)
        {
            StandardContextDisableForSandboxed_BeforeQueryStatus(sender, e);
        }

        /// <summary>
        /// Handles the BeforeQueryStatus event of the StandardContextFarmOrSandboxed control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void StandardContextFarmOrSandboxed_BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand cmd = sender as OleMenuCommand;
            if (null != cmd)
            {
                EnvDTE.DTE dte = this.DTE;
                cmd.Visible = cmd.Supported && IsSharePointInstalled && ProjectUtilities.IsLoadedSharePointProjectSelected(dte, false);
                cmd.Enabled = cmd.Visible && IsNotBuilding(dte);
            }
        }

        /// <summary>
        /// Handles the BeforeQueryStatus event of the StandardSolutionFarmOrSandboxed control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void StandardSolutionFarmOrSandboxed_BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand cmd = sender as OleMenuCommand;
            if (null != cmd)
            {
                EnvDTE.DTE dte = this.DTE;
                cmd.Visible = cmd.Supported && IsSharePointInstalled && DoLoadedSharePointProjectsExist(dte, false);
                cmd.Enabled = cmd.Visible && IsNotBuilding(dte);
            }
        }

        /// <summary>
        /// Handles the BeforeQueryStatus event of the StandardSelectionFarmOrSandboxed control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void StandardSelectionFarmOrSandboxed_BeforeQueryStatus(object sender, EventArgs e)
        {
            StandardContextFarmOrSandboxed_BeforeQueryStatus(sender, e);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Updates the selection text.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="suffix">The suffix.</param>
        private void UpdateSelectionText(OleMenuCommand cmd, string prefix, string suffix)
        {
            EnvDTE.DTE dte = this.DTE;

            try
            {
                // As per internal VS behaviour, add project name if only one project is selected and it is loaded.
                Array asp = ((Array)dte.ActiveSolutionProjects);
                string name = "Selection";
                if (asp.Length == 1)
                {
                    EnvDTE.Project activeProject = (EnvDTE.Project)asp.GetValue(0);
                    if ((activeProject.ConfigurationManager != null) && (ProjectUtilities.IsSharePointProject(activeProject, false)))
                    {
                        // Only loaded projects have a configuration manager.
                        // We also only apply this text if it is a SharePoint project for consistency with VS itself.
                        name = activeProject.Name;
                    }
                }
                cmd.Text = prefix + " " + name + " " + suffix;
            }
            catch (COMException)
            {
            }
        }

        #endregion

        #region Quick Deploy Menu Handlers

        /// <summary>
        /// Handles the BeforeQueryStatus event of the QuickDeployContext control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void QuickDeployContext_BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand cmd = sender as OleMenuCommand;
            if (null != cmd)
            {
                EnvDTE.DTE dte = this.DTE;

                // Quick Deploy menu is only shown if a SP project is selected, but we do not require it to be a farm solution
                // as some of the sub-commands can operate on sandboxed solutions.  We also show it if building, but require
                // that the sub-commands disable themselves.
                cmd.Visible = cmd.Supported && IsSharePointInstalled && ProjectUtilities.IsLoadedSharePointProjectSelected(dte, false);
                cmd.Enabled = cmd.Visible;
            }
        }

        /// <summary>
        /// Handles the BeforeQueryStatus event of the QuickDeploySolution control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void QuickDeploySolution_BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand cmd = sender as OleMenuCommand;
            if (null != cmd)
            {
                // Quick Deploy menu is only shown if a SP project is selected, but we do not require it to be a farm solution
                // as some of the sub-commands can operate on sandboxed solutions.  We also show it if building, but require
                // that the sub-commands disable themselves.
                EnvDTE.DTE dte = this.DTE;
                cmd.Visible = cmd.Supported && IsSharePointInstalled && DoLoadedSharePointProjectsExist(dte, false);
                cmd.Enabled = cmd.Visible;
            }
        }

        /// <summary>
        /// Handles the BeforeQueryStatus event of the QuickDeploySelection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void QuickDeploySelection_BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand cmd = sender as OleMenuCommand;
            if (null != cmd)
            {
                // Quick Deploy menu is only shown if a SP project is selected, but we do not require it to be a farm solution
                // as some of the sub-commands can operate on sandboxed solutions.  We also show it if building, but require
                // that the sub-commands disable themselves.
                cmd.Visible = cmd.Supported && IsSharePointInstalled && ProjectUtilities.IsLoadedSharePointProjectSelected(DTE, false);
                cmd.Enabled = cmd.Visible;
                this.UpdateSelectionText(cmd, "Quick Deploy", "(CKSDEV)");
            }
        }

        #endregion

        #region Copy to Root (File + Folder) Handlers

        private void CopySharePointRootFile(object sender)
        {
            ISharePointProjectService projectService = CKSDEVPackageSharePointProjectService;
            OleMenuCommand cmd = sender as OleMenuCommand;
            if (null != cmd)
            {
                // Default to not show this command at all.
                cmd.Visible = cmd.Enabled = false;

                // Determine if we have at least one deployable SharePoint item - return as soon as we find one.
                EnvDTE.DTE dte = this.DTE;
                foreach (SelectedItem item in dte.SelectedItems)
                {
                    // Only respond if we actually have a project item (i.e. not to projects).
                    if (item.ProjectItem != null)
                    {
                        if (projectService != null && projectService.IsSharePointInstalled)
                        {
                            List<QuickCopyableSharePointArtefact> artefacts = DeploymentUtilities.ResolveProjectItemToArtefacts(projectService, item.ProjectItem);
                            if ((artefacts != null) && (artefacts.Count > 0))
                            {
                                // We have a quick copyable SharePoint file, so at least show the command.
                                cmd.Visible = true;

                                // For now, just enable the command evne if the item is not included in the package.
                                // This is for performance reasons - we should error if it is not on click.
                                cmd.Enabled = true;
                                return;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the BeforeQueryStatus event of the CopySharePointRootFolder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CopySharePointRootFolder_BeforeQueryStatus(object sender, EventArgs e)
        {
            this.CopySharePointRootFile(sender);
        }

        /// <summary>
        /// Handles the BeforeQueryStatus event of the CopySharePointRootFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CopySharePointRootFile_BeforeQueryStatus(object sender, EventArgs e)
        {
            this.CopySharePointRootFile(sender);
        }

        /// <summary>
        /// Handles the BeforeQueryStatus event of the OpenDeploymentLocationFolder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OpenDeploymentLocationFolder_BeforeQueryStatus(object sender, EventArgs e)
        {
            ISharePointProjectService projectService = CKSDEVPackageSharePointProjectService;

            OleMenuCommand cmd = sender as OleMenuCommand;
            if (null != cmd)
            {
                // Default to not show this command at all.
                cmd.Visible = cmd.Enabled = false;

                // Determine if we have a single item selected
                EnvDTE.DTE dte = this.DTE;
                if (dte.SelectedItems.Count == 1)
                {
                    // 1-based index!
                    SelectedItem item = dte.SelectedItems.Item(1);
                    // Only respond if we actually have a project item (i.e. not to projects).
                    if ((item != null) && (item.ProjectItem != null))
                    {
                        if (projectService != null && projectService.IsSharePointInstalled)
                        {
                            bool found = false;
                            ISharePointProjectItemFile spFile = projectService.Convert<ProjectItem, ISharePointProjectItemFile>(item.ProjectItem);
                            if ((spFile != null) && (!String.IsNullOrEmpty(spFile.DeploymentPath)))
                            {
                                found = true;
                            }
                            else
                            {
                                IMappedFolder spFolder = projectService.Convert<ProjectItem, IMappedFolder>(item.ProjectItem);
                                if (spFolder != null)
                                {
                                    found = true;
                                }
                            }

                            ProjectItem parentProjItem = item.ProjectItem.Collection.Parent as ProjectItem;
                            String folderPath = String.Empty;
                            while ((!found) && (parentProjItem != null))
                            {
                                IMappedFolder spFolder = projectService.Convert<ProjectItem, IMappedFolder>(parentProjItem);
                                if (spFolder != null)
                                {
                                    found = true;
                                }
                                parentProjItem = parentProjItem.Collection.Parent as ProjectItem;
                            }

                            cmd.Visible = found;
                            cmd.Enabled = found;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the OpenDeploymentLocationFolder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OpenDeploymentLocationFolder_Click(object sender, EventArgs arguments)
        {
            ISharePointProjectService projectService = CKSDEVPackageSharePointProjectService;

            // Convert the DTE project into a SharePoint project. If the conversion fails, this is not a SP project.
            EnvDTE.DTE dte = this.DTE;
            if (dte.SelectedItems.Count == 1)
            {
                // 1-based index!
                SelectedItem item = dte.SelectedItems.Item(1);
                // Only respond if we actually have a project item (i.e. not to projects).
                if ((item != null) && (item.ProjectItem != null))
                {
                    if (projectService != null && projectService.IsSharePointInstalled)
                    {
                        String path = null;

                        ISharePointProjectItemFile spFile = projectService.Convert<ProjectItem, ISharePointProjectItemFile>(item.ProjectItem);
                        if ((spFile != null) && (!String.IsNullOrEmpty(spFile.DeploymentPath)))
                        {
                            path = Path.Combine(DeploymentUtilities.SubstituteRootTokens(spFile.Project, spFile.DeploymentRoot), spFile.DeploymentPath);
                        }
                        else
                        {
                            IMappedFolder spFolder = projectService.Convert<ProjectItem, IMappedFolder>(item.ProjectItem);
                            if (spFolder != null)
                            {
                                path = Path.Combine(DeploymentUtilities.SubstituteRootTokens(spFolder.Project, "{SharePointRoot}"), spFolder.DeploymentLocation);
                            }
                        }

                        ProjectItem projItem = item.ProjectItem;
                        String folderPath = String.Empty;
                        while ((String.IsNullOrEmpty(path)) && (projItem != null))
                        {

                            IMappedFolder spFolder = projectService.Convert<ProjectItem, IMappedFolder>(projItem);
                            if (spFolder != null)
                            {
                                path = Path.Combine(DeploymentUtilities.SubstituteRootTokens(spFolder.Project, "{SharePointRoot}"), spFolder.DeploymentLocation, folderPath);
                            }

                            if (projItem.Kind == vsProjectItemKindPhysicalFolder)
                            {
                                folderPath = projItem.Name + "\\" + folderPath;
                            }

                            projItem = projItem.Collection.Parent as ProjectItem;
                        }

                        if (!String.IsNullOrEmpty(path))
                        {
                            OpenDeploymentLocationFolder(path);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Opens the deployment location folder
        /// </summary>
        /// <param name="path">Path of the folder to be opened.</param>
        public void OpenDeploymentLocationFolder(String path)
        {
            //TODO: refactor this into the process utilities class as its more about starting the process
            if (Directory.Exists(path))
            {
                System.Diagnostics.Process explorer = new System.Diagnostics.Process();
                explorer.StartInfo.FileName = "explorer.exe";
                explorer.StartInfo.Arguments = path;
                explorer.Start();
            }
            else
            {
                ISharePointProjectService projectService = CKSDEVPackageSharePointProjectService;
                projectService.Logger.ActivateOutputWindow();
                projectService.Logger.WriteLine(String.Format("Path '{0}' not found.", path), LogCategory.Status);
            }
        }

        #endregion

        #region CopySharePointRoot Command Handlers

        /// <summary>
        /// Handles the Click event of the CopySharePointRootFolder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CopySharePointRootFolder_Click(object sender, EventArgs arguments)
        {
            CustomDeploySelectedItems();
        }

        /// <summary>
        /// Handles the Click event of the CopySharePointRootFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CopySharePointRootFile_Click(object sender, EventArgs arguments)
        {
            CustomDeploySelectedItems();
        }

        /// <summary>
        /// Handles the Click event of the CopySharePointRootContext control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void CopySharePointRootContext_Click(object sender, EventArgs arguments)
        {
            //Comes from the click on the project within the solution explorer
            this.CustomDeploy(DeployType.CopySharePointRoot, false);
        }

        /// <summary>
        /// Handles the Click event of the CopySharePointRootSolution control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void CopySharePointRootSolution_Click(object sender, EventArgs arguments)
        {
            //Comes from the build menu click
            this.CustomDeploy(DeployType.CopySharePointRoot, true);
        }

        /// <summary>
        /// Handles the Click event of the CopySharePointRootSelection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void CopySharePointRootSelection_Click(object sender, EventArgs arguments)
        {
            this.CustomDeploy(DeployType.CopySharePointRoot, false);
        }

        #endregion

        #region CopyBinary Command Handlers

        /// <summary>
        /// Handles the Click event of the CopyBinaryContext control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void CopyBinaryContext_Click(object sender, EventArgs arguments)
        {
            this.CustomDeploy(DeployType.CopyBinary, false);
        }

        /// <summary>
        /// Handles the Click event of the CopyBinarySolution control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void CopyBinarySolution_Click(object sender, EventArgs arguments)
        {
            this.CustomDeploy(DeployType.CopyBinary, true);
        }

        /// <summary>
        /// Handles the Click event of the CopyBinarySelection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void CopyBinarySelection_Click(object sender, EventArgs arguments)
        {
            this.CustomDeploy(DeployType.CopyBinary, false);
        }

        #endregion

        #region CopyBoth Command Handlers

        /// <summary>
        /// Handles the Click event of the CopyBothContext control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void CopyBothContext_Click(object sender, EventArgs arguments)
        {
            this.CustomDeploy(DeployType.CopyBoth, false);
        }

        /// <summary>
        /// Handles the Click event of the CopyBothSolution control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void CopyBothSolution_Click(object sender, EventArgs arguments)
        {
            this.CustomDeploy(DeployType.CopyBoth, true);
        }

        /// <summary>
        /// Handles the Click event of the CopyBothSelection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void CopyBothSelection_Click(object sender, EventArgs arguments)
        {
            this.CustomDeploy(DeployType.CopyBoth, false);
        }

        #endregion

        #region Attach to Processes Command Handlers

        /// <summary>
        /// Sets the attach to process command visibility.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="processName">Name of the process.</param>
        private void SetAttachToProcessCommandVisibility(object sender, string processName)
        {

            OleMenuCommand cmd = sender as OleMenuCommand;
            if (null != cmd)
            {
                EnvDTE.DTE dte = this.DTE;

                cmd.Visible = cmd.Supported;
                cmd.Enabled = IsSharePointInstalled && IsNotBuilding(dte);

                if (cmd.Enabled)
                {
                    // Only show the command if worker processes exist.
                    try
                    {
                        ProcessUtilities utils = new ProcessUtilities(dte);
                        cmd.Enabled = utils.IsProcessAvailableByName(processName);
                    }
                    catch (Exception ex)
                    {
                        DTEManager.ProjectService.Logger.ActivateOutputWindow();
                        DTEManager.ProjectService.Logger.WriteLine(ex.ToString(), LogCategory.Error);
                    }
                }
            }
        }

        #region Attach to All SharePoint Processes

        /// <summary>
        /// Handles the BeforeQueryStatus event of the AttachToAllSharePointProcesses control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AttachToAllSharePointProcesses_BeforeQueryStatus(object sender, EventArgs e)
        {
            //Assume if IIS is ok then we will show attach to all
            SetAttachToProcessCommandVisibility(sender, ProcessConstants.IISWorkerProcess);
        }

        /// <summary>
        /// Handles the Click event of the AttachToAllSharePointProcesses menu.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AttachToAllSharePointProcesses_Click(object sender, EventArgs arguments)
        {
            SharePointVersion version = ProjectUtilities.WhichSharePointVersionIsProjectDeployingTo();
            if (version == SharePointVersion.SP2010)
            {
                AttachToAllSharePoint2010Processes();
            }
            else if (version == SharePointVersion.SP2013)
            {
                AttachToAllSharePoint2013Processes();
            }
        }

        /// <summary>
        /// Attaches to all SharePoint 2010 processes.
        /// </summary>
        private void AttachToAllSharePoint2010Processes()
        {
            ProcessUtilities utils = new ProcessUtilities(this.DTE);

            //Try attach to the iis worker processes
            if (utils.IsProcessAvailableByName(ProcessConstants.IISWorkerProcess))
            {
                try
                {
                    DTEManager.ProjectService.Logger.ActivateOutputWindow();
                    DTEManager.ProjectService.Logger.WriteLine("========== Attaching to IIS Worker Process ==========", LogCategory.Status);

                    DTEManager.SetStatus("Attaching to IIS Worker Process...");

                    new ProcessUtilities(this.DTE).AttachToProcessByName(ProcessConstants.IISWorkerProcess);

                    DTEManager.SetStatus("Attaching to IIS Worker Process... All Done!");
                    DTEManager.ProjectService.Logger.WriteLine("Done!", LogCategory.Status);
                }
                catch (Exception ex)
                {
                    DTEManager.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
                    DTEManager.SetStatus("Attaching to IIS Worker Process.. ERROR!");
                }
            }

            //Try attach to the user code process
            if (utils.IsProcessAvailableByName(ProcessConstants.SPUCWorkerProcess))
            {
                try
                {
                    DTEManager.ProjectService.Logger.ActivateOutputWindow();
                    DTEManager.ProjectService.Logger.WriteLine("========== Attaching to User Code Process ==========", LogCategory.Status);

                    DTEManager.SetStatus("Attaching to User Code Process...");

                    new ProcessUtilities(this.DTE).AttachToProcessByName(ProcessConstants.SPUCWorkerProcess);

                    DTEManager.SetStatus("Attaching to User Code Process... All Done!");
                    DTEManager.ProjectService.Logger.WriteLine("Done!", LogCategory.Status);
                }
                catch (Exception ex)
                {
                    DTEManager.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
                    DTEManager.SetStatus("Attaching to User Code Process.. ERROR!");
                }
            }

            //Try attach to the OWS timer process
            if (utils.IsProcessAvailableByName(ProcessConstants.OWSTimerProcess))
            {
                try
                {
                    DTEManager.ProjectService.Logger.ActivateOutputWindow();
                    DTEManager.ProjectService.Logger.WriteLine("========== Attaching to Timer Service Process ==========", LogCategory.Status);

                    DTEManager.SetStatus("Attaching to Timer Service Process...");

                    new ProcessUtilities(this.DTE).AttachToProcessByName(ProcessConstants.OWSTimerProcess);

                    DTEManager.SetStatus("Attaching to Timer Service Process... All Done!");
                    DTEManager.ProjectService.Logger.WriteLine("Done!", LogCategory.Status);
                }
                catch (Exception ex)
                {
                    DTEManager.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
                    DTEManager.SetStatus("Attaching to Timer Service Process.. ERROR!");
                }
            }

            //Try attach to the VSSPHost5 processes
            if (utils.IsProcessAvailableByName(ProcessConstants.VSSHost5Process))
            {
                try
                {
                    DTEManager.ProjectService.Logger.ActivateOutputWindow();
                    DTEManager.ProjectService.Logger.WriteLine("========== Attaching to VSShost5 Process ==========", LogCategory.Status);

                    DTEManager.SetStatus("Attaching to VSShost5 Process...");

                    new ProcessUtilities(this.DTE).AttachToProcessByName(ProcessConstants.VSSHost5Process);

                    DTEManager.SetStatus("Attaching to VSShost5 Process... All Done!");
                    DTEManager.ProjectService.Logger.WriteLine("Done!", LogCategory.Status);
                }
                catch (Exception ex)
                {
                    DTEManager.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
                    DTEManager.SetStatus("Attaching to VSShost5 Process.. ERROR!");
                }
            }
        }

        /// <summary>
        /// Attaches to all SharePoint 2013 processes.
        /// </summary>
        private void AttachToAllSharePoint2013Processes()
        {
            ProcessUtilities utils = new ProcessUtilities(this.DTE);

            //Try attach to the iis worker processes
            if (utils.IsProcessAvailableByName(ProcessConstants.IISWorkerProcess))
            {
                try
                {
                    DTEManager.ProjectService.Logger.ActivateOutputWindow();
                    DTEManager.ProjectService.Logger.WriteLine("========== Attaching to IIS Worker Process ==========", LogCategory.Status);

                    DTEManager.SetStatus("Attaching to IIS Worker Process...");

                    new ProcessUtilities(this.DTE).AttachToProcessByName(ProcessConstants.IISWorkerProcess);

                    DTEManager.SetStatus("Attaching to IIS Worker Process... All Done!");
                    DTEManager.ProjectService.Logger.WriteLine("Done!", LogCategory.Status);
                }
                catch (Exception ex)
                {
                    DTEManager.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
                    DTEManager.SetStatus("Attaching to IIS Worker Process.. ERROR!");
                }
            }

            //Try attach to the user code process
            if (utils.IsProcessAvailableByName(ProcessConstants.SPUCWorkerProcess))
            {
                try
                {
                    DTEManager.ProjectService.Logger.ActivateOutputWindow();
                    DTEManager.ProjectService.Logger.WriteLine("========== Attaching to User Code Process ==========", LogCategory.Status);

                    DTEManager.SetStatus("Attaching to User Code Process...");

                    new ProcessUtilities(this.DTE).AttachToProcessByName(ProcessConstants.SPUCWorkerProcess);

                    DTEManager.SetStatus("Attaching to User Code Process... All Done!");
                    DTEManager.ProjectService.Logger.WriteLine("Done!", LogCategory.Status);
                }
                catch (Exception ex)
                {
                    DTEManager.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
                    DTEManager.SetStatus("Attaching to User Code Process.. ERROR!");
                }
            }

            //Try attach to the OWS timer process
            if (utils.IsProcessAvailableByName(ProcessConstants.OWSTimerProcess))
            {
                try
                {
                    DTEManager.ProjectService.Logger.ActivateOutputWindow();
                    DTEManager.ProjectService.Logger.WriteLine("========== Attaching to Timer Service Process ==========", LogCategory.Status);

                    DTEManager.SetStatus("Attaching to Timer Service Process...");

                    new ProcessUtilities(this.DTE).AttachToProcessByName(ProcessConstants.OWSTimerProcess);

                    DTEManager.SetStatus("Attaching to Timer Service Process... All Done!");
                    DTEManager.ProjectService.Logger.WriteLine("Done!", LogCategory.Status);
                }
                catch (Exception ex)
                {
                    DTEManager.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
                    DTEManager.SetStatus("Attaching to Timer Service Process.. ERROR!");
                }
            }

            //Try attach to the VSSPHost5 processes
            if (utils.IsProcessAvailableByName(ProcessConstants.VSSHost5Process))
            {
                try
                {
                    DTEManager.ProjectService.Logger.ActivateOutputWindow();
                    DTEManager.ProjectService.Logger.WriteLine("========== Attaching to VSShost5 Process ==========", LogCategory.Status);

                    DTEManager.SetStatus("Attaching to VSShost5 Process...");

                    new ProcessUtilities(this.DTE).AttachToProcessByName(ProcessConstants.VSSHost5Process);

                    DTEManager.SetStatus("Attaching to VSShost5 Process... All Done!");
                    DTEManager.ProjectService.Logger.WriteLine("Done!", LogCategory.Status);
                }
                catch (Exception ex)
                {
                    DTEManager.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
                    DTEManager.SetStatus("Attaching to VSShost5 Process.. ERROR!");
                }
            }
        }

        #endregion

        #region Attach to IIS Processes

        /// <summary>
        /// Handles the BeforeQueryStatus event of the AttachToIISProcesses control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AttachToIISProcesses_BeforeQueryStatus(object sender, EventArgs e)
        {
            SetAttachToProcessCommandVisibility(sender, ProcessConstants.IISWorkerProcess);
        }

        /// <summary>
        /// Handles the Click event of the AttachToIISProcesses menu.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AttachToIISProcesses_Click(object sender, EventArgs arguments)
        {
            SharePointVersion version = ProjectUtilities.WhichSharePointVersionIsProjectDeployingTo();
            if (version == SharePointVersion.SP2010)
            {
                AttachToIISProcessesSharePoint2010();
            }
            else if (version == SharePointVersion.SP2013)
            {
                AttachToIISProcessesSharePoint2013();
            }
        }

        /// <summary>
        /// Attaches to IIS processes SharePoint 2010.
        /// </summary>
        private void AttachToIISProcessesSharePoint2010()
        {
            try
            {
                DTEManager.ProjectService.Logger.ActivateOutputWindow();
                DTEManager.ProjectService.Logger.WriteLine("========== Attaching to IIS Worker Process ==========", LogCategory.Status);

                DTEManager.SetStatus("Attaching to IIS Worker Process...");

                new ProcessUtilities(this.DTE).AttachToProcessByName(ProcessConstants.IISWorkerProcess);

                DTEManager.SetStatus("Attaching to IIS Worker Process... All Done!");
                DTEManager.ProjectService.Logger.WriteLine("Done!", LogCategory.Status);
            }
            catch (Exception ex)
            {
                DTEManager.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
                DTEManager.SetStatus("Attaching to IIS Worker Process.. ERROR!");
            }
        }

        /// <summary>
        /// Attaches to IIS processes SharePoint 2013.
        /// </summary>
        private void AttachToIISProcessesSharePoint2013()
        {
            try
            {
                DTEManager.ProjectService.Logger.ActivateOutputWindow();
                DTEManager.ProjectService.Logger.WriteLine("========== Attaching to IIS Worker Process ==========", LogCategory.Status);

                DTEManager.SetStatus("Attaching to IIS Worker Process...");

                new ProcessUtilities(this.DTE).AttachToProcessByName(ProcessConstants.IISWorkerProcess);

                DTEManager.SetStatus("Attaching to IIS Worker Process... All Done!");
                DTEManager.ProjectService.Logger.WriteLine("Done!", LogCategory.Status);
            }
            catch (Exception ex)
            {
                DTEManager.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
                DTEManager.SetStatus("Attaching to IIS Worker Process.. ERROR!");
            }
        }

        #endregion

        #region Attach to User Code Process

        /// <summary>
        /// Handles the BeforeQueryStatus event of the AttachToUserCodeProcess control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AttachToUserCodeProcess_BeforeQueryStatus(object sender, EventArgs e)
        {
            SetAttachToProcessCommandVisibility(sender, ProcessConstants.SPUCWorkerProcess);
        }

        /// <summary>
        /// Handles the Click event of the AttachToUserCodeProcess control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AttachToUserCodeProcess_Click(object sender, EventArgs arguments)
        {
            SharePointVersion version = ProjectUtilities.WhichSharePointVersionIsProjectDeployingTo();
            if (version == SharePointVersion.SP2010)
            {
                AttachToUserCodeProcessSharePoint2010();
            }
            else if (version == SharePointVersion.SP2013)
            {
                AttachToUserCodeProcessSharePoint2013();
            }
        }

        /// <summary>
        /// Attaches to user code process SharePoint 2010.
        /// </summary>
        private void AttachToUserCodeProcessSharePoint2010()
        {
            try
            {
                DTEManager.ProjectService.Logger.ActivateOutputWindow();
                DTEManager.ProjectService.Logger.WriteLine("========== Attaching to User Code Process ==========", LogCategory.Status);

                DTEManager.SetStatus("Attaching to User Code Process...");

                new ProcessUtilities(this.DTE).AttachToProcessByName(ProcessConstants.SPUCWorkerProcess);

                DTEManager.SetStatus("Attaching to User Code Process... All Done!");
                DTEManager.ProjectService.Logger.WriteLine("Done!", LogCategory.Status);
            }
            catch (Exception ex)
            {
                DTEManager.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
                DTEManager.SetStatus("Attaching to User Code Process.. ERROR!");
            }
        }

        /// <summary>
        /// Attaches to user code process SharePoint 2013.
        /// </summary>
        private void AttachToUserCodeProcessSharePoint2013()
        {
            try
            {
                DTEManager.ProjectService.Logger.ActivateOutputWindow();
                DTEManager.ProjectService.Logger.WriteLine("========== Attaching to User Code Process ==========", LogCategory.Status);

                DTEManager.SetStatus("Attaching to User Code Process...");

                new ProcessUtilities(this.DTE).AttachToProcessByName(ProcessConstants.SPUCWorkerProcess);

                DTEManager.SetStatus("Attaching to User Code Process... All Done!");
                DTEManager.ProjectService.Logger.WriteLine("Done!", LogCategory.Status);
            }
            catch (Exception ex)
            {
                DTEManager.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
                DTEManager.SetStatus("Attaching to User Code Process.. ERROR!");
            }
        }

        #endregion

        #region Attach to OWSTimer Process

        /// <summary>
        /// Handles the BeforeQueryStatus event of the AttachToOWSTimerProcess control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AttachToOWSTimerProcess_BeforeQueryStatus(object sender, EventArgs e)
        {
            SetAttachToProcessCommandVisibility(sender, ProcessConstants.OWSTimerProcess);
        }

        /// <summary>
        /// Handles the Click event of the AttachToOWSTimerProcess control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AttachToOWSTimerProcess_Click(object sender, EventArgs arguments)
        {
            SharePointVersion version = ProjectUtilities.WhichSharePointVersionIsProjectDeployingTo();
            if (version == SharePointVersion.SP2010)
            {
                AttachToOWSTimerProcessSharePoint2010();
            }
            else if (version == SharePointVersion.SP2013)
            {
                AttachToOWSTimerProcessSharePoint2013();
            }
        }

        /// <summary>
        /// Attaches to OWS timer process SharePoint 2010.
        /// </summary>
        private void AttachToOWSTimerProcessSharePoint2010()
        {
            try
            {
                DTEManager.ProjectService.Logger.ActivateOutputWindow();
                DTEManager.ProjectService.Logger.WriteLine("========== Attaching to OWS Timer Process ==========", LogCategory.Status);

                DTEManager.SetStatus("Attaching to OWS Timer Process...");

                new ProcessUtilities(this.DTE).AttachToProcessByName(ProcessConstants.OWSTimerProcess);

                DTEManager.SetStatus("Attaching to OWS Timer Process... All Done!");
                DTEManager.ProjectService.Logger.WriteLine("Done!", LogCategory.Status);
            }
            catch (Exception ex)
            {
                DTEManager.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
                DTEManager.SetStatus("Attaching to OWS Timer Process.. ERROR!");
            }
        }

        /// <summary>
        /// Attaches to OWS timer process SharePoint 2013.
        /// </summary>
        private void AttachToOWSTimerProcessSharePoint2013()
        {
            try
            {
                DTEManager.ProjectService.Logger.ActivateOutputWindow();
                DTEManager.ProjectService.Logger.WriteLine("========== Attaching to OWS Timer Process ==========", LogCategory.Status);

                DTEManager.SetStatus("Attaching to OWS Timer Process...");

                new ProcessUtilities(this.DTE).AttachToProcessByName(ProcessConstants.OWSTimerProcess);

                DTEManager.SetStatus("Attaching to OWS Timer Process... All Done!");
                DTEManager.ProjectService.Logger.WriteLine("Done!", LogCategory.Status);
            }
            catch (Exception ex)
            {
                DTEManager.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
                DTEManager.SetStatus("Attaching to OWS Timer Process.. ERROR!");
            }
        }

        #endregion

        #region Attach to VSSPHost5 Process

        /// <summary>
        /// Handles the BeforeQueryStatus event of the AttachToVSSPHost5Process control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AttachToVSSPHost5Process_BeforeQueryStatus(object sender, EventArgs e)
        {
            SetAttachToProcessCommandVisibility(sender, ProcessConstants.VSSHost5Process);
        }

        /// <summary>
        /// Handles the Click event of the AttachToVSSPHost5Process control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AttachToVSSPHost5Process_Click(object sender, EventArgs arguments)
        {
            SharePointVersion version = ProjectUtilities.WhichSharePointVersionIsProjectDeployingTo();
            if (version == SharePointVersion.SP2010)
            {
                AttachToVSSPHost5ProcessSharePoint2010();
            }
            else if (version == SharePointVersion.SP2013)
            {
                AttachToVSSPHost5ProcessSharePoint2013();
            }
        }

        /// <summary>
        /// Attaches to VSSP host5 process SharePoint 2010.
        /// </summary>
        private void AttachToVSSPHost5ProcessSharePoint2010()
        {
            try
            {
                DTEManager.ProjectService.Logger.ActivateOutputWindow();
                DTEManager.ProjectService.Logger.WriteLine("========== Attaching to VSS Host4 Process ==========", LogCategory.Status);

                DTEManager.SetStatus("Attaching to VSS Host5 Process...");

                new ProcessUtilities(this.DTE).AttachToProcessByName(ProcessConstants.VSSHost5Process);

                DTEManager.SetStatus("Attaching to VSS Host5 Process... All Done!");
                DTEManager.ProjectService.Logger.WriteLine("Done!", LogCategory.Status);
            }
            catch (Exception ex)
            {
                DTEManager.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
                DTEManager.SetStatus("Attaching to VSS Host5 Process.. ERROR!");
            }
        }

        /// <summary>
        /// Attaches to VSSP host5 process SharePoint 2013.
        /// </summary>
        private void AttachToVSSPHost5ProcessSharePoint2013()
        {
            try
            {
                DTEManager.ProjectService.Logger.ActivateOutputWindow();
                DTEManager.ProjectService.Logger.WriteLine("========== Attaching to VSS Host5 Process ==========", LogCategory.Status);

                DTEManager.SetStatus("Attaching to VSS Host5 Process...");

                new ProcessUtilities(this.DTE).AttachToProcessByName(ProcessConstants.VSSHost5Process);

                DTEManager.SetStatus("Attaching to VSS Host5 Process... All Done!");
                DTEManager.ProjectService.Logger.WriteLine("Done!", LogCategory.Status);
            }
            catch (Exception ex)
            {
                DTEManager.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
                DTEManager.SetStatus("Attaching to VSS Host5 Process.. ERROR!");
            }
        }

        #endregion

        #endregion

        #region Restart Processes Command Handlers

        /// <summary>
        /// Sets the restart process command visibility.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="processName">Name of the process.</param>
        private void SetRestartProcessCommandVisibility(object sender, string processName)
        {
            OleMenuCommand cmd = sender as OleMenuCommand;
            if (null != cmd)
            {
                EnvDTE.DTE dte = this.DTE;

                cmd.Visible = cmd.Supported;
                cmd.Enabled = IsSharePointInstalled && IsNotBuilding(dte);

                if (cmd.Enabled)
                {
                    // Only show the command if worker processes exist.
                    try
                    {
                        ProcessUtilities utils = new ProcessUtilities(dte);
                        cmd.Enabled = utils.IsProcessAvailableByName(processName);
                    }
                    catch (Exception ex)
                    {
                        DTEManager.ProjectService.Logger.ActivateOutputWindow();
                        DTEManager.ProjectService.Logger.WriteLine(ex.ToString(), LogCategory.Error);
                    }
                }
            }
        }

        #region Restart IIS Process

        /// <summary>
        /// Handles the Click event of the RestartIIS control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void RestartIIS_Click(object sender, EventArgs arguments)
        {
            RestartIIS();
        }

        /// <summary>
        /// Restarts the IIS.
        /// </summary>
        private void RestartIIS()
        {
            try
            {
                DTEManager.ProjectService.Logger.ActivateOutputWindow();
                DTEManager.ProjectService.Logger.WriteLine("========== Restarting IIS ==========", LogCategory.Status);

                DTEManager.SetStatus("Restarting IIS...");

                new ProcessUtilities(this.DTE).RestartIIS(DTEManager.ActiveSharePointProject);

                DTEManager.SetStatus("Restarting IIS... All Done!");
                DTEManager.ProjectService.Logger.WriteLine("Done!", LogCategory.Status);
            }
            catch (Exception ex)
            {
                DTEManager.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
                DTEManager.SetStatus("Restarting IIS... ERROR!");
            }
        }

        #endregion

        #region Restart User Code Process

        /// <summary>
        /// Handles the BeforeQueryStatus event of the RestartUserCodeProcess control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void RestartUserCodeProcess_BeforeQueryStatus(object sender, EventArgs e)
        {
            SetRestartProcessCommandVisibility(sender, ProcessConstants.SPUCWorkerProcess);
        }

        /// <summary>
        /// Handles the Click event of the RestartUserCodeProcess control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void RestartUserCodeProcess_Click(object sender, EventArgs arguments)
        {
            RestartUserCodeProcess();
        }

        /// <summary>
        /// Restarts the user code process.
        /// </summary>
        private void RestartUserCodeProcess()
        {
            try
            {
                DTEManager.ProjectService.Logger.ActivateOutputWindow();
                DTEManager.ProjectService.Logger.WriteLine("========== Restarting User Code Process ==========", LogCategory.Status);

                DTEManager.SetStatus("Restarting User Code Process...");

                new ProcessUtilities(this.DTE).RestartProcess(ProcessConstants.SPUCWorkerProcessName, 10);

                DTEManager.SetStatus("Restarting User Code Process... All Done!");
                DTEManager.ProjectService.Logger.WriteLine("Done!", LogCategory.Status);
            }
            catch (Exception ex)
            {
                DTEManager.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
                DTEManager.SetStatus("Restarting User Code Process... ERROR!");
            }
        }

        #endregion

        #region Restart OWSTimer Process

        /// <summary>
        /// Handles the BeforeQueryStatus event of the RestartOWSTimerProcess control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void RestartOWSTimerProcess_BeforeQueryStatus(object sender, EventArgs e)
        {
            SetRestartProcessCommandVisibility(sender, ProcessConstants.OWSTimerProcess);
        }

        /// <summary>
        /// Handles the Click event of the RestartOWSTimerProcess control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void RestartOWSTimerProcess_Click(object sender, EventArgs arguments)
        {
            RestartOWSTimerProcess();
        }

        /// <summary>
        /// Restarts the OWS timer process.
        /// </summary>
        private void RestartOWSTimerProcess()
        {
            try
            {
                DTEManager.ProjectService.Logger.ActivateOutputWindow();
                DTEManager.ProjectService.Logger.WriteLine("========== Restarting OWS Timer Process ==========", LogCategory.Status);

                DTEManager.SetStatus("Restarting OWS Timer Process...");

                new ProcessUtilities(this.DTE).RestartProcess(ProcessConstants.OWSTimerProcessName, 10);

                DTEManager.SetStatus("Restarting OWS Timer Process... All Done!");
                DTEManager.ProjectService.Logger.WriteLine("Done!", LogCategory.Status);
            }
            catch (Exception ex)
            {
                DTEManager.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
                DTEManager.SetStatus("Restarting OWS Timer Process... ERROR!");
            }
        }

        #endregion

        #region Recycle App Pools

        /// <summary>
        /// Handles the Click event of the RecycleAppPoolContext control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void RecycleAppPoolContext_Click(object sender, EventArgs arguments)
        {
            RecycleSelectedPools(false);
        }

        /// <summary>
        /// Handles the Click event of the RecycleAppPoolSolution control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void RecycleAppPoolSolution_Click(object sender, EventArgs arguments)
        {
            RecycleSelectedPools(true);
        }

        /// <summary>
        /// Handles the Click event of the RecycleAppPoolSelection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void RecycleAppPoolSelection_Click(object sender, EventArgs arguments)
        {
            RecycleSelectedPools(false);
        }

        /// <summary>
        /// Recycles the selected pools.
        /// </summary>
        /// <param name="isSolutionContext">if set to <c>true</c> [is solution context].</param>
        private void RecycleSelectedPools(bool isSolutionContext)
        {
            try
            {
                DTEManager.ProjectService.Logger.ActivateOutputWindow();
                DTEManager.ProjectService.Logger.WriteLine("========== Recycle Selected Project App Pool ==========", LogCategory.Status);

                DTEManager.SetStatus("Recycling Selected Project App Pool...");

                ProcessUtilities utils = new ProcessUtilities(this.DTE);

                ISharePointProject[] projects = ProjectUtilities.GetSharePointProjects(isSolutionContext, false);
                List<string> appPoolNames = new List<string>();

                foreach (ISharePointProject project in projects)
                {
                    string name = utils.GetApplicationPoolName(CKSDEVPackageSharePointProjectService, project.SiteUrl.ToString());
                    if (!String.IsNullOrEmpty(name))
                    {
                        appPoolNames.Add(name);
                    }
                }

                RecycleAppPools(appPoolNames.Distinct().ToArray());

                DTEManager.SetStatus("Recycling Selected Project App Pool... All Done!");
                DTEManager.ProjectService.Logger.WriteLine("Done!", LogCategory.Status);
            }
            catch (Exception ex)
            {
                DTEManager.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
                DTEManager.SetStatus("Recycle Selected Project App Pool.. ERROR!");
            }
        }

        /// <summary>
        /// Handles the Click event of the RecycleAllAppPools control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void RecycleAllAppPools_Click(object sender, EventArgs arguments)
        {
            try
            {
                DTEManager.ProjectService.Logger.ActivateOutputWindow();
                DTEManager.ProjectService.Logger.WriteLine("========== Recycle All App Pools ==========", LogCategory.Status);

                DTEManager.SetStatus("Recycling All App Pools...");

                ProcessUtilities utils = new ProcessUtilities(this.DTE);
                string[] names = utils.GetAllApplicationPoolNames(this.CKSDEVPackageSharePointProjectService);
                RecycleAppPools(names);

                DTEManager.SetStatus("Recycling All App Pools... All Done!");
                DTEManager.ProjectService.Logger.WriteLine("Done!", LogCategory.Status);
            }
            catch (Exception ex)
            {
                DTEManager.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
                DTEManager.SetStatus("Recycle All App Pools.. ERROR!");
            }
        }

        /// <summary>
        /// Recycles the app pools.
        /// </summary>
        /// <param name="names">The names.</param>
        private void RecycleAppPools(string[] names)
        {
            DTEManager.ProjectService.Logger.ActivateOutputWindow();
            DTEManager.ProjectService.Logger.WriteLine("========== Recycling Application Pool(s) ==========", LogCategory.Status);

            DTEManager.SetStatus("Recycling Application Pool(s)...");

            if (names.Length == 0)
            {
                DTEManager.ProjectService.Logger.WriteLine("No Application Pools found to recycle!", LogCategory.Warning);
                return;
            }

            ProcessUtilities utils = new ProcessUtilities(this.DTE);

            foreach (string name in names)
            {
                try
                {
                    DTEManager.ProjectService.Logger.WriteLine("Recycling Application Pool: " + name, LogCategory.Status);

                    utils.RecycleApplicationPool(name);
                }
                catch (Exception ex)
                {
                    DTEManager.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
                }
            }

            DTEManager.SetStatus("Recycling Application Pool(s)... All Done!");
            DTEManager.ProjectService.Logger.WriteLine("========== Recycling Application Pool(s) completed ==========", LogCategory.Status);
        }

        #endregion

        #endregion

        #region Auto Save and Build Handlers

        private List<string> successfulBuiltProjects = null;
        private bool hasAnyBuildFailed = false;

        /// <summary>
        /// Builds the events_ on build begin.
        /// </summary>
        /// <param name="Scope">The scope.</param>
        /// <param name="Action">The action.</param>
        private void BuildEvents_OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
        {
            // Clear our cheeky little cache of successfuly built projects.
            this.successfulBuiltProjects = new List<string>();
            this.hasAnyBuildFailed = false;
        }

        /// <summary>
        /// Builds the events_ on build proj config done.
        /// </summary>
        /// <param name="Project">The project.</param>
        /// <param name="ProjectConfig">The project config.</param>
        /// <param name="Platform">The platform.</param>
        /// <param name="SolutionConfig">The solution config.</param>
        /// <param name="Success">if set to <c>true</c> [success].</param>
        public void BuildEvents_OnBuildProjConfigDone(string Project, string ProjectConfig, string Platform, string SolutionConfig, bool Success)
        {
            if (Success)
            {
                // Add this project to the list of those built successfully.  This list is used
                // so that we can do one Auto Quick Deploy at completion, and only if all builds were successful.
                this.successfulBuiltProjects.Add(Project);
            }
            else
            {
                this.hasAnyBuildFailed = true;
            }
        }

        /// <summary>
        /// Builds the events_ on build done.
        /// </summary>
        /// <param name="Scope">The scope.</param>
        /// <param name="Action">The action.</param>
        public void BuildEvents_OnBuildDone(vsBuildScope Scope, vsBuildAction Action)
        {
            // Don't auto copy if there is no SharePoint, or ANY build failed.
            if (!CKSDEVPackageSharePointProjectService.IsSharePointInstalled || hasAnyBuildFailed)
            {
                return;
            }

            // We will never auto copy on deploy - just build or rebuild.
            if (Action == vsBuildAction.vsBuildActionBuild || Action == vsBuildAction.vsBuildActionRebuildAll)
            {
                // Get all farm SP projects where the auto copy flag is set, and where the project was built succesfully.
                IEnumerable<ISharePointProject> spProjects = ProjectUtilities.GetSharePointProjects(Scope == vsBuildScope.vsBuildScopeSolution, true)
                    .Where(proj => AutoCopyAssembliesProperty.GetFromProject(proj)
                    && successfulBuiltProjects.Any(sbp => proj.FullPath.EndsWith(sbp)));
                if (spProjects.Count() > 0)
                {
                    // We don't clear the log since we want this to show straight after the build details.
                    DTEManager.SetStatus("Auto Copying to GAC/BIN...");
                    DTEManager.ProjectService.Logger.WriteLine("========== Auto Copying to GAC/BIN ==========", LogCategory.Status);

                    List<string> appPools = new List<string>();

                    foreach (ISharePointProject spProject in spProjects)
                    {
                        new SharePointPackageArtefact(spProject).QuickCopyBinaries(true);

                        // Get the app pool for recycling while we are here.
                        string appPool = new ProcessUtilities(this.DTE).GetApplicationPoolName(CKSDEVPackageSharePointProjectService, spProject.SiteUrl.ToString());
                        if (!String.IsNullOrEmpty(appPool))
                        {
                            appPools.Add(appPool);
                        }
                    }

                    DTEManager.ProjectService.Logger.WriteLine("========== Auto Copy to GAC/BIN succeeded ==========", LogCategory.Status);

                    DTEManager.SetStatus("Auto Copying to GAC/BIN... All Done!");

                    if (appPools.Count > 0)
                    {
                        this.RecycleAppPools(appPools.Distinct().ToArray());
                    }
                    else
                    {
                        this.RestartIIS();
                    }
                }
            }
        }

        /// <summary>
        /// Docs the events_ document saved.
        /// </summary>
        /// <param name="document">The document.</param>
        public void DocEvents_DocumentSaved(Document document)
        {
            if (!CKSDEVPackageSharePointProjectService.IsSharePointInstalled)
            {
                return;
            }

            // Check this document is in a SP project.
            EnvDTE.Project dteProject = document.ProjectItem.ContainingProject;
            ISharePointProject spProject = DTEManager.ProjectService.Convert<EnvDTE.Project, ISharePointProject>(dteProject);


            if (spProject != null)
            {
                // Document was contained in a SharePoint project.  Check if the Auto Copy flag is set.
                bool isAutoCopyToRoot = AutoCopyToSharePointRootProperty.GetFromProject(spProject);
                if (isAutoCopyToRoot)
                {
                    IEnumerable<QuickCopyableSharePointArtefact> items = DeploymentUtilities.ResolveProjectItemToArtefacts(CKSDEVPackageSharePointProjectService, document.ProjectItem);
                    items = items.Where(af => af.IsPackaged(CKSDEVPackageSharePointProjectService));
                    if (items != null && items.Count() > 0)
                    {
                        DTEManager.SetStatus("Auto Copying to SharePoint Root...");
                        DTEManager.ProjectService.Logger.ActivateOutputWindow();
                        DTEManager.ProjectService.Logger.WriteLine("========== Auto Copying to SharePoint Root ==========", LogCategory.Status);

                        foreach (QuickCopyableSharePointArtefact item in items)
                        {
                            item.QuickCopy(CKSDEVPackageSharePointProjectService, true);
                        }

                        DTEManager.ProjectService.Logger.WriteLine("========== Auto Copy to SharePoint Root succeeded ==========", LogCategory.Status);

                        DTEManager.SetStatus("Auto Copying to SharePoint Root... All Done!");
                    }
                }
            }
        }

        #endregion

        #region Help Handlers

        /// <summary>
        /// Handles the BeforeQueryStatus event of the CodePlexHome control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CodePlexHome_BeforeQueryStatus(object sender, EventArgs e)
        {
            ISharePointProjectService projectService = CKSDEVPackageSharePointProjectService;
            OleMenuCommand cmd = sender as OleMenuCommand;
            if (null != cmd)
            {
                cmd.Visible = true;
                cmd.Enabled = true;

                return;
            }
        }

        /// <summary>
        /// Handles the Click event of the CodePlexHome control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CodePlexHome_Click(object sender, EventArgs arguments)
        {
            ISharePointProjectService projectService = CKSDEVPackageSharePointProjectService;

            ProcessUtilities utils = new ProcessUtilities(this.DTE);
            DTEManager.ProjectService.Logger.ActivateOutputWindow();
            DTEManager.ProjectService.Logger.WriteLine("========== Opening CKSDev Home Page ==========", LogCategory.Status);

            utils.ExecuteBrowserUrlProcess(CKSProperties.Url_CodePlexHome);
        }

        /// <summary>
        /// Handles the BeforeQueryStatus event of the CodePlexDocumentation control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CodePlexDocumentation_BeforeQueryStatus(object sender, EventArgs e)
        {
            ISharePointProjectService projectService = CKSDEVPackageSharePointProjectService;
            OleMenuCommand cmd = sender as OleMenuCommand;
            if (null != cmd)
            {
                cmd.Visible = true;
                cmd.Enabled = true;

                return;
            }
        }

        /// <summary>
        /// Handles the Click event of the CodePlexDocumentation control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CodePlexDocumentation_Click(object sender, EventArgs arguments)
        {
            ISharePointProjectService projectService = CKSDEVPackageSharePointProjectService;

            ProcessUtilities utils = new ProcessUtilities(this.DTE);
            DTEManager.ProjectService.Logger.ActivateOutputWindow();
            DTEManager.ProjectService.Logger.WriteLine("========== Opening CKSDev Documentation ==========", LogCategory.Status);

            utils.ExecuteBrowserUrlProcess(CKSProperties.Url_CodePlexDocumentation);
        }

        /// <summary>
        /// Handles the BeforeQueryStatus event of the CodePlexNewFeature control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CodePlexNewFeature_BeforeQueryStatus(object sender, EventArgs e)
        {
            ISharePointProjectService projectService = CKSDEVPackageSharePointProjectService;
            OleMenuCommand cmd = sender as OleMenuCommand;
            if (null != cmd)
            {
                cmd.Visible = true;
                cmd.Enabled = true;

                return;
            }
        }

        /// <summary>
        /// Handles the Click event of the CodePlexNewFeature control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CodePlexNewFeature_Click(object sender, EventArgs arguments)
        {
            ISharePointProjectService projectService = CKSDEVPackageSharePointProjectService;

            ProcessUtilities utils = new ProcessUtilities(this.DTE);
            DTEManager.ProjectService.Logger.ActivateOutputWindow();
            DTEManager.ProjectService.Logger.WriteLine("========== Opening CKSDev New Feature ==========", LogCategory.Status);

            utils.ExecuteBrowserUrlProcess(CKSProperties.Url_CodePlexNewFeature);
        }

        /// <summary>
        /// Handles the BeforeQueryStatus event of the CodePlexNewIssue control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CodePlexNewIssue_BeforeQueryStatus(object sender, EventArgs e)
        {
            ISharePointProjectService projectService = CKSDEVPackageSharePointProjectService;
            OleMenuCommand cmd = sender as OleMenuCommand;
            if (null != cmd)
            {
                cmd.Visible = true;
                cmd.Enabled = true;

                return;
            }
        }

        /// <summary>
        /// Handles the Click event of the CodePlexNewIssue control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="arguments">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CodePlexNewIssue_Click(object sender, EventArgs arguments)
        {
            ISharePointProjectService projectService = CKSDEVPackageSharePointProjectService;

            ProcessUtilities utils = new ProcessUtilities(this.DTE);
            DTEManager.ProjectService.Logger.ActivateOutputWindow();
            DTEManager.ProjectService.Logger.WriteLine("========== Opening CKSDev New Issue ==========", LogCategory.Status);

            utils.ExecuteBrowserUrlProcess(CKSProperties.Url_CodePlexNewIssue);
        }

        #endregion

        #region Visibility Utility Methods

        /// <summary>
        /// Determines whether [is not building] [the specified DTE].
        /// </summary>
        /// <param name="dte">The DTE.</param>
        /// <returns>
        /// 	<c>true</c> if [is not building] [the specified DTE]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsNotBuilding(EnvDTE.DTE dte)
        {
            return dte.Solution.SolutionBuild.BuildState != vsBuildState.vsBuildStateInProgress;
        }




        /// <summary>
        /// Does the loaded share point projects exist.
        /// </summary>
        /// <param name="dte">The DTE.</param>
        /// <param name="requireFarm">if set to <c>true</c> [require farm].</param>
        /// <returns></returns>
        private bool DoLoadedSharePointProjectsExist(EnvDTE.DTE dte, bool requireFarm)
        {
            //TODO: move this into the project utilities

            // We use the fact that ConfigurationManager is null for an unloaded project to detect loaded projects.
            foreach (EnvDTE.Project project in dte.Solution.Projects)
            {
                if (project.ConfigurationManager != null && ProjectUtilities.IsSharePointProject(project, requireFarm))
                {
                    return true;
                }
            }
            return false;
        }


        #endregion

        #region Custom Deployment - Project Level

        /// <summary>
        /// Gets the active project.
        /// </summary>
        /// <param name="dte">The DTE.</param>
        /// <returns></returns>
        internal static EnvDTE.Project GetActiveProject(DTE dte)
        {
            //TODO: move to project utilities
            EnvDTE.Project activeProject = null;

            Array activeSolutionProjects = dte.ActiveSolutionProjects as Array;
            if (activeSolutionProjects != null && activeSolutionProjects.Length > 0)
            {
                activeProject = activeSolutionProjects.GetValue(0) as EnvDTE.Project;
            }

            return activeProject;
        }

        /// <summary>
        /// Customs the deploy. This handles all the calls for 'quick deploy copying'.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="isSolution">if set to <c>true</c> [is solution].</param>
        private void CustomDeploy(DeployType type, bool isSolution)
        {
            // Get reference to DTE automation object and project service.
            EnvDTE.DTE dte = this.DTE;
            ISharePointProjectService projectService = CKSDEVPackageSharePointProjectService;

            bool canDeploy = true;

            //Get an array of the solutions SP Projects because we'll be doing some significant stuff with them
            List<ISharePointProject> projects = DTEManager.SharePointProjects;

            // Calling Build.Package will correctly call Package on all projects in solution, or the selected project(s).
            DTEManager.ProjectService.Logger.ActivateOutputWindow();
            //foreach (ISharePointProject spProject in projects)
            //{
            //    DTEManager.ProjectService.Logger.WriteLine("Packaging: " + spProject.Name, LogCategory.Status);
            //    spProject.Package.BuildPackage();
            //}

            //Build the SP Projects if they are required or set to auto save/build
            canDeploy = BuildSharePointProjects(type, isSolution, canDeploy, projects);

            if (((canDeploy)) && (type == DeployType.CopySharePointRoot || type == DeployType.CopyBoth))
            {
                DTEManager.SetStatus("Copying to SharePoint Root...");
                DTEManager.ProjectService.Logger.WriteLine("========== Copying to SharePoint Root ==========", LogCategory.Status);

                foreach (ISharePointProject project in projects)
                {
                    SharePointPackageArtefact package = new SharePointPackageArtefact(project);
                    package.QuickCopy(true);
                }

                DTEManager.ProjectService.Logger.WriteLine("========== Copy to SharePoint Root succeeded ==========", LogCategory.Status);

                DTEManager.SetStatus("Copying to SharePoint Root... All Done!");
            }

            if (((canDeploy)) && (type == DeployType.CopyBinary || type == DeployType.CopyBoth))
            {
                DTEManager.SetStatus("Copying to GAC/BIN...");
                DTEManager.ProjectService.Logger.WriteLine("========== Copying to GAC/BIN ==========", LogCategory.Status);

                foreach (ISharePointProject project in projects)
                {
                    SharePointPackageArtefact package = new SharePointPackageArtefact(project);
                    package.QuickCopyBinaries(true);
                }

                DTEManager.ProjectService.Logger.WriteLine("========== Copying to GAC/BIN succeeded ==========", LogCategory.Status);

                DTEManager.SetStatus("Copying to GAC/BIN... All Done!");
            }
        }


        private void CustomDeploySelectedItems()
        {
            ISharePointProjectService projectService = CKSDEVPackageSharePointProjectService;


            // Convert the DTE project into a SharePoint project. If the conversion fails, this is not a SP project.
            EnvDTE.DTE dte = this.DTE;
            foreach (SelectedItem item in dte.SelectedItems)
            {
                // Only respond if we actually have a project item (i.e. not to projects).
                if (item.ProjectItem != null)
                {
                    if (projectService != null && projectService.IsSharePointInstalled)
                    {
                        IEnumerable<QuickCopyableSharePointArtefact> artefacts = artefacts = DeploymentUtilities.ResolveProjectItemToArtefacts(projectService, item.ProjectItem);
                        artefacts = artefacts.Where(af => af.IsPackaged(projectService));

                        DTEManager.SetStatus("Copying to SharePoint Root...");
                        DTEManager.ProjectService.Logger.WriteLine("========== Copying to SharePoint Root ==========", LogCategory.Status);

                        if (artefacts.Count() > 0)
                        {
                            foreach (QuickCopyableSharePointArtefact artefact in artefacts)
                            {
                                artefact.QuickCopy(projectService, true);
                            }

                            DTEManager.ProjectService.Logger.WriteLine("========== Copy to SharePoint Root succeeded ==========", LogCategory.Status);

                            DTEManager.SetStatus("Copying to SharePoint Root... All Done!");
                        }
                        else
                        {
                            DTEManager.ProjectService.Logger.WriteLine("ERROR: Unable to Copy to SharePoint Root. Ensure at least one selected item is included in a package.", LogCategory.Error);

                            DTEManager.ProjectService.Logger.WriteLine("========== Copying to SharePoint Root failed ==========", LogCategory.Status);

                            DTEManager.SetStatus("Copying to SharePoint Root... ERROR!");
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Builds the share point projects.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="isSolution">if set to <c>true</c> [is solution].</param>
        /// <param name="canDeploy">if set to <c>true</c> [can deploy]. This causes an application pool recycle.</param>
        /// <param name="projects">The projects.</param>
        /// <returns></returns>
        private bool BuildSharePointProjects(DeployType type,
            bool isSolution,
            bool canDeploy,
            List<ISharePointProject> projects)
        {
            //Process each SP project for build
            //TODO: should also consider whether to determine inter solution project references and build any none-sp projects
            //Information that will help is:
            // http://msdn.microsoft.com/en-us/library/aa984610(v=vs.71).aspx
            // http://msdn.microsoft.com/en-us/library/aa984584(v=vs.71).aspx
            // http://msdn.microsoft.com/en-us/library/aa984522(v=vs.71).aspx
            // http://msdn.microsoft.com/en-us/library/aa984592(v=vs.71).aspx


            if (type == DeployType.CopyBinary || type == DeployType.CopyBoth)
            {
                // Forcefully build any projects where this flag is set.  We let VS handle the "dirtiness" of a build,
                // so even if the user selected BUILD then COPY TO GAC, this should have minimal impact.
                IEnumerable<ISharePointProject> buildProjects = projects.Where(proj => BuildOnCopyAssembliesProperty.GetFromProject(proj));

                foreach (ISharePointProject buildProject in buildProjects)
                {
                    // just to be sure...
                    if ((buildProject != null) && (buildProject.Project != null))
                    {
                        String projectName = buildProject.Project.Name;
                        Project project = DTEManager.GetProjectByName(projectName, false);

                        if (project != null)
                        {
                            DTEManager.SetStatus(String.Format("Building project: '{0}'...", projectName));
                            DTEManager.ProjectService.Logger.WriteLine(String.Format("========== Building project '{0}' ==========", projectName),
                                LogCategory.Status);

                            string config = project.ConfigurationManager.ActiveConfiguration.ConfigurationName;

                            SolutionBuild2 builder = this.m_ApplicationObject2.Solution.SolutionBuild as SolutionBuild2;
                            builder.BuildProject(
                                config,
                                project.UniqueName,
                                true);
                            canDeploy = (builder.LastBuildInfo == 0);

                            // we don't build further projects if one fails
                            if (!canDeploy)
                            {
                                DTEManager.SetStatus(String.Format("Building of project '{0}' failed, deployment stopped", projectName));
                                DTEManager.ProjectService.Logger.WriteLine(String.Format("========== Building project '{0}' failed ==========", projectName),
                                    LogCategory.Status);
                                break;
                            }

                            DTEManager.SetStatus(String.Format("Building project: '{0}'... Done!", projectName));
                            DTEManager.ProjectService.Logger.WriteLine(String.Format("========== Building project '{0}' succeeded ==========", projectName),
                                LogCategory.Status);
                        }
                    }
                }

                // Recycle selected application pools first (if applicable), so we do not duplicate recycle for the same target 
                // URL used in multiple projects.
                if (canDeploy)
                {
                    this.RecycleSelectedPools(isSolution);
                }
            }
            return canDeploy;
        }

        #endregion
    }

}