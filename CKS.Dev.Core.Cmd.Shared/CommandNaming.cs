namespace CKS.Dev.Core.Cmd.Shared
{
    public static class CommandNaming
    {
        public const string SUFFIX =
#if V14
".14";
#elif V15
".15";
#elif V16
".16";
#else
"";
#endif
    }
}
