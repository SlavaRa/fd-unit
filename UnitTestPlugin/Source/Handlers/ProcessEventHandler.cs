using PluginCore;
using TestExplorerPanel.Forms;

namespace TestExplorerPanel.Source.Handlers
{
    class ProcessEventHandler : IEventHandler
    {
        private PluginUI ui;

        public ProcessEventHandler(PluginUI pluginUI)
        {
            this.ui = pluginUI;
        }

        public void HandleEvent(object sender, PluginCore.NotifyEvent e, PluginCore.HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.ProcessStart:
                    ui.BeginUpdate();
                    break;

                case EventType.ProcessEnd:
                    ui.EndUpdate();
                    break;
            }
        }
    }
}