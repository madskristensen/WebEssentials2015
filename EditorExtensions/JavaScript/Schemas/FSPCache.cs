using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MadsKristensen.EditorExtensions.JavaScript
{
    internal class FSPCache
    {
        private static string _path = Environment.ExpandEnvironmentVariables(@"%localappdata%\Microsoft\FSPCache\SchemaStore.json");
        private const int _days = 3;
        private static DateTime _lastCheck = DateTime.MinValue;
        private static AsyncLock _mutex = new AsyncLock();

        public async Task SyncIntellisenseFiles()
        {
            using (await _mutex.LockAsync())
            {
                if (_lastCheck > DateTime.Now.AddDays(-_days))
                    return;

                _lastCheck = DateTime.Now;

                try
                {
                    string catalog = await GetCataLog();

                    if (!string.IsNullOrEmpty(catalog))
                    {
                        await DownloadIntellisenseFiles(catalog);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
        }

        private static async Task DownloadIntellisenseFiles(string catalog)
        {
            var urls = ParseJsonCatalog(catalog);
            var list = new List<Task>();

            foreach (string url in urls)
            {
                int index = url.LastIndexOf('/');
                if (index == -1)
                    continue;

                var directory = Path.GetDirectoryName(_path);

                string fileName = Path.Combine(directory, url.Substring(index + 1));
                list.Add(SaveUrlToFile(url, fileName));
            }

            await Task.WhenAll(list);
        }

        private static async Task SaveUrlToFile(string url, string fileName)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    await client.DownloadFileTaskAsync(url, fileName);
                }
            }
            catch (Exception ex)
            {
                // Fail silently. The server is having issues and this is not critical.
                System.Diagnostics.Debug.Write(ex);
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

            using (StreamReader reader = new StreamReader(_path))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private static async Task DownloadCatalog()
        {
            try
            {
                await SaveUrlToFile("http://schemastore.org/api/javascript/catalog.json", _path);
            }
            catch
            {
                Logger.Log("JSON Schema: Couldn't download the catalog file");
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

            return list;
        }
    }
}