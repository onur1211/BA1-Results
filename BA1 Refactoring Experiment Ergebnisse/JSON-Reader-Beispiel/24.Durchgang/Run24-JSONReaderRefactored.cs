using System;
using System.Text;
using System.Text.Json;

public class CustomJSONReader
{
    public void UseReader(string json)
    {
        try
        {
            JsonParser jsonParser = new JsonParser(json);

            Console.WriteLine("JSON File content:");

            while (jsonParser.Read())
            {
                ProcessTitle(jsonParser);
                ProcessPrice(jsonParser);
            }

            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private void ProcessTitle(JsonParser jsonParser)
    {
        if (jsonParser.IsNextProperty("title"))
        {
            string title = jsonParser.GetString();

            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentException("Title cannot be null or empty.");
            }

            Console.WriteLine($"Title: {title}");
        }
    }

    private void ProcessPrice(JsonParser jsonParser)
    {
        if (jsonParser.IsNextProperty("price"))
        {
            decimal price = jsonParser.GetDecimal();
            Console.WriteLine($"Price: {price:C}");
        }
    }
}

public class JsonParser
{
    private readonly Utf8JsonReader reader;

    public JsonParser(string json)
    {
        byte[] data = Encoding.UTF8.GetBytes(json);
        reader = new Utf8JsonReader(data);
    }

    public bool Read()
    {
        return reader.Read();
    }

    public bool IsNextProperty(string propertyName)
    {
        return reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == propertyName;
    }

    public string GetString()
    {
        return reader.GetString();
    }

    public decimal GetDecimal()
    {
        if (reader.TryGetDecimal(out decimal value))
        {
            return value;
        }
        else
        {
            throw new ArgumentException("Invalid decimal value.");
        }
    }
}