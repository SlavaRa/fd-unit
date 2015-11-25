using System.Collections.Generic;
using PluginCore;
using PluginCore.Managers;
using TestExplorerPanel.Forms;
using TestExplorerPanel.Source.Handlers.MessageHandlers.FlexUnit;

namespace TestExplorerPanel.Source.Handlers.MessageHandlers
{
    class TraceHandler : IEventHandler
    {
        private readonly PluginUI ui;
        private readonly ITraceMessageHandler implementation;
        private int lastLogIndex;

        public TraceHandler(PluginUI pluginUI)
        {
            ui = pluginUI;
            implementation = new FlexUnitMessageHandler(ui);
            lastLogIndex = 0;
        }

        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.Trace:
                    ui.BeginUpdate();
                    ProcessTraces();
                    ui.EndUpdate();
                    break;
            }
        }

        void ProcessTraces()
        {
            IList<TraceItem> log = TraceManager.TraceLog;
            int logCount = log.Count;
            for (int i = lastLogIndex; i < logCount; i++)
                implementation.ProcessMessage(log[i].Message);
            lastLogIndex = logCount;
        }
    }
}