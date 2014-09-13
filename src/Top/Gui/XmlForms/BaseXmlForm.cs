
using System;
using System.Windows.Forms;
using System.IO;

using NetFocus.Components.AddIns.Codons;
using NetFocus.DataStructure.Gui.XmlForms;
using NetFocus.DataStructure.Services;

namespace NetFocus.DataStructure.Gui.XmlForms
{
	public abstract class BaseXmlForm : Form
	{
		static PropertyService     propertyService = null;
		static IMessageService     messageService  = null;
		static StringParserService stringParserService = null;
		static FileUtilityService  fileUtilityService = null;
		static ResourceService     resourceService = null;
		protected XmlLoader xmlLoader;

		public ControlDictionary ControlDictionary 
		{
			get 
			{
				return xmlLoader.ControlDictionary;
			}
		}
		
		protected static ResourceService ResourceService {
			get {
				if (resourceService == null) {
					resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
				}
				return resourceService;
			}
		}

		protected static PropertyService PropertyService {
			get {
				if (propertyService == null) {
					propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
				}
				return propertyService;
			}
		}
		
		protected static IMessageService MessageService {
			get {
				if (messageService == null) {
					messageService = (IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				}
				return messageService;
			}
		}
		
		protected static StringParserService StringParserService {
			get {
				if (stringParserService == null) {
					stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
				}
				return stringParserService;
			}
		}
		
		protected static FileUtilityService FileUtilityService {
			get {
				if (fileUtilityService == null) {
					fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				}
				return fileUtilityService;
			}
		}
		
		
		public BaseXmlForm(string fileName)
		{
			SetupFromXmlFile(fileName);
		}
		
		public BaseXmlForm()
		{
		}

		protected void SetupFromXmlFile(string fileName)
		{
			SuspendLayout();
			xmlLoader = new XmlLoader();
			InitializeXmlLoader();
			if (fileName != null && fileName.Length > 0) 
			{
				xmlLoader.LoadObjectFromFileDefinition(this, fileName);
			}
			ResumeLayout(false);
		}
		
		protected void SetupFromXmlStream(Stream stream)
		{
			SuspendLayout();
			xmlLoader = new XmlLoader();
			InitializeXmlLoader();
			if (stream != null) 
			{
				xmlLoader.LoadObjectFromStream(this, stream);
			}
			ResumeLayout(false);
		}
		
		protected virtual void InitializeXmlLoader()
		{
			xmlLoader.ObjectCreator        = new DataStructureObjectCreator();
		}
		
		public void SetEnabledStatus(bool enabled, params string[] controlNames)
		{
			foreach (string controlName in controlNames) {
				Control control = ControlDictionary[controlName];
				if (control == null) {
					MessageService.ShowError("¿Ø¼þ " + controlName + " Ã»ÕÒµ½!");
				} else {
					control.Enabled = enabled;
				}
			}
		}
		
		
	}
}
