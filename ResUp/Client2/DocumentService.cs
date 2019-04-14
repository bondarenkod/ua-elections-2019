using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;

namespace Client2
{
    public class DocumentService
    {
        private const string SiteUrl = "https://e-vybory.org/";
        public async Task<(bool ІsSuccessful, string DocumentId)> CreateRemoteDocument(string region_id, string polling_station, string district_number, string comments, Dictionary<string, string> cookies, string csrf_token, string clarified = "1")
        {
            var client = new RestClient(SiteUrl);

            var request = new RestRequest("new-doc");

            request.AddHeader("Cache-Control", "max-age=0");

            foreach (var k in cookies)
            {
                request.AddCookie(k.Key, k.Value);
            }

            request.AddParameter("csrf_token", csrf_token);
            request.AddParameter("region_id", region_id);
            request.AddParameter("polling_station", polling_station);
            request.AddParameter("district_number", district_number);
            request.AddParameter("clarified", clarified);
            request.AddParameter("comments", comments);

            var res = await client.ExecutePostTaskAsync(request);

            var isOk = res.IsSuccessful;

            var id = "";

            if (res.IsSuccessful)
            {
                if (res.ResponseUri != null)
                {
                    var ls = res.ResponseUri.Segments?.LastOrDefault();
                    if (!string.IsNullOrEmpty(ls))
                    {
                        id = ls;
                    }
                }
            }

            if (string.IsNullOrEmpty(id))
            {
                isOk = false;
            }

            return (isOk, id);
        }

        public async Task<bool> UploadToRemoteDocument(string remoteDocumentId, Dictionary<string, string> cookies, string csrf_token, string[] files)
        {
            var client = new RestClient(SiteUrl);

            foreach (var file in files)
            {
                var request = new RestRequest($"upload-handler/{remoteDocumentId}", Method.POST);

                request.AddHeader("Cache-Control", "max-age=0");
                request.AddHeader("X-Requested-With", "XMLHttpRequest");
                request.AddHeader("X-CSRFToken", csrf_token);

                foreach (var k in cookies)
                {
                    request.AddCookie(k.Key, k.Value);
                }

                request.AddParameter("csrf_token", csrf_token);
                request.AddFile("files[]", file, "image/png");

                var res = await client.ExecutePostTaskAsync(request);

                if (!res.IsSuccessful)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<Document[]> GetLocalDocuments(string parentFolder)
        {
            if (!Directory.Exists(parentFolder))
            {
                return null;
            }

            var dirs = Directory.EnumerateDirectories(parentFolder).ToList();

            var res = new List<Document>();

            foreach (var dir in dirs)
            {
                var d = new Document();
                try
                {
                    d.Path = dir;
                    var files = Directory.GetFiles(dir, "*.*", SearchOption.TopDirectoryOnly);
                    var diri = new DirectoryInfo(dir);
                    var parts = diri.Name.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length < 1)
                    {
                        continue;
                    }

                    var files_proc = new List<string>();

                    var sent = false;
                    var error = false;

                    foreach (var file in files.Select(x => new FileInfo(x)))
                    {
                        switch (file.Extension)
                        {
                            case Document.ExtSent:
                                sent = true;
                                break;
                            case Document.ExtError:
                                error = true;
                                break;
                            case ".jpeg":
                            case ".jpg":
                            case ".png":
                                files_proc.Add(file.FullName);
                                break;
                        }
                    }

                    d.PollingStation = parts[1];
                    d.DistrictNumber = parts[0];
                    d.Name = $"{d.DistrictNumber}-{d.PollingStation}";
                    d.Files = files_proc.ToArray();
                    d.Sent = sent;
                    d.HasError = error;
                }
                catch (Exception e)
                {
                    d.HasError = true;
                }

                res.Add(d);
            }

            return res.ToArray();
        }
    }
}

