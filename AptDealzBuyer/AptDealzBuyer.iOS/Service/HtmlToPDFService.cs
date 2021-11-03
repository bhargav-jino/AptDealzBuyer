using AptDealzBuyer.Interfaces;
using AptDealzBuyer.iOS.Service;
using CoreGraphics;
using Foundation;
using QuickLook;
using System;
using System.IO;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(HtmlToPDFService))]
namespace AptDealzBuyer.iOS.Service
{
    public class HtmlToPDFService : IHtmlToPDF
    {
        public string SafeHTMLToPDF(string html, string filename)
        {
            UIWebView webView = new UIWebView(new CGRect(0, 0, 6.5 * 72, 9 * 72));

            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var file = Path.Combine(documents, filename + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToShortTimeString() + ".pdf");

            webView.Delegate = new WebViewCallBack(file);
            webView.ScalesPageToFit = true;
            webView.UserInteractionEnabled = false;
            webView.BackgroundColor = UIColor.White;
            webView.LoadHtmlString(html, null);

            return file;
        }

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
            FileInfo fi = new FileInfo(filePath);

            QLPreviewController previewController = new QLPreviewController();
            previewController.DataSource = new PDFPreviewControllerDataSource(fi.FullName, fi.Name);

            UINavigationController controller = FindNavigationController();
            if (controller != null)
                controller.PresentViewController(previewController, true, null);
        }

        private UINavigationController FindNavigationController()
        {
            foreach (var window in UIApplication.SharedApplication.Windows)
            {
                if (window.RootViewController.NavigationController != null)
                    return window.RootViewController.NavigationController;
                else
                {
                    UINavigationController val = CheckSubs(window.RootViewController.ChildViewControllers);
                    if (val != null)
                        return val;
                }
            }

            return null;
        }

        private UINavigationController CheckSubs(UIViewController[] controllers)
        {
            foreach (var controller in controllers)
            {
                if (controller.NavigationController != null)
                    return controller.NavigationController;
                else
                {
                    UINavigationController val = CheckSubs(controller.ChildViewControllers);
                    if (val != null)
                        return val;
                }
            }
            return null;
        }

        public class PDFItem : QLPreviewItem
        {
            string title;
            string uri;

            public PDFItem(string title, string uri)
            {
                this.title = title;
                this.uri = uri;
            }

            public override string ItemTitle
            {
                get { return title; }
            }

            public override NSUrl ItemUrl
            {
                get { return NSUrl.FromFilename(uri); }
            }
        }

        public class PDFPreviewControllerDataSource : QLPreviewControllerDataSource
        {
            string url = "";
            string filename = "";

            public PDFPreviewControllerDataSource(string url, string filename)
            {
                this.url = url;
                this.filename = filename;
            }

            public override IQLPreviewItem GetPreviewItem(QLPreviewController controller, nint index)
            {
                return new PDFItem(filename, url);
            }

            public override nint PreviewItemCount(QLPreviewController controller)
            {
                return 1;
            }
        }
        class WebViewCallBack : UIWebViewDelegate
        {
            string filename = null;
            public WebViewCallBack(string path)
            {
                this.filename = path;
            }

            public override void LoadingFinished(UIWebView webView)
            {
                double height, width;
                int header, sidespace;

                width = 595.2;
                height = 841.8;
                header = 10;
                sidespace = 10;

                UIEdgeInsets pageMargins = new UIEdgeInsets(header, sidespace, header, sidespace);
                webView.ViewPrintFormatter.ContentInsets = pageMargins;

                UIPrintPageRenderer renderer = new UIPrintPageRenderer();
                renderer.AddPrintFormatter(webView.ViewPrintFormatter, 0);

                CGSize pageSize = new CGSize(width, height);
                CGRect printableRect = new CGRect(sidespace,
                                  header,
                                  pageSize.Width - (sidespace * 2),
                                  pageSize.Height - (header * 2));
                CGRect paperRect = new CGRect(0, 0, width, height);
                renderer.SetValueForKey(NSValue.FromObject(paperRect), (NSString)"paperRect");
                renderer.SetValueForKey(NSValue.FromObject(printableRect), (NSString)"printableRect");
                NSData file = PrintToPDFWithRenderer(renderer, paperRect);
                File.WriteAllBytes(filename, file.ToArray());
            }

            private NSData PrintToPDFWithRenderer(UIPrintPageRenderer renderer, CGRect paperRect)
            {
                NSMutableData pdfData = new NSMutableData();
                UIGraphics.BeginPDFContext(pdfData, paperRect, null);

                renderer.PrepareForDrawingPages(new NSRange(0, renderer.NumberOfPages));

                CGRect bounds = UIGraphics.PDFContextBounds;

                for (int i = 0; i < renderer.NumberOfPages; i++)
                {
                    UIGraphics.BeginPDFPage();
                    renderer.DrawPage(i, paperRect);
                }
                UIGraphics.EndPDFContent();
                return pdfData;
            }
        }
    }
}