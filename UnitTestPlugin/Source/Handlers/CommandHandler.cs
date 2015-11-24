using PluginCore;
using TestExplorerPanel.Forms;

namespace TestExplorerPanel.Source.Handlers
{
    class CommandHandler : IEventHandler
    {
        private PluginUI ui;

        public CommandHandler(PluginUI pluginUI)
        {
            ui = pluginUI;
        }

        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
        }
    }
}