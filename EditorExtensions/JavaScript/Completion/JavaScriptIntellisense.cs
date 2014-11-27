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
            RegisterFile("resources\\scripts\\Modern.Intellisense.js", root);
        }

        private static void RegisterFile(string path, RegistryKey root)
        {
            try
            {
                string assembly = Assembly.GetExecutingAssembly().Location;
                string folder = Path.GetDirectoryName(assembly).ToLowerInvariant();
                string file = Path.Combine(folder, path);

                if (!File.Exists(file))
                    return;

                using (RegistryKey key = root.OpenSubKey("JavaScriptLanguageService", true))
                {
                    if (key == null)
                        return;

                    key.SetValue("ReferenceGroups_WE", "Implicit (Web)|" + file + ";");
                    return;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
    }
}