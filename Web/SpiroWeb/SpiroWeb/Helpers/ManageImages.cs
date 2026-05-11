using System;
using System.Drawing;
using System.IO;

namespace SpiroWeb.Helpers
{
    public class ManageImage
    {
        public static Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            var ms = new MemoryStream(imageBytes, 0, imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            var image = Image.FromStream(ms, true);

            return image;
        }
        public static Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }


        public static String ImageToBase64(string image)
        {
            using (var ms = new MemoryStream())
            {
                // Convert Image to byte[]
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                String base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        public static byte[] GetBase64OfImagePath(string imagePath)
        {
            if (System.IO.File.Exists(imagePath))
            {
                using (Image image = Image.FromFile(imagePath))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();

                        // Convert byte[] to Base64 String
                        return imageBytes;
                    }
                }
            }
            else
            {
                return null;
            }
        }

        public static byte[] Base64ToBytes(string base64String)
        {
            return Convert.FromBase64String(base64String);
        }

    }
}