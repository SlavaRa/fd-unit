using System;
using System.Reflection;
using System.Resources;
using PluginCore.Localization;

namespace TestExplorerPanel.Source.Localization {

    class LocalizationHelper {

        private static ResourceManager resources = null;

        public static void Initialize( LocaleVersion locale ) {
            String path = "TestExplorerPanel.Source.Localization." + locale.ToString();
            resources = new ResourceManager( path , Assembly.GetExecutingAssembly() );
        }

        public static String GetString( String identifier ) {
            return resources.GetString( identifier );
        }
    }
}
