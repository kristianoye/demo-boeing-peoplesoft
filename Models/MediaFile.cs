using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace demo_boeing_peoplesoft.Models
{
    /// <summary>
    /// Database representation of a file in our S3 bucket
    /// </summary>
    public class MediaFile
    {
        public MediaFile() { }

        public MediaFile(bool isOrphan)
        {
            IsOrphan = isOrphan;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MediaId { get; set; }

        [Required]
        public int UserId { get; set; }

        [MaxLength(100)]
        public string AltText { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Filename { get; set; } = string.Empty;

        [MaxLength(1024)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(1024)]
        public string S3Url { get; set; } = string.Empty;

        public long Filesize { get; set; } = -1;

        [Required, MaxLength(255)]
        public string MimeType { get; set; } = string.Empty;

        public DateTime CreateDate { get; set; } = DateTime.Now;

        [NotMapped]
        public bool IsOrphan { get; private set; }
    }
}
