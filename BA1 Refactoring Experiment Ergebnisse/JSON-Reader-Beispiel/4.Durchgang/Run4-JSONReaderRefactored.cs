public class CustomJSONReader
{
    public void UseReader(string json)
    {
        using (JsonDocument document = JsonDocument.Parse(json))
        {
            Console.WriteLine("JSON File content:");

            foreach (JsonElement element in document.RootElement.EnumerateObject())
            {
                if (element.NameEquals("title") && element.Value.ValueKind == JsonValueKind.String)
                {
                    string title = element.Value.GetString();
                    ProcessTitle(title);
                }

                if (element.NameEquals("price") && element.Value.ValueKind == JsonValueKind.Number)
                {
                    decimal price = element.Value.GetDecimal();
                    ProcessPrice(price);
                }
            }
        }

        Console.ReadKey();
    }

    private void ProcessTitle(string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            throw new ArgumentNullException(nameof(title));
        }

        // Additional processing for title if needed
    }

    private void ProcessPrice(decimal price)
    {
        // Additional processing for price if needed
        Console.WriteLine($"Price: {price:C}");
    }
}