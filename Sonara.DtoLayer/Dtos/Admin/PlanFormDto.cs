namespace Sonara.DtoLayer.Dtos.Admin
{
    public class PlanFormDto
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public decimal Price { get; set; }
        public int MaxDeviceCount { get; set; }
        public bool HasAds { get; set; }
        public bool HasOfflineDownload { get; set; }
        public bool HasHighQualityAudio { get; set; }
        public int DurationInDays { get; set; }
    }
}