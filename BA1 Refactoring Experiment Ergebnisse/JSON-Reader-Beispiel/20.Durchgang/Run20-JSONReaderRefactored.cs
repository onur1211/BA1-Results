public class CustomJSONReader
{
    public void UseReader(string json)
    {
        var data = Encoding.UTF8.GetBytes(json);

        using (var reader = new Utf8JsonReader(data, new JsonReaderOptions { AllowTrailingCommas = true }))
        {
            Console.WriteLine("JSON File content:");

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    ProcessJsonObject(reader);
                }
            }
        }

        Console.ReadKey();
    }

    private void ProcessJsonObject(Utf8JsonReader reader)
    {
        string title = string.Empty;
        decimal price = 0;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                if (!string.IsNullOrEmpty(title))
                {
                    Console.WriteLine($"{title}, price: {price:C}");
                }
                break;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString();

                if (propertyName == "title")
                {
                    reader.Read();
                    title = reader.GetString();
                }
                else if (propertyName == "price")
                {
                    reader.Read();
                    if (reader.TryGetDecimal(out price))
                    {
                        // Optional: Add validation for price if needed
                    }
                }
            }
        }
    }
}