using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;

namespace MadsKristensen.EditorExtensions.JavaScript
{
    public static class JavaScriptIntellisense
    {
        public static void Register(RegistryKey root)
        {
            RegisterFile("JavaScript\\Resources\\Modern.Intellisense.js", root);
        }

        private static void RegisterFile(string path, RegistryKey root)
        {
            try
            {
				string userPath = GetUserFilePath(Path.GetFileName(path));
				string assembly = Assembly.GetExecutingAssembly().Location;
                string folder = Path.GetDirectoryName(assembly).ToLowerInvariant();
                string file = Path.Combine(folder, path);

                if (!File.Exists(file))
                    return;

				File.Copy(file, userPath, true);

				using (RegistryKey key = root.OpenSubKey("JavaScriptLanguageService", true))
                {
                    if (key == null)
                        return;

                    key.SetValue("ReferenceGroups_WE", "Implicit (Web)|" + userPath + ";");
                    return;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

		private static string GetUserFilePath(string fileName)
		{
			string folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

			string WE_folder = Path.Combine(folder, "Web Essentials 2015");

			if (!Directory.Exists(WE_folder))
				Directory.CreateDirectory(WE_folder);

			return Path.Combine(WE_folder, fileName);
		}
	}
}