using CKS.Dev2015.Core.Cmd.Imp.v4.Properties;
using Microsoft.SharePoint.Administration;
using Microsoft.VisualStudio.SharePoint.Commands;
using System;

namespace CKS.Dev2015.VisualStudio.SharePoint.Commands
{
    /// <summary>
    /// The developer dashboard commands
    /// </summary>
    internal class DeveloperDashboardCommands
    {
        /// <summary>
        /// Gets current farm developer dashboard display level setting value.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The setting value as a string</returns>
        [SharePointCommand(DeveloperDashboardCommandIds.GetDeveloperDashBoardDisplayLevelSetting)]
        private static string GetDeveloperDashBoardDisplayLevelSetting(ISharePointCommandContext context)
        {
            SPDeveloperDashboardSettings settings = context.Site.WebApplication.WebService.DeveloperDashboardSettings;

            return settings.DisplayLevel.ToString();
        }

        /// <summary>
        /// Sets current farm developer dashboard display level setting value.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="newLevel">The new level to set it to.</param>
        [SharePointCommand(DeveloperDashboardCommandIds.SetDeveloperDashBoardDisplayLevelSetting)]
        private static void SetDeveloperDashBoardDisplayLevelSetting(ISharePointCommandContext context, string newLevel)
        {
            try
            {
                SPDeveloperDashboardLevel level = ((SPDeveloperDashboardLevel)Enum.Parse(typeof(SPDeveloperDashboardLevel), newLevel));

                SPDeveloperDashboardSettings settings = context.Site.WebApplication.WebService.DeveloperDashboardSettings;

                settings.DisplayLevel = level;

                settings.Update(true);
            }
            catch (ArgumentNullException ex)
            {
                context.Logger.WriteLine(String.Format(Resources.DeveloperDashboardCommands_SetException,
                          ex.Message,
                          Environment.NewLine,
                          ex.StackTrace), LogCategory.Error);
            }
            catch (ArgumentException ex)
            {
                context.Logger.WriteLine(String.Format(Resources.DeveloperDashboardCommands_SetException,
                          ex.Message,
                          Environment.NewLine,
                          ex.StackTrace), LogCategory.Error);
            }
            catch (OverflowException ex)
            {
                context.Logger.WriteLine(String.Format(Resources.DeveloperDashboardCommands_SetException,
                          ex.Message,
                          Environment.NewLine,
                          ex.StackTrace), LogCategory.Error);
            }
        }
    }
}
