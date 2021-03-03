using EnvDTE;
using Microsoft.VisualStudio.SharePoint;
using System.Collections.Generic;
using System.Linq;
using VSLangProj;

namespace CKS.Dev2015.VisualStudio.SharePoint.Deployment
{
    /// <summary>
    /// Extensions to the ISharePointProjectItem object.
    /// </summary>
    public static class ISharePointProjectItemExtensions
    {
        #region Methods

        /// <summary>
        /// Gets all features in the given project whether packaged or not that contain this SPI.
        /// </summary>
        /// <param name="item">The ISharePointProjectItem being extended.</param>
        /// <param name="project">The current project.</param>
        /// <returns>Returns all features in the given project.</returns>
        public static List<ISharePointProjectFeature> ParentFeaturesInProject(this ISharePointProjectItem item, ISharePointProject project)
        {
            List<ISharePointProjectFeature> features = new List<ISharePointProjectFeature>();
            features.AddRange(
                  from feature
                    in project.Features
                  where feature.ProjectItems.Contains(item)
                  select feature
                );
            return features;
        }

        /// <summary>
        /// Determines if this SPI is packaged in the given project directly (i.e. not as part of a feature).
        /// </summary>
        /// <param name="item">The ISharePointProjectItem being extended.</param>
        /// <param name="project">The current project.</param>
        /// <returns>Returns true if this SPI is packaged in the given project directly (i.e. not as part of a feature).</returns>
        public static bool IsDirectPartOfProjectPackage(this ISharePointProjectItem item, ISharePointProject project)
        {
            return project.Package.ProjectItems.Contains(item);
        }

        /// <summary>
        /// Determines if this SPI is packaged in any project in the solution directly (i.e. not as part of a feature).
        /// </summary>
        /// <param name="item">The ISharePointProjectItem being extended.</param>
        /// <param name="service">The sharepoint project service.</param>
        /// <returns>Returns true if this SPI is packaged in any project in the solution directly (i.e. not as part of a feature).</returns>
        public static bool IsDirectPartOfAnyProjectPackage(this ISharePointProjectItem item, ISharePointProjectService service)
        {
            return service.Projects.Any(p => p.Package.ProjectItems.Contains(item));
        }

        /// <summary>
        /// Determines if this SPI is included inside a feature that is itself packaged in the given project.
        /// </summary>
        /// <param name="item">The ISharePointProjectItem being extended.</param>
        /// <param name="project">The current project.</param>
        /// <returns>Returns true if this SPI is included inside a feature that is itself packaged in the given project.</returns>
        public static bool IsPartOfPackagedProjectFeature(this ISharePointProjectItem item, ISharePointProject project)
        {
            return item.ParentFeaturesInProject(project).Any(feature => feature.IsPartOfProjectPackage(project));
        }

        /// <summary>
        /// Determines if this SPI is included inside a feature that is packaged in any project in the solution.
        /// </summary>
        /// <param name="item">The ISharePointProjectItem being extended.</param>
        /// <param name="service">The sharepoint project service.</param>
        /// <returns>Returns true if this SPI is included inside a feature that is packaged in any project in the solution.</returns>
        public static bool IsPartOfAnyPackagedProjectFeature(this ISharePointProjectItem item, ISharePointProjectService service)
        {
            foreach (ISharePointProject project in service.Projects)
            {
                if (item.IsPartOfPackagedProjectFeature(project))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if this SPI is packaged in the given project whether by feature or direct (e.g. mapped folder).
        /// </summary>
        /// <param name="item">The ISharePointProjectItem being extended.</param>
        /// <param name="project">The current project.</param>
        /// <returns>Returns true if this SPI is packaged in the given project whether by feature or direct (e.g. mapped folder).</returns>
        public static bool IsPartOfProjectPackage(this ISharePointProjectItem item, ISharePointProject project)
        {
            return item.IsDirectPartOfProjectPackage(project) || item.IsPartOfPackagedProjectFeature(project);
        }

        /// <summary>
        /// Determines if this SPI is packaged in any project in the solution whether by feature or direct (e.g. mapped folder).
        /// </summary>
        /// <param name="item">The ISharePointProjectItem being extended.</param>
        /// <param name="service">The sharepoint project service.</param>
        /// <returns>Returns true if this SPI is packaged in any project in the solution whether by feature or direct (e.g. mapped folder).</returns>
        public static bool IsPartOfAnyProjectPackage(this ISharePointProjectItem item, ISharePointProjectService service)
        {
            return item.IsDirectPartOfAnyProjectPackage(service) || item.IsPartOfAnyPackagedProjectFeature(service);
        }

        /// <summary>
        /// Retrieves all projects in the solution where this SPI is packaged.
        /// </summary>
        /// <param name="item">The ISharePointProjectItem being extended.</param>
        /// <param name="service">The sharepoint project service.</param>
        /// <returns>Returns all projects in the solution where this SPI is packaged.</returns>
        public static IEnumerable<ISharePointProject> GetProjectsWhereInPackage(this ISharePointProjectItem item, ISharePointProjectService service)
        {
            List<ISharePointProject> pkgProjects = new List<ISharePointProject>();

            foreach (ISharePointProject project in service.Projects)
            {
                if (item.IsPartOfProjectPackage(project))
                {
                    pkgProjects.Add(project);
                }
            }

            return pkgProjects;
        }

        /// <summary>
        /// Retrieves all packaged features this SPI is a part of within a specific project.
        /// </summary>
        /// <param name="item">The ISharePointProjectItem being extended.</param>
        /// <param name="project">The current project.</param>
        /// <returns>Returns all packaged features this SPI is a part of within a specific project.</returns>
        public static IEnumerable<ISharePointProjectFeature> GetFeaturesWhereInPackage(this ISharePointProjectItem item, ISharePointProject project)
        {
            return item.ParentFeaturesInProject(project).Where(feature => feature.IsPartOfProjectPackage(project));
        }

        /// <summary>
        /// Converts to project item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static ProjectItem ConvertToProjectItem(this ISharePointProjectItem item)
        {
            return item.DefaultFile.ProjectItem.Project.ProjectService.Convert<ISharePointProjectItemFile, ProjectItem>(item.DefaultFile);
        }

        /// <summary>
        /// Converts to VS project item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static VSProjectItem ConvertToVSProjectItem(this ISharePointProjectItem item)
        {
            ProjectItem dteItem = item.DefaultFile.ProjectItem.Project.ProjectService.Convert<ISharePointProjectItemFile, ProjectItem>(item.DefaultFile);
            return dteItem.Object as VSProjectItem;
        }

        #endregion
    }
}
