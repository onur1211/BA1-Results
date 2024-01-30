public class CustomJSONReader
{
    public void UseReader(string json)
    {
        using (JsonDocument document = JsonDocument.Parse(json))
        {
            Console.WriteLine("JSON File content:");

            foreach (JsonProperty property in document.RootElement.EnumerateObject())
            {
                if (property.NameEquals("title") && property.Value.ValueKind == JsonValueKind.String)
                {
                    string title = property.Value.GetString();

                    if (string.IsNullOrEmpty(title))
                    {
                        throw new ArgumentNullException(nameof(title), "Title cannot be null or empty.");
                    }

                    Console.WriteLine($"Title: {title}");
                }

                if (property.NameEquals("price") && property.Value.ValueKind == JsonValueKind.Number)
                {
                    decimal price = property.Value.GetDecimal();
                    Console.WriteLine($"Price: {price:C}");
                }
            }
        }

        Console.ReadKey();
    }
}