using System.Text.RegularExpressions;
using Microsoft.Html.Editor.Schemas.Interfaces;

namespace MadsKristensen.EditorExtensions.Html
{
	class HtmlSchemaFileInfo : IHtmlSchemaFileInfo
	{
		private static Regex _regex = new Regex("vs:customattrprefix=\"(?<prefix>[^\"]+)\"", RegexOptions.IgnoreCase);

		public HtmlSchemaFileInfo()
		{
			IsSupplemental = true;
			IsXml = false;
		}

		public string CustomPrefix { get; set; }
		public string File { get; set; }
		public string FriendlyName { get; set; }
		public bool IsSupplemental { get; set; }
		public bool IsXml { get; set; }
		public string Uri { get; set; }

		public static HtmlSchemaFileInfo FromFile(string file)
		{
			HtmlSchemaFileInfo info = new HtmlSchemaFileInfo();
			info.File = file;

			string input = System.IO.File.ReadAllText(file);
			Match match = _regex.Match(input);

			if (match.Success)
			{
				info.CustomPrefix = match.Groups["prefix"].Value;
			}

			return info;
		}
	}
}
