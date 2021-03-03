using System;

namespace CKS.Dev2015.VisualStudio.SharePoint.Environment
{
    public class GuidList
    {
        /// <summary>
        /// The package unique ID
        /// </summary>




        public const string guidCKSDEV_ComponentPickerPageSharePoint = "BB8F11D5-5618-4764-86C9-6A3D06AA7E3D";

        public const string guidCKSDEV_Extensions_PackageCmdSetString = "d0ec82a9-563b-46a9-8ac7-4a561f69b0cb";

        public static readonly Guid guidCKSDEV_Extensions_PackageCmdSet = new Guid(guidCKSDEV_Extensions_PackageCmdSetString);

        public const string guidCKSDEVPkgString = "F120F40F-F543-4d15-8BBB-4F4B174C6A23";

        #region Symbols

        /// <summary>
        /// The guid for the GuidSymbol name="guidCKSDEV11_Extensions_PackagePkgString" value="{cd4a976c-3044-47e1-ab20-171731092ad6}"  in the VSPackage.vsct
        /// </summary>
        public const string guidCKS_Dev11_PkgString = "cd4a976c-3044-47e1-ab20-171731092ad6";
        public static readonly Guid guidCKS_Dev11_Pkg = new Guid(guidCKS_Dev11_PkgString);

        /// <summary>
        /// The guid for the GuidSymbol name="guidCKSDEV11_Extensions_PackagePkgString" value="{555c8e33-1a5e-437a-b98a-62b610221cd7}"  in the VSPackage.vsct
        /// </summary>
        public const string guidCKS_Dev12_PkgString = "555c8e33-1a5e-437a-b98a-62b610221cd7";
        public static readonly Guid guidCKS_Dev12_Pkg = new Guid(guidCKS_Dev12_PkgString);

        /// <summary>
        /// The guid for the GuidSymbol name="guidImages" value="{aade825c-900a-4a50-a409-28d4d35c88e5}"  in the VSPackage.vsct
        /// </summary>
        public const string guidImagesString = "aade825c-900a-4a50-a409-28d4d35c88e5";
        public static readonly Guid guidImages = new Guid(guidImagesString);

        /// <summary>
        /// The guid for the GuidSymbol name="guidSharePointCmdSet" value="{F8FC4244-3BA1-4bf5-A65A-23B2F3D3CA9F}" in the VSPackage.vsct
        /// </summary>
        public const string guidCKSDEVCmdSetString = "F8FC4244-3BA1-4bf5-A65A-23B2F3D3CA9F";
        public static readonly Guid guidCKSDEVCmdSet = new Guid(guidCKSDEVCmdSetString);

        #endregion

        public const string guidToolWindowPersistanceString = "dc1e7d75-ea65-44bf-83cc-1afd0d5fb261";


        /// <summary>
        /// This is the project kind for SP projects
        /// </summary>
        public const string guidCKSDEVCSProjectFactoryString = "593B0543-81F6-4436-BA1E-4747859CAAE2";
        public const string guidCKSDEVVBProjectFactoryString = "EC05E597-79D4-47f3-ADA0-324C4F7C7484";
        public const string guidUIContextNoSolutionString = "adfc4e64-0397-11d1-9f4e-00a0c911004f";
        public const string guidUIContextSolutionExistsString = "f1536ef8-92ec-443c-9ed7-fdadf150da82";




        //Formally from VSPacjageGuids
        public const string UIContext_SharePointProject = "7E396C85-D374-4531-95B9-43E5E1A1CF3C";
        public const string PackageText = "23c6cc48-1264-4469-94c9-73a6b68584b1";
        public const string CommandSetText = "13b396e9-5f1e-447c-836c-c13393219d3b";
        //TODO: remove this
        //public const string SPMetalGeneratorText = "6D15FC13-E27F-4BB1-88E3-B0B091675A0A";
        //TODO: remove this
        //public const string SandboxedVisualWebPartGeneratorText = "99DA1F10-2DA3-4A62-9C9F-05BD348F3D57";
        //TODO: remove this
        //ublic const string SharePointReferencesPageText = "BB8F11D5-5618-4764-86C9-6A3D06AA7E3D";
        public static readonly Guid Package = new Guid(PackageText);
        public static readonly Guid CommandSet = new Guid(CommandSetText);
        //TODO: remove this
        //public static readonly Guid SPMetalGenerator = new Guid(SPMetalGeneratorText);
        //TODO: remove this
        //public static readonly Guid SandboxedVisualWebPartGenerator = new Guid(SandboxedVisualWebPartGeneratorText);
        //TODO: remove this
        //public static readonly Guid SharePointReferencesPage = new Guid(SharePointReferencesPageText);
        public const int QuickDeployMenu = 0x101010;
        public const int ProjectNodeGroup = 0x101;
        public const int QuickDeployGroup = 0x201;
        public const int QuickDeployButton = 0x2010;
    }
}
