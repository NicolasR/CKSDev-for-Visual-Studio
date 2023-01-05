using Microsoft.VisualStudio.SharePoint;
using Microsoft.WizardFramework;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards.Pages
{
    /// <summary>
    /// The base wizard page to provide the base functionality.
    /// </summary>
    class BaseWizardPage : WizardPage
    {
        /// <summary>
        /// 
        /// </summary>
        private bool _isLoadingView;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseWizardPage"/> class.
        /// </summary>
        public BaseWizardPage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseWizardPage"/> class.
        /// </summary>
        /// <param name="wizard">The wizard.</param>
        protected BaseWizardPage(ArtifactWizardForm wizard)
            : base(wizard)
        {
            base.ShowInfoPanel = false;
        }

        /// <summary>
        /// Applies the changes to presentation model.
        /// </summary>
        protected virtual void ApplyChangesToPresentationModel()
        {
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Determines whether [is completely valid].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is completely valid]; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsCompletelyValid()
        {
            return true;
        }

        /// <summary>
        /// Loads the settings.
        /// </summary>
        protected void LoadSettings()
        {
            if (!IsLoadingView)
            {
                try
                {
                    IsLoadingView = true;
                    LoadSettingsFromPresentationModel();
                }
                finally
                {
                    IsLoadingView = false;
                }
            }
        }

        /// <summary>
        /// Loads the settings from presentation model.
        /// </summary>
        protected virtual void LoadSettingsFromPresentationModel()
        {
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        public override void OnActivated()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                base.OnActivated();
                LoadSettings();
                if ((Wizard.NextPage != null) && !Wizard.NextPage.Visited)
                {
                    Wizard.DefaultButton = ButtonType.Next;
                }
            }
            catch (Exception ex)
            {
                DTEManager.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
                throw;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// Called when [deactivate].
        /// </summary>
        /// <returns></returns>
        public override bool OnDeactivate()
        {
            bool flag;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                flag = (base.OnDeactivate() && IsDataValid) && IsCompletelyValid();
            }
            catch (Exception ex)
            {
                DTEManager.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
                throw;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
            return flag;
        }

        /// <summary>
        /// Called when [deactivated].
        /// </summary>
        public override void OnDeactivated()
        {
            try
            {
                ApplyChangesToPresentationModel();
                base.OnDeactivated();
            }
            catch (Exception ex)
            {
                DTEManager.ProjectService.Logger.WriteLine("EXCEPTION: " + ex.Message, LogCategory.Error);
                throw;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is loading view.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is loading view; otherwise, <c>false</c>.
        /// </value>
        protected bool IsLoadingView
        {
            get
            {
                return _isLoadingView;
            }
            set
            {
                _isLoadingView = value;
            }
        }

        /// <summary>
        /// Gets the wizard.
        /// </summary>
        /// <value>The wizard.</value>
        public ArtifactWizardForm Wizard
        {
            [DebuggerStepThrough]
            get
            {
                return (ArtifactWizardForm)base.Wizard;
            }
        }
    }
}
