using System;

namespace CKS.Dev.Core.Cmd.Shared
{
    public static class CommandNaming
    {
        public const string SUFFIX =
#if V14
            ".14"
#elif V15
            ".15"
#elif V16
            ".16"
#endif
            +
#if VS15
            ".VS15"
#elif VS16
            ".VS16"
#elif VS17
            ".VS17"
#endif
            ;
    }
}
