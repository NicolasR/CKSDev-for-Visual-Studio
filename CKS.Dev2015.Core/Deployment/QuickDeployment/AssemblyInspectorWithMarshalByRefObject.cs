using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace CKS.Dev2015.VisualStudio.SharePoint.Deployment.QuickDeployment
{
    /// <summary>
    /// AssemblyInspectorWithMarshalByRefObject
    /// </summary>
    public class AssemblyInspectorWithMarshalByRefObject : MarshalByRefObject, IAssemblyInspector
    {
        #region IAssemblyInspector Members

        /// <summary>
        /// Gets the replaceable GUID tokens.
        /// </summary>
        /// <param name="assemblyPath">The assembly path.</param>
        /// <returns>The AssemblyInspectorResult</returns>
        public AssemblyInspectorResult GetReplaceableGuidTokens(string assemblyPath)
        {
            AssemblyInspectorResult result = new AssemblyInspectorResult();

            try
            {
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += new ResolveEventHandler(CurrentDomain_ReflectionOnlyAssemblyResolve);
                result.Messages.Add("Reflection handler registered");

                Assembly assembly = Assembly.ReflectionOnlyLoadFrom(assemblyPath);
                Type[] types = assembly.GetTypes();

                foreach (Type t in types)
                {

                    MemberInfo inf = t;
                    result.Messages.Add("Processing type: " + inf.Name);

                    List<CustomAttributeData> attributes = CustomAttributeData.GetCustomAttributes(inf).ToList();

                    if (attributes != null)
                    {
                        foreach (CustomAttributeData ass in attributes)
                        {
                            if (ass.Constructor.DeclaringType == typeof(System.Runtime.InteropServices.GuidAttribute))
                            {
                                string str2 = ass.ConstructorArguments[0].Value.ToString().ToLowerInvariant();
                                result.Messages.Add("Processing guid: " + str2);

                                string str3 = string.Format(CultureInfo.InvariantCulture, "SharePoint.Type.{0}.AssemblyQualifiedName", new object[] { str2 });
                                string str4 = string.Format(CultureInfo.InvariantCulture, "SharePoint.Type.{0}.FullName", new object[] { str2 });
                                result.Tokens.Add(str3, t.FullName + ", " + assembly.FullName);
                                result.Tokens.Add(str4, t.FullName);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                result.Messages.Add(String.Format("Unexpected exception occurred:\r\n{0}\r\n{1}", exception.Message, exception.StackTrace));
            }
            finally
            {
            }

            return result;
        }

        /// <summary>
        /// Handles the ReflectionOnlyAssemblyResolve event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.ResolveEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.ReflectionOnlyLoad(args.Name);
        }

        #endregion
    }
}
