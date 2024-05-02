
using System.Text.Json;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace InferImages
{
    internal class ImageRetriever(MerakiAPIProxy proxy)
    {
        public async Task<List<DataObject>> GetImage()
        {
            var data = DataExtraction.Getdata();

            foreach (var row in data)
            {
                var payload = new Dictionary<string, string> {{ "timestamp", DateTimeOffset.FromUnixTimeMilliseconds(row.Timestamp).UtcDateTime.ToString("o") }, { "fullframe", "false"}};

                var result = await proxy.GetImageUrl(payload, row.CameraSerial);

                if (result == "Error retrieving url") continue;

                var jsonDocument = JsonDocument.Parse(result);

                if (!jsonDocument.RootElement.TryGetProperty("url", out var urlElement))
                {
                    Console.WriteLine("No Url");
                    continue;
                };

                var urlValue = urlElement.GetString();

                if (urlValue == null) row.ImageStream = null;

                var imageStream = await proxy.GetImage(urlValue!, row.CameraSerial);

                row.ImageStream = imageStream;
            }

            return data;
        }
    }

}
