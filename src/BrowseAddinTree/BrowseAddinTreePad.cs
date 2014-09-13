using System;
using System.Collections.Generic;
using System.Text;
using NetFocus.DataStructure.Gui;
using System.Windows.Forms;
using System.Drawing;
using NetFocus.Components.AddIns;
using NetFocus.Components.AddIns.Codons;
using System.Collections;

namespace BrowseAddinTree.Pads
{
    public class BrowseAddinTreePad : AbstractPadContent
    {
        private Panel panel = new Panel();
        private Button button1 = new Button();
        private Button button2 = new Button();
        private TextBox textBox = new TextBox();

        public override Control Control
        {
            get
            {
                return panel;
            }
        }

        public BrowseAddinTreePad()
            : base("浏览插件树目录", "Icons.16x16.PropertiesIcon")
        {
            button1.Text = "显示Add In，Extention Path，Codon";
            button1.Click += new EventHandler(button1_Click);
            button2.Text = "显示整个插件树";
            button2.Click += new EventHandler(button2_Click);
            panel.Controls.Add(textBox);
            panel.Controls.Add(button2);
            panel.Controls.Add(button1);
            button2.Dock = DockStyle.Top;
            button1.Dock = DockStyle.Top;
            textBox.Location = new Point(0, 30);
            textBox.ScrollBars = ScrollBars.Both;
            textBox.Multiline = true;
            textBox.Font = new Font("新宋体", 16f);
            textBox.Dock = DockStyle.Fill;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox.Text = string.Empty;
            textBox.Text = GetAddinExtensionAndCodon();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            textBox.Text = string.Empty;
            textBox.Text = GetWholeAddinTreeStructure();
        }

        private string GetAddinExtensionAndCodon()
        {
            StringBuilder sb = new StringBuilder();
            foreach (AddIn addin in AddInTreeSingleton.AddInTree.AddIns)
            {
                sb.AppendLine();
                sb.AppendLine("----------------------------------------------------------------------------------------------------");
                sb.AppendFormat("Addin:{0}", addin.Name);
                sb.AppendLine();
                sb.AppendLine("----------------------------------------------------------------------------------------------------");
                foreach (AddIn.Extension extension in addin.Extensions)
                {
                    sb.AppendFormat("    Extension Path:{0}", extension.Path);
                    sb.AppendLine();
                    foreach (ICodon codon in extension.CodonCollection)
                    {
                        sb.AppendFormat("        Codon ID:{0}", codon.ID);
                        sb.AppendLine();
                    }
                }
            }

            return sb.ToString();
        }

        private string GetWholeAddinTreeStructure()
        {
            StringBuilder sb = new StringBuilder();
            IEnumerator enumerator = AddInTreeSingleton.AddInTree.GetTreeNode(null).ChildNodes.Values.GetEnumerator();
            enumerator.MoveNext();
            IAddInTreeNode rootNode = enumerator.Current as IAddInTreeNode;
            PrintNode(rootNode, sb);

            return sb.ToString();
        }
        private void PrintNode(IAddInTreeNode node, StringBuilder sb)
        {
            sb.AppendFormat("Tree Node Path:{0}, Codon:{1}", node.GetFullPath(), node.Codon == null ? null : node.Codon.ID);
            sb.AppendLine();
            foreach (IAddInTreeNode childNode in node.ChildNodes.Values)
            {
                PrintNode(childNode, sb);
            }
        }
    }
}
