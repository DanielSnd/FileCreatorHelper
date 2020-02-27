using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileCreatorHelper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Create the ContextMenuStrip.
            docMenu = new ContextMenuStrip();

            //Create some menu items.
            ToolStripMenuItem deleteLabel = new ToolStripMenuItem();
            deleteLabel.Text = "Delete";

            //Add the menu items to the menu.
            docMenu.Items.AddRange(new ToolStripMenuItem[]{
        deleteLabel});
            deleteLabel.Click += DeleteNode;
            // Set the ContextMenuStrip property to the ContextMenuStrip.
            treeView1.ContextMenuStrip = docMenu;
        }

        private void DeleteNode(object sender, EventArgs e)
        {
            // use SourceControl property.. ContextMenuStrip must be associated with TreeView
            ContextMenuStrip cms = (ContextMenuStrip)((ToolStripMenuItem)sender).Owner;
            TreeView treeView = (TreeView)cms.SourceControl;
            TreeNode node = treeView.GetNodeAt(treeView.PointToClient(cms.Location));
            if (currentEditingNode == node) currentEditingNode = null;
            if (templateNode == node) templateNode = null;
            treeView.Nodes.Remove(node);
        }

        string templateString;
        TreeNode templateNode = null;

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private ContextMenuStrip docMenu;

        private void label1_Click(object sender, EventArgs e)
        {

        }

        TreeNode _currentNode;
        TreeNode currentEditingNode
        {
            get
            {
                return _currentNode;
            }
            set
            {
                if(_currentNode != null)
                {
                    _currentNode.BackColor = Color.Empty;
                }
                _currentNode = value;
                if(_currentNode != null)
                {
                    _currentNode.BackColor = Color.Lavender;
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeNodeMouseClickEventArgs e)
        {
            currentEditingNode = e.Node;
            fastColoredTextBox1.Text = (string)e.Node.Tag;
            //MessageBox.Show((string)e.Node.Tag);
        }

        private void fastColoredTextBox1_Load(object sender, EventArgs e)
        {

        }

        string newFileExtensions = "";
        private void setExtensionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newFileExtensions = InputBox.ShowDialog("What extension would you like for the exported files?", "Set Extension", ".tile");
            ExtensionName.Text = "Extension: " + newFileExtensions;
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(templateNode == null || templateNode.Nodes.Count <=0)
            {
                MessageBox.Show("No data found to export");
                return;
            }
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string folderPath = dialog.FileName;
                int amountToExport = templateNode.Nodes.Count;
                string example = folderPath + @"\" + templateNode.Nodes[0].Text+newFileExtensions;
                DialogResult dialogResult = MessageBox.Show(string.Format("This will export {0} files with the extension '{1}'.\nExample file: ' {2} ' \n Proceed?", amountToExport,newFileExtensions, example), "Exporting", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    bool fail = false;
                    foreach (TreeNode item in templateNode.Nodes)
                    {
                        string filePath = folderPath + @"\" + item.Text + newFileExtensions;
                        try
                        {
                            if (File.Exists(filePath)) File.Delete(filePath);
                            File.WriteAllText(filePath, (string)item.Tag);
                        } catch (UnauthorizedAccessException)
                        {
                            MessageBox.Show("You don't have enough authorization to save files in this folder. If you need to save it there then make sure you run the app as an administrator");
                            fail = true;
                            break;
                        }
                    }
                    if(!fail)
                    {
                        DialogResult nDialogResult = MessageBox.Show("Files exported successfully. Would you like to open the export folder?", "Success", MessageBoxButtons.YesNo);
                        if (nDialogResult == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(folderPath);
                        }
                        else if (nDialogResult == DialogResult.No)
                        {
                            //do something else
                        }
                    }
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do something else
                }
            }
        }

        private void loadTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = false;
            templateNode = null;
            treeView1.Nodes.Clear();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    foreach (var item in openFileDialog1.FileNames)
                    {
                        using (StreamReader sr = new StreamReader(item))
                        {
                            templateString = sr.ReadToEnd();
                            TreeNode node = new TreeNode("Template");
                            node.Text = "Template";
                            if(!templateString.Contains("{0}"))
                            {
                                MessageBox.Show("{0} not found. Double click Template to edit the template and add {0} where you want the filenames of the files to appear");
                            }
                            node.ToolTipText = "Template";
                            node.Tag = templateString;
                            treeView1.Nodes.Add(node);
                            templateNode = node;
                            //string fileName = Path.GetFileNameWithoutExtension(item);
                            //Template.Text = string.Format(line, fileName);
                        }
                    }
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        private void loadFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(templateNode == null)
            {
                MessageBox.Show("First you need to load a template file");
                return;
            }
            openFileDialog1.Multiselect = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    foreach (var item in openFileDialog1.FileNames)
                    {
                        string fileNameNoExtension = Path.GetFileNameWithoutExtension(item);
                        TreeNode node = new TreeNode(fileNameNoExtension);
                        if (templateString.Contains("{0}"))
                            node.Tag = templateString.Replace("{0}", fileNameNoExtension);
                        else node.Tag = templateString;
                        node.Text = fileNameNoExtension;
                        node.ToolTipText = fileNameNoExtension;
                        if (templateNode != null)
                        {
                            templateNode.Nodes.Add(node);
                        }
                        else
                        {
                            treeView1.Nodes.Add(node);
                        }
                        treeView1.ExpandAll();
                    }
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to erase all the temporary data? You can't recover that data.", "Cler All", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                _currentNode = null;
                templateNode = null;
                treeView1.Nodes.Clear();
            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }
        }
    }
}
