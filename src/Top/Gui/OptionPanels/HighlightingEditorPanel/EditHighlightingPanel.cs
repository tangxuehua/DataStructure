
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Text;

using NetFocus.DataStructure.Properties;
using NetFocus.Components.AddIns.Codons;
using NetFocus.DataStructure.Services;

using NetFocus.DataStructure.TextEditor.Document;
using NetFocus.DataStructure.Gui.Dialogs;

using NetFocus.DataStructure.Gui.OptionPanels.HighlightingEditor.Nodes;


namespace NetFocus.DataStructure.Gui.Dialogs.OptionPanels
{
	class EditHighlightingPanel : AbstractOptionPanel {
		
		ListBox userList;
		Button  modifyButton;
		ArrayList highlightItemNameList = new ArrayList();
		ArrayList errors = new ArrayList();

		public override bool StorePanelContents()
		{
			NetFocus.DataStructure.TextEditor.Document.HighlightingManager.Manager.ReloadSyntaxModes();
			return true;
		}
		
		public override void LoadPanelContents()
		{
			SetupFromXmlFile(Path.Combine(PropertyService.DataDirectory, @"resources\panels\HighlightingEditor\OptionPanel.xfrm"));
			
			userList     = (ListBox)ControlDictionary["userList"];
			modifyButton = (Button)ControlDictionary["modifyButton"];
			modifyButton.Click   += new EventHandler(ModifyButtonClick);
			
			FillLists();
		}


		void ValidationHandler(object sender, ValidationEventArgs args)
		{
			errors.Add(args);
		}

		void ReportErrors()
		{
			StringBuilder msg = new StringBuilder();

			msg.Append(ResourceService.GetString("Dialog.Options.TextEditorOptions.EditHighlighting.LoadError") + "\n\n");
			foreach(ValidationEventArgs args in errors) 
			{
				msg.Append(args.Message);
				msg.Append(Console.Out.NewLine);
			}
			MessageService.ShowWarning(msg.ToString());
		}

		
		SchemeNode CreateSchemeNode(XmlTextReader reader, bool userList)
		{
			try 
			{
				XmlValidatingReader validatingReader = new XmlValidatingReader(reader);
				Stream schemaStream = typeof(SyntaxMode).Assembly.GetManifestResourceStream("Mode.xsd");
				validatingReader.Schemas.Add("", new XmlTextReader(schemaStream));
				validatingReader.ValidationType = ValidationType.Schema;
				validatingReader.ValidationEventHandler += new ValidationEventHandler(ValidationHandler);
				
				XmlDocument doc = new XmlDocument();
				doc.Load(validatingReader);
				
				if (errors.Count != 0) 
				{
					ReportErrors();
					validatingReader.Close();
					return null;
				} 
				else 
				{
					validatingReader.Close();
					return new SchemeNode(doc.DocumentElement, userList);	
				}
			} 
			catch (Exception e) 
			{
				MessageService.ShowError(e, "${res:Dialog.Options.TextEditorOptions.EditHighlighting.LoadError}");
				return null;
			} 
			finally 
			{
				reader.Close();
			}
			
		}
		
		void ModifyButtonClick(object sender, EventArgs ev)
		{
			if (userList.SelectedIndex == -1) 
			{
				return;
			}
			
			XmlTextReader reader = new XmlTextReader(userList.SelectedValue.ToString());
			SchemeNode node = CreateSchemeNode(reader,true);
			using (EditHighlightingDialog dlg = new EditHighlightingDialog(node)) 
			{
				DialogResult res = dlg.ShowDialog();
				
				if (res == DialogResult.OK) 
				{
					using (StreamWriter sw = new StreamWriter(userList.SelectedValue.ToString(), false)) 
					{
						sw.WriteLine(node.ToXml().Replace("\n", "\r\n"));
					}
				}
				
				try 
				{
					node.Remove();
				} 
				catch {}
				
			}
		}

		
		void FillLists()
		{
			string uPath = Path.Combine(PropertyService.DataDirectory, "modes");
			StringCollection uCol;
			
			if (Directory.Exists(uPath)) 
			{
				uCol = FileUtilityService.SearchDirectory(uPath, "*.xshd", true);
			} 
			else 
			{
				Directory.CreateDirectory(uPath);
				uCol = new StringCollection();
			}
			
			foreach(string str in uCol)
			{
				XmlTextReader reader = new XmlTextReader(str);
				while (reader.Read()) 
				{
					if (reader.NodeType == XmlNodeType.Element) 
					{
						switch (reader.Name) 
						{
							case "SyntaxDefinition":
								string name = reader.GetAttribute("name");
								if(name != null)
								{
									highlightItemNameList.Add(new DictionaryEntry(name,str));
								}
								goto exit;
							default:
								MessageBox.Show("不可识别的根节点在高亮度显示策略文件中:" + str, "错误", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
								goto exit;
						}
					}
				}
			exit:
				reader.Close();
			}
			userList.Items.Clear();
			userList.DataSource = highlightItemNameList;
			userList.DisplayMember = "Key";
			userList.ValueMember = "Value";

		}

	}
}
