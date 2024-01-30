public class CustomJSONReader
{
    public void UseReader(string json)
    {
        var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        Console.WriteLine("JSON File content:");

        foreach (var property in root.EnumerateObject())
        {
            if (property.Name == "title" && property.Value.ValueKind == JsonValueKind.String)
            {
                var title = property.Value.GetString();

                if (string.IsNullOrEmpty(title))
                {
                    throw new ArgumentNullException(nameof(title), "Title cannot be null or empty.");
                }

                Console.Write($"Title: {title}, ");
            }

            if (property.Name == "price" && property.Value.ValueKind == JsonValueKind.Number)
            {
                var price = property.Value.GetDecimal();
                Console.WriteLine($"Price: {price:C}");
            }
        }

        Console.ReadKey();
    }
}