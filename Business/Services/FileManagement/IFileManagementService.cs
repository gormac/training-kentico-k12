using System.Web;

namespace Business.Services.FileManagement
{
    // TODO: Document.
    public interface IFileManagementService : IService
    {
        byte[] GetPostedFileBinary(HttpPostedFileBase file);

        string EnsureFilesystemPath(string subfolder);

        string MakeStringUrlCompliant(string input);

        void WriteFileIfDoesntExist(string path, byte[] fileBinary, bool forceOverwrite = false);

        string GetServerRelativePath(HttpRequestBase request, string physicalPath);
    }
}