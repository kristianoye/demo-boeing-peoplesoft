using Amazon.Runtime;
using Amazon.S3;
using demo_boeing_peoplesoft.Data;
using demo_boeing_peoplesoft.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace demo_boeing_peoplesoft
{
    public class Program
    {
        internal static IConfiguration? Configuration { get; set; }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews()
                .AddRazorRuntimeCompilation()
                .AddRazorRuntimeCompilation();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie("Cookies", options => { 
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Accounts/Logout";
                });
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<IBoeingFileManager, BoeingFileManager>();
            builder.Services.AddScoped<IAmazonS3>(sp =>
            {
                var awsOptions = Configuration.GetAWSOptions();
                var amazonConfig = Configuration.GetS3Config();
                if (amazonConfig != null)
                {
                    var creds = new BasicAWSCredentials(amazonConfig.AccessKey, amazonConfig.SecretKey);

                    var clientConfig = new AmazonS3Config
                    {
                        RegionEndpoint = awsOptions.Region
                    };
                    var result = new AmazonS3Client(creds, clientConfig);
                    return result;
                }
                throw new NullReferenceException("Failed to create S3 client");
            });

            builder.Services.AddScoped<BoeingDbContext>(provider =>
            {
                var config = builder.Configuration;
                return new BoeingDbContext(config);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            Configuration = app.Configuration;

            app.Run();
        }
    }
}
