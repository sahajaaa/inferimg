using System;
using System.Net.Http;
using System.Threading.Tasks;
using InferImages;
using Microsoft.Extensions.DependencyInjection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace YourNamespace
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Create a service collection
            var services = new ServiceCollection();

            services.AddHttpClient();

            var serviceProvider = services.BuildServiceProvider();

            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

            MerakiAPIProxy proxy = new MerakiAPIProxy(httpClientFactory);

            ImageRetriever retriever = new ImageRetriever(proxy);

            var dataObjAfterImg = await retriever.GetImage();
            
            Renderer renderer = new Renderer();

            await renderer.ExecuteAsync(dataObjAfterImg);
        }
    }
}

public class DataObject
{
    public long Timestamp { get; set; }
    public float CoordinatesYmin { get; set; }
    public float CoordinatesXmin { get; set; }
    public float CoordinatesYmax { get; set; }
    public float CoordinatesXmax { get; set; }
    public string CameraSerial { get; set; }
    public int Class { get; set; }
    public float Confidence { get; set; }
    public Stream? ImageStream { get; set; }
}


/*
 * 1. Extract the data from excel log sheet (We require Timestamp, Coordinated rounded upto 2, Cameraserial, Class)
 * 2. Place a post url http request with camera serial, timestamp
 * 3. Using the get url send get request again and save the stream
 * 4. Draw the rendering boxes 
 * 5. Store them daywise & camerawise seperately in a folder
 */