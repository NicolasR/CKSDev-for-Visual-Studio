using Microsoft.VisualStudio.SharePoint;
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards
{
    class WizardHelpers
    {
        public static Uri GetDefaultProjectUrl()
        {
            List<ISharePointProject> spProjects = DTEManager.SharePointProjects;

            foreach (var project in spProjects)
            {
                return project.SiteUrl;

            }

            throw new WizardCancelledException("WizardResources.WizardOnlyValidInSharePointProject");
        }

        public static void CheckMissingSiteUrl(Uri projectUrl)
        {
            if (projectUrl != null)
            {
                return;
            }
            else
            {
                //WizardHelpers.ShowError(WizardResources.SiteUrlMissing, WizardResources.ValidationErrorCaption);
                throw new WizardCancelledException("WizardResources.SiteUrlMissing");
            }
        }

        /// <summary>
        /// Makes the name compliant.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string MakeNameCompliant(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }
            if (char.IsDigit(name.ToCharArray()[0]))
            {
                name = "_" + name;
            }
            StringBuilder builder = new StringBuilder(name.Length);
            string str = name;
            for (int i = 0; i < str.Length; i++)
            {
                char currentChar = str.ToCharArray()[i];
                if (IsValidCharForName(currentChar))
                {
                    builder.Append(currentChar);
                }
                else
                {
                    builder.Append('_');
                }
            }
            name = builder.ToString();
            return name;
        }

        /// <summary>
        /// Determines whether [is valid char for name] [the specified current char].
        /// </summary>
        /// <param name="currentChar">The current char.</param>
        /// <returns>
        /// 	<c>true</c> if [is valid char for name] [the specified current char]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidCharForName(char currentChar)
        {
            UnicodeCategory unicodeCategory = char.GetUnicodeCategory(currentChar);
            if (((((unicodeCategory != UnicodeCategory.UppercaseLetter) && (unicodeCategory != UnicodeCategory.LowercaseLetter))
                && ((unicodeCategory != UnicodeCategory.OtherLetter) && (unicodeCategory != UnicodeCategory.ConnectorPunctuation)))
                && (((unicodeCategory != UnicodeCategory.ModifierLetter) && (unicodeCategory != UnicodeCategory.NonSpacingMark)) && ((unicodeCategory != UnicodeCategory.SpacingCombiningMark) && (unicodeCategory != UnicodeCategory.TitlecaseLetter)))) && (((unicodeCategory != UnicodeCategory.Format) && (unicodeCategory != UnicodeCategory.LetterNumber)) && ((unicodeCategory != UnicodeCategory.DecimalDigitNumber) && (currentChar != '.'))))
            {
                return (currentChar == '_');
            }
            return true;

        }

        //TODO: make this call down into the 'projectutilities' class for this
        //public static string GetTargetOfficeVersion(WizardRunKind kind, Dictionary<string, string> replacementsDictionary)
        //{
        //    string targetOfficeVersion = null;
        //    if (kind != WizardRunKind.AsNewItem)
        //    {
        //        if (replacementsDictionary != null)
        //        {
        //            replacementsDictionary.TryGetValue("$TargetOfficeVersion$", out targetOfficeVersion);
        //        }
        //    }
        //    else
        //    {
        //        ISharePointProject activeSharePointProject = WizardHelpers.GetActiveSharePointProject();
        //        targetOfficeVersion = activeSharePointProject.TargetOfficeVersion;
        //    }
        //    if (string.IsNullOrEmpty(targetOfficeVersion))
        //    {
        //        targetOfficeVersion = "14.0";
        //    }
        //    return targetOfficeVersion;
        //}

        /// <summary>
        /// Determines whether [is valid URI string] [the specified URI].
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="kind">The kind.</param>
        /// <returns>
        ///   <c>true</c> if [is valid URI string] [the specified URI]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidUriString(string uri, UriKind kind)
        {
            Uri uri1 = null;
            return IsValidUriString(uri, kind, out uri1);
        }

        /// <summary>
        /// Determines whether [is valid URI string] [the specified URI].
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="kind">The kind.</param>
        /// <param name="result">The result.</param>
        /// <returns>
        ///   <c>true</c> if [is valid URI string] [the specified URI]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidUriString(string uri, UriKind kind, out Uri result)
        {
            return Uri.TryCreate(uri, kind, out result);
        }
    }
}
