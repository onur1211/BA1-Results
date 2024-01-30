public class CustomJSONReader
{
    public void UseReader(string json)
    {
        bool isNextPrice = false;
        bool isNextTitle = false;
        string? title = default;
        byte[] data = Encoding.UTF8.GetBytes(json);
        Utf8JsonReader reader = new(data);

        Console.WriteLine("JSON File content:");

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "title")
            {
                isNextTitle = true;
            }

            if (reader.TokenType == JsonTokenType.String && isNextTitle)
            {
                title = reader.GetString();

                if (string.IsNullOrEmpty(title))
                {
                    throw new ArgumentNullException();
                }
                else
                {
                    isNextTitle = false;
                }
            }

            if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "price")
            {
                isNextPrice = true;
            }

            if (reader.TokenType == JsonTokenType.Number && isNextPrice &&
            reader.TryGetDecimal(out decimal price))
            {
                Console.WriteLine($"{title}, price: {price:C}");
                isNextPrice = false;
            }
        }

        Console.ReadKey();
    }
}