using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PluginCore;
using PluginCore.FRService;
using ScintillaNet;

namespace TestExplorerPanel.Forms
{
    public enum TestResult
    {
        Passed,
        Failed,
        Error
    }

    public struct TestInformation
    {
        public string Name;
        public string FunctionName;
        public string Path;
        public string Tooltip;
        public int Line;
        public TestResult Result;
    }

    public partial class PluginUI : DockPanelControl
    {
        #region Images Indexes Constants

        private const int ImagePassedIndex = 0;
        private const int ImageErrorIndex = 1;
        private const int ImageFailedIndex = 2;

        #endregion

        private ImageList imageList;

        private int totalPassed;
        private int totalFailed;
        private int totalError;

        private string runTime;

        public PluginUI()
        {
            totalPassed = 0;
            totalFailed = 0;
            totalError = 0;

            runTime = "0";

            InitializeComponent();
        }

        #region Update Information

        public void BeginUpdate()
        {
            TestsTreeView.Nodes.Clear();

            totalPassed = 0;
            totalFailed = 0;
            totalError = 0;

            runTime = "0";

            TestsTreeView.BeginUpdate();
        }

        public void EndUpdate()
        {
            TestsTreeView.EndUpdate();

            UpdateProgress();
        }

        public void UpdateProgress()
        {
            int totalTests = totalError + totalFailed + totalPassed;

            RunsLabel.Text = "Runs: " + totalPassed + " / " + totalTests + " (" + runTime + "s)";
            ErrorsLabel.Text = "Errors: " + totalError;
            FailuresLabel.Text = "Failures: " + totalFailed;

            if (totalTests == 0)
            {
                TestProgress.Value = 0;
            }
            else
            {
                TestProgress.Maximum = totalTests;
                TestProgress.Minimum = 0;

                TestProgress.Value = totalPassed;
            }
        }

        #endregion

        #region Test Manipulation

        public void AddTest(TestInformation info)
        {
            TreeNode node = GetNode(info.Name);
            node.ToolTipText = info.Tooltip;
            node.Tag = info;

            SetStyleBasedOnResult(node, info.Result);

            AddTestResultToStatistics(info.Result);
        }

        public void SetRunTime(string time)
        {
            runTime = time;
        }

        public bool IsTesting(string name)
        {
            return TestsTreeView.Nodes.Find(name, true).Length > 0;
        }

        public void SetTestPathAndLine(TestInformation testInfo)
        {
            testInfo.Name = testInfo.Name.Replace('/', '.');

            TreeNode testNode = GetNode(testInfo.Name);

            TestInformation info = (TestInformation) testNode.Tag;
            info.FunctionName = testInfo.FunctionName;
            info.Path = testInfo.Path;
            info.Line = testInfo.Line;

            testNode.Tag = info;

            TreeNode errorNode = testNode.Nodes.Add(info.Tooltip);

            SetStyleBasedOnResult(errorNode, info.Result);
        }

        private void AddTestResultToStatistics(TestResult result)
        {
            switch (result)
            {
                case TestResult.Passed:
                    totalPassed++;
                    break;

                case TestResult.Failed:
                    totalFailed++;
                    break;

                case TestResult.Error:
                    totalError++;
                    break;
            }
        }

        #endregion

        #region TreeNode Manipulation

        private TreeNode GetNode(string name)
        {
            string[] groups = name.Split('.');

            TreeNode lastNode = null;

            foreach (string group in groups)
                lastNode = GetChildrenNode(lastNode, group);

            return lastNode;
        }

        private TreeNode GetChildrenNode(TreeNode node, string name)
        {
            TreeNodeCollection searchCollection;

            if (node == null)
                searchCollection = TestsTreeView.Nodes;
            else
                searchCollection = node.Nodes;

            if (searchCollection.ContainsKey(name))
                return searchCollection.Find(name, true)[0];

            return searchCollection.Add(name, name);
        }

        private void SetStyleBasedOnResult(TreeNode node, TestResult result)
        {
            switch (result)
            {
                case TestResult.Passed:
                    node.ImageIndex = ImagePassedIndex;
                    node.SelectedImageIndex = ImagePassedIndex;
                    node.ForeColor = Color.Green;
                    break;

                case TestResult.Failed:
                    node.ImageIndex = ImageFailedIndex;
                    node.SelectedImageIndex = ImageFailedIndex;
                    node.ForeColor = Color.Blue;
                    node.EnsureVisible();
                    break;

                case TestResult.Error:
                    node.ImageIndex = ImageErrorIndex;
                    node.SelectedImageIndex = ImageErrorIndex;
                    node.ForeColor = Color.Red;
                    node.EnsureVisible();
                    break;
            }

            if (node.Level > 0)
                SetStyleBasedOnResult(node.Parent, result);
        }

        #endregion

        #region Event Handling

        private void OnTestMouseClick(object sender, TreeNodeMouseClickEventArgs clickEvent)
        {
            TreeNode clickedNode = clickEvent.Node;

            TestInformation info;

            try
            {
                info = (TestInformation) clickedNode.Tag;
            }
            catch (InvalidCastException)
            {
                info = new TestInformation();
            }
            catch (NullReferenceException)
            {
                info = new TestInformation();
            }

            SelectTextOnFileLine(info.Path, info.Line, info.FunctionName);
        }

        private void OnPluginUILoad(object sender, EventArgs e)
        {
            TestProgress.Value = 0;
            TestsTreeView.Nodes.Clear();
            UpdateProgress();
            imageList = new ImageList
            {
                ColorDepth = ColorDepth.Depth32Bit,
                TransparentColor = Color.Transparent
            };
            imageList.Images.Add(PluginBase.MainForm.FindImage("32")); // passed
            imageList.Images.Add(PluginBase.MainForm.FindImage("197")); // error
            imageList.Images.Add(PluginBase.MainForm.FindImage("196")); // failed
            TestsTreeView.ImageList = imageList;
            ErrorsLabel.Image = imageList.Images[ImageErrorIndex];
            FailuresLabel.Image = imageList.Images[ImageFailedIndex];
        }

        #endregion

        #region Document Hightlight

        private void SelectTextOnFileLine(string path, int line, string text)
        {
            if (!File.Exists(path))
                return;

            PluginBase.MainForm.OpenEditableDocument(path);

            ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
            sci.RemoveHighlights();

            List<SearchMatch> matches = GetResults(sci, text);

            sci.AddHighlights(matches, 0xff0000);
        }

        private List<SearchMatch> GetResults(ScintillaControl sci, string text)
        {
            FRSearch search = new FRSearch(text)
            {
                Filter = SearchFilter.OutsideCodeComments,
                NoCase = true,
                WholeWord = true
            };
            return search.Matches(sci.Text);
        }

        #endregion
    }
}