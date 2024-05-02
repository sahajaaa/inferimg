using SkiaSharp;


namespace InferImages
{
    internal class Renderer
    {
        public async ValueTask ExecuteAsync(List<DataObject> data)
        {
            foreach (var row in data)
            {
                try
                {
                    if (row.ImageStream != null)
                    {
                        var boundedBoxImg = await DrawBoundingBox(row.ImageStream,
                                                    row.CoordinatesXmin, row.CoordinatesYmin, row.CoordinatesXmax, row.CoordinatesYmax);

                        if (boundedBoxImg != null)
                        {
                            string cameraDirectory = Path.Combine($"C:\\Users\\sahaja\\Desktop\\InferImages\\InferImages\\Images\\Apr19\\{row.CameraSerial}");
                            Directory.CreateDirectory(cameraDirectory);

                            string imagePath = Path.Combine(cameraDirectory, $"{row.CameraSerial}_{row.Timestamp}_{row.Class}_{row.Confidence}.jpg");

                            using (var imageStream = new FileStream(imagePath, FileMode.Create))
                            {
                                boundedBoxImg.Encode(SKEncodedImageFormat.Jpeg, 100).SaveTo(imageStream);
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        internal async ValueTask<SKBitmap?> DrawBoundingBox(Stream image, float CoordinatesXmin, float CoordinatesYmin, float CoordinatesXmax, float CoordinatesYmax) =>

            await Task.Run(() =>
            {
                SKBitmap? outputImage = null;

                // Create a new image for drawing
                using var originalImage = SKBitmap.Decode(image);
                outputImage = new SKBitmap(originalImage.Width, originalImage.Height);

                // Draw the original image onto the canvas
                using var canvas = new SKCanvas(outputImage);
                canvas.DrawBitmap(originalImage, new SKPoint(0, 0));

                    // Convert the normalized coordinates to absolute pixel coordinates
                    // location values contain normalized coordinates of the bounding box. it follows [xmax, xmin, ymin, ymax] format
                    var absXmin = (int)(CoordinatesXmin * originalImage.Width);
                    var absXmax = (int)(CoordinatesXmax * originalImage.Width);
                    var absYmin = (int)(CoordinatesYmin * originalImage.Height);
                    var absYmax = (int)(CoordinatesYmax * originalImage.Height);

                    using var paint = new SKPaint();
                    paint.Style = SKPaintStyle.Stroke;
                    paint.Color = SKColors.Red;
                    paint.StrokeWidth = 2;

                    // Create the rectangle
                    var rect = new SKRect(absXmin, absYmin, absXmax, absYmax);

                    // Draw the rectangle on the canvas
                    canvas.DrawRect(rect, paint);

              
                return outputImage;
            });
    }
}
