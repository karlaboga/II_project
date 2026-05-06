using System.Net.Http;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;

namespace BengosRestaurantApp
{
    public static class IpHelper
    {
        // Get public IP from external API
        public static async Task<string> GetPublicIpAddressAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    return await client.GetStringAsync("https://api.ipify.org?format=text");
                }
            }
            catch
            {
                return "Unable to retrieve public IP";
            }
        }

        // Generate QR code image from content
        public static BitmapImage GenerateQrCode(string content)
        {
            using (var generator = new QRCodeGenerator())
            {
                var data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
                using (var qrCode = new QRCode(data))
                {
                    using (var bitmap = qrCode.GetGraphic(20))
                    using (var memory = new MemoryStream())
                    {
                        bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                        memory.Position = 0;
                        var bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = memory;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze();
                        return bitmapImage;
                    }
                }
            }
        }
    }
}
