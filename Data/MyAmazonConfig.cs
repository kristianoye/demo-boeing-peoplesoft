namespace demo_boeing_peoplesoft.Data
{
    /// <summary>
    /// AWS config stuff
    /// </summary>
    public class MyAmazonConfig
    {
        /// <summary>
        /// AWS CLI profile name to use when working with the site
        /// </summary>
        public string? Profile { get; set; }

        /// <summary>
        /// The region we are working in
        /// </summary>
        public string? Region { get; set; }

        /// <summary>
        /// Settings specific to our S3 bucket
        /// </summary>
        public MyAmazonS3Config S3 { get; set; } = new MyAmazonS3Config();
    }

    /// <summary>
    /// Config section for Amazon S3 settings
    /// </summary>
    public class MyAmazonS3Config
    {
        /// <summary>
        /// Placeholder file used to ensure a directory can be created even without real content
        /// </summary>
        public const string PlaceholderFilename = ".placeholder";

        /// <summary>
        /// AccessKey for user with fileusers perms
        /// </summary>
        public string? AccessKey { get; set; }

        /// <summary>
        /// Secret for user with fileusers perms
        /// </summary>
        public string? SecretKey { get; set; }

        /// <summary>
        /// The bucket we will use to store files associated with this site
        /// </summary>
        public string DefaultBucketName { get; set; } = "boeingmedia";
    }
}
