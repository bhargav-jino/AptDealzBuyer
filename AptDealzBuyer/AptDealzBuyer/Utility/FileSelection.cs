using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AptDealzBuyer.Utility
{
    public class FileSelection
    {
        public static string fileName = string.Empty;
        public static byte[] fileByte = null;
        public static string fileExtension = string.Empty;
        public static string relativePath;

        public static async Task FilePickup()
        {
            try
            {
                FileData fileData = await CrossFilePicker.Current.PickFile();
                if (fileData == null)
                {
                    fileName = string.Empty;
                    fileByte = null;
                    fileExtension = string.Empty;
                    return;
                }

                fileName = fileData.FileName;
                fileByte = fileData.DataArray;
                fileExtension = Path.GetExtension(fileName);
            }
            catch (Exception ex)
            {
                Common.DisplayErrorMessage("FileSelection/FilePickup: " + ex.Message);
            }
        }

        public static string DisplayImage(string RelativePath)
        {
            string extensionDoc = string.Empty;

            if (!Common.EmptyFiels(RelativePath))
            {
                string extension = Path.GetExtension(RelativePath).ToLower();

                if (extension == ".mp3" || extension == ".wma" || extension == ".acc")
                    extensionDoc = "iconMusic.png";
                else if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                    extensionDoc = RelativePath;
                else if (extension == ".mp4" || extension == ".mov" || extension == ".wmv" || extension == ".qt" || extension == ".gif")
                    extensionDoc = "iconVideo.png";
                else
                    extensionDoc = "iconFiles2.png";
            }

            return extensionDoc;
        }
    }
}
