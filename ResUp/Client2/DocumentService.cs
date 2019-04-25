using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using RestSharp;

namespace SiteClient
{
    public class DocumentService
    {
        private const string SiteUrl = "https://e-vybory.org/";
        public async Task<(bool ІsSuccessful, string DocumentId)> CreateRemoteDocument(string region_id, string polling_station, string district_number, string comments, Dictionary<string, string> cookies, string csrf_token, bool clarified)
        {
            var client = new RestClient(SiteUrl);

            var request = new RestRequest("new-doc");

            request.AddHeader("Host", "e-vybory.org");
            request.AddHeader("Cache-Control", "max-age=0");
            request.AddHeader("Upgrade-Insecure-Requests", "1");
            request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36");
            request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            request.AddHeader("Referer", "https://e-vybory.org/my-docs");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", "en-US,en;q=0.9");

            foreach (var k in cookies)
            {
                request.AddCookie(k.Key, k.Value);
            }

            request.AddParameter("csrf_token", csrf_token);
            request.AddParameter("region_id", region_id);
            request.AddParameter("polling_station", polling_station);
            request.AddParameter("district_number", district_number);
            request.AddParameter("clarified", clarified ? "1" : "0");
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

        public async Task<(bool, string error)> UploadToRemoteDocument(string remoteDocumentId, Dictionary<string, string> cookies, string csrf_token, string[] files)
        {
            try
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
                        return (false, res.ErrorMessage);
                    }
                }
            }
            catch (Exception e)
            {
                return (false, e.Message);
            }

            return (true, null);
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
                    if (parts.Length < 2)
                    {
                        continue;
                    }

                    var files_proc = new List<string>();

                    var sent = false;
                    var error = false;

                    foreach (var file in files.Select(x => new FileInfo(x)))
                    {
                        switch ((file.Extension ?? "").ToLowerInvariant())
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

                    var tryRegionId = await GetRegionOfPollingStation(d.PollingStation);

                    if (!string.IsNullOrEmpty(tryRegionId.Item1))
                    {
                        d.RegionId = tryRegionId.Item1;
                    }
                    else
                    {
                        d.MakeError(tryRegionId.errorMessage);
                    }

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

        private static District[] _xmlDistricts;
        private static PSListPollingStation[] _xmlPs;
        private static bool _isInited = false;
        public async Task<(bool, string)> InitPollingData()
        {
            try
            {
                _xmlDistricts = XmlListOfDistricts();
                _xmlPs = XmlListOfPollingStations();
                _isInited = true;
                return (true, null);
            }
            catch (Exception e)
            {
                _isInited = false;
                return (false, e.Message);
            }
        }

        public async Task<(string[], string errorMessage)> GetListOfPollingStations(string pollingDistrict)
        {
            if (!_isInited)
                return (null, "дані не ініціалізовано");

            try
            {
                return (_xmlPs.Where(x => x.T005F01 == pollingDistrict).Select(x => x.T012F03).ToArray(), null);
            }
            catch (Exception e)
            {
                return (null, e.Message);
            }
        }

        public async Task<(PollingDistrict[], string errorMessage)> GetListOfPollingDistricts()
        {
            if (!_isInited)
                return (null, "дані не ініціалізовано");

            try
            {
                return (_xmlDistricts.Select(x => new PollingDistrict()
                {
                    PollingDistrictName = x.T005F02,
                    RegionId = x.ID100,
                    PollingDistrictNumber = x.T005F01,
                }).ToArray(), null);
            }
            catch (Exception e)
            {
                return (null, e.Message);
            }
        }

        public async Task<(string, string errorMessage)> GetRegionOfPollingStation(string pollingStation)
        {
            if (!_isInited)
                return (null, "дані не ініціалізовано");

            try
            {
                var ps = _xmlPs.FirstOrDefault(x => x.T012F03 == pollingStation);

                if (ps != null)
                {
                    return (ps.ID100, null);
                }

                return (null, $"номер виборчої дільниці не знайдено - [{pollingStation}]");
            }
            catch (Exception e)
            {
                return (null, e.Message);
            }

        }

        private District[] XmlListOfDistricts()
        {
            var serializer = new XmlSerializer(typeof(Districts));
            using (var reader = new FileStream("od_district.xml", FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                var item = (Districts)serializer.Deserialize(reader);
                return item.DISTRICT;
            }
        }

        private PSListPollingStation[] XmlListOfPollingStations()
        {
            var serializer = new XmlSerializer(typeof(PSList));
            using (var reader = new FileStream("od_ps.xml", FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                var item = (PSList)serializer.Deserialize(reader);
                return item.PollingStation;
            }
        }

        public (bool, string) CheckIfDocumentValid(Document document)
        {
            if (!_isInited)
                return (false, "дані не ініціалізовано");

            var dbItem = _xmlPs.FirstOrDefault(x =>
                x.ID100 == document.RegionId && x.T005F01 == document.DistrictNumber &&
                x.T012F03 == document.PollingStation);

            if (dbItem == null)
            {
                return (false, $"виборчої дільниці з ID {document.RegionId}:{document.Name} не знайдено");
            }

            return (true, "");
        }
    }

    public class Element
    {
        public Element(int region, string name)
        {
            Header = $"{region.ToString(),2}-{name}";
            ItemId = name;
        }
        public string Header { get; set; }
        public string ItemId { get; set; }
    }


    public class PollingDistrict
    {
        public PollingDistrict()
        {
        }

        /// <summary>
        ///  ID100   - Код регіону України згідно КОАТУУ
        /// </summary>
        public string RegionId { get; set; }

        /// <summary>
        /// T005F01 - Номер територіального виборчого округу
        /// </summary>
        public string PollingDistrictNumber { get; set; }

        /// <summary>
        /// T005F02 - Назва територіального виборчого округу
        /// </summary>
        public string PollingDistrictName { get; set; }

    }

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false, ElementName = "PSList")]
    public partial class PSList
    {
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("PollingStation")]
        public PSListPollingStation[] PollingStation { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PSListPollingStation
    {
        /// <summary>
        ///  ID100   - Код регіону України згідно КОАТУУ
        /// </summary>
        public string ID100 { get; set; }
        /// <summary>
        /// T005F01 - Номер одномандатного виборчого округу
        /// </summary>
        public string T005F01 { get; set; }
        /// <summary>
        /// T012F03 - Номер виборчої дільниці
        /// </summary>
        public string T012F03 { get; set; }
        /// <summary>
        /// D602F03 - Тип виборчої дільниці
        /// </summary>
        public string D602F03 { get; set; }
        /// <summary>
        /// T012F06 - Опис меж виборчої дільниці
        /// </summary>
        public string T012F06 { get; set; }
        /// <summary>
        /// T012F09 - Адреса приміщення дільничної виборчої комісії
        /// </summary>
        public string T012F09 { get; set; }
        /// <summary>
        /// T012F36 - Місцезнаходження приміщення дільничної виборчої комісії
        /// </summary>
        public string T012F36 { get; set; }
        /// <summary>
        /// T012F08 - Адреса приміщення для голосування
        /// </summary>
        public string T012F08 { get; set; }
        /// <summary>
        /// T012F02 - Місцезнаходження приміщення для голосування
        /// </summary>
        public string T012F02 { get; set; }
    }



    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false, ElementName = "DISTRICTS")]
    public partial class Districts
    {
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DISTRICT")]
        public District[] DISTRICT { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class District
    {
        /// <summary>
        /// ID100   - Код регіону України згідно КОАТУУ
        /// </summary>
        public string ID100 { get; set; }
        /// <summary>
        /// T005F01 - Номер територіального виборчого округу
        /// </summary>
        public string T005F01 { get; set; }
        /// <summary>
        /// T005F02 - Назва територіального виборчого округу
        /// </summary>
        public string T005F02 { get; set; }
        /// <summary>
        /// T005F07 - Центр територіального виборчого округу
        /// </summary>
        public string T005F07 { get; set; }
        /// <summary>
        /// T005F06 - Опис меж територіального виборчого округу
        /// </summary>
        public string T005F06 { get; set; }
        /// <summary>
        /// T005F09 - Адреса приміщення окружної виборчої комісії
        /// </summary>
        public string T005F09 { get; set; }
        /// <summary>
        /// T005F41 - Місцезнаходження приміщення окружної виборчої комісії
        /// </summary>
        public string T005F41 { get; set; }
    }
}

