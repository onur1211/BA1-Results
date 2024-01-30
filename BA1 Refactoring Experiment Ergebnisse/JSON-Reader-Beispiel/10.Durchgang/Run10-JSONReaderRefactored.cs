public class CustomJSONReader
{
    public void UseReader(string json)
    {
        JsonDocument document = JsonDocument.Parse(json);

        Console.WriteLine("JSON File content:");

        foreach (var element in document.RootElement.EnumerateObject())
        {
            if (element.NameEquals("title") && element.Value.ValueKind == JsonValueKind.String)
            {
                string title = element.Value.GetString();

                if (string.IsNullOrEmpty(title))
                {
                    throw new ArgumentNullException(nameof(title), "Title cannot be null or empty.");
                }
                else
                {
                    Console.Write($"Title: {title}, ");
                }
            }

            if (element.NameEquals("price") && element.Value.ValueKind == JsonValueKind.Number)
            {
                decimal price = element.Value.GetDecimal();
                Console.WriteLine($"Price: {price:C}");
            }
        }

        Console.ReadKey();
    }
}