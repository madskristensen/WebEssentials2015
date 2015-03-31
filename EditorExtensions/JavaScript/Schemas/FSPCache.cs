using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.JSON.Core.Schema;
using Microsoft.Web.Editor;
using Microsoft.Web.Editor.Host;
using Minimatch;
using Newtonsoft.Json.Linq;

namespace MadsKristensen.EditorExtensions.JavaScript
{
    internal class FSPCache
    {
        private static string _path = Environment.ExpandEnvironmentVariables(@"%localappdata%\Microsoft\FSPCache\SchemaStore.json");
        private static bool _isDownloading;
        private const int _days = 3;
        private static DateTime _lastCheck = DateTime.MinValue;

        public void SyncIntellisenseFiles()
        {
            if (_lastCheck > DateTime.Now.AddDays(-_days))
                return;

            Task.Run(async () =>
            {
                try {
                    string catalog = await GetCataLog();

                    if (!string.IsNullOrEmpty(catalog))
                    {
                        DownloadIntellisenseFiles(catalog);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            });
        }

        private static void DownloadIntellisenseFiles(string catalog)
        {
            var urls = ParseJsonCatalog(catalog);
            foreach (string url in urls)
            {
                int index = url.LastIndexOf("/");
                if (index == -1)
                    continue;

                var directory = Path.GetDirectoryName(_path);

                string fileName = Path.Combine(directory, url.Substring(index + 1));
                SaveUrlToFile(url, fileName).DoNotWait("Downloading " + url);
            }
        }

        private static async Task<bool> SaveUrlToFile(string url, string fileName)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    await client.DownloadFileTaskAsync(url, fileName);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return false;
            }
        }

        private static async Task<string> GetCataLog()
        {
            FileInfo file = new FileInfo(_path);

            if (!file.Directory.Exists)
                file.Directory.Create();

            if (!file.Exists || file.LastWriteTime < DateTime.Now.AddDays(-_days))
            {
                await DownloadCatalog();
            }

            return File.ReadAllText(_path);
        }

        private static async Task DownloadCatalog()
        {
            if (_isDownloading)
                return;

            try
            {
                _isDownloading = true;
                await SaveUrlToFile("http://schemastore.org/api/javascript/catalog.json", _path);
            }
            catch
            {
                Logger.Log("JSON Schema: Couldn't download the catalog file");
            }
            finally
            {
                _isDownloading = false;
            }
        }

        private static IEnumerable<string> ParseJsonCatalog(string catalog)
        {
            List<string> list = new List<string>();
            try
            {
                JObject json = JObject.Parse(catalog);
                var schemas = (JArray)json["schemas"];

                foreach (var schema in schemas)
                {
                    try
                    {
                        string url = (string)schema["url"];
                        if (!list.Contains(url))
                            list.Add(url);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            _lastCheck = DateTime.Now;
            return list;
        }
    }
}