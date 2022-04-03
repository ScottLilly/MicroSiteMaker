using System.Drawing;
using System.Drawing.Imaging;

namespace MicroSiteMaker.Core;

public static class ExtensionMethods
{
    public static void SaveJpeg(this Image img, string filePath, long quality)
        => img.Save(filePath, GetEncoder(ImageFormat.Jpeg), GetEncoder(quality));


    public static void SaveJpeg(this Image img, Stream stream, long quality)
        => img.Save(stream, GetEncoder(ImageFormat.Jpeg), GetEncoder(quality));

    private static EncoderParameters GetEncoder(long quality)
    {
        var encoderParameters = new EncoderParameters(1);
        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
        return encoderParameters;
    }

    private static ImageCodecInfo GetEncoder(ImageFormat format) =>
        ImageCodecInfo.GetImageDecoders().Single(codec => codec.FormatID == format.Guid);
}