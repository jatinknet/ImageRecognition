let video = null;
let canvas = null;
let stream = null;

window.startWebcam = async () => {
    try {
        video = document.getElementById('webcam');
        canvas = document.getElementById('canvas');
        
        if (!video || !canvas) {
            throw new Error('Webcam elements not found');
        }

        // Request higher quality video
        stream = await navigator.mediaDevices.getUserMedia({ 
            video: { 
                width: { ideal: 1920 },
                height: { ideal: 1080 },
                facingMode: 'environment', // Prefer rear camera if available
                frameRate: { ideal: 30 }
            } 
        });
        
        video.srcObject = stream;
        video.style.display = 'block';
        await video.play();

        // Wait for video to be ready
        await new Promise((resolve) => {
            video.onloadedmetadata = () => {
                resolve();
            };
        });
    } catch (err) {
        console.error('Error accessing webcam:', err);
        throw new Error('Error accessing webcam: ' + err.message);
    }
};

window.captureImage = () => {
    if (!video || !canvas) {
        throw new Error('Webcam elements not found');
    }

    if (!stream) {
        throw new Error('Webcam not started');
    }

    try {
        // Set canvas dimensions to match video
        const aspectRatio = video.videoWidth / video.videoHeight;
        const maxWidth = 1920;
        const maxHeight = 1080;
        
        let width = video.videoWidth;
        let height = video.videoHeight;

        // Scale down if too large while maintaining aspect ratio
        if (width > maxWidth) {
            width = maxWidth;
            height = width / aspectRatio;
        }
        if (height > maxHeight) {
            height = maxHeight;
            width = height * aspectRatio;
        }

        canvas.width = width;
        canvas.height = height;
        
        // Draw the current video frame to canvas with better quality
        const context = canvas.getContext('2d', { alpha: false });
        context.imageSmoothingEnabled = true;
        context.imageSmoothingQuality = 'high';
        context.drawImage(video, 0, 0, width, height);

        // Convert canvas to blob with high quality
        return new Promise((resolve, reject) => {
            canvas.toBlob((blob) => {
                if (!blob) {
                    reject(new Error('Failed to capture image'));
                    return;
                }

                // Create a File object from the blob with high quality
                const file = new File([blob], 'captured_image.jpg', { 
                    type: 'image/jpeg',
                    lastModified: new Date().getTime()
                });
                
                // Create a new FileList-like object
                const dataTransfer = new DataTransfer();
                dataTransfer.items.add(file);

                // Find the file input and update its files
                const input = document.querySelector('input[type="file"]');
                if (!input) {
                    reject(new Error('File input not found'));
                    return;
                }

                input.files = dataTransfer.files;
                
                // Dispatch change event
                const event = new Event('change', { bubbles: true });
                input.dispatchEvent(event);
                
                resolve();
            }, 'image/jpeg', 0.95); // High quality JPEG
        });
    } catch (err) {
        console.error('Error capturing image:', err);
        throw new Error('Error capturing image: ' + err.message);
    }
};

window.stopWebcam = () => {
    if (stream) {
        stream.getTracks().forEach(track => track.stop());
        stream = null;
    }
    if (video) {
        video.srcObject = null;
        video.style.display = 'none';
    }
};

window.clearFileInput = () => {
    const input = document.querySelector('input[type="file"]');
    if (input) {
        input.value = '';
        // Create and dispatch a change event to trigger any listeners
        const event = new Event('change', { bubbles: true });
        input.dispatchEvent(event);
    }
};

window.downloadText = (filename, base64Content) => {
    const link = document.createElement('a');
    link.href = 'data:text/plain;base64,' + base64Content;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}; 