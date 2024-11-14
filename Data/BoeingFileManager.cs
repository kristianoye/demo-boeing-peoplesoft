using Amazon.S3;
using Amazon.S3.Model;
using demo_boeing_peoplesoft.Extensions;
using demo_boeing_peoplesoft.Models;
using System.Text.RegularExpressions;

namespace demo_boeing_peoplesoft.Data
{
    public interface IBoeingFileManager
    {
        string DefaultBucket { get; }

        Task<List<MediaFile>> ListFiles(string path, bool recursive = false, string? bucketName = null);
    }

    /// <summary>
    /// Manages the coupling between a S3 bucket and the database
    /// </summary>
    public class BoeingFileManager: IBoeingFileManager
    {
        /// <summary>
        /// Used to extract a user ID from an S3 key
        /// </summary>
        static Regex UserIdFromFilenameRegex = new Regex(@"[/]?users/(?<userId>\d+)/", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public BoeingFileManager(IConfiguration config, IAmazonS3 s3client, BoeingDbContext boeingDbContext)
        {
            _S3Client = s3client;
            _AmazonConfig = config.GetMyAmazonConfig();
            _BoeingDbContext = boeingDbContext;
            DefaultBucket = _AmazonConfig.S3.DefaultBucketName;
        }

        #region Properties

        private readonly IConfiguration _Config;

        private readonly MyAmazonConfig _AmazonConfig;
        private readonly BoeingDbContext _BoeingDbContext;

        /// <summary>
        /// Default bucket to use in operations
        /// </summary>
        public string DefaultBucket { get; private set; } = string.Empty;

        /// <summary>
        /// Client instance used to interact with S3
        /// </summary>
        private readonly IAmazonS3 _S3Client;

        #endregion

        #region Methods

        public async Task<List<MediaFile>> ListFiles(string path, bool recursive = false, string? bucketName = null)
        {
            var result = new List<MediaFile>();

            bucketName = bucketName ?? DefaultBucket;
            var request = new ListObjectsV2Request()
            {
                BucketName = bucketName ?? DefaultBucket,
                Prefix = path
            };

            var objects = new List<S3Object>();
            ListObjectsV2Response response;
            do
            {
                response = await _S3Client.ListObjectsV2Async(request);
                if (response != null)
                {
                    //  Do not include placeholder files in output
                    objects.AddRange(response.S3Objects.Where(o =>
                    {
                        if (o.Key.EndsWith(MyAmazonS3Config.PlaceholderFilename))
                            return false;
                        else if (recursive)
                            return true;
                        var leftOver = o.Key.Substring(request.Prefix.Length);
                        return leftOver.IndexOf("/") == -1;
                    }));
                    request.ContinuationToken = response.NextContinuationToken;
                }
            }
            while (response?.IsTruncated == true);


            var fileEntries = _BoeingDbContext.MediaFiles
                .Where(f => f.Filename.StartsWith(path))
                .ToList();

            result.AddRange(objects.Select(o =>
            {
                var entry = fileEntries.SingleOrDefault(fo => fo.Filename == o.Key);

                if (entry != null)
                    return entry;

                var m = UserIdFromFilenameRegex.Match(o.Key);
                var userId = -1;

                if (m?.Success == true && m.Groups["userId"].Success)
                {
                    userId = int.Parse(m.Groups["userId"].Value);
                }

                return new MediaFile(true)
                {
                    Filename = o.Key,
                    CreateDate = o.LastModified ?? DateTime.Now,
                    AltText = "",
                    Description = "",
                    Filesize = o.Size ?? -1,
                    UserId = userId,
                    S3Url = $"https://{bucketName}.s3.{_AmazonConfig.Region}.amazonnaws.com/{o.Key}",
                };
            }));
            return result;
        }

        #endregion
    }
}
