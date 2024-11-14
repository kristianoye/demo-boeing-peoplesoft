namespace demo_boeing_peoplesoft.Models
{
    /// <summary>
    /// Represents a "directory" in S3
    /// </summary>
    public class MediaDirectory
    {
        public string Name { get; set; } = string.Empty;

        public List<MediaDirectory> SubDirectories { get; set; } = new List<MediaDirectory>();

        public List<MediaFile> Files { get; set; } = new List<MediaFile>();
    }
}
