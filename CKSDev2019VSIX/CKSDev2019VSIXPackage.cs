using CKS.Dev2015.VisualStudio.SharePoint.Environment;
using CKS.Dev2015.VisualStudio.SharePoint.Environment.Options;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace CKSDev2019VSIX
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [Guid(PackageGuidString)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "2.0.0.0", IconResourceID = 400)]
    [ProvideAutoLoad(GuidList.UIContext_SharePointProject, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(EnabledExtensionsOptionsPage), "CKS Development Tools Edition", "Extensions", 101, 102, true)]
    [ProvideProfile(typeof(EnabledExtensionsOptionsPage), "CKS Development Tools Edition", "Extensions", 101, 102, true, DescriptionResourceID = 101)]
    public sealed class CKSDev2019VSIXPackage : AsyncPackage, ICKSDevVSPackage, IVsShellPropertyEvents
    {
        /// <summary>
        /// CKSDev2019VSIXPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "9bdb89e1-812c-4769-910a-f3e7b2971e72";

        /// <summary>
        /// Gets or sets the event manager.
        /// </summary>
        /// <value>
        /// The event manager.
        /// </value>
        internal EventHandlerManager EventManager
        {
            get;
            set;
        }

        public object GetServiceInternal(Type type)
        {
            return this.GetService(type);
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // Cache the SP project service for reuse as necessary.
            // We will always have a SP project, otherwise CKSDEV will not work at all and should not have been installed.
            //this.SharePointProjectService = this.GetService(typeof(ISharePointProjectService)) as ISharePointProjectService;

            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // Call out to our event manager to register and handle project events.
            this.EventManager = new EventHandlerManager(this);
            this.EventManager.RegisterHandlers();

            // Added by MattSmithNZ
            // Workaround DTE not being available until startup is complete (under certain conditions for loading devenv.exe)
            // by receiving notification when the shell is loaded.
            if (await GetServiceAsync(typeof(SVsShell)) is IVsShell shellService)
            {
                shellService.AdviseShellPropertyChanges(this, out dteCookie);
            }

            // Call to ensure the DTE stuff is awake, again needed under certain conditions where DTE is not available yet.
            InitialiseDTEDependantObjects();
        }

        #endregion

        #region IVsShellPropertyEvents Members

        bool hasInitialised = false;

        // Cached copies of event handler objects to avoid COM destroying our handlers.
        private uint dteCookie;
        private uint projTrackCookie;

        /// <summary>
        /// Handler to react to the OnShellPropertyChanged event.
        /// </summary>
        /// <param name="propid">The property id.</param>
        /// <param name="var">The value.</param>
        /// <returns>The result as int.</returns>
        public int OnShellPropertyChange(int propid, object var)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if ((int)__VSSPROPID.VSSPROPID_Zombie != propid)
            {
                return VSConstants.S_OK;
            }

            if ((bool)var != false)
            {
                return VSConstants.S_OK;
            }

            InitialiseDTEDependantObjects();

            // Zombie state clean-up code.
            if (GetService(typeof(SVsShell)) is IVsShell shellService)
            {
                shellService.UnadviseShellPropertyChanges(this.dteCookie);
                shellService.UnadviseShellPropertyChanges(this.projTrackCookie);
            }
            this.dteCookie = 0;
            this.projTrackCookie = 0;

            return VSConstants.S_OK;
        }

        private void InitialiseDTEDependantObjects()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (hasInitialised)
            {
                return;
            }


            EnvDTE80.DTE2 dte2 = GetService(typeof(SDTE)) as EnvDTE80.DTE2;
            if (dte2 == null)
            {
                return;
            }

            // Zombie state dependent code.
            dte2 = GetService(typeof(SDTE)) as EnvDTE80.DTE2;
            EnvDTE80.Events2 events = (EnvDTE80.Events2)dte2.Events;
            _ = GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;

            // Register event handlers that need the DTE active to do work.
            EventManager.RegisterDteDependentHandlers(dte2, events);

            // Initialize the output logger.  This is a bit hacky but a quick fix for Alpha release.
            //StatusBarLogger.InitializeInstance(dte);
            hasInitialised = true;
        }

        #endregion
    }
}
