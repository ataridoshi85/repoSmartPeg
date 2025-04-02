// See https://aka.ms/new-console-template for more information
using Google.Cloud.Storage.V1;
using Google.Apis.Auth;
using System;
using System.Diagnostics;
using System.IO;
using IronPdf;
using Aspose.Html;
using Aspose.Html.Converters;
using Aspose.Html.Saving;
using Markdown2Pdf;
using Markdig;
using PuppeteerSharp;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Playwright;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

class Program
{
    static async Task Main()
    {

        // Set up authentication
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", 
            @"C:\env\positive-lambda-452115-j5-bc5897c34cfc.json");

        // Initialize the client
        var storageClient = StorageClient.Create();

        

        // Specify the bucket and file
        string bucketName = "masime-prototipo1";
        string objectName = "01_mdp.giustizia-amministrativa.it.txt_claude-3-7-sonnet-20250219_criterio_redazione_massime_prompt_21_03_2025.txt";

        string destinationFileName = @"C:\env\downloaded-markdown.md";
        string htmlDestinationFileName = @"C:\env\downloaded-markdown-to-html.html";
        string pdfDestinationFileName = @"C:\env\markdown-to-pdf.pdf";


        

        // Download the file
        try
        {
            using (var outputFile = File.Create(destinationFileName))
            {
                storageClient.DownloadObject(bucketName, objectName, outputFile);
            }
            Console.WriteLine($"File {objectName} downloaded to {destinationFileName}");


            // Convert Markdown to HTML using Markdig
            string markdownContent = File.ReadAllText(destinationFileName);
            var pipeline = new MarkdownPipelineBuilder().Build();
            string htmlContent = Markdig.Markdown.ToHtml(markdownContent);
            File.WriteAllText(htmlDestinationFileName, htmlContent);
            Console.WriteLine($"HTML saved to {htmlDestinationFileName}");

            // Convert HTML to PDF using Selenium
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddUserProfilePreference("plugins.always_open_pdf_externally", true);
            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            var driver = new ChromeDriver(chromeDriverService, chromeOptions);

            driver.Navigate().GoToUrl($"file://{Path.GetFullPath(htmlDestinationFileName)}");

            // Simulate print to PDF
            var js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("setTimeout(function() { window.print(); }, 0);");

            // Note: Selenium can't directly handle print dialog. You might need to manually save as PDF.
            // For automation, consider using other libraries like Wkhtmltopdf or Puppeteer Sharp.

            driver.Quit();


            //// Convert Markdown to PDF using Markdown2Pdf
            //var converter = new Markdown2PdfConverter();
            //string resultPath = await converter.Convert(destinationFileName);
            //Console.WriteLine($"PDF saved to {resultPath}");

            //// Convert Markdown to PDF using Markdown2Pdf
            //var converter = new Markdown2PdfConverter();
            //await converter.Convert(destinationFileName, pdfDestinationFileName);
            //Console.WriteLine($"PDF saved to {pdfDestinationFileName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }


        //var bucket = storageClient.GetBucket(bucketName);

        //foreach (var obj in storageClient.ListObjects(bucketName))
        //{
        //    Console.WriteLine(obj.Name);
        //}


        // Path to Markdown file
        //string sourcePath = @"C:\env\markdown.md";

        // Read Markdown file content
        //string markdownContent = File.ReadAllText(sourcePath);

        // Remove '#' symbols
        //markdownContent = markdownContent.Replace("#", "");

        // Instantiate Renderer
        //ChromePdfRenderer renderer = new ChromePdfRenderer();

        // Render Markdown string to PDF
        //PdfDocument pdf = renderer.RenderMarkdownStringAsPdf(markdownContent);

        // Save the PDF
        //pdf.SaveAs("output.pdf");
    }
}
