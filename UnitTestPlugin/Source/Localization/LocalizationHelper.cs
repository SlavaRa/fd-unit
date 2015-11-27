using System.Reflection;
using System.Resources;
using PluginCore.Localization;

namespace TestExplorerPanel.Source.Localization
{
    class LocalizationHelper
    {
        static ResourceManager resources;

        public static void Initialize(LocaleVersion locale)
        {
            string path = $"{nameof(TestExplorerPanel.Source.Localization)}.{locale}";
            resources = new ResourceManager(path, Assembly.GetExecutingAssembly());
        }

        public static string GetString(string identifier) => resources.GetString(identifier);
    }
}