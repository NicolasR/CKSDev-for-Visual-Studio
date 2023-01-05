using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace CKS.Dev.VisualStudio.SharePoint.Environment
{
    public interface ICKSDevVSPackage
    {
        object GetServiceInternal(Type type);
    }
}
