using System.Collections.Generic;

namespace elections_select
{
    public class Region
    {
        public string RegionName { get; set; }
        public string CityName { get; set; }
        /// <summary>
        /// округи
        /// </summary>
        public ElectionDistrict ElectionDistrict { get; set; }
    }

    /// <summary>
    /// округ
    /// </summary>
    public class ElectionDistrict
    {
        public ElectionDistrict()
        {
            Sections = new Dictionary<string, ElectionDistrictsSection>();
            ElectionSectionsSpecial = new ElectionDistrictsSection[0];
            ElectionSections = new string[0];
        }

        public string DistrictName { get; set; }

        /// <summary>
        /// спеціальні дільниці
        /// </summary>
        public ElectionDistrictsSection[] ElectionSectionsSpecial { get; set; }

        /// <summary>
        /// номер дільниці, текст
        /// </summary>
        public string[] ElectionSections { get; set; }

        /// <summary>
        /// Оброблені дільниці
        /// </summary>
        public Dictionary<string, ElectionDistrictsSection> Sections { get; set; }
    }

    /// <summary>
    /// дільниця
    /// </summary>
    public class ElectionDistrictsSection
    {
        public ElectionDistrictsSection()
        {
            Results = new List<Result>();
        }
        public string Id { get; set; }
        public string Location { get; set; }
        public bool IsSpecial { get; set; }

        public List<Result> Results { get; set; }

        public FinaleResult FinalResult { get; set; }
    }


    public class Result
    {
        public bool HasErrors { get; set; }
        public bool HasParsed => ParsedCount > 0;
        public int ParsedCount { get; set; }
        public int SourceCount { get; set; }
    }

    public class FinaleResult
    {
        public string oblast { get; set; }
        public string city { get; set; }
        public string tbo_numer { get; set; }
        public string dilnitcia { get; set; }
        public bool spec { get; set; }
        public bool has_parsed { get; set; }
        public bool has_errors { get; set; }
        public int errors_count_total { get; set; }
        public int parsed_count_total { get; set; }
        public int total_sources { get; set; }
        public string specAddr { get; set; }
        public bool checke { get; set; }
        public string comment { get; set; }
    }
}
