using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace DrCleanCare.Helpers
{
    public class Common
    {
        /// <summary>
        /// Function to check a file extension is image
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool IsImage(HttpPostedFileBase file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" }; // add more if u like...

            // linq from Henrik Stenbæk
            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Function to resize an image
        /// </summary>
        /// <param name="imgToResize"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Image ResizeImage(Image sourceImage, Size size)
        {
            return (Image)(new Bitmap(sourceImage, size));
        }

        /// <summary>
        /// Function to scale image
        /// </summary>
        /// <param name="sourceImage"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static Image ScaleImage(Image sourceImage, int width)
        {
            float aspectRatio = (float)width / (float)sourceImage.Size.Width;
            Size newSize = new Size(width, (int)(sourceImage.Size.Height * aspectRatio));
            return ResizeImage(sourceImage, newSize);
        }

        public static string SaveImage(Image image, string uploadPath, String filename)
        {
            var filePath = Path.Combine(HttpContext.Current.Server.MapPath(uploadPath) + filename);

            var duplicateCount = 0;
            while (System.IO.File.Exists(filePath))
            {
                duplicateCount++;

                var fileName = Path.GetFileNameWithoutExtension(filename) + duplicateCount.ToString();
                var fileExt = Path.GetExtension(filename);
                filePath = Path.Combine(HttpContext.Current.Server.MapPath(uploadPath) + fileName + fileExt);
            }

            // save file 
            image.Save(filePath);

            return Path.GetFileName(filePath);
        }

        public static string ConvertToUnSign(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
    }
}