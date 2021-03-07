using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.SharePoint;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace CKS.Dev.VisualStudio.SharePoint
{
    /// <summary>
    /// The DTE manager to provide functionality for the DTE.
    /// </summary>
    public static class DTEManager
    {
        /// <summary>
        /// The SharePoint Project type ID
        /// </summary>
        static readonly Guid SharePointProjectTypeID = new Guid("BB1F664B-9266-4FD6-B973-E1E44974B511");

        static IOleServiceProvider _globalOleServiceProvider;
        static IServiceProvider _globalServiceProvider;
        static ISharePointProjectService _projectService;

        /// <summary>
        /// Gets or sets the DTE.
        /// </summary>
        /// <value>The DTE.</value>
        public static DTE DTE { get; private set; }

        /// <summary>
        /// Gets the active solution.
        /// </summary>
        /// <value>The active solution.</value>
        public static Solution ActiveSolution
        {
            get { return DTE.Solution; }
        }

        /// <summary>
        /// Gets the active project.
        /// </summary>
        /// <returns></returns>
        public static EnvDTE.Project ActiveProject
        {
            get
            {
                EnvDTE.Project project = null;
                object[] projects = DTE.ActiveSolutionProjects as object[];
                if ((projects != null) && (projects.Length > 0))
                {
                    project = projects[0] as EnvDTE.Project;
                }
                return project;
            }
        }

        /// <summary>
        /// Gets the active SharePoint project.
        /// </summary>
        /// <returns></returns>
        public static ISharePointProject ActiveSharePointProject
        {
            get
            {
                EnvDTE.Project project = null;

                object[] projects = DTE.ActiveSolutionProjects as object[];
                if ((projects != null) && (projects.Length > 0))
                {
                    project = projects[0] as EnvDTE.Project;
                    return ProjectService.Convert<EnvDTE.Project, Microsoft.VisualStudio.SharePoint.ISharePointProject>(project);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the all SharePoint projects.
        /// </summary>
        /// <returns></returns>
        public static List<ISharePointProject> SharePointProjects
        {
            get
            {
                List<ISharePointProject> projects = new List<ISharePointProject>();
                foreach (ISharePointProject project in ProjectService.Projects)
                {
                    projects.Add(project);
                }
                return projects;
            }
        }

        /// <summary>
        /// Gets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public static SelectedItem SelectedItem
        {
            get
            {
                return DTE.SelectedItems.Item(1);
            }
        }

        /// <summary>
        /// Gets the project service.
        /// </summary>
        /// <value>The project service.</value>
        public static ISharePointProjectService ProjectService
        {
            get
            {
                if (_projectService == null)
                {
                    _projectService = (ISharePointProjectService)_globalServiceProvider.GetService(typeof(ISharePointProjectService));
                }
                return _projectService;
            }
        }

        /// <summary>
        /// Initializes the <see cref="DTEManager"/> class.
        /// </summary>
        static DTEManager()
        {
            DTE = (DTE)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(EnvDTE.DTE));
            _globalOleServiceProvider = (IOleServiceProvider)DTE;
            _globalServiceProvider = new ServiceProvider(_globalOleServiceProvider);
        }

        /// <summary>
        /// Gets the language file extension.
        /// </summary>
        /// <param name="projectItem">The project item.</param>
        /// <returns></returns>
        public static string GetLanguageFileExtension(ProjectItem projectItem)
        {
            switch (projectItem.ContainingProject.CodeModel.Language)
            {
                case "{B5E9BD34-6D3E-4B5D-925E-8A43B79820B4}":
                    return ".cs";

                case "{B5E9BD33-6D3E-4B5D-925E-8A43B79820B4}":
                    return ".vb";
            }
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the default root name space.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns></returns>
        public static string GetDefaultRootNameSpace(Project project)
        {
            return (string)project.Properties.Item("RootNameSpace").Value;
        }

        /// <summary>
        /// Gets the selected projects.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IVsHierarchy> GetSelectedProjects()
        {
            List<IVsHierarchy> list = new List<IVsHierarchy>();
            IntPtr hierarchyPointer = IntPtr.Zero;
            uint itemCount = 0;
            IntPtr ppSC = IntPtr.Zero;
            int singleItemPointer = 0;
            IVsMultiItemSelect multiSelect = null;
            try
            {
                IVsMonitorSelection monitorSelection = (IVsMonitorSelection)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsShellMonitorSelection));
                ErrorHandler.ThrowOnFailure(monitorSelection.GetCurrentSelection(
                    out hierarchyPointer, out itemCount, out multiSelect, out ppSC));
                if (hierarchyPointer != IntPtr.Zero)
                {
                    IVsHierarchy hierarchy = (IVsHierarchy)Marshal.GetObjectForIUnknown(hierarchyPointer);
                    list.Add(hierarchy);
                }
                else if (multiSelect != null)
                {
                    ErrorHandler.ThrowOnFailure(multiSelect.GetSelectionInfo(out itemCount, out singleItemPointer));
                    VSITEMSELECTION[] items = new VSITEMSELECTION[itemCount];
                    ErrorHandler.ThrowOnFailure(multiSelect.GetSelectedItems(0, itemCount, items));
                    list.AddRange(items.Select(item => item.pHier));
                }
                return list;
            }
            finally
            {
                if (hierarchyPointer != IntPtr.Zero)
                {
                    Marshal.Release(hierarchyPointer);
                }
                if (ppSC != IntPtr.Zero)
                {
                    Marshal.Release(ppSC);
                }
            }
        }

        /// <summary>
        /// Determines whether [is share point project] [the specified project].
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>
        /// 	<c>true</c> if [is share point project] [the specified project]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSharePointProject(EnvDTE.Project project)
        {
            return IsSharePointProject(project, false);
        }

        /// <summary>
        /// Determines whether [is share point project] [the specified project].
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="requireFarm">if set to <c>true</c> [require farm].</param>
        /// <returns>
        /// 	<c>true</c> if [is share point project] [the specified project]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSharePointProject(EnvDTE.Project project, bool requireFarm)
        {
            // Convert the DTE project into a SharePoint project. If the conversion fails, this is not a SP project.
            ISharePointProject p = ProjectService.Convert<EnvDTE.Project, Microsoft.VisualStudio.SharePoint.ISharePointProject>(project);
            if (p != null && requireFarm)
            {
                return !p.IsSandboxedSolution;
            }
            else
            {
                return p != null;
            }
        }

        /// <summary>
        /// Determines whether [is share point project] [the specified h].
        /// </summary>
        /// <param name="h">The h.</param>
        /// <returns>
        /// 	<c>true</c> if [is share point project] [the specified h]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSharePointProject(IVsHierarchy h)
        {
            IVsAggregatableProject project = h as IVsAggregatableProject;
            if (project != null)
            {
                string guidString = null;
                project.GetAggregateProjectTypeGuids(out guidString);
                IEnumerable<Guid> guids = guidString.Split(';').Select(s => new Guid(s));
                return guids.Contains(SharePointProjectTypeID);
            }
            return false;
        }

        /// <summary>
        /// Creates the new file.
        /// </summary>
        /// <param name="fileType">Type of the file.</param>
        /// <param name="title">The title.</param>
        /// <param name="fileContents">The file contents.</param>
        public static void CreateNewFile(
            string fileType, string title, string fileContents)
        {
            Document file = DTE.ItemOperations.NewFile(fileType, title).Document;
            TextSelection selection = file.Selection as TextSelection;
            selection.SelectAll();
            selection.Text = "";
            selection.Insert(fileContents);
            selection.StartOfDocument();
        }

        /// <summary>
        /// Creates the new text file.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="fileContents">The file contents.</param>
        public static void CreateNewTextFile(string title, string fileContents)
        {
            CreateNewFile(@"General\Text File", title, fileContents);
        }

        /// <summary>
        /// Finds the name of the item by.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="name">The name.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <returns>The project item.</returns>
        public static ProjectItem FindItemByName(ProjectItems collection, string name, bool recursive)
        {
            if (collection != null)
            {
                foreach (ProjectItem item in collection)
                {
                    if (item.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return item;
                    }
                    if (recursive)
                    {
                        ProjectItem item2 = FindItemByName(item.ProjectItems, name, recursive);
                        if (item2 != null)
                        {
                            return item2;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the name of the project by.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="isUniqueName">if set to <c>true</c> [is unique name].</param>
        /// <returns></returns>
        public static Project GetProjectByName(String projectName, bool isUniqueName = true)
        {
            Project result = null;
            foreach (Project project in DTEManager.DTE.Solution.Projects)
            {
                if (((isUniqueName) && (project.UniqueName == projectName)) || ((!isUniqueName) && (project.Name == projectName)))
                {
                    result = project;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Safes the delete project item.
        /// </summary>
        /// <param name="projectItem">The project item.</param>
        public static void SafeDeleteProjectItem(ProjectItem projectItem)
        {
            if (projectItem != null)
            {
                projectItem.Delete();
            }
        }

        /// <summary>
        /// Finds the name of the SPI item by.
        /// </summary>
        /// <param name="spiName">Name of the spi.</param>
        /// <param name="project">The project.</param>
        /// <returns></returns>
        public static ISharePointProjectItem FindSPIItemByName(string spiName, ISharePointProject project)
        {
            return (from ISharePointProjectItem spi
                    in project.ProjectItems
                    where spi.Name == spiName
                    select spi).FirstOrDefault();
        }

        /// <summary>
        /// Tries the get setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settingName">Name of the setting.</param>
        /// <param name="category">The category.</param>
        /// <param name="page">The page.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool TryGetSetting<T>(string settingName, string category, string page, out T value)
        {
            bool result = false;

            value = default(T);

            DTE dte = DTEManager.DTE;
            if (dte != null)
            {
                //                EnvDTE.Properties properties = dte.get_Properties(category, page) as EnvDTE.Properties;

                var properties = dte.get_Properties(category, page);
                if (properties != null)
                {
                    value = (T)properties.Item(settingName).Value;
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Set the status of the status bar.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public static void SetStatus(string message)
        {
            DTEManager.DTE.StatusBar.Text = message;
        }
    }
}
