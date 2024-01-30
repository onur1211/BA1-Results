public class Product
{
    public string Title { get; set; }
    public decimal Price { get; set; }
}

public class CustomJSONReader
{
    public List<Product> ParseProducts(string json)
    {
        List<Product> products = new List<Product>();
        byte[] data = Encoding.UTF8.GetBytes(json);
        Utf8JsonReader reader = new Utf8JsonReader(data);

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                Product product = ParseProduct(ref reader);
                products.Add(product);
            }
        }

        return products;
    }

    private Product ParseProduct(ref Utf8JsonReader reader)
    {
        Product product = new Product();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case "title":
                        product.Title = reader.GetString();
                        break;
                    case "price":
                        if (reader.TryGetDecimal(out decimal price))
                        {
                            product.Price = price;
                        }
                        break;
                }
            }
        }

        if (string.IsNullOrEmpty(product.Title))
        {
            throw new ArgumentNullException(nameof(product.Title), "Title cannot be null or empty.");
        }

        return product;
    }

    public void UseReader(string json)
    {
        Console.WriteLine("JSON File content:");

        try
        {
            List<Product> products = ParseProducts(json);

            foreach (var product in products)
            {
                Console.WriteLine($"{product.Title}, price: {product.Price:C}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing JSON: {ex.Message}");
        }

        Console.ReadKey();
    }
}