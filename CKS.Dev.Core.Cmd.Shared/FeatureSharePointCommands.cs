using CKS.Dev.VisualStudio.SharePoint.Commands.Info;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.VisualStudio.SharePoint.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace CKS.Dev.VisualStudio.SharePoint.Commands
{
    /// <summary>
    /// Feature commands.
    /// </summary>
    class FeatureSharePointCommands
    {
        #region Methods

        /// <summary>
        /// Gets the feature dependencies.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="featureID">The feature ID.</param>
        /// <returns>The feature dependancy infos.</returns>
        [SharePointCommand(FeatureSharePointCommandIds.GetFeatureDependencies)]
        public static FeatureDependencyInfo[] GetFeatureDependencies(ISharePointCommandContext context, FeatureInfo featureID)
        {
            List<FeatureDependencyInfo> dependencies = new List<FeatureDependencyInfo>();
            SPFeatureDefinition definition = SPFarm.Local.FeatureDefinitions[featureID.FeatureID];
            foreach (SPFeatureDependency dependency in definition.ActivationDependencies)
            {
                SPFeatureDefinition dependencyDefinition = SPFarm.Local.FeatureDefinitions[dependency.FeatureId];
                dependencies.Add(new FeatureDependencyInfo()
                {
                    FeatureID = dependency.FeatureId,
                    MinimumVersion = dependency.MinimumVersion.ToString(),
                    Title = dependencyDefinition.GetTitle(CultureInfo.CurrentCulture)
                });
            }
            return dependencies.ToArray();
        }

        /// <summary>
        /// Gets the feature elements.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="feature">The feature.</param>
        /// <returns>The feature element info.</returns>
        [SharePointCommand(FeatureSharePointCommandIds.GetFeatureElements)]
        public static FeatureElementInfo[] GetFeatureElements(ISharePointCommandContext context, FeatureInfo feature)
        {
            List<FeatureElementInfo> elements = new List<FeatureElementInfo>();
            SPFeatureDefinition definition = SPFarm.Local.FeatureDefinitions[feature.FeatureID];
            foreach (SPElementDefinition element in
                definition.GetElementDefinitions(CultureInfo.CurrentCulture)
                .OfType<SPElementDefinition>()
                .Distinct(new SPElementIDComparer()))
            {
                elements.Add(new FeatureElementInfo()
                {
                    ElementID = element.Id,
                    UIVersion = element.UIVersion,
                    FeatureID = feature.FeatureID,
                    Name = GetNameFromNode(element.XmlDefinition),
                    ElementType = element.ElementType
                });
            }
            return elements.ToArray();
        }

        /// <summary>
        /// Gets the name from node.
        /// </summary>
        /// <param name="xmlNode">The XML node.</param>
        /// <returns></returns>
        private static string GetNameFromNode(System.Xml.XmlNode xmlNode)
        {
            switch (xmlNode.Name)
            {
                case "ContentType":
                    return GetRootAttribute(xmlNode, "Name");
                case "ContentTypeBinding":
                    return "ContentTypeBinding";
                case "Control":
                    return GetRootAttribute(xmlNode, "Id");
                case "CustomAction":
                    return GetRootAttribute(xmlNode, "Title");
                case "CustomActionGroup":
                    return GetRootAttribute(xmlNode, "Title");
                case "DocumentConverter":
                    return GetRootAttribute(xmlNode, "Name");
                case "FeatureSiteTemplateAssociation":
                    return "FeatureSiteTemplateAssociation";
                case "Field":
                    return GetRootAttribute(xmlNode, "Name");
                case "GroupMigrator":
                    return "GroupMigrator";
                case "HideCustomAction":
                    return GetRootAttribute(xmlNode, "Id");
                case "ListInstance":
                    return GetRootAttribute(xmlNode, "Title");
                case "ListTemplate":
                    return GetRootAttribute(xmlNode, "Name");
                case "Module":
                    return GetRootAttribute(xmlNode, "Name");
                case "PropertyBag":
                    return "PropertyBag";
                case "Receivers":
                    return "Receivers";
                case "UserMigrator":
                    return "UserMigrator";
                case "WebPartAdderExtension":
                    return "WebPartAdderExtension";
                case "WebTemplate":
                    return GetRootAttribute(xmlNode, "Name");
                case "Workflow":
                    return GetRootAttribute(xmlNode, "Name");
                case "WorkflowActions":
                    return "WorkflowActions";
                case "WorkflowAssociation":
                    return GetRootAttribute(xmlNode, "Name");
            }
            return null;
        }

        /// <summary>
        /// Gets the root attribute.
        /// </summary>
        /// <param name="xmlNode">The XML node.</param>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        private static string GetRootAttribute(System.Xml.XmlNode xmlNode, string p)
        {
            XmlAttribute attribute = xmlNode.Attributes[p];
            return attribute != null ? attribute.Value : null;
        }

        /// <summary>
        /// Gets the element definition.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="elementInfo">The element info.</param>
        /// <returns>The defintion.</returns>
        [SharePointCommand(FeatureSharePointCommandIds.GetElementDefinition)]
        public static string GetElementDefinition(ISharePointCommandContext context,
            FeatureElementInfo elementInfo)
        {
            SPFeatureDefinition definition = SPFarm.Local.FeatureDefinitions[elementInfo.FeatureID];
            SPElementDefinition element = definition.GetElementDefinitions(CultureInfo.CurrentCulture)
                .OfType<SPElementDefinition>()
                .Where(searchElement => searchElement.ElementType == elementInfo.ElementType)
                .Where(searchElement => GetNameFromNode(searchElement.XmlDefinition) == elementInfo.Name)
                .FirstOrDefault();
            return element.XmlDefinition.OuterXml;
        }

        /// <summary>
        /// Determines whether if the feature is enabled.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="featureID">The feature ID.</param>
        /// <returns>
        /// 	<c>true</c> if feature is enabled otherwise, <c>false</c>.
        /// </returns>
        [SharePointCommand(FeatureSharePointCommandIds.IsFeatureEnabled)]
        public static bool IsFeatureEnabled(ISharePointCommandContext context, FeatureInfo featureID)
        {
            SPFeatureCollection featureCollection = GetFeatureCollectionForFeature(context, featureID);
            return featureCollection[featureID.FeatureID] != null;
        }

        /// <summary>
        /// Gets the feature collection for feature.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="featureID">The feature ID.</param>
        /// <returns>The feature collection.</returns>
        static SPFeatureCollection GetFeatureCollectionForFeature(ISharePointCommandContext context, FeatureInfo featureID)
        {
            if (featureID.Scope == FeatureScope.Web)
            {
                return context.Web.Features;
            }
            else if (featureID.Scope == FeatureScope.Site)
            {
                return context.Site.Features;
            }
            else if (featureID.Scope == FeatureScope.WebApplication)
            {
                return context.Site.WebApplication.Features;
            }
            else if (featureID.Scope == FeatureScope.Farm)
            {
                return SPWebService.ContentService.Features;
            }

            throw new ArgumentException("Unrecognized deployment scope.");
        }

        /// <summary>
        /// Enables the feature.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="featureID">The feature ID.</param>
        [SharePointCommand(FeatureSharePointCommandIds.EnableFeature)]
        public static void EnableFeature(ISharePointCommandContext context, FeatureInfo featureID)
        {
            SPFeatureCollection featureCollection = GetFeatureCollectionForFeature(context, featureID);
            featureCollection.Add(featureID.FeatureID, true);
        }

        /// <summary>
        /// Disables the feature.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="featureID">The feature ID.</param>
        [SharePointCommand(FeatureSharePointCommandIds.DisableFeature)]
        public static void DisableFeature(ISharePointCommandContext context, FeatureInfo featureID)
        {
            SPFeatureCollection featureCollection = GetFeatureCollectionForFeature(context, featureID);
            featureCollection.Remove(featureID.FeatureID, true);
        }

        #endregion
    }

    /// <summary>
    /// SPElementID comparer.
    /// </summary>
    class SPElementIDComparer : IEqualityComparer<SPElementDefinition>
    {
        /// <summary>
        /// Equalses the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public bool Equals(SPElementDefinition x, SPElementDefinition y)
        {
            return x.Id == y.Id;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(SPElementDefinition obj)
        {
            return obj.GetHashCode();
        }
    }
}
