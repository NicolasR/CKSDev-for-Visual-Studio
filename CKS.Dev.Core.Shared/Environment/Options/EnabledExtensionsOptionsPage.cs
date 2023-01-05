using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace CKS.Dev.VisualStudio.SharePoint.Environment.Options
{
    /// <summary>
    /// The CKSDev general options page.
    /// </summary>
    [DefaultProperty("CancelAddingSPIs")]
    public class EnabledExtensionsOptionsPage : DialogPage
    {
        #region Environment
        private bool _CancelAddingSPIs = true;
        /// <summary>
        /// Gets or sets a value indicating whether [cancel adding SP is].
        /// </summary>
        /// <value><c>true</c> if [cancel adding SP is]; otherwise, <c>false</c>.</value>
        [Category("Environment")]
        [DisplayName("Cancel Adding SharePoint Project Items")]
        [Description("Disables the default behavior of automatically adding SharePoint Project Items to Features.")]
        [DefaultValue(true)]
        public bool CancelAddingSPIs { get { return _CancelAddingSPIs; } set { _CancelAddingSPIs = value; } }

        private bool _CopyAssemblyName = true;
        /// <summary>
        /// Gets or sets a value indicating whether [copy assembly name].
        /// </summary>
        /// <value><c>true</c> if [copy assembly name]; otherwise, <c>false</c>.</value>
        [Category("Environment")]
        [DisplayName("Copy Assembly Name")]
        [Description("Copies the assembly name of a SharePoint project onto the clipboard.")]
        [DefaultValue(true)]
        public bool CopyAssemblyName { get { return _CopyAssemblyName; } set { _CopyAssemblyName = value; } }

        private bool _SPIReferences = true;
        /// <summary>
        /// Gets or sets a value indicating whether [SPI references].
        /// </summary>
        /// <value><c>true</c> if [SPI references]; otherwise, <c>false</c>.</value>
        [Category("Environment")]
        [DisplayName("SPI References")]
        [Description("Allows you to easily manage SPI references")]
        [DefaultValue(true)]
        public bool SPIReferences { get { return _SPIReferences; } set { _SPIReferences = value; } }

        private bool _ActivateSelectedFeatures = true;
        /// <summary>
        /// Gets or sets a value indicating whether [activate selected features].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [activate selected features]; otherwise, <c>false</c>.
        /// </value>
        [Category("Environment")]
        [DisplayName("Activate Selected Features")]
        [Description("Allows you to select which Features should be activated during the deployment process.")]
        [DefaultValue(true)]
        public bool ActivateSelectedFeatures { get { return _ActivateSelectedFeatures; } set { _ActivateSelectedFeatures = value; } }
        #endregion

        #region Exploration
        #region Site node
        private bool _DeveloperDashboardSettings = true;
        /// <summary>
        /// Gets or sets a value indicating whether [developer dashboard settings].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [developer dashboard settings]; otherwise, <c>false</c>.
        /// </value>
        [Category("Exploration")]
        [DisplayName("Developer Dashboard Settings")]
        [Description("Allows the sites farm developer dashboard settings to be changed from with Visual Studio 2010.")]
        [DefaultValue(true)]
        public bool DeveloperDashboardSettings { get { return _DeveloperDashboardSettings; } set { _DeveloperDashboardSettings = value; } }

        private bool _OpenInSharePointDesigner = true;
        /// <summary>
        /// Gets or sets a value indicating whether [open in share point designer].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [open in share point designer]; otherwise, <c>false</c>.
        /// </value>
        [Category("Exploration")]
        [DisplayName("Open in SharePoint Designer")]
        [DefaultValue(true)]
        public bool OpenInSharePointDesigner { get { return _OpenInSharePointDesigner; } set { _OpenInSharePointDesigner = value; } }

        private bool _SiteGenerateEntityClasses = true;
        /// <summary>
        /// Gets or sets a value indicating whether [site generate entity classes].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [site generate entity classes]; otherwise, <c>false</c>.
        /// </value>
        [Category("Exploration")]
        [DisplayName("Generate Entity Classes (site)")]
        [Description("Create the SPMetal classes for the site.")]
        [DefaultValue(true)]
        public bool SiteGenerateEntityClasses { get { return _SiteGenerateEntityClasses; } set { _SiteGenerateEntityClasses = value; } }

        private bool _ViewMasterPageAndPageLayoutGallery = true;
        /// <summary>
        /// Gets or sets a value indicating whether [view master page and page layout gallery].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [view master page and page layout gallery]; otherwise, <c>false</c>.
        /// </value>
        [Category("Exploration")]
        [DisplayName("View Master Page and Page Layout Gallery")]
        [Description("Adds the Master Page Gallery node which allows you to browse through Master Pages and Page Layouts. Additionally it allows you to view and edit the contents of the files from the Master Page Gallery.")]
        [DefaultValue(true)]
        public bool ViewMasterPageAndPageLayoutGallery { get { return _ViewMasterPageAndPageLayoutGallery; } set { _ViewMasterPageAndPageLayoutGallery = value; } }

        private bool _ViewSolutionGallery = true;
        /// <summary>
        /// Gets or sets a value indicating whether [view solution gallery].
        /// </summary>
        /// <value><c>true</c> if [view solution gallery]; otherwise, <c>false</c>.</value>
        [Category("Exploration")]
        [DisplayName("View Solution Gallery")]
        [DefaultValue(true)]
        public bool ViewSolutionGallery { get { return _ViewSolutionGallery; } set { _ViewSolutionGallery = value; } }
        #endregion

        #region Features node
        private bool _ViewFeatureDependencies = true;
        /// <summary>
        /// Gets or sets a value indicating whether [view feature dependencies].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [view feature dependencies]; otherwise, <c>false</c>.
        /// </value>
        [Category("Exploration")]
        [DisplayName("View Feature Dependencies")]
        [Description("Adds subnodes to the Feature node to drill down into feature dependencies.")]
        [DefaultValue(true)]
        public bool ViewFeatureDependencies { get { return _ViewFeatureDependencies; } set { _ViewFeatureDependencies = value; } }

        private bool _ViewFeatureElements = true;
        /// <summary>
        /// Gets or sets a value indicating whether [view feature elements].
        /// </summary>
        /// <value><c>true</c> if [view feature elements]; otherwise, <c>false</c>.</value>
        [Category("Exploration")]
        [DisplayName("View Feature Elements")]
        [Description("Adds subnodes to the Feature node to drill down into feature element definitions. Also allows to open the XML definition of each element.")]
        [DefaultValue(true)]
        public bool ViewFeatureElements { get { return _ViewFeatureElements; } set { _ViewFeatureElements = value; } }

        private bool _ActivateDeactivateFeature = true;
        /// <summary>
        /// Gets or sets a value indicating whether [activate deactivate feature].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [activate deactivate feature]; otherwise, <c>false</c>.
        /// </value>
        [Category("Exploration")]
        [DisplayName("Activate/Deactivate Feature")]
        [Description("Adds a content menu item to each feature node to enable and disable the feature on the current site, site collection, web application or farm.")]
        [DefaultValue(true)]
        public bool ActivateDeactivateFeature { get { return _ActivateDeactivateFeature; } set { _ActivateDeactivateFeature = value; } }

        private bool _FeatureCopyID = true;
        /// <summary>
        /// Gets or sets a value indicating whether [feature copy ID].
        /// </summary>
        /// <value><c>true</c> if [feature copy ID]; otherwise, <c>false</c>.</value>
        [Category("Exploration")]
        [DisplayName("Copy ID (Feature)")]
        [DefaultValue(true)]
        public bool FeatureCopyID { get { return _FeatureCopyID; } set { _FeatureCopyID = value; } }
        #endregion

        #region Style Library node
        private bool _ViewStyleLibrary = true;
        /// <summary>
        /// Gets or sets a value indicating whether [view style library].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [view style library]; otherwise, <c>false</c>.
        /// </value>
        [Category("Exploration")]
        [DisplayName("View Style Library")]
        [Description("Adds the Style Library node which allows you to browse through the contents of the Style Library. Additionally it allows you to view and edit the contents of the files from the Style Library.")]
        [DefaultValue(true)]
        public bool ViewStyleLibrary { get { return _ViewStyleLibrary; } set { _ViewStyleLibrary = value; } }
        #endregion

        #region Design Catalog node
        private bool _ViewDesignCatalog = true;
        /// <summary>
        /// Gets or sets a value indicating whether [view design catalog].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [view design catalog]; otherwise, <c>false</c>.
        /// </value>
        [Category("Exploration")]
        [DisplayName("View Design Catalog")]
        [Description("Adds the Design Catalog node which allows you to browse through the contents of the Design Catalog. Additionally it allows you to view and edit the contents of the files from the Design Catalog.")]
        [DefaultValue(true)]
        public bool ViewDesignCatalog { get { return _ViewDesignCatalog; } set { _ViewDesignCatalog = value; } }
        #endregion

        #region Theme Gallery node
        private bool _ListThemes = true;
        /// <summary>
        /// Gets or sets a value indicating whether [list themes].
        /// </summary>
        /// <value><c>true</c> if [list themes]; otherwise, <c>false</c>.</value>
        [Category("Exploration")]
        [DisplayName("List Themes")]
        [Description("Browse themes and view their properties.")]
        [DefaultValue(true)]
        public bool ListThemes { get { return _ListThemes; } set { _ListThemes = value; } }
        #endregion

        #region Web Part Gallery node
        private bool _ListWebParts = true;
        /// <summary>
        /// Gets or sets a value indicating whether [list web parts].
        /// </summary>
        /// <value><c>true</c> if [list web parts]; otherwise, <c>false</c>.</value>
        [Category("Exploration")]
        [DisplayName("List Web Parts")]
        [Description("Browse Web Parts and view their properties.")]
        [DefaultValue(true)]
        public bool ListWebParts { get { return _ListWebParts; } set { _ListWebParts = value; } }
        #endregion

        #region List node
        private bool _ListGenerateEntityClasses = true;
        /// <summary>
        /// Gets or sets a value indicating whether [list generate entity classes].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [list generate entity classes]; otherwise, <c>false</c>.
        /// </value>
        [Category("Exploration")]
        [DisplayName("Generate Entity Classes (List)")]
        [Description("Create the SPMetal classes for the list.")]
        [DefaultValue(true)]
        public bool ListGenerateEntityClasses { get { return _ListGenerateEntityClasses; } set { _ListGenerateEntityClasses = value; } }

        private bool _ViewListEventReceivers = true;
        /// <summary>
        /// Gets or sets a value indicating whether [view list event receivers].
        /// </summary>
        /// <value>
        /// <c>true</c> if [view list event receivers]; otherwise, <c>false</c>.
        /// </value>
        [Category("Exploration")]
        [DisplayName("View List Event Receivers")]
        [Description("Adds a subnode to the List node to drill down into Event Receivers associated with the List.")]
        [DefaultValue(true)]
        public bool ViewListEventReceivers { get { return _ViewListEventReceivers; } set { _ViewListEventReceivers = value; } }
        #endregion

        #region File node
        private bool _FileOperations = true;
        /// <summary>
        /// Gets or sets a value indicating whether [file operations].
        /// </summary>
        /// <value><c>true</c> if [file operations]; otherwise, <c>false</c>.</value>
        [Category("Exploration")]
        [DisplayName("File operations")]
        [DefaultValue(true)]
        public bool FileOperations { get { return _FileOperations; } set { _FileOperations = value; } }

        private bool _FileProperties = true;
        /// <summary>
        /// Gets or sets a value indicating whether [file properties].
        /// </summary>
        /// <value><c>true</c> if [file properties]; otherwise, <c>false</c>.</value>
        [Category("Exploration")]
        [DisplayName("File properties")]
        [DefaultValue(true)]
        public bool FileProperties { get { return _FileProperties; } set { _FileProperties = value; } }
        #endregion

        #region Content Type Group node
        private bool _ContentTypesGroupedView = true;
        /// <summary>
        /// Gets or sets a value indicating whether [content types grouped view].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [content types grouped view]; otherwise, <c>false</c>.
        /// </value>
        [Category("Exploration")]
        [DisplayName("Content Types grouped view")]
        [Description("Grouped view for Content Types")]
        [DefaultValue(true)]
        public bool ContentTypesGroupedView { get { return _ContentTypesGroupedView; } set { _ContentTypesGroupedView = value; } }

        private bool _ImportContentTypeGroup = true;
        /// <summary>
        /// Gets or sets a value indicating whether [import content type group].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [import content type group]; otherwise, <c>false</c>.
        /// </value>
        [Category("Exploration")]
        [DisplayName("Import Content Type Group")]
        [DefaultValue(true)]
        public bool ImportContentTypeGroup { get { return _ImportContentTypeGroup; } set { _ImportContentTypeGroup = value; } }
        #endregion

        #region Content Type node
        private bool _CreatePageLayoutFromContentType = true;
        /// <summary>
        /// Gets or sets a value indicating whether [create page layout from content type].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [create page layout from content type]; otherwise, <c>false</c>.
        /// </value>
        [Category("Exploration")]
        [DisplayName("Create Page Layout From Content Type")]
        [Description("Adds a menu to content types that allows you to generate the contents of a Page Layout for the given Content Type.")]
        [DefaultValue(true)]
        public bool CreatePageLayoutFromContentType { get { return _CreatePageLayoutFromContentType; } set { _CreatePageLayoutFromContentType = value; } }

        private bool _ImportContentType = true;
        /// <summary>
        /// Gets or sets a value indicating whether [import content type].
        /// </summary>
        /// <value><c>true</c> if [import content type]; otherwise, <c>false</c>.</value>
        [Category("Exploration")]
        [DisplayName("Import Content Type")]
        [DefaultValue(true)]
        public bool ImportContentType { get { return _ImportContentType; } set { _ImportContentType = value; } }

        private bool _ContentTypeCopyID = true;
        /// <summary>
        /// Gets or sets a value indicating whether [content type copy ID].
        /// </summary>
        /// <value><c>true</c> if [content type copy ID]; otherwise, <c>false</c>.</value>
        [Category("Exploration")]
        [DisplayName("Copy ID (Content Type)")]
        [DefaultValue(true)]
        public bool ContentTypeCopyID { get { return _ContentTypeCopyID; } set { _ContentTypeCopyID = value; } }
        #endregion

        #region Site Column Group node
        private bool _SiteColumnsGroupedView = true;
        /// <summary>
        /// Gets or sets a value indicating whether [site columns grouped view].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [site columns grouped view]; otherwise, <c>false</c>.
        /// </value>
        [Category("Exploration")]
        [DisplayName("Site Columns grouped view")]
        [Description("Grouped view for Site Columns")]
        [DefaultValue(true)]
        public bool SiteColumnsGroupedView { get { return _SiteColumnsGroupedView; } set { _SiteColumnsGroupedView = value; } }

        private bool _ImportSiteColumnGroup = true;
        /// <summary>
        /// Gets or sets a value indicating whether [import site column group].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [import site column group]; otherwise, <c>false</c>.
        /// </value>
        [Category("Exploration")]
        [DisplayName("Import Site Column Group")]
        [DefaultValue(true)]
        public bool ImportSiteColumnGroup { get { return _ImportSiteColumnGroup; } set { _ImportSiteColumnGroup = value; } }
        #endregion

        #region Field node
        private bool _ImportField = true;
        /// <summary>
        /// Gets or sets a value indicating whether [import field].
        /// </summary>
        /// <value><c>true</c> if [import field]; otherwise, <c>false</c>.</value>
        [Category("Exploration")]
        [DisplayName("Import Field")]
        [DefaultValue(true)]
        public bool ImportField { get { return _ImportField; } set { _ImportField = value; } }

        private bool _FieldCopyID = true;
        /// <summary>
        /// Gets or sets a value indicating whether [field copy ID].
        /// </summary>
        /// <value><c>true</c> if [field copy ID]; otherwise, <c>false</c>.</value>
        [Category("Exploration")]
        [DisplayName("Copy ID (Field)")]
        [DefaultValue(true)]
        public bool FieldCopyID { get { return _FieldCopyID; } set { _FieldCopyID = value; } }

        #endregion
        #endregion

        /// <summary>
        /// Resets the settings.
        /// </summary>
        public override void ResetSettings()
        {
            base.ResetSettings();

            // Enable all extensions

            CancelAddingSPIs = true;
            CopyAssemblyName = true;
            SPIReferences = true;
            ActivateSelectedFeatures = true;

            DeveloperDashboardSettings = true;
            OpenInSharePointDesigner = true;
            SiteGenerateEntityClasses = true;
            ViewMasterPageAndPageLayoutGallery = true;
            ViewSolutionGallery = true;

            ViewFeatureDependencies = true;
            ViewFeatureElements = true;
            ActivateDeactivateFeature = true;
            FeatureCopyID = true;

            ViewStyleLibrary = true;
            ViewDesignCatalog = true;

            ListThemes = true;

            ListWebParts = true;

            ListGenerateEntityClasses = true;
            ViewListEventReceivers = true;

            FileOperations = true;
            FileProperties = true;

            ContentTypesGroupedView = true;
            ImportContentTypeGroup = true;

            CreatePageLayoutFromContentType = true;
            ImportContentType = true;
            ContentTypeCopyID = true;

            ImportField = true;
            FieldCopyID = true;
        }

        /// <summary>
        /// Gets the setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settingName">Name of the setting.</param>
        /// <returns></returns>
        public static T GetSetting<T>(string settingName)
        {
            return GetSetting<T>(settingName, default(T));
        }

        /// <summary>
        /// Gets the setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settingName">Name of the setting.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static T GetSetting<T>(string settingName, T defaultValue)
        {
            T setting = defaultValue;

            //if (!DTEManager.TryGetSetting<T>(settingName, "CKS Development Tools Edition", "Extensions", out setting))
            //{
            //    setting = defaultValue;
            //}

            return setting;
        }
    }
}
