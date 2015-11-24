/*
The MIT License (MIT)

Copyright (c) 2014 Gustavo S. Wolff

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using TestExplorerPanel.Forms;
using TestExplorerPanel.Source.Handlers;
using TestExplorerPanel.Source.Handlers.MessageHandlers;
using TestExplorerPanel.Source.Localization;
using WeifenLuo.WinFormsUI.Docking;

namespace TestExplorerPanel.Source
{
    public class PluginMain : IPlugin
    {
        private string settingsFilename;
        private PluginUI ui;
        private Image image;
        private DockContent panel;
        private IEventHandler processHandler;
        private IEventHandler traceHandler;
        private IEventHandler commandHandler;

        #region IPlugin Getters

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public string Name => "TestExplorerPanel";

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid => "93C76C98-D991-4F19-99EE-6188D7E534E2";

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public string Author => "Gustavo S. Wolff";

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public string Description { get; private set; } = "FlashDevelop Plugin for Unit Testing for Haxe and AS3";

        /// <summary>
        /// Web address for help
        /// </summary> 
        public string Help => "http://www.flashdevelop.org/community/";

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        public object Settings { get; private set; }

        #endregion

        #region IPlugin Implementation

        /// <summary>
        /// Initializes the plugin
        /// </summary>
        public void Initialize()
        {
            InitBasics();
            LoadSettings();
            InitLocalization();
            CreatePluginPanel();
            CreateMenuItem();
            AddEventHandlers();
        }

        /// <summary>
        /// Disposes the plugin
        /// </summary>
        public void Dispose() => SaveSettings();

        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
        }

        #endregion

        #region Custom Methods

        /// <summary>
        /// Initializes important variables
        /// </summary>
        void InitBasics()
        {
            string dataPath = Path.Combine(PathHelper.DataDir, nameof(TestExplorerPanel));
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            settingsFilename = Path.Combine(dataPath, "Settings.fdb");
            image = PluginBase.MainForm.FindImage("101");
        }

        /// <summary>
        /// Initializes the localization of the plugin
        /// </summary>
        void InitLocalization()
        {
            LocaleVersion language = PluginBase.MainForm.Settings.LocaleVersion;
            switch (language)
            {
                case LocaleVersion.de_DE:
                case LocaleVersion.eu_ES:
                case LocaleVersion.ja_JP:
                case LocaleVersion.zh_CN:
                case LocaleVersion.en_US:
                default:
                    LocalizationHelper.Initialize(LocaleVersion.en_US);
                    break;
            }
            Description = LocalizationHelper.GetString("Description");
        }
        
        /// <summary>
        /// Creates a plugin panel for the plugin
        /// </summary>
        void CreatePluginPanel()
        {
            ui = new PluginUI() {Text = LocalizationHelper.GetString("PluginPanel")};
            panel = PluginBase.MainForm.CreateDockablePanel(ui, Guid, image, DockState.DockRight);
            processHandler = new ProcessEventHandler(ui);
            traceHandler = new TraceHandler(ui);
            commandHandler = new CommandHandler();
        }

        /// <summary>
        /// Creates a menu item for the plugin and adds a ignored key
        /// </summary>
        void CreateMenuItem()
        {
            string label = LocalizationHelper.GetString("ViewMenuItem");
            ToolStripMenuItem viewMenu = (ToolStripMenuItem) PluginBase.MainForm.FindMenuItem("ViewMenu");
            ToolStripMenuItem newItem = new ToolStripMenuItem(label, image, OpenPanel);
            viewMenu.DropDownItems.Add(newItem);
        }
        
        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        void AddEventHandlers()
        {
            EventManager.AddEventHandler(processHandler, EventType.ProcessStart | EventType.ProcessEnd);
            EventManager.AddEventHandler(traceHandler, EventType.Trace);
            EventManager.AddEventHandler(commandHandler, EventType.Command);
        }

        void OpenPanel(object sender, EventArgs e) => panel.Show();

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            Settings = new Settings();
            if (!File.Exists(settingsFilename)) this.SaveSettings();
            else Settings = (Settings)ObjectSerializer.Deserialize(settingsFilename, Settings);
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings()
        {
            ObjectSerializer.Serialize(settingsFilename, Settings);
        }

        #endregion
    }
}