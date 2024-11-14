namespace demo_boeing_peoplesoft.Models.api.File
{
    public enum FileUploadResponseCode: int
    {
        /// <summary>
        /// All files uploaded successfully
        /// </summary>
        Success = 0,

        /// <summary>
        /// One or more files uploaded successfully
        /// </summary>
        PartialSuccess = 1,

        /// <summary>
        /// No files uploaded
        /// </summary>
        Failure = 2
    }

    public class FileUploadResponseItem
    {
        /// <summary>
        /// Any error messages associated with the processing of the image
        /// </summary>
        public List<string> Errors { get; private set; } = [];

        /// <summary>
        /// Filename as sent by the remote client
        /// </summary>
        public string OriginalFilename { get; set; } = string.Empty;

        /// <summary>
        /// The filename relative to the server's root filestore
        /// </summary>
        public string? RemoteFilename { get; set; } = null;

        /// <summary>
        /// If null then upload was skipped/aborted, false is failure, true is success
        /// </summary>
        public bool? Success { get; private set; } = null;

        /// <summary>
        /// Add an error to the model and indicate failure
        /// </summary>
        /// <param name="error"></param>
        public void AddError(string error)
        {
            Success = false;
            Errors.Add(error);
        }

        /// <summary>
        /// Indicate we are done working with this record
        /// </summary>
        internal void Complete(string? remoteFilename = null)
        {
            if (!string.IsNullOrEmpty(remoteFilename) && Errors.Count == 0)
            {
                Success = true;
                RemoteFilename = remoteFilename;
            }
        }
    }

    public class FileUploadResponse
    {
        public FileUploadResponse(FileUploadRequest request)
        {
            Items = request.Files.Select((f, i) => new FileUploadResponseItem { OriginalFilename = request.Files[i].FileName }).ToList();
            Length = Items.Count;
        }

        public FileUploadResponseCode ResponseCode
        {
            get
            {
                var successful = Items.Where(i => i.Success == true).Count();

                if (successful == Length)
                    return FileUploadResponseCode.Success;
                else if (successful > 0)
                    return FileUploadResponseCode.PartialSuccess;
                else
                    return FileUploadResponseCode.Failure;
            }
        }

        /// <summary>
        /// Individual response data for each file included in the request
        /// </summary>
        public List<FileUploadResponseItem> Items { get; private set; }

        /// <summary>
        /// Number of items in the original request
        /// </summary>
        public int Length { get; private set; }

        public void AddError(int index, Exception ex)
        {
            AddError(index, ex.Message);
        }

        public void AddError(int index, string errorMessage)
        {
            if (index < 0 || index >= Length)
                throw new IndexOutOfRangeException();
            else
                Items[index].AddError(errorMessage);
        }

        /// <summary>
        /// Indicate we are done with a particular entry
        /// </summary>
        /// <param name="index">The index of the item to mark as complete</param>
        /// <param name="remoteFilename">A relative server path if the object was stored successfully</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void Complete(int index, string? remoteFilename = null)
        {
            if (index < 0 || index >= Length)
                throw new IndexOutOfRangeException();
            else
                Items[index].Complete(remoteFilename);
        }
    }
}
