# Create tessdata directory if it doesn't exist
$tessdataPath = Join-Path $PSScriptRoot "tessdata"
if (-not (Test-Path $tessdataPath)) {
    New-Item -ItemType Directory -Path $tessdataPath | Out-Null
}

# Download required Tesseract data files
$files = @(
    @{
        Name = "eng.traineddata"
        Url = "https://github.com/tesseract-ocr/tessdata/raw/main/eng.traineddata"
    },
    @{
        Name = "osd.traineddata"
        Url = "https://github.com/tesseract-ocr/tessdata/raw/main/osd.traineddata"
    }
)

foreach ($file in $files) {
    $outputPath = Join-Path $tessdataPath $file.Name
    Write-Host "Downloading $($file.Name)..."
    Invoke-WebRequest -Uri $file.Url -OutFile $outputPath
    Write-Host "Downloaded $($file.Name) to $outputPath"
}

Write-Host "`nTesseract data files have been downloaded successfully!"
Write-Host "You can now run the application." 