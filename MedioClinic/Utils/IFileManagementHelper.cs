using System.Web;

namespace MedioClinic.Utils
{
    // TODO: Document.
    public interface IFileManagementHelper
    {
        byte[] GetPostedFileBinary(HttpPostedFileBase file);

        string EnsureFilePath(string subfolder);

        string MakeStringUrlCompliant(string input);

        void WriteFileIfDoesntExist(string path, byte[] fileBinary, bool forceOverwrite = false);

        string GetServerRelativePath(HttpRequestBase request, string physicalPath);
    }
}