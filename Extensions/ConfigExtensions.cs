using demo_boeing_peoplesoft.Data;
using System.Configuration;

namespace demo_boeing_peoplesoft.Extensions
{
    public static class ConfigExtensions
    {
        public static MyAmazonConfig GetMyAmazonConfig(this IConfiguration configuration)
        {
            var amazonConfig = configuration.GetSection("AWS").Get<MyAmazonConfig>();
            if (amazonConfig == null)
                throw new NullReferenceException("Amazon configuration is not set");
            return amazonConfig;
        }

        public static MyAmazonS3Config GetS3Config(this IConfiguration configuration)
        {
            var amazonConfig = configuration.GetMyAmazonConfig();
            return amazonConfig.S3;
        }

        public static MyAmazonS3Config GetS3Config(this MyAmazonConfig configuration)
        {
            return configuration.S3;
        }
    }
}
