# Image Text Extractor

A Blazor Server application that extracts text from images using OCR (Optical Character Recognition) technology. The application provides a user-friendly interface for uploading images or capturing them through a webcam, and then extracts text from these images.

## Features

- **Image Upload**: Upload images from your device
- **Webcam Capture**: Capture images directly from your webcam
- **Text Extraction**: Extract text from images using Tesseract OCR
- **Text Download**: Download extracted text as a .txt file
- **Progress Tracking**: Real-time progress indication during processing
- **Responsive Design**: Clean and modern UI using Bootstrap 5
- **Error Handling**: Comprehensive error handling and user feedback

## Prerequisites

- .NET 8.0 SDK
- Visual Studio 2022 or Visual Studio Code
- Webcam (for webcam capture feature)

## Setup Instructions

1. Clone the repository:
   ```bash
   git clone [repository-url]
   cd ImageTextExtractor
   ```

2. Install required NuGet packages:
   ```bash
   dotnet restore
   ```

3. Download Tesseract data files:
   - Run the `download-tessdata.ps1` script to download required Tesseract data files
   - This will create a `tessdata` directory with necessary files

4. Build and run the application:
   ```bash
   dotnet build
   dotnet run
   ```

5. Open your browser and navigate to:
   ```
   https://localhost:5001
   ```

## Usage Guide

### Uploading Images
1. Click the "Choose File" button
2. Select an image from your device
3. Wait for the text extraction process to complete
4. View the extracted text in the right panel
5. Use the "Download Text" button to save the extracted text

### Using Webcam
1. Click "Start Webcam" to initialize your webcam
2. Position the text you want to capture
3. Click "Capture Image" to take a photo
4. Wait for the text extraction process to complete
5. View and download the extracted text

### Clearing Selection
- Use the clear button (X) to reset the current selection
- This will clear both the image and extracted text

## Technical Details

### Technologies Used
- Blazor Server (.NET 8)
- Tesseract OCR
- Bootstrap 5
- SixLabors.ImageSharp

### Project Structure
```
ImageTextExtractor/
├── Pages/
│   ├── Index.razor
│   └── _Host.cshtml
├── Services/
│   └── ImageProcessingService.cs
├── wwwroot/
│   ├── css/
│   └── js/
│       └── app.js
└── tessdata/
    ├── eng.traineddata
    └── osd.traineddata
```

## Troubleshooting

### Common Issues
1. **Tesseract Data Files Missing**
   - Run the `download-tessdata.ps1` script
   - Ensure files are in the `tessdata` directory

2. **Webcam Not Working**
   - Check browser permissions
   - Ensure webcam is properly connected
   - Try refreshing the page

3. **Text Extraction Issues**
   - Ensure image is clear and well-lit
   - Try adjusting image quality
   - Check if text is readable in the image

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Tesseract OCR for text recognition
- Bootstrap for UI components
- SixLabors.ImageSharp for image processing 
