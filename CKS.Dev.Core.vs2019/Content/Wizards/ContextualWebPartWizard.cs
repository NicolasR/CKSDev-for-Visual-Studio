using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using System.Collections.Generic;
using System.IO;

namespace CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards
{
    /// <summary>
    /// The contextual tab web part wizard.
    /// </summary>
    class ContextualWebPartWizard : BaseWizard
    {
        /// <summary>
        /// Field for temp storage during the sprite move.
        /// </summary>
        ProjectItem spiItem;

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

        /// <summary>
        /// Run project item finished generating which will move the sprite png into the SPI due to
        /// some VS quirk which means you can't call 'replace params' in the vstemplate for png files.
        /// </summary>
        /// <param name="projectItem">The project item</param>
        public override void RunProjectItemFinishedGenerating(ProjectItem projectItem)
        {
            base.RunProjectItemFinishedGenerating(projectItem);

            if (projectItem.Name.ToLower() == "tempsprites.png")
            {
                //Find the temp sprite project item
                ProjectItem spritesItem = DTEManager.FindItemByName(projectItem.ContainingProject.ProjectItems, "tempsprites.png", true);
                //We don't want this so bin it from the solution
                spritesItem.Remove();

                ProjectItem parent = spiItem.Collection.Parent as ProjectItem;

                FileInfo sourceFile = new FileInfo(spritesItem.FileNames[0]);

                FileInfo parentFile = new FileInfo(parent.FileNames[0]);
                string newFileName = parentFile.Directory.ToString() + Path.DirectorySeparatorChar + "sprites.png";

                File.Move(sourceFile.ToString(), newFileName);

                parent.ProjectItems.AddFromFileCopy(newFileName);

                //Final tidy up
                if (sourceFile.Exists)
                {
                    sourceFile.Delete();
                }
            }
            else
            {
                spiItem = projectItem;
            }
        }
    }
}