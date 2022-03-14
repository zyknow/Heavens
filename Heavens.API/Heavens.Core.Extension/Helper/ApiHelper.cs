using Microsoft.AspNetCore.Mvc;

namespace Heavens.Core.Extension.Helper;

public class ApiHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static FileStreamResult GetFileResult(string path, string fileName = null)
    {
        return new FileStreamResult(File.OpenRead(path), "application/octet-stream") { FileDownloadName = fileName ?? Path.GetFileName(path) };
    }

    public static FileStreamResult GetFileResult(Stream stream, string fileName)
    {
        return new FileStreamResult(stream, "application/octet-stream") { FileDownloadName = fileName ?? Path.GetFileName(fileName) };
    }

    public static FileStreamResult GetFileResult(byte[] bytes, string fileName)
    {
        return new FileStreamResult(bytes.ToMemoryStream(), "application/octet-stream") { FileDownloadName = fileName ?? Path.GetFileName(fileName) };
    }
}
