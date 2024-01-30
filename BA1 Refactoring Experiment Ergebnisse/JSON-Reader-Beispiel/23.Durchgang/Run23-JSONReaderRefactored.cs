public class CustomJSONReader
{
    public void UseReader(string json)
    {
        JsonDocument jsonDocument = JsonDocument.Parse(json);

        Console.WriteLine("JSON File content:");

        foreach (JsonElement element in jsonDocument.RootElement.EnumerateObject())
        {
            if (element.NameEquals("title"))
            {
                string title = element.Value.GetString();

                if (string.IsNullOrEmpty(title))
                {
                    throw new ArgumentNullException(nameof(title), "Title cannot be null or empty.");
                }

                Console.Write($"{title}, ");
            }
            else if (element.NameEquals("price"))
            {
                if (element.Value.TryGetDecimal(out decimal price))
                {
                    Console.WriteLine($"price: {price:C}");
                }
                else
                {
                    throw new InvalidOperationException("Unable to parse price.");
                }
            }
        }

        Console.ReadKey();
    }
}