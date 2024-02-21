public class CustomJSONReader
{
    public void UseReader(string json)
    {
        var jsonDocument = JsonDocument.Parse(json);

        Console.WriteLine("JSON File content:");

        foreach (var element in jsonDocument.RootElement.EnumerateObject())
        {
            if (element.NameEquals("title") && element.Value.ValueKind == JsonValueKind.String)
            {
                var title = element.Value.GetString();

                if (string.IsNullOrEmpty(title))
                {
                    throw new ArgumentNullException(nameof(title), "Title cannot be null or empty.");
                }

                Console.WriteLine($"Title: {title}");
            }
            else if (element.NameEquals("price") && element.Value.ValueKind == JsonValueKind.Number)
            {
                var price = element.Value.GetDecimal();
                Console.WriteLine($"Price: {price:C}");
            }
        }

        Console.ReadKey();
    }
}