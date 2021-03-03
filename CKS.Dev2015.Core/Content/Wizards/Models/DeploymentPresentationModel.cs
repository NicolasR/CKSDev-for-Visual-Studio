using CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards.WizardProperties;
using System;

namespace CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards.Models
{
    class DeploymentPresentationModel : BasePresentationModel
    {
        #region Fields

        // Fields
        private DeploymentProperties _deploymentProperties;
        private bool _enableUserSolutionInput;

        #endregion

        #region Properties

        public bool EnableUserSolutionInput
        {
            get
            {
                return this._enableUserSolutionInput;
            }
            set
            {
                this._enableUserSolutionInput = value;
                if (!this._enableUserSolutionInput)
                {
                    this.IsSandboxedSolution = false;
                }
            }
        }

        public bool IsSandboxedSolution
        {
            get;
            set;
        }

        public bool IsUrlFormatValid
        {
            get
            {
                return ((this.Url.Length > 0) && Uri.IsWellFormedUriString(Uri.EscapeUriString(this.Url), UriKind.Absolute));
            }
        }

        public bool IsUrlModified
        {
            get
            {
                return !string.Equals(this.Url.Trim(), this._deploymentProperties.Url.OriginalString.Trim(), StringComparison.OrdinalIgnoreCase);
            }
        }

        public bool IsValidatingWithSharePoint
        {
            get
            {
                return false;// SharePointWeb.IsValidatingSite;
            }
        }

        public object MruUrlList
        {
            get
            {
                return this._deploymentProperties.MruUrlList;
            }
        }

        public string Url
        {
            get;
            set;
        }

        public bool ValidateUrlWithSharePoint
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public DeploymentPresentationModel(DeploymentProperties deploymentProperties, bool isOptional, bool validateUrlWithSharePoint)
            : base(isOptional)
        {
            this._deploymentProperties = deploymentProperties;
            this._enableUserSolutionInput = true;
            this.Url = this._deploymentProperties.Url.ToString();
            this.IsSandboxedSolution = this._deploymentProperties.IsSandboxedSolution;
            this.ValidateUrlWithSharePoint = validateUrlWithSharePoint;
        }

        public bool IsUrlValid(out string errorMessage)
        {
            errorMessage = string.Empty;
            if (!this.IsUrlFormatValid)
            {
                errorMessage = "WizardResources.InvalidUrlFormat"; //TODO: resource this
                return false;
            }
            bool flag = false;
            try
            {
                if (this.IsValidatingWithSharePoint)
                {
                    errorMessage = "WizardResources.SharePointConnectionBusyErrorMessage";//TODO: resource this
                    return false;
                }
                Uri url = new Uri(this.Url);
                //SharePointWeb.ValidateSiteWhileShowingProgress(url); 
                flag = true;
            }
            catch (Exception exception)
            {
                errorMessage = exception.ToString();// SharePointWeb.GetSharePointConnectionErrorMessage(exception, this.Url);
                flag = false;
                //WizardHelpers.Logger.LogException(exception);
            }
            return flag;
        }

        public override void SaveChanges()
        {
            this._deploymentProperties.Url = new Uri(EnsureTrailingChar(this.Url.Trim(), '/'));
            this._deploymentProperties.IsSandboxedSolution = this.IsSandboxedSolution;
        }

        internal static string EnsureTrailingChar(string path, char trailingChar)
        {
            if (!string.IsNullOrEmpty(path) && ((path.ToCharArray().Length - 1) != trailingChar))
            {
                return (path + trailingChar);
            }
            return path;
        }




        #endregion
    }
}
