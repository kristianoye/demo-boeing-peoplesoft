using Amazon.S3;
using Amazon.S3.Model;

namespace demo_boeing_peoplesoft.Extensions
{
    public static class S3Extensions
    {
        public static async Task<bool> DeleteDirectoryAsync(this IAmazonS3 s3client, string path)
        {
            throw new NotImplementedException();
        }

        public static async Task<bool> DeleteFilesAsync(this IAmazonS3 s3Client, string bucketName, IEnumerable<string> files)
        {
            try
            {
                var request = new DeleteObjectsRequest
                {
                    BucketName = bucketName,
                    Objects = files.Select(fn => new KeyVersion { Key = fn }).ToList()
                };

                await s3Client.DeleteObjectsAsync(request);
                return true;
            }
            catch (Exception) { }
            return false;
        }

        public static async Task<List<S3Object>> ListDirectory(this IAmazonS3 s3client, string bucketName, string prefixPath, bool recursive = false)
        {
            throw new NotImplementedException();
        }
    }
}
