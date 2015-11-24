using PluginCore;
using TestExplorerPanel.Forms;

namespace TestExplorerPanel.Source.Handlers
{
    class CommandHandler : IEventHandler
    {
        private PluginUI ui;

        public CommandHandler(PluginUI pluginUI)
        {
            this.ui = pluginUI;
        }

        public void HandleEvent(object sender, PluginCore.NotifyEvent e, PluginCore.HandlingPriority priority)
        {
        }
    }
}