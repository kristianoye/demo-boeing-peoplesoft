using Amazon.S3;
using Amazon.S3.Model;
using demo_boeing_peoplesoft.Data;
using demo_boeing_peoplesoft.Extensions;
using demo_boeing_peoplesoft.Models;
using demo_boeing_peoplesoft.Models.api.File;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace demo_boeing_peoplesoft.Controllers.api
{
    [Route("api/[controller]")]
    [Authorize(Roles = "user")]
    [ApiController]
    public class FileController : ControllerBase
    {
        public FileController(IAmazonS3 s3client, IConfiguration config, BoeingDbContext dbContext, IBoeingFileManager fm)
        {
            _S3Client = s3client;
            _AmazonConfig = config.GetMyAmazonConfig();
            _BoeingDbContext = dbContext;
            _FileManager = fm;
        }

        #region Properties

        private readonly IAmazonS3 _S3Client;

        private readonly MyAmazonConfig _AmazonConfig;

        private readonly BoeingDbContext _BoeingDbContext;

        private readonly IBoeingFileManager _FileManager;

        #endregion

        /// <summary>
        /// Upload one or more files to the S3 bucket
        /// </summary>
        /// <param name="upload">Data sent by the client including files, altText text, and descriptions</param>
        /// <returns></returns>
        [HttpPost("addFiles")]
        public async Task<IActionResult> AddFiles([FromForm] FileUploadRequest upload)
        {
            var len = upload.GetLength();

            if (upload == null)
                return BadRequest("Form data was invalid");
            else if (len == 0)
                return BadRequest("Must upload at least one uploadedFile");
            else
            {
                var userId = User.GetUserID();

                if (userId < 1)
                    return BadRequest("UserId is invalid");

                var response = upload.PrepareResponse();
                var prefix = upload.Prefix ?? string.Empty;

                for (int i = 0; i < len; i++)
                {
                    try
                    {
                        var (uploadedFile, altText, description) = upload.GetEntry(i);

                        using (var ms = new MemoryStream())
                        {

                            await uploadedFile.CopyToAsync(ms);
                            var mimeType = MimeMapping.MimeUtility.GetMimeMapping(uploadedFile.FileName);
                            var fullPath = $"users/{userId}/{prefix}/{uploadedFile.FileName}";

                            var request = new PutObjectRequest()
                            {
                                BucketName = _AmazonConfig.S3.DefaultBucketName,
                                Key = fullPath,
                                InputStream = ms,
                                ContentType = mimeType
                            };

                            var resp = await _S3Client.PutObjectAsync(request);

                            MediaFile mediaFile = new MediaFile()
                            {
                                AltText = altText,
                                Description = description,
                                S3Url = $"https://{_AmazonConfig.S3.DefaultBucketName}.s3.{_AmazonConfig.Region}.amazonnaws.com/{fullPath}",
                                Filename = fullPath,
                                UserId = userId,
                                Filesize = (int)uploadedFile.Length,
                                MimeType = mimeType
                            };
                            _BoeingDbContext.Add(mediaFile);
                            response.Complete(i, fullPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        response.AddError(i, ex);
                    }
                }
                //  Only commit to the database 
                if (response.ResponseCode != FileUploadResponseCode.Failure)
                {
                    try
                    {
                        await _BoeingDbContext.SaveChangesAsync();
                    }
                    catch(Exception)
                    {
                        //  DB failed, lets roll back and delete the objects from S3
                        await _S3Client.DeleteFilesAsync(_AmazonConfig.S3.DefaultBucketName, response.Items
                            .Select(i => i.RemoteFilename ?? string.Empty)
                            .Where(fn => fn.Length > 0));
                    }
                }
                return new JsonResult(response);
            }
        }

        [HttpGet("createDirectory")]
        public async Task<IActionResult> CreateDirectory(string path, string prefix = "", bool ignoreExisting = true)
        {
            if (!string.IsNullOrEmpty(prefix) && !User.IsAdmin())
                return BadRequest("Permission denied");
            using (MemoryStream ms = new MemoryStream())
            {
                var userId = User.GetUserID();
                var fullPath = string.IsNullOrEmpty(prefix) ? $"users/{userId}/{path}/{MyAmazonS3Config.PlaceholderFilename}" : $"{prefix}/{path}/{MyAmazonS3Config.PlaceholderFilename}";
                ms.Write(Encoding.Default.GetBytes(path));
                ms.Position = 0;

                var request = new PutObjectRequest()
                {
                    BucketName = _AmazonConfig.S3.DefaultBucketName,
                    Key = fullPath,
                    InputStream = ms,
                    ContentType = "text/plain"
                };

                await _S3Client.PutObjectAsync(request);
            }
            return Ok();
        }

        [HttpDelete("deleteDirectory")]
        public async Task<IActionResult> DeleteDirectory(string path, string prefix = "", bool forceDelete = false)
        {
            return Ok();
        }

        /// <summary>
        /// Delete a file from the S3 bucket
        /// </summary>
        /// <param name="fileId">The ID from the database</param>
        /// <returns></returns>
        [HttpDelete("deleteFile/{fileId}")]
        public async Task<IActionResult> DeleteFile(int fileId)
        {
            var fileInfo = _BoeingDbContext.MediaFiles.FirstOrDefault(f => f.MediaId == fileId);
            if (fileInfo == null)
                return BadRequest($"Object {fileId} not found");
            else if (fileInfo.UserId != User.GetUserID() && !User.IsAdmin())
                return BadRequest("Permission denied");
            else
            {
                await _S3Client.DeleteAsync(_AmazonConfig.S3.DefaultBucketName, fileInfo.Filename, null);
                _BoeingDbContext.MediaFiles.Remove(fileInfo);
                _BoeingDbContext.SaveChanges();
            }
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("getFile/{fileId}")]
        public async Task<IActionResult> GetFile(int fileId)
        {
            return Ok();
        }

        /// <summary>
        /// Enumerate files in a given directory
        /// </summary>
        /// <param name="path">The S3 Prefix to enumerate</param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        [HttpGet("getFiles")]
        public async Task<IActionResult> GetFiles(string path = "", string prefix = "", bool recursive = false)
        {
            if (!string.IsNullOrEmpty(prefix) && !User.IsAdmin())
                return BadRequest("Permission denied");
            else
            {
                var userId = User.GetUserID();
                var fullPath = !string.IsNullOrEmpty(prefix) && User.IsAdmin() ? $"{prefix}/{path}" : $"users/{userId}/{path}";
                return new JsonResult(await _FileManager.ListFiles(fullPath, recursive));
            }
        }
    }
}
