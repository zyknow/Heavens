
using Bing.Drawing;
using System.Drawing;
using System.Drawing.Imaging;

namespace Heavens.Core.Extension.Helper;

public class ImgHelper
{

    private static ImageFormat GetImageFormat(string extension)
    {
        ImageFormat? format = null;
        string? ex = extension.ToLower();
        switch (ex)
        {
            case ".jpg":
            case ".jpeg":
            case "jpg":
            case "jpeg":
                format = ImageFormat.Jpeg;
                break;
            case ".png":
            case "png":
                format = ImageFormat.Png;
                break;
            case ".ico":
            case ".icon":
            case "ico":
            case "icon":
                format = ImageFormat.Icon;
                break;
            case ".bmp":
            case "bmp":
                format = ImageFormat.Bmp;
                break;
            case ".gif":
            case "gif":
                format = ImageFormat.Gif;
                break;
            default:
                format = ImageFormat.Jpeg;
                break;
        }
        return format;
    }


    /// <summary>
    /// 保存缩略图
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="savePath"></param>
    /// <param name="resolution"></param>
    public static void SaveThumbnail(Stream stream, string savePath, int resolution = 320, string extension = ".jpg")
    {
        SaveThumbnail(stream.GetAllBytes(), savePath, resolution, extension);
    }

    /// <summary>
    /// 保存缩略图
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="savePath"></param>
    /// <param name="resolution"></param>
    public static void SaveThumbnail(byte[] bytes, string savePath, int resolution = 320, string extension = ".jpg")
    {
        using Image? img = ResizeToImage(bytes.ToMemoryStream(), resolution);
        img.Save(savePath, GetImageFormat(extension));
    }

    /// <summary>
    /// 缩放图片
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="resolution">分辨率 1440/1080/720 等，填数字</param>
    public static byte[] ResizeToBytes(byte[] bytes, int resolution, string extension = ".png")
    {
        using Image? img = ResizeToImage(bytes.ToMemoryStream(), resolution);
        return img.ToBytes(GetImageFormat(extension));
    }

    /// <summary>
    /// 缩放图片
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="resolution">分辨率 1440/1080/720 等，填数字</param>
    /// <param name="extension"></param>
    /// <returns></returns>
    public static Stream ResizeToStream(byte[] bytes, int resolution, string extension = ".png")
    {
        using Image? img = ResizeToImage(bytes.ToMemoryStream(), resolution);
        return img.ToBytes(GetImageFormat(extension)).ToMemoryStream();
    }

    /// <summary>
    /// 缩放图片
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="extension"></param>
    /// <returns></returns>
    public static Image ResizeToImage(Stream stream, int resolution)
    {
        Bitmap bitmap = new Bitmap(stream);

        int refWidth = bitmap.Width <= bitmap.Height ? resolution : 0;
        int height = refWidth > 0 ? 0 : resolution;

        // 尺寸小于该分辨率，直接返回
        if (bitmap.Width <= resolution || bitmap.Height <= resolution)
            return bitmap;
        return ImageHelper.MakeThumbnail(bitmap, refWidth, height, refWidth > 0 ? ThumbnailMode.FixedW : ThumbnailMode.FixedH);
    }

    ///// <summary>
    ///// 保存缩略图
    ///// </summary>
    ///// <param name="stream"></param>
    ///// <param name="savePath"></param>
    ///// <param name="resolution"></param>
    //public static void SaveThumbnail(Stream stream, string savePath, int resolution = 320, string extension = ".jpg")
    //{
    //    SaveThumbnail(stream.GetAllBytes(), savePath, resolution, extension);
    //}

    ///// <summary>
    ///// 保存缩略图
    ///// </summary>
    ///// <param name="bytes"></param>
    ///// <param name="savePath"></param>
    ///// <param name="resolution"></param>
    //public static void SaveThumbnail(byte[] bytes, string savePath, int resolution = 320, string extension = ".jpg")
    //{
    //    using var mat = ResizeToMat(bytes, resolution);
    //    mat.SaveImage(savePath);
    //}

    ///// <summary>
    ///// 缩放图片
    ///// </summary>
    ///// <param name="bytes"></param>
    ///// <param name="resolution">分辨率 1440/1080/720 等，填数字</param>
    //public static byte[] ResizeToBytes(byte[] bytes, int resolution, string extension = ".png")
    //{
    //    using Mat mat = ResizeToMat(bytes,resolution);
    //    return mat.ToBytes(extension);
    //}

    ///// <summary>
    ///// 缩放图片
    ///// </summary>
    ///// <param name="bytes"></param>
    ///// <param name="resolution">分辨率 1440/1080/720 等，填数字</param>
    ///// <param name="extension"></param>
    ///// <returns></returns>
    //public static Stream ResizeToStream(byte[] bytes, int resolution, string extension = ".png")
    //{
    //    //using Mat mat = ResizeToMat(bytes, resolution);
    //    //return mat.ToMemoryStream(extension,new ImageEncodingParam(ImwriteFlags.Jpeg2000CompressionX1000,3));

    //    var dt = ImageHelper.MakeThumbnail(bytes, resolution, 0, ThumbnailMode.FixedW).ToBytes(ImageFormat.Jpeg).ToMemoryStream();

    //    return dt;
    //}

    ///// <summary>
    ///// 缩放图片
    ///// </summary>
    ///// <param name="bytes"></param>
    ///// <param name="extension"></param>
    ///// <returns></returns>
    //public static Mat ResizeToMat(byte[] bytes, int resolution)
    //{
    //    Mat mat = Mat.FromImageData(bytes, ImreadModes.Unchanged);
    //    var refWidth = mat.Width <= mat.Height;

    //    var towidth = 0;
    //    var toheight = 0;

    //    if (refWidth)
    //    {
    //        if (mat.Width <= resolution)
    //            return mat;

    //        towidth = resolution;
    //        toheight = mat.Height * resolution / mat.Width;
    //    }
    //    else
    //    {
    //        if (mat.Height <= resolution)
    //            return mat;

    //        toheight = resolution;
    //        towidth = mat.Width * resolution / mat.Height;

    //    }

    //    var mat2 = mat.Resize(new OpenCvSharp.Size(towidth, toheight), 0, 0,InterpolationFlags.Area);
    //    mat.Dispose();
    //    return mat2;
    //}
}
