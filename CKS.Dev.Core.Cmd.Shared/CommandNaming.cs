namespace CKS.Dev.Core.Cmd.Shared
{
    public static class CommandNaming
    {
        public const string SUFFIX =
#if V14
".{commandName}.14";
#elif V15
".{commandName}.15";
#elif V16
".{commandName}.16";
#else
".{commandName}";
#endif
    }
}
