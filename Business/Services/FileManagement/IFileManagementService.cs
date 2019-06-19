using System.Web;

namespace Business.Services.FileManagement
{
    /// <summary>
    /// Handles filesystem-related tasks.
    /// </summary>
    public interface IFileManagementService : IService
    {
        /// <summary>
        /// Gets a byte array of a posted file.
        /// </summary>
        /// <param name="file">The posted file.</param>
        /// <returns>The byte array of the file.</returns>
        byte[] GetPostedFileBinary(HttpPostedFileBase file);

        /// <summary>
        /// Makes sure that a local folder exists.
        /// </summary>
        /// <param name="folderPhysicalPath">Folder physical path.</param>
        void EnsureFolderExistence(string folderPhysicalPath);

        /// <summary>
        /// Makes a string contain only characters allowed in URLs.
        /// </summary>
        /// <param name="input">String to check.</param>
        /// <returns>String converted to be URL-compliant.</returns>
        string MakeStringUrlCompliant(string input);

        /// <summary>
        /// Makes sure that a local file exists.
        /// </summary>
        /// <param name="path">File physical path.</param>
        /// <param name="fileBinary">File byte array.</param>
        /// <param name="forceOverwrite">Flag to overwrite an existing file.</param>
        void EnsureFileExistence(string path, byte[] fileBinary, bool forceOverwrite = false);

        /// <summary>
        /// Converts a physical into a server-relative path.
        /// </summary>
        /// <param name="request">HTTP request.</param>
        /// <param name="physicalPath">Physical path.</param>
        /// <returns>A server-relative path.</returns>
        string GetServerRelativePath(HttpRequestBase request, string physicalPath);
    }
}