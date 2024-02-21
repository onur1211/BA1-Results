public class CustomJSONReader
{
    public void UseReader(string json)
    {
        JsonDocument doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Console.WriteLine("JSON File content:");

        foreach (var property in root.EnumerateObject())
        {
            if (property.Name == "title" && property.Value.ValueKind == JsonValueKind.String)
            {
                string title = property.Value.GetString();
                ProcessTitle(title);
            }

            if (property.Name == "price" && property.Value.ValueKind == JsonValueKind.Number)
            {
                decimal price = property.Value.GetDecimal();
                ProcessPrice(price);
            }
        }

        Console.ReadKey();
    }

    private void ProcessTitle(string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            throw new ArgumentNullException();
        }
    }

    private void ProcessPrice(decimal price)
    {
        Console.WriteLine($"Title: {title}, Price: {price:C}");
    }
}