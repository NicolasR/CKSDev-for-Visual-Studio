using EnvDTE;
using Microsoft.VisualStudio.SharePoint;
using System;
using System.Collections.Generic;

namespace CKS.Dev2015.VisualStudio.SharePoint.Content
{
    /// <summary>
    /// FullTrustProxy Refactoring
    /// </summary>
    class FullTrustProxyRefactoring
    {
        #region Methods

        /// <summary>
        /// Starts the listening.
        /// </summary>
        /// <param name="typeDefinition">The type definition.</param>
        internal static void StartListening(ISharePointProjectItemTypeDefinition typeDefinition)
        {
            typeDefinition.FileAdded += new EventHandler<SharePointProjectItemFileEventArgs>(TypeDefinition_FileAdded);

        }

        /// <summary>
        /// Handles the FileAdded event of the TypeDefinition control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.VisualStudio.SharePoint.SharePointProjectItemFileEventArgs"/> instance containing the event data.</param>
        static void TypeDefinition_FileAdded(object sender, SharePointProjectItemFileEventArgs e)
        {
            ISharePointProjectItemFile file = e.ProjectItemFile;
            if (file.DeploymentType == DeploymentType.NoDeployment)
            {
                ProjectItem dteItem = file.ProjectItem.Project.ProjectService.Convert<ISharePointProjectItemFile, ProjectItem>(file);
                if (dteItem.Kind == "Compile" && dteItem.FileCodeModel != null)
                {
                    foreach (EnvDTE.CodeClass codeClass in FindClasses(dteItem.FileCodeModel))
                    {
                        if (IsOperationClass(codeClass))
                        {

                        }
                    }
                }
            }
        }

        /// <summary>
        /// Finds the classes.
        /// </summary>
        /// <param name="fileCodeModel">The file code model.</param>
        /// <returns></returns>
        private static IEnumerable<CodeClass> FindClasses(FileCodeModel fileCodeModel)
        {
            return FindClassesRecursive(fileCodeModel.CodeElements);
        }

        /// <summary>
        /// Finds the classes recursive.
        /// </summary>
        /// <param name="codeElements">The code elements.</param>
        /// <returns></returns>
        private static IEnumerable<CodeClass> FindClassesRecursive(CodeElements codeElements)
        {
            foreach (CodeElement element in codeElements)
            {
                if (element is CodeClass)
                {
                    yield return (CodeClass)element;
                }
                else if (element is CodeNamespace)
                {
                    CodeNamespace codeNamespace = (CodeNamespace)element;
                    foreach (CodeClass codeClass in FindClassesRecursive(codeNamespace.Children))
                    {
                        yield return codeClass;
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether [is operation class] [the specified code class].
        /// </summary>
        /// <param name="codeClass">The code class.</param>
        /// <returns>
        /// 	<c>true</c> if [is operation class] [the specified code class]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsOperationClass(CodeClass codeClass)
        {
            return codeClass.get_IsDerivedFrom("SPProxyOperation");
        }

        #endregion
    }
}

