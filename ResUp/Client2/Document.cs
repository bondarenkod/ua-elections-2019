using System.IO;

namespace Client2
{
    public class Document
    {
        public string[] Files { get; set; }
        public string DistrictNumber { get; set; }
        public string PollingStation { get; set; }
        public string RegionId { get; set; }
        public bool Sent { get; set; }
        public string Path { get; set; }
        public bool HasError { get; set; }
        public string Name { get; set; }

        public const string ExtSent = ".sent";
        public const string ExtError = ".error";

        public void MakeDone()
        {
            Sent = true;
            var fp = System.IO.Path.Combine(Path, $"res{ExtSent}");
            if (!File.Exists(fp))
            {
                File.Create(fp, 10, FileOptions.None);
            }
        }

        public void MakeError()
        {
            HasError = true;
            var fp = System.IO.Path.Combine(Path, $"res{ExtError}");
            if (!File.Exists(fp))
            {
                File.Create(fp, 10, FileOptions.None);
            }
        }
    }
}