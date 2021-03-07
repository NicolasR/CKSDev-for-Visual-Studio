using System.Collections.Generic;

namespace CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards
{
    /// <summary>
    /// Helper to create certificates
    /// </summary>
    class DefaultCertificateGenerator : ICertificateGenerator
    {
        #region Fields

        /// <summary>
        /// The _strong name generated
        /// </summary>
        bool _strongNameGenerated;

        /// <summary>
        /// The project manager
        /// </summary>
        StrongNameProjectManager projectManager = new StrongNameProjectManager();

        #endregion

        #region Methods

        /// <summary>
        /// Adds the key file.
        /// </summary>
        /// <param name="project">The project.</param>
        public void AddKeyFile(EnvDTE.Project project)
        {
            if (this._strongNameGenerated)
            {
                this.projectManager.AddKeyFileToProject(project);
                this._strongNameGenerated = false;
            }
        }

        /// <summary>
        /// Generates the key file.
        /// </summary>
        /// <param name="replacementsDictionary">The replacements dictionary.</param>
        public void GenerateKeyFile(Dictionary<string, string> replacementsDictionary)
        {
            this.projectManager.GenerateKey();
            this.projectManager.AddKeyToDictionary(replacementsDictionary);
            this._strongNameGenerated = true;
        }

        #endregion
    }
}
