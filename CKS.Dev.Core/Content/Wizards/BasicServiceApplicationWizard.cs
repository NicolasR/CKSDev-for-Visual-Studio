using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using System.Collections.Generic;

namespace CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards
{
    class BasicServiceApplicationWizard : BaseWizard
    {
        /// <summary>
        /// Set the project properties
        /// </summary>
        /// <param name="project">The project</param>
        public override void SetProjectProperties(EnvDTE.Project project)
        {
            //ProjectManager projectManager = ProjectManager.Create(project);
        }

        /// <summary>
        /// Gets a flag indicating whether a SharePoint connection is required
        /// </summary>
        /// <value></value>
        protected override bool IsSharePointConnectionRequired
        {
            get { return true; }
        }

        /// <summary>
        /// Create the wizard form
        /// </summary>
        /// <param name="designTimeEnvironment">The design time environment</param>
        /// <param name="runKind">The wizard run kind</param>
        /// <returns>The IWizardFormExtension</returns>
        public override IWizardFormExtension CreateWizardForm(DTE designTimeEnvironment, WizardRunKind runKind)
        {
            return null;
        }

        /// <summary>
        /// Initialise from the the wizard data
        /// </summary>
        /// <param name="replacementsDictionary">The replacements dictionary</param>
        public override void InitializeFromWizardData(Dictionary<string, string> replacementsDictionary)
        {
            base.InitializeFromWizardData(replacementsDictionary);

            if (replacementsDictionary.ContainsKey("$rootname$"))
            {
                replacementsDictionary.Add("$subnamespace$", WizardHelpers.MakeNameCompliant(replacementsDictionary["$rootname$"]));
            }
        }
    }
}
