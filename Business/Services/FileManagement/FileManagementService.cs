using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

using Business.Services.Errors;

namespace Business.Services.FileManagement
{
    public class FileManagementService : BaseService, IFileManagementService
    {
        public static HashSet<string> AllowedExtensions =>
            new HashSet<string>(new[]
                {
                    ".gif",
                    ".png",
                    ".jpg",
                    ".jpeg",
                    ".tiff",
                    ".tif"
                }, StringComparer.OrdinalIgnoreCase);

        IErrorHelperService ErrorHelper { get; set; }

        public FileManagementService(IErrorHelperService errorHelper)
        {
            ErrorHelper = errorHelper ?? throw new ArgumentNullException(nameof(errorHelper));
        }

        public byte[] GetPostedFileBinary(HttpPostedFileBase file)
        {
            byte[] data = new byte[file.ContentLength];
            file.InputStream.Seek(0, SeekOrigin.Begin);
            file.InputStream.Read(data, 0, file.ContentLength);

            return data;
        }

        public void EnsureFolderExistence(string folderPhysicalPath)
        {
            if (!Directory.Exists(folderPhysicalPath))
            {
                DirectoryInfo directoryInfo;

                try
                {
                    directoryInfo = Directory.CreateDirectory(folderPhysicalPath);
                }
                catch (Exception ex)
                {
                    ErrorHelper.LogException(nameof(FileManagementService), nameof(EnsureFolderExistence), ex);

                    throw;
                }
            }
        }

        public string MakeStringUrlCompliant(string input)
        {
            var allowedCharacters = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~:/?#[]@!$&'()*+,;=";
            var stringBuilder = new StringBuilder();

            foreach (var character in input)
            {
                var charToAdd = allowedCharacters.Contains(character) ? character : '_';
                stringBuilder.Append(charToAdd);
            }

            return stringBuilder.ToString();
        }

        public void EnsureFileExistence(string physicalPath, byte[] fileBinary, bool forceOverwrite = false)
        {
            if (!File.Exists(physicalPath) || forceOverwrite)
            {
                try
                {
                    File.WriteAllBytes(physicalPath, fileBinary);
                }
                catch (Exception ex)
                {
                    ErrorHelper.LogException(nameof(FileManagementService), nameof(EnsureFileExistence), ex);

                    throw;
                } 
            }
        }

        public string GetServerRelativePath(HttpRequestBase request, string physicalPath)
        {
            var trimmed = physicalPath.Substring(request.PhysicalApplicationPath.Length);

            return $"~/{trimmed.Replace('\\', '/')}";
        }
    }
}