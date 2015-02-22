using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using Microsoft.Html.Editor.Schemas.Interfaces;
using Microsoft.VisualStudio.Utilities;

namespace MadsKristensen.EditorExtensions.Html
{
	[Export(typeof(IHtmlSchemaFileInfoProvider))]
	[Name("Dynamic Schema Provider")]

	internal class DynamicSchemaFileInfoProvider : IHtmlSchemaFileInfoProvider
	{
		public IEnumerable<IHtmlSchemaFileInfo> GetSchemas(string defaultSchemaPath)
		{
			foreach (string file in Directory.EnumerateFiles(GetSchemaFolder(), "*.xsd"))
			{
				yield return HtmlSchemaFileInfo.FromFile(file);
			}
		}

		private static string GetSchemaFolder()
		{
			string assembly = Assembly.GetExecutingAssembly().Location;
			string folder = Path.GetDirectoryName(assembly);
			return Path.Combine(folder, "html\\schema\\schemas\\");
		}
	}
}

