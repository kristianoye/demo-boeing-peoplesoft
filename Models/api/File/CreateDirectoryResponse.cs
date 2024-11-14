namespace demo_boeing_peoplesoft.Models.api.File
{
    public class CreateDirectoryResponse
    {
        public string RemotePath { get; set; } = string.Empty;

        public bool Success { get; set; }

        public string? Error { get; set; }
    }
}
