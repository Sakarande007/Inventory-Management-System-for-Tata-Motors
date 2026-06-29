using QRCoder;
using System.IO;
using System.Windows.Media.Imaging;

namespace PaintShopIMS.Helpers
{
    public static class QRGeneratorHelper
    {
        public static BitmapImage GenerateQR(string payload)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new BitmapByteQRCode(qrCodeData);
            byte[] qrCodeAsBitmapByteArr = qrCode.GetGraphic(20);

            var image = new BitmapImage();
            using (var mem = new MemoryStream(qrCodeAsBitmapByteArr))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze(); // Allow access from other threads
            return image;
        }
    }
}
