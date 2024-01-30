public class CustomJSONReader
{
    public void UseReader(string json)
    {
        JsonDocument jsonDocument = JsonDocument.Parse(json);
        Console.WriteLine("JSON File content:");

        foreach (var element in jsonDocument.RootElement.EnumerateObject())
        {
            ProcessProperty(element);
        }

        Console.ReadKey();
    }

    private void ProcessProperty(JsonProperty property)
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