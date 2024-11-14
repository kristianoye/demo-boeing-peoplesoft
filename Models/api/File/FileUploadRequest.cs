using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace demo_boeing_peoplesoft.Models.api.File
{
    /// <summary>
    /// Contains a file upload request; May contain more than one file, but only one target directory.
    /// </summary>
    public class FileUploadRequest
    {
        /// <summary>
        /// The files being uploaded
        /// </summary>
        public IFormFile[] Files { get; set; } = [];

        /// <summary>
        /// Alt text the map to the file by index
        /// </summary>
        public string[] AltText { get; set; } = [];

        /// <summary>
        /// Full descriptions that map to the file by index
        /// </summary>
        public string[] Description { get; set; } = [];

        /// <summary>
        /// Prefix to append to path
        /// </summary>
        public string? Prefix { get; set; } = null;

        /// <summary>
        /// Get the number of files uploaded
        /// </summary>
        /// <returns>The number of files or 0 if no files were uploaded</returns>
        public int GetLength()
        {
            if (Files == null)
                return 0;
            return Files.Length;
        }

        /// <summary>
        /// Get a tuple for a particular index
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Returns the Nth file entry</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public (IFormFile file, string AltText, string Description) GetEntry(int index)
        {
            if (index < 0 || index >= Files.Length)
                throw new IndexOutOfRangeException();
            else
                return (Files[index], AltText[index], Description[index]);
        }

        /// <summary>
        /// Allocate a response for this request
        /// </summary>
        /// <returns></returns>
        public FileUploadResponse PrepareResponse()
        {
            return new FileUploadResponse(this);
        }
    }
}
