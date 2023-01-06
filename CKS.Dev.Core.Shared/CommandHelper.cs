namespace CKS.Dev.VisualStudio.SharePoint
{
    public static class CommandHelper
    {
        public static string GetSPCommandName(string commandName)
        {
            var SPVersion = ProjectUtilities.WhichSharePointVersionIsProjectDeployingTo();
            return $"{commandName}.{(int)SPVersion}";
        }
    }
}
