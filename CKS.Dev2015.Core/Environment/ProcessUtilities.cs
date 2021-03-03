using CKS.Dev2015.VisualStudio.SharePoint.Commands;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.SharePoint;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.DirectoryServices;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using CKSProperties = CKS.Dev2015.Core.Properties.Resources;

namespace CKS.Dev2015.VisualStudio.SharePoint.Environment
{
    /// <summary>
    /// Process Utilities
    /// </summary>
    class ProcessUtilities
    {

        /// <summary>
        /// Gets or sets the current DTE.
        /// </summary>
        /// <value>The current DTE.</value>
        public DTE CurrentDTE { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessUtilities"/> class.
        /// </summary>
        public ProcessUtilities()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessUtilities"/> class.
        /// </summary>
        /// <param name="dte">The DTE.</param>
        public ProcessUtilities(DTE dte)
        {
            CurrentDTE = dte;
        }

        /// <summary>
        /// Attaches the name of to process by.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="processName">Name of the process.</param>
        public void AttachToProcessByName(ISharePointProject project, String processName)
        {
            EnvDTE.Project dteProj = project.ProjectService.Convert<ISharePointProject, EnvDTE.Project>(project);
            project.ProjectService.Logger.ActivateOutputWindow();
            project.ProjectService.Logger.WriteLine(String.Format("Trying to attach to {0} process", processName), LogCategory.Message);

            if (AttachToProcessByName(processName))
            {
                project.ProjectService.Logger.WriteLine(String.Format("Debugger attached to {0} process", processName), LogCategory.Message);
            }
            else
            {
                project.ProjectService.Logger.WriteLine(String.Format("Failed to attach to {0} process", processName), LogCategory.Warning);
            }
        }

        /// <summary>
        /// Attaches the name of to process by.
        /// </summary>
        /// <param name="processName">Name of the process.</param>
        /// <returns></returns>
        public bool AttachToProcessByName(String processName)
        {
            Debugger2 debugger = CurrentDTE.Application.Debugger as Debugger2;
            if (debugger != null)
            {
                foreach (EnvDTE80.Process2 process in debugger.LocalProcesses)
                {
                    if (process.Name.ToUpper().LastIndexOf(processName.ToUpper()) == (process.Name.Length - processName.Length))
                    {
                        process.Attach();
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether [is process available by name] [the specified process name].
        /// </summary>
        /// <param name="processName">Name of the process.</param>
        /// <returns>
        /// 	<c>true</c> if [is process available by name] [the specified process name]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsProcessAvailableByName(String processName)
        {
            Debugger2 debugger = CurrentDTE.Application.Debugger as Debugger2;
            if (debugger != null)
            {
                foreach (EnvDTE80.Process2 process in debugger.LocalProcesses)
                {
                    if (process.Name.ToUpper().LastIndexOf(processName.ToUpper()) == (process.Name.Length - processName.Length))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Restarts the process. No need to create an object with DTE.
        /// </summary>
        /// <param name="processName">Name of the process.</param>
        /// <param name="timeOut">The time out in seconds.</param>
        /// <returns></returns>
        public bool RestartProcess(String processName, int timeOut)
        {
            try
            {
                TimeSpan timeOutSpan = TimeSpan.FromSeconds(timeOut);
                ServiceController sc = new ServiceController(processName);
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, timeOutSpan);
                }
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.Running, timeOutSpan);
                return true;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Executes the browser URL process.
        /// </summary>
        /// <param name="url">The URL.</param>
        public void ExecuteBrowserUrlProcess(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                ExecuteBrowserUrlProcess(uri);
            }
            catch (UriFormatException)
            {
            }
            catch (ArgumentNullException)
            {
            }
        }

        /// <summary>
        /// Executes the browser URL process.
        /// </summary>
        /// <param name="uri">The URI.</param>
        public void ExecuteBrowserUrlProcess(Uri uri)
        {
            try
            {
                // Start Internet Explorer. Defaults to the home page.

                string browser = GetDefaultBrowser();

                if (!String.IsNullOrEmpty(browser))
                {
                    System.Diagnostics.Process.Start(browser, uri.AbsoluteUri);
                }
                else
                {
                    System.Diagnostics.Process.Start(CKSProperties.ProcessUtilities_IEProcessName, uri.AbsoluteUri);
                }
            }
            catch (UriFormatException)
            {
            }
            catch (ArgumentNullException)
            {
            }
        }

        /// <summary>
        /// Gets the default browser.
        /// </summary>
        /// <returns></returns>
        private string GetDefaultBrowser()
        {
            string browser = string.Empty;
            RegistryKey key = null;
            try
            {
                key = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

                //trim off quotes
                browser = key.GetValue(null).ToString().ToLower().Replace("\"", "");
                if (!browser.EndsWith("exe"))
                {
                    //get rid of everything after the ".exe"
                    browser = browser.Substring(0, browser.LastIndexOf(".exe") + 4);
                }
            }
            finally
            {
                if (key != null)
                {
                    key.Close();
                }
            }
            return browser;
        }

        /// <summary>
        /// Recycles all application pools.
        /// </summary>
        /// <param name="service">The service.</param>
        public void RecycleAllApplicationPools(ISharePointProjectService service)
        {
            string[] names = GetAllApplicationPoolNames(service);
            foreach (string name in names)
            {
                RecycleApplicationPool(name);
            }
        }

        /// <summary>
        /// Recycles the application pool.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public bool RecycleApplicationPool(ISharePointProjectService service, string url)
        {
            string name = GetApplicationPoolName(service, url);
            if (String.IsNullOrEmpty(name))
            {
                return false;
            }
            else
            {
                RecycleApplicationPool(name);
                return true;
            }
        }

        /// <summary>
        /// Recycles the application pool.
        /// </summary>
        /// <param name="name">The name.</param>
        public void RecycleApplicationPool(string name)
        {
            try
            {
                using (DirectoryEntry appPool = new DirectoryEntry(GetApplicationPoolPath(name)))
                {
                    appPool.Invoke("Stop", null);
                    appPool.Invoke("Start", null);
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Gets the application pool path.
        /// </summary>
        /// <param name="appPoolName">Name of the app pool.</param>
        /// <returns></returns>
        private string GetApplicationPoolPath(string appPoolName)
        {
            string path = "IIS://localhost/w3svc/apppools/" + appPoolName;
            return path;
        }

        /// <summary>
        /// Gets the name of the application pool.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public string GetApplicationPoolName(ISharePointProjectService service, string url)
        {
            string name = service.SharePointConnection.ExecuteCommand<string, string>(DeploymentSharePointCommandIds.GetApplicationPoolName, url);
            return name;
        }

        /// <summary>
        /// Gets all application pool names.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public string[] GetAllApplicationPoolNames(ISharePointProjectService service)
        {
            string[] names = service.SharePointConnection.ExecuteCommand<string[]>(DeploymentSharePointCommandIds.GetAllApplicationPoolNames);
            return names;
        }

        /// <summary>
        /// Restarts the IIS.
        /// </summary>
        public void RestartIIS(ISharePointProject project)
        {
            StartProcess(project, System.Environment.SystemDirectory + Path.DirectorySeparatorChar + ProcessConstants.IISProcess, "localhost");
        }

        /// <summary>
        /// Starts the process.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="path">The path.</param>
        /// <param name="arguments">The URL.</param>
        public void StartProcess(ISharePointProject project,
            string path,
            string arguments)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(path,
                arguments);
            processStartInfo.RedirectStandardError = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.UseShellExecute = false;
            ProcessStartInfo processStartInfo1 = processStartInfo;
            System.Diagnostics.Process process = System.Diagnostics.Process.Start(processStartInfo1);
            ThreadSafeStreamReader threadSafeStreamReader = new ThreadSafeStreamReader(process.StandardOutput);
            ThreadSafeStreamReader threadSafeStreamReader1 = new ThreadSafeStreamReader(process.StandardError);
            System.Threading.Thread thread = new System.Threading.Thread(new ThreadStart(threadSafeStreamReader.Go));
            System.Threading.Thread thread1 = new System.Threading.Thread(new ThreadStart(threadSafeStreamReader.Go));
            thread.Start();
            thread1.Start();
            thread.Join();
            thread1.Join();
            process.WaitForExit();
            if (!string.IsNullOrEmpty(threadSafeStreamReader1.Text))
            {
                project.ProjectService.Logger.WriteLine(threadSafeStreamReader1.Text, LogCategory.Error);
                project.ProjectService.Logger.WriteLine("Executing failed", LogCategory.Status);
                return;
            }
            else
            {
                project.ProjectService.Logger.WriteLine(threadSafeStreamReader.Text, LogCategory.Message);
                project.ProjectService.Logger.WriteLine("Installing assembly to GAC succeeded", LogCategory.Status);
                return;
            }
        }

        /// <summary>
        /// Copies to GAC.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="assemblyFullPath">The assembly full path.</param>
        public void CopyToGAC(ISharePointProject project,
            string assemblyFullPath)
        {
            //For more info on the GACUtil.exe http://msdn.microsoft.com/en-gb/library/ex0ss12c(v=vs.110).aspx
            StartProcess(project, "c:\\Program Files (x86)\\Microsoft SDKs\\Windows\\v8.0A\\bin\\NETFX 4.0 Tools\\gacutil.exe",
                string.Format(@"/if ""{0}""", assemblyFullPath));

        }
    }
}

