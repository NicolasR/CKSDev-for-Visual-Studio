using CKS.Dev.VisualStudio.SharePoint.Environment;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace CKS.Dev.Core
{
    public class CKSDevPackageInitializer
    {
        private readonly ICKSDevVSPackage package;
        private EventHandlerManager eventManager;

        public CKSDevPackageInitializer(ICKSDevVSPackage package)
        {
            this.package = package;
        }

        public delegate void VSUIReadyHandler(object sender, EventArgs data);
        public event VSUIReadyHandler VSUIReady;

        public void Initialize()
        {
            if (null == this.package)
            {
                throw new ArgumentException();
            }

            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            VSUIReady += CKSDevPackageInitializer_VSUIReady;
            KnownUIContexts.ShellInitializedContext.WhenActivated(() =>
            {
                OnVSUIReady(this, EventArgs.Empty);
            });

        }

        //private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        //{
        //    // Use latest strong name & version when trying to load SDK assemblies
        //    var requestedAssembly = new AssemblyName(args.Name);
        //    //if (requestedAssembly.Name != shortName)
        //    //    return null;
        //    DTEManager.ProjectService.Logger.ActivateOutputWindow();
        //    string text = "Redirecting assembly load of " + args.Name
        //                  + ",\tloaded by " + (args.RequestingAssembly == null ? "(unknown)" : args.RequestingAssembly.FullName);
        //    DTEManager.ProjectService.Logger.WriteLine(text, LogCategory.Status);

        //    //requestedAssembly.Version = targetVersion;
        //    //requestedAssembly.SetPublicKeyToken(new AssemblyName("x, PublicKeyToken=" + publicKeyToken).GetPublicKeyToken());
        //    //requestedAssembly.CultureInfo = CultureInfo.InvariantCulture;

        //    //AppDomain.CurrentDomain.AssemblyResolve -= handler;

        //    return Assembly.Load(requestedAssembly);
        //}

        private void CKSDevPackageInitializer_VSUIReady(object sender, EventArgs data)
        {
            // Call out to our event manager to register and handle project events.
            eventManager = new EventHandlerManager(package);
            eventManager.RegisterHandlers();

            InitialiseDTEDependantObjects();
        }

        protected void OnVSUIReady(object sender, EventArgs data)
        {
            // Check if there are any Subscribers
            // Call the Event
            VSUIReady?.Invoke(this, data);
        }

        #region IVsShellPropertyEvents Members

        bool hasInitialised = false;

        private void InitialiseDTEDependantObjects()
        {
            if (hasInitialised)
            {
                return;
            }


            EnvDTE80.DTE2 dte2 = package.GetServiceInternal(typeof(SDTE)) as EnvDTE80.DTE2;
            if (dte2 == null)
            {
                return;
            }

            // Zombie state dependent code.
            dte2 = package.GetServiceInternal(typeof(SDTE)) as EnvDTE80.DTE2;
            EnvDTE80.Events2 events = (EnvDTE80.Events2)dte2.Events;
            _ = package.GetServiceInternal(typeof(EnvDTE.DTE)) as EnvDTE.DTE;

            // Register event handlers that need the DTE active to do work.
            eventManager.RegisterDteDependentHandlers(dte2, events);

            // Initialize the output logger.  This is a bit hacky but a quick fix for Alpha release.
            //StatusBarLogger.InitializeInstance(dte);
            hasInitialised = true;
        }

        #endregion
    }
}
