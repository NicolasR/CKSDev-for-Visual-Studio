using CKS.Dev2015.VisualStudio.SharePoint.Environment;
using CKS.Dev2015.VisualStudio.SharePoint.Environment.Options;
using Microsoft.VisualStudio.SharePoint;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace VSIXProject42
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
    /// // This attribute is needed to let the shell know that this package exposes some menus.
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    //TODO: change this version number to effect the VS help about dialog
    [InstalledProductRegistration("#110", "#112", "2.0.0.0", IconResourceID = 400)]
    [Guid(GuidList.guidCKS_Dev12_PkgString)]
    [ProvideAutoLoad(GuidList.UIContext_SharePointProject, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(EnabledExtensionsOptionsPage), "CKS Development Tools Edition", "Extensions", 101, 102, true)]
    [ProvideProfile(typeof(EnabledExtensionsOptionsPage), "CKS Development Tools Edition", "Extensions", 101, 102, true, DescriptionResourceID = 101)]
    public sealed class CKSDEV2019VSIXPackage : AsyncPackage, ICKSDevVSPackage
    {
        /// <summary>
        /// VSIXProject42Package GUID string.
        /// </summary>
        public const string PackageGuidString = "b44f6f01-24ff-44ad-97cf-0d0f5ea81b3e";

        #region Package Members

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

            // Call out to our event manager to register and handle project events.
            this.EventManager = new EventHandlerManager(this);
            this.EventManager.RegisterHandlers();

            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        }

        #endregion
    }


    //[Export(typeof(ISharePointProjectExtension))]
    //internal class ExampleProjectExtension : ISharePointProjectExtension
    //{
    //    public void Initialize(ISharePointProjectService projectService)
    //    {
    //        projectService.ProjectAdded += projectService_ProjectAdded;
    //        projectService.ProjectInitialized += projectService_ProjectInitialized;
    //        projectService.ProjectNameChanged += projectService_ProjectNameChanged;
    //        projectService.ProjectPropertyChanged += projectService_ProjectPropertyChanged;
    //        projectService.ProjectRemoved += projectService_ProjectRemoved;
    //        projectService.ProjectDisposing += projectService_ProjectDisposing;
    //    }

    //    // A project was added.
    //    void projectService_ProjectAdded(object sender, SharePointProjectEventArgs e)
    //    {
    //        ISharePointProject project = (ISharePointProject)sender;
    //        string message = String.Format("The following project was added: {0}", e.Project.Name);
    //        project.ProjectService.Logger.WriteLine(message, LogCategory.Message);
    //    }

    //    // A project is loaded in the IDE.
    //    void projectService_ProjectInitialized(object sender, SharePointProjectEventArgs e)
    //    {
    //        ISharePointProject project = (ISharePointProject)sender;
    //        string message = String.Format("The following project is being initialized: {0}", e.Project.Name);
    //        project.ProjectService.Logger.WriteLine(message, LogCategory.Message);
    //    }

    //    // The name of a project was changed.
    //    void projectService_ProjectNameChanged(object sender, NameChangedEventArgs e)
    //    {
    //        ISharePointProject project = (ISharePointProject)sender;
    //        string message = String.Format("The project named {0} was changed to {1}.", e.OldName, project.Name);
    //        project.ProjectService.Logger.WriteLine(message, LogCategory.Message);
    //    }

    //    // A project property value was changed.
    //    private void projectService_ProjectPropertyChanged(object sender, PropertyChangedEventArgs e)
    //    {
    //        ISharePointProject project = (ISharePointProject)sender;
    //        string message = String.Format("The following property of the {0} project was changed: {1}",
    //            project.Name, e.PropertyName);
    //        project.ProjectService.Logger.WriteLine(message, LogCategory.Message);
    //    }

    //    // A project is being removed or unloaded.
    //    void projectService_ProjectRemoved(object sender, SharePointProjectEventArgs e)
    //    {
    //        ISharePointProject project = (ISharePointProject)sender;
    //        string message = String.Format("The following project is being removed or unloaded: {0}", e.Project.Name);
    //        project.ProjectService.Logger.WriteLine(message, LogCategory.Message);
    //    }

    //    // A project was removed or unloaded.
    //    void projectService_ProjectDisposing(object sender, SharePointProjectEventArgs e)
    //    {
    //        ISharePointProject project = (ISharePointProject)sender;
    //        string message = String.Format("The following project was removed or unloaded: {0}", e.Project.Name);
    //        project.ProjectService.Logger.WriteLine(message, LogCategory.Message);
    //    }
    //}
}
