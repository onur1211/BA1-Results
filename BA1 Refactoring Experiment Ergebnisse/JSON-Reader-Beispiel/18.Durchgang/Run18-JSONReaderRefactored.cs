public class JsonData
{
    public string Title { get; set; }
    public decimal Price { get; set; }
}

public class CustomJSONReader
{
    public static void UseReader(string json)
    {
        List<JsonData> jsonDataList = ParseJson(json);

        Console.WriteLine("JSON File content:");

        foreach (var jsonData in jsonDataList)
        {
            Console.WriteLine($"{jsonData.Title}, price: {jsonData.Price:C}");
        }

        Console.ReadKey();
    }

    private static List<JsonData> ParseJson(string json)
    {
        List<JsonData> jsonDataList = new List<JsonData>();
        using (JsonDocument document = JsonDocument.Parse(json))
        {
            foreach (JsonElement element in document.RootElement.EnumerateObject())
            {
                JsonData jsonData = new JsonData();

                foreach (JsonProperty property in element.EnumerateObject())
                {
                    if (property.NameEquals("title"))
                    {
                        jsonData.Title = property.Value.GetString() ?? throw new ArgumentNullException("Title cannot be null");
                    }
                    else if (property.NameEquals("price"))
                    {
                        if (property.Value.TryGetDecimal(out decimal price))
                        {
                            jsonData.Price = price;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid price format");
                        }
                    }
                }

                if (string.IsNullOrEmpty(jsonData.Title))
                {
                    throw new ArgumentNullException("Title cannot be null or empty");
                }

                jsonDataList.Add(jsonData);
            }
        }

        return jsonDataList;
    }
}