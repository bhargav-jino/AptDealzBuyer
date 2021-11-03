using Android.Content;
using AptDealzBuyer.Droid.DependencService;
using AptDealzBuyer.Interfaces;
using System;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(HtmlToPDFService))]

namespace AptDealzBuyer.Droid.DependencService
{
    public class HtmlToPDFService : IHtmlToPDF
    {
        public string SaveFiles(string filename, byte[] bytes)
        {
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            var filePath = Path.Combine(documentsPath, filename);
            if (System.IO.File.Exists(filePath))
                return filePath;

            System.IO.File.WriteAllBytes(filePath, bytes);
            OpenFile(filePath, filename);
            return filePath;
        }

        public void OpenFile(string filePath, string filename)
        {

            var bytes = System.IO.File.ReadAllBytes(filePath);

            //Copy the private file's data to the EXTERNAL PUBLIC location
            string externalStorageState = global::Android.OS.Environment.ExternalStorageState;
            string application = "";

            string extension = System.IO.Path.GetExtension(filePath);

            switch (extension.ToLower())
            {
                case ".doc":
                case ".docx":
                    application = "application/msword";
                    break;
                case ".pdf":
                    application = "application/pdf";
                    break;
                case ".xls":
                case ".xlsx":
                    application = "application/vnd.ms-excel";
                    break;
                case ".jpg":
                case ".jpeg":
                case ".png":
                    application = "image/jpeg";
                    break;
                default:
                    application = "*/*";
                    break;
            }
            var externalPath = global::Android.OS.Environment.ExternalStorageDirectory.Path + "/" + filename + extension;
            System.IO.File.WriteAllBytes(externalPath, bytes);

            Java.IO.File file = new Java.IO.File(externalPath);
            file.SetReadable(true);
            //Android.Net.Uri uri = Android.Net.Uri.Parse("file://" + filePath);
            Android.Net.Uri uri = Android.Net.Uri.FromFile(file);
            Intent intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(uri, application);
            intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);

            try
            {
                Xamarin.Forms.Forms.Context.StartActivity(intent);
            }
            catch (Exception)
            {
                //     Toast.MakeText(Xamarin.Forms.Forms.Context, "There's no app to open dpf files", ToastLength.Short).Show();
            }
        }

    }
}