public class CustomJSONReader
{
    public void UseReader(string json)
    {
        var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        Console.WriteLine("JSON File content:");

        foreach (var property in root.EnumerateObject())
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

        Console.ReadKey();
    }

    private void ProcessTitle(JsonElement titleElement)
    {
        if (titleElement.ValueKind == JsonValueKind.String)
        {
            string title = titleElement.GetString();
            ValidateAndPrintTitle(title);
        }
    }

    private void ValidateAndPrintTitle(string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            throw new ArgumentNullException(nameof(title), "Title cannot be null or empty.");
        }
        else
        {
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