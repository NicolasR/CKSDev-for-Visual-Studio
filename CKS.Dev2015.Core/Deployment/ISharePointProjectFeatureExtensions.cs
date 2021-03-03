using Microsoft.VisualStudio.SharePoint;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CKS.Dev2015.VisualStudio.SharePoint.Deployment
{
    /// <summary>
    /// Extensions to the ISharePointProjectFeature object.
    /// </summary>
    public static class ISharePointProjectFeatureExtensions
    {
        #region Methods

        /// <summary>
        /// Determines if this Feature is packaged in the given project.
        /// </summary>
        /// <param name="feature">The ISharePointProjectFeature being extended.</param>
        /// <param name="project">The current sharepoint project.</param>
        /// <returns>Returns true if this Feature is packaged in the given project.</returns>
        public static bool IsPartOfProjectPackage(this ISharePointProjectFeature feature, ISharePointProject project)
        {
            return project.Package.Features.Contains(feature);
        }

        /// <summary>
        /// Determines if this Feature is packaged in any project in the solution.
        /// </summary>
        /// <param name="feature">The ISharePointProjectFeature being extended.</param>
        /// <param name="service">The current sharepoint service.</param>
        /// <returns>Returns true if this Feature is packaged in any project in the solution.</returns>
        public static bool IsPartOfAnyProjectPackage(this ISharePointProjectFeature feature, ISharePointProjectService service)
        {
            return service.Projects.Any(p => p.Package.Features.Contains(feature));
        }

        /// <summary>
        /// Retrieves all projects in the solution where this feature is packaged.
        /// </summary>
        /// <param name="feature">The ISharePointProjectFeature being extended.</param>
        /// <param name="service">The current sharepoint service.</param>
        /// <returns>Returns all projects in the solution where this feature is packaged.</returns>
        public static IEnumerable<ISharePointProject> GetProjectsWhereInPackage(this ISharePointProjectFeature feature, ISharePointProjectService service)
        {
            return service.Projects.Where(p => p.Package.Features.Contains(feature));
        }

        //TODO: is this right? it has the same token twice
        /// <summary>
        /// Uns the tokenize.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="tokenString">The token string.</param>
        /// <returns></returns>
        public static string UnTokenize(this ISharePointProjectFeature feature, string tokenString)
        {
            if (feature != null)
            {
                tokenString = tokenString.Replace("$SharePoint.Project.FileNameWithoutExtension$", Path.GetFileNameWithoutExtension(feature.Project.FullPath));
                tokenString = tokenString.Replace("$SharePoint.Feature.FileNameWithoutExtension$", DeploymentUtilities.GetLastFolderName(feature.FullPath));
            }
            return tokenString;
        }

        #endregion
    }
}
