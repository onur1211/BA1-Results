public class CustomJSONReader
{
    public void UseReader(string json)
    {
        JsonDocument jsonDocument = JsonDocument.Parse(json);

        Console.WriteLine("JSON File content:");

        foreach (var element in jsonDocument.RootElement.EnumerateObject())
        {
            if (element.NameEquals("title"))
            {
                HandleTitle(element.Value.GetString());
            }
            else if (element.NameEquals("price"))
            {
                HandlePrice(element.Value.GetDecimal());
            }
        }

        Console.ReadKey();
    }

    private void HandleTitle(string? title)
    {
        if (string.IsNullOrEmpty(title))
        {
            throw new ArgumentNullException(nameof(title));
        }

        // Handle title as needed (e.g., store it for later use)
    }

    private void HandlePrice(decimal price)
    {
        // Handle price as needed (e.g., display it)
        Console.WriteLine($"Price: {price:C}");
    }
}