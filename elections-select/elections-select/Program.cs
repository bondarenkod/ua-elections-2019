using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace elections_select
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            new Program().Run().Wait();
        }

        private async Task Run()
        {
            var files = Directory.GetFiles(Environment.CurrentDirectory, "input-data-*.json");
            var input = new List<Region>();
            foreach (var item in files)
            {
                if (item.Contains("000"))
                {
                    continue;
                }

                //if (!item.Contains("148"))
                //{
                //    continue;
                //}

                var data = LoadAndParseDistrict(item);
                input.Add(data);
            }

            foreach (var i in input)
            {
                await LoadDistrictResults(i);
            }

            var xlFile = Utils.GetFileInfo("output.xlsx", false);
            using (var package = new ExcelPackage(xlFile))
            {
                FillAll(package, input);
                FillSummary(package, input);
                package.Save();
                //package.SaveAs(xlFile);
            }
        }

        private void FillSummary(ExcelPackage package, List<Region> regions)
        {
            // Add a new worksheet to the empty workbook
            var worksheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "Summary");
            if (worksheet == null)
            {
                worksheet = package.Workbook.Worksheets.Add("Summary");
            }

            for (int i = 2; i < 100; i++)
            {
                worksheet.DeleteRow(2);
            }

            //Місце 
            //ТВО 
            //Кількість дільниць  
            //Скільки оброблено     2   
            //З них помилок  2
            //Веріфіковано 2    

            int start = 2;
            int col = 1;

            foreach (var region in regions)
            {
                var d = region.ElectionDistrict;
                var place = string.IsNullOrEmpty(region.CityName) ? region.RegionName : $"{region.RegionName} - {region.CityName}";

                var obr = d.Sections.Count(x => x.Value.FinalResult.has_parsed);
                var err = d.Sections.Count(x => x.Value.FinalResult.has_errors);
                var total = d.Sections.Count;
                var res = new
                {
                    place = place,
                    tbo = d.DistrictName,
                    dil_count = total,
                    obr = obr,
                    obr_per =((obr * 100) / total)/100.0,
                    errors = err,
                    errors_per = ((err * 100) / total)/100.0,
                    verif = 0,
                    verif_per = 0
                };

                worksheet.Cells[start, col++].Value = res.place;
                worksheet.Cells[start, col++].Value = res.tbo;
                worksheet.Cells[start, col++].Value = res.dil_count;
                worksheet.Cells[start, col++].Value = res.obr;
                worksheet.Cells[start, col++].Value = res.obr_per;
                worksheet.Cells[start, col++].Value = res.errors;
                worksheet.Cells[start, col++].Value = res.errors_per;
                worksheet.Cells[start, col++].Value = res.verif;
                worksheet.Cells[start, col++].Value = res.verif_per;
                col = 1;
                start++;
            }
        }

        private void FillAll(ExcelPackage package, List<Region> regions)
        {
            // Add a new worksheet to the empty workbook
            //var has = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "All");
            //if (has != null)
            //{
            //    package.Workbook.Worksheets.Delete(has);
            //}

            //var worksheet = package.Workbook.Worksheets.Add("All");

            var worksheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "All");
            if (worksheet == null)
            {
                worksheet = package.Workbook.Worksheets.Add("All");
            }

            for (int i = 0; i < 3000; i++)
            {
                worksheet.DeleteRow(2);
            }

            int start = 1;
            int col = 1;
            //Add the headers
            worksheet.Cells[start, col++].Value = "область";
            worksheet.Cells[start, col++].Value = "місто";
            worksheet.Cells[start, col++].Value = "№ ТВО";
            worksheet.Cells[start, col++].Value = "Дільниця";
            worksheet.Cells[start, col++].Value = "Різних";
            worksheet.Cells[start, col++].Value = "Є оціфровані";
            worksheet.Cells[start, col++].Value = "Є помилки";
            worksheet.Cells[start, col++].Value = "Кількість оцифровок";
            worksheet.Cells[start, col++].Value = "Кільість помилок";
            worksheet.Cells[start, col++].Value = "Спецдільниця";
            worksheet.Cells[start, col++].Value = "адреса для спець-дільниць";

            worksheet.Cells[start, col++].Value = "Перевірено?";
            worksheet.Cells[start, col++].Value = "Комментар";



            start++;

            //    var sb = new StringBuilder();

            //sb.AppendLine($"область|№ ТВО|дільниця|Спецдільниця|Є оціфровані|Є помилки|Кількість оцифровок|Кільість помилок|адреса для спець-дільниць");

            foreach (var region in regions)
            {
                foreach (var s in region.ElectionDistrict.Sections)
                {
                    //область 
                    //ТВО номер 
                    //дільниця    
                    //Спецдільниця 
                    //Є оціфровані 
                    //Є помилки 
                    //Кількість оцифровок 
                    //Кільість помилок 
                    //кільк
                    //адреса для спець-дільниць

                    var res = new FinaleResult
                    {
                        oblast = region.RegionName,
                        city = region.CityName,
                        tbo_numer = region.ElectionDistrict.DistrictName,
                        dilnitcia = s.Value.Id,
                        spec = s.Value.IsSpecial,
                        has_parsed = s.Value.Results.Any(x => x.ParsedCount > 0),
                        has_errors = s.Value.Results.Any(x => x.HasErrors),
                        errors_count_total = s.Value.Results.Where(x => x.HasErrors).Count(),
                        parsed_count_total = s.Value.Results.Where(x => x.ParsedCount > 0).Count(),
                        total_sources = s.Value.Results.Count(),
                        specAddr = s.Value.Location,

                        checke = false,
                        comment = ""
                    };

                    s.Value.FinalResult = res;


                    col = 1;

                    worksheet.Cells[start, col++].Value = res.oblast;//"область";
                    worksheet.Cells[start, col++].Value = res.city;//"місто";
                    worksheet.Cells[start, col++].Value = res.tbo_numer;//"№ ТВО";
                    worksheet.Cells[start, col++].Value = res.dilnitcia;//"дільниця";
                    worksheet.Cells[start, col++].Value = res.total_sources;//"кількість різних даних";
                    worksheet.Cells[start, col++].Value = BTS(res.has_parsed);//"Є оціфровані";
                    worksheet.Cells[start, col++].Value = BTS(res.has_errors);//"Є помилки";
                    worksheet.Cells[start, col++].Value = res.parsed_count_total;//"Кількість оцифровок";
                    worksheet.Cells[start, col++].Value = res.errors_count_total;//"Кільість помилок";
                    worksheet.Cells[start, col++].Value = BTS(res.spec);//"Спецдільниця";
                    worksheet.Cells[start, col++].Value = res.specAddr;//"адреса для спець-дільниці";

                    worksheet.Cells[start, col++].Value = BTS(res.checke);
                    worksheet.Cells[start, col++].Value = res.comment;

                    start++;
                }
            }
        }

        private string BTS(bool inp)
        {
            return inp ? "+" : "";
        }

        private async Task LoadDistrictResults(Region src)
        {
            var dst = src.ElectionDistrict;
            var url = $"https://e-vybory.org/feed?region=&district={dst.DistrictName}&station=&error=";
            var web = new HtmlAgilityPack.HtmlWeb();
            var htmlDoc = web.Load(url);
            var tablebody = htmlDoc.DocumentNode.SelectSingleNode("//table[@class='table table-hover']/tbody");

            var trs = tablebody.SelectNodes("//tbody//tr");

            foreach (var tr in trs)
            {
                var td_nodes = tr.SelectNodes("./td");

                var t_time = td_nodes[0].InnerText;
                var t_region = td_nodes[1].InnerText;
                var t_okrug = td_nodes[2].InnerText;
                var t_dilnitcia = td_nodes[3].InnerText;
                var t_errors = td_nodes[4].InnerText;

                var t_comments = td_nodes[5];

                if (t_dilnitcia == "530581")
                {

                }

                var detailsPhotos = t_comments.SelectNodes("./a//i[@class='fa fa-camera']");
                var detailsParsed = t_comments.SelectNodes("./a//i[@class='fa fa-table']");
                //Console.WriteLine(); Console.WriteLine();


                var sec = dst.Sections[t_dilnitcia];
                sec.Results.Add(new Result()
                {
                    HasErrors = !string.IsNullOrEmpty(t_errors),
                    ParsedCount = detailsParsed == null ? 0 : detailsParsed.Count,
                    SourceCount = detailsPhotos == null ? 0 : detailsPhotos.Count,
                });
            }

        }

        private Region LoadAndParseDistrict(string inputFileName)
        {
            var text = File.ReadAllText(inputFileName);
            var raw = Newtonsoft.Json.JsonConvert.DeserializeObject<Region>(text);
            var ed = raw.ElectionDistrict;
            foreach (var spec in ed.ElectionSectionsSpecial)
            {
                spec.IsSpecial = true;
                ed.Sections[spec.Id] = spec;
            }

            foreach (var simple in ed.ElectionSections)
            {
                ed.Sections[simple] = new ElectionDistrictsSection()
                {
                    Id = simple,
                    IsSpecial = false
                };
            }

            return raw;
        }
    }

    public class Utils
    {
        public static FileInfo GetFileInfo(string file, bool deleteIfExists = true)
        {
            var fi = new FileInfo(file);
            if (deleteIfExists && fi.Exists)
            {
                fi.Delete();  // ensures we create a new workbook
            }
            return fi;
        }
    }
}
