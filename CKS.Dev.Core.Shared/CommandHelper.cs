namespace CKS.Dev.VisualStudio.SharePoint
{
    public static class CommandHelper
    {
        public static string GetSPCommandName(string commandName)
        {
            var SPVersion = ProjectUtilities.WhichSharePointVersionIsProjectDeployingTo();
#if VS15
            return $"{commandName}.{(int)SPVersion}.VS15";
#elif VS16
            return $"{commandName}.{(int)SPVersion}.VS16";
#elif VS17
            return $"{commandName}.{(int)SPVersion}.VS17";
#endif
        }
    }
}
