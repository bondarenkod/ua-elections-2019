using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Extensions;

namespace ResUp
{
    public class Document
    {
        public string[] Files { get; set; }
        public string ElectionDistrict { get; set; }
        public string ElectionBranch { get; set; }
    }
    public class UploadService
    {
        public async Task Test(string region_id, string polling_station, string district_number, string comments, Dictionary<string, string> cookies, string csrf_token, string clarified = "1")
        {
            var client = new RestClient("https://e-vybory.org/");

            var request = new RestRequest("new-doc");
            //request.AddParameter("name", "value"); // adds to POST or URL querystring based on Method
            //request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

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

            int i = 0;

            //request.AddHeader("", "");
            //request.AddHeader("", "");
            //request.AddHeader("", "");
            //request.AddHeader("", "");
            //request.AddHeader("", "");
            //request.AddHeader("", "");

            //csrf_token = ImUxNDJlYzk1ZTk5MWIyNDc5MDFhZjNlZTdjYjllMjBhNDViMmZlZGYi.XKztoQ.KHGNM9gvDmevtJQ_qVQENWUOnh8 & region_id = 68 & polling_station = 681325 & district_number = 192 & clarified = 1 & comments = test123456



            //            Referer: https://e-vybory.org/my-docs
            //Cache-Control: max-age=0
            //Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
            //Accept-Language: en-US,en;q=0.7,uk;q=0.3
            //Content-Type: application/x-www-form-urlencoded
            //Upgrade-Insecure-Requests: 1
            //User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.140 Safari/537.36 Edge/18.17763
            //Accept-Encoding: gzip, deflate, br
            //Host: e-vybory.org
            //Content-Length: 190
            //Connection: Keep-Alive
            //Cookie: session=.eJwdj0tqBDEMRO_idROsj22pL2P8kUgImQF3D1mE3D0mtSyox6ufUH3Z9R7Oe73sCPVjhjMo8kicExH5KH3m0SGlLmXsUk2L5zmTJJImZKhKnNH63OHYlGNB90w2SB1mHJC5eASfVKDkRjLJ-xTZGBAXpIbRxEsalHNLLRxhXMvr_fy0x_YxYLShyVShIxeN0JzMyuhqGBunjm7T9-5h39Wf66s2v22Fc3uzUFZMbwxFGPAIr8vW_1Hh3z_miUln.XKztnw.ueBbtPA57_0QHB4JFpJiGFq0T5Y; _gid=GA1.2.1814379141.1554816955; _ga=GA1.2.2125886027.1553451190; __cfduid=d40d860438070697bc3f8b1c2632ee5b11553451188; _fbp=fb.1.1554360556248.599901714


        }

        public async Task<bool> UploadFiles(Document d, string uriUploadTarget)
        {
            return false;
        }
        public async Task<string> CreateUploadTarget(Document d)
        {

            return "";
        }

        public async Task<Document[]> GetDocuments(string folder)
        {
            if (!Directory.Exists(folder))
            {
                return null;
            }

            var dirs = Directory.EnumerateDirectories(folder).ToList();

            var res = new List<Document>();

            foreach (var dir in dirs)
            {
                var d = new Document();

                var files = Directory.GetFiles(dir);

                var diri = new DirectoryInfo(dir);

                var parts = diri.Name.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 1)
                {
                    continue;
                }

                d.ElectionBranch = parts[0];
                d.ElectionDistrict = parts[1];
                d.Files = files;
                res.Add(d);
            }

            return res.ToArray();
        }
    }
}

