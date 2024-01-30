public class Product
{
    public string Title { get; set; }
    public decimal Price { get; set; }
}

public class CustomJSONReader
{
    public void UseReader(string json)
    {
        Console.WriteLine("JSON File content:");

        using (JsonDocument document = JsonDocument.Parse(json))
        {
            ProcessJSON(document.RootElement);
        }

        Console.ReadKey();
    }

    private void ProcessJSON(JsonElement element)
    {
        string title = null;
        decimal price = 0;
        bool isNextTitle = false;
        bool isNextPrice = false;

        foreach (var property in element.EnumerateObject())
        {
            if (property.NameEquals("title"))
            {
                isNextTitle = true;
            }

            if (property.NameEquals("price"))
            {
                isNextPrice = true;
            }

            if (isNextTitle && property.Value.ValueKind == JsonValueKind.String)
            {
                title = property.Value.GetString();
                isNextTitle = false;

                if (string.IsNullOrEmpty(title))
                {
                    throw new ArgumentNullException(nameof(title), "Title cannot be null or empty.");
                }
            }

            if (isNextPrice && property.Value.ValueKind == JsonValueKind.Number)
            {
                price = property.Value.GetDecimal();
                isNextPrice = false;

                Console.WriteLine($"{title}, price: {price:C}");
            }

            if (property.Value.ValueKind == JsonValueKind.Object)
            {
                // Recursive call for nested objects
                ProcessJSON(property.Value);
            }
        }
    }
}