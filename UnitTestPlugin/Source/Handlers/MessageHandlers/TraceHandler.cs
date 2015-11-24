using System;
using System.Collections.Generic;
using PluginCore;
using PluginCore.Managers;
using TestExplorerPanel.Forms;
using TestExplorerPanel.Source.Handlers.MessageHandlers.FlexUnit;

namespace TestExplorerPanel.Source.Handlers.MessageHandlers
{
    class TraceHandler : IEventHandler
    {
        private PluginUI ui;

        private ITraceMessageHandler implementation;

        private int lastLogIndex;

        public TraceHandler(PluginUI pluginUI)
        {
            ui = pluginUI;

            implementation = new FlexUnitMessageHandler(pluginUI);

            lastLogIndex = 0;
        }

        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.Trace:
                    ProcessTraces();
                    ui.EndUpdate();
                    break;
            }
        }

        private void ProcessTraces()
        {
            IList<TraceItem> log = TraceManager.TraceLog;

            for (int i = lastLogIndex; i < log.Count; i++)
                implementation.ProcessMessage(log[i].Message);

            lastLogIndex = log.Count;
        }
    }
}