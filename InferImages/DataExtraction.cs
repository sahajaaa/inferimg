

namespace InferImages
{
    public class DataExtraction
    {
        public static List<DataObject> Getdata()
        {
            string filePath = "C:\\Users\\sahaja\\Desktop\\InferImages\\InferImages\\Excels\\Apr19.csv";

            string[] lines = File.ReadAllLines(filePath);

            List<DataObject> dataObjects = new List<DataObject>();

            foreach (string line in lines)
            {
                string[] values = line.Split(',');

                if (values.Length == 15)
                {
                    DataObject dataObject = new DataObject
                    {
                        Timestamp = long.Parse(values[8]), 
                        CoordinatesXmin = (float)Math.Round(float.Parse(values[4]), 3),
                        CoordinatesXmax = (float)Math.Round(float.Parse(values[5]), 3),
                        CoordinatesYmin = (float)Math.Round(float.Parse(values[6]), 3),
                        CoordinatesYmax = (float)Math.Round(float.Parse(values[7]), 3),
                        CameraSerial = values[0],
                        Class = int.Parse(values[1]),
                        Confidence = (float)Math.Round(float.Parse(values[3]), 2)
                    };

                    dataObjects.Add(dataObject);
                }
            }
            
            /*foreach (DataObject dataObject in dataObjects)
            {
                Console.WriteLine($"Timestamp: {dataObject.Timestamp}, CoordinatesYmin: {dataObject.CoordinatesYmin}, CoordinatesXmin: {dataObject.CoordinatesXmin}, CoordinatesYmax: {dataObject.CoordinatesYmax}, CoordinatesXmax: {dataObject.CoordinatesXmax}, CameraSerial: {dataObject.CameraSerial}, Class: {dataObject.Class}, Confidence: {dataObject.Confidence}");
            }*/

            return dataObjects;
        }
    }
}
