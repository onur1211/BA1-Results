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
                    ValidateAndPrintTitle(title);
                }

                if (element.NameEquals("price") && element.Value.ValueKind == JsonValueKind.Number)
                {
                    decimal price = element.Value.GetDecimal();
                    Console.WriteLine($"Title: {title}, Price: {price:C}");
                }
            }
        }

        Console.ReadKey();
    }

    private static void ValidateAndPrintTitle(string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            throw new ArgumentNullException(nameof(title), "Title cannot be null or empty.");
        }

        Console.WriteLine($"Title: {title}");
    }
}