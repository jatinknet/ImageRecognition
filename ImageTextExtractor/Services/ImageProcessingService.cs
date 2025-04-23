using System;
using System.IO;
using System.Threading.Tasks;
using Tesseract;
using Microsoft.AspNetCore.Components.Forms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ImageTextExtractor.Services
{
    public class ImageProcessingService
    {
        private readonly string _tessdataPath;
        private const int MaxImageDimension = 2000; // Maximum dimension for processing
        private const string RequiredDataFile = "eng.traineddata";

        public ImageProcessingService()
        {
            // Look for tessdata in the application's root directory first
            var rootPath = AppDomain.CurrentDomain.BaseDirectory;
            _tessdataPath = Path.Combine(rootPath, "tessdata");
            
            // If not found in root, try the project directory
            if (!Directory.Exists(_tessdataPath) || !File.Exists(Path.Combine(_tessdataPath, RequiredDataFile)))
            {
                var projectPath = Directory.GetCurrentDirectory();
                _tessdataPath = Path.Combine(projectPath, "tessdata");
            }

            InitializeTessData();
        }

        private void InitializeTessData()
        {
            if (!Directory.Exists(_tessdataPath))
            {
                Directory.CreateDirectory(_tessdataPath);
            }

            var dataFilePath = Path.Combine(_tessdataPath, RequiredDataFile);
            if (!File.Exists(dataFilePath))
            {
                throw new FileNotFoundException(
                    $"Tesseract data file '{RequiredDataFile}' not found in {_tessdataPath}. " +
                    "Please run the download-tessdata.ps1 script to download the required data files.");
            }
        }

        public async Task<(string Text, string Status)> ExtractTextFromImage(IBrowserFile file, IProgress<(int Percentage, string Status)> progress)
        {
            try
            {
                // Verify tessdata exists before processing
                var dataFilePath = Path.Combine(_tessdataPath, RequiredDataFile);
                if (!File.Exists(dataFilePath))
                {
                    throw new FileNotFoundException(
                        $"Tesseract data file '{RequiredDataFile}' not found. " +
                        "Please run the download-tessdata.ps1 script to download the required data files.");
                }

                // Report start of file reading
                progress.Report((10, "Reading file..."));
                using var memoryStream = new MemoryStream();
                await file.OpenReadStream().CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                // Report start of image optimization
                progress.Report((30, "Optimizing image..."));
                using var optimizedStream = await OptimizeImage(memoryStream);
                optimizedStream.Position = 0;

                // Report start of OCR processing
                progress.Report((50, "Initializing OCR engine..."));
                using var engine = new TesseractEngine(_tessdataPath, "eng", EngineMode.Default);

                progress.Report((60, "Loading image..."));
                using var img = Pix.LoadFromMemory(optimizedStream.ToArray());

                progress.Report((70, "Performing text recognition..."));
                using var page = engine.Process(img);
                
                var text = page.GetText();
                progress.Report((100, "Complete!"));
                
                return (text, "Text extraction completed successfully.");
            }
            catch (Exception ex)
            {
                progress.Report((0, $"Error: {ex.Message}"));
                throw new Exception($"Error processing image: {ex.Message}", ex);
            }
        }

        private async Task<MemoryStream> OptimizeImage(MemoryStream inputStream)
        {
            using var image = await Image.LoadAsync(inputStream);
            
            // Resize if image is too large
            if (image.Width > MaxImageDimension || image.Height > MaxImageDimension)
            {
                var ratio = Math.Min((float)MaxImageDimension / image.Width, (float)MaxImageDimension / image.Height);
                var newWidth = (int)(image.Width * ratio);
                var newHeight = (int)(image.Height * ratio);

                image.Mutate(x => x.Resize(newWidth, newHeight));
            }

            var outputStream = new MemoryStream();
            await image.SaveAsJpegAsync(outputStream);
            return outputStream;
        }
    }
} 