public class CustomJSONReader
{
    public void UseReader(string json)
    {
        var document = JsonDocument.Parse(json);
        ProcessJsonDocument(document);
    }

    private void ProcessJsonDocument(JsonDocument document)
    {
        Console.WriteLine("JSON File content:");

        foreach (var element in document.RootElement.EnumerateObject())
        {
            if (element.NameEquals("title"))
            {
                ProcessTitle(element);
            }
            else if (element.NameEquals("price"))
            {
                ProcessPrice(element);
            }
        }

        Console.ReadKey();
    }

    private void ProcessTitle(JsonProperty titleProperty)
    {
        if (titleProperty.Value.ValueKind == JsonValueKind.String)
        {
            var title = titleProperty.Value.GetString();

            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException("Title cannot be null or empty.");
            }

            Console.WriteLine($"Title: {title}");
        }
    }

    private void ProcessPrice(JsonProperty priceProperty)
    {
        if (priceProperty.Value.ValueKind == JsonValueKind.Number && priceProperty.Value.TryGetDecimal(out decimal price))
        {
            Console.WriteLine($"Price: {price:C}");
        }
    }
}