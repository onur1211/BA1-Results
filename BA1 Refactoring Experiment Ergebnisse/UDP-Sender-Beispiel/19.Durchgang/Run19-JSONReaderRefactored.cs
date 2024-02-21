using System;
using System.Text;
using System.Text.Json;

public class CustomJSONReader
{
    public void UseReader(string json)
    {
        var jsonParser = new JsonParser(json);
        jsonParser.Parse();
    }
}

public class JsonParser
{
    private readonly Utf8JsonReader reader;
    private bool isNextTitle;
    private bool isNextPrice;
    private string? title;

    public JsonParser(string json)
    {
        byte[] data = Encoding.UTF8.GetBytes(json);
        reader = new Utf8JsonReader(data);
    }

    public void Parse()
    {
        Console.WriteLine("JSON File content:");

        while (reader.Read())
        {
            ProcessToken();
        }

        Console.ReadKey();
    }

    private void ProcessToken()
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.PropertyName:
                ProcessPropertyName();
                break;
            case JsonTokenType.String:
                ProcessString();
                break;
            case JsonTokenType.Number:
                ProcessNumber();
                break;
        }
    }

    private void ProcessPropertyName()
    {
        if (reader.GetString() == "title")
        {
            isNextTitle = true;
        }

        if (reader.GetString() == "price")
        {
            isNextPrice = true;
        }
    }

    private void ProcessString()
    {
        if (isNextTitle)
        {
            title = reader.GetString();

            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException(nameof(title));
            }
            else
            {
                isNextTitle = false;
            }
        }
    }

    private void ProcessNumber()
    {
        if (isNextPrice && reader.TryGetDecimal(out decimal price))
        {
            Console.WriteLine($"{title}, price: {price:C}");
            isNextPrice = false;
        }
    }
}