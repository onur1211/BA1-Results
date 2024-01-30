public class CustomJSONReader
{
    public void UseReader(string json)
    {
        JsonDocument document = JsonDocument.Parse(json);
        ProcessJsonDocument(document);
    }

    private void ProcessJsonDocument(JsonDocument document)
    {
        Console.WriteLine("JSON File content:");

        foreach (JsonProperty property in document.RootElement.EnumerateObject())
        {
            if (property.NameEquals("title"))
            {
                ProcessTitle(property.Value);
            }
            else if (property.NameEquals("price"))
            {
                ProcessPrice(property.Value);
            }
        }
    }

    private void ProcessTitle(JsonElement titleElement)
    {
        if (titleElement.ValueKind == JsonValueKind.String)
        {
            string title = titleElement.GetString();

            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException();
            }

            Console.WriteLine($"Title: {title}");
        }
    }

    private void ProcessPrice(JsonElement priceElement)
    {
        if (priceElement.ValueKind == JsonValueKind.Number &&
            priceElement.TryGetDecimal(out decimal price))
        {
            Console.WriteLine($"Price: {price:C}");
        }
    }
}