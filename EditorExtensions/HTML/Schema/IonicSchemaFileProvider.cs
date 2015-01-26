//using System;
//using System.Collections.Generic;
//using System.ComponentModel.Composition;
//using System.IO;
//using Microsoft.Html.Editor.Schemas.Interfaces;
//using Microsoft.Html.Editor.Schemas.Model;
//using Microsoft.VisualStudio.Utilities;

//namespace MadsKristensen.EditorExtensions
//{
//	[Export(typeof(IHtmlSchemaFileInfoProvider))]
//	[Name("Ionic")]
//	[Order(Before = "Default")]
//	internal class IonicSchemaFileInfoProvider : IHtmlSchemaFileInfoProvider
//	{
//		private const string _file = @"C:\Users\madsk\ionic.xsd";

//		public IEnumerable<IHtmlSchemaFileInfo> GetSchemas(string defaultSchemaPath)
//		{
//			if (!File.Exists(_file))
//				yield break;

//			HtmlSchemaFileInfo fileInfo = new HtmlSchemaFileInfo()
//			{
//				File = _file,
//				FriendlyName = "Ionic",
//				CustomPrefix = "ion-",
//				Uri = "http://schemas.microsoft.com/intellisense/ionic",
//				IsSupplemental = true,
//				IsXml = false
//			};

//			yield return fileInfo;
//		}
//	}

//	class HtmlSchemaFileInfo : IHtmlSchemaFileInfo
//	{
//		public string CustomPrefix { get; set; }

//		public string File { get; set; }

//		public string FriendlyName { get; set; }

//		public bool IsSupplemental { get; set; }

//		public bool IsXml { get; set; }

//		public string Uri { get; set; }
//	}
//}

