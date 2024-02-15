public class CustomJSONReader
{
    public void UseReader(string json)
    {
        using (JsonDocument document = JsonDocument.Parse(json))
        {
            Console.WriteLine("JSON File content:");

            foreach (JsonProperty property in document.RootElement.EnumerateObject())
            {
                if (property.NameEquals("title"))
                {
                    string title = property.Value.GetString();

                    if (string.IsNullOrEmpty(title))
                    {
                        throw new ArgumentNullException();
                    }

                    Console.Write($"Title: {title}, ");
                }

                if (property.NameEquals("price"))
                {
                    decimal price = property.Value.GetDecimal();
                    Console.WriteLine($"Price: {price:C}");
                }
            }
        }

        Console.ReadKey();
    }
}