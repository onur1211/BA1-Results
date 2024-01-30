public class Product
{
    public string Title { get; set; }
    public decimal Price { get; set; }
}

public class CustomJSONReader
{
    public void UseReader(string json)
    {
        List<Product> products = ParseJSON(json);

        Console.WriteLine("JSON File content:");

        foreach (var product in products)
        {
            Console.WriteLine($"{product.Title}, price: {product.Price:C}");
        }

        Console.ReadKey();
    }

    private List<Product> ParseJSON(string json)
    {
        List<Product> products = new List<Product>();
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
        {
            using (Utf8JsonReader reader = new Utf8JsonReader(stream))
            {
                string currentPropertyName = string.Empty;
                while (reader.Read())
                {
                    switch (reader.TokenType)
                    {
                        case JsonTokenType.PropertyName:
                            currentPropertyName = reader.GetString();
                            break;

                        case JsonTokenType.String:
                            if (currentPropertyName == "title")
                            {
                                string title = reader.GetString();
                                ValidateAndAddProduct(products, title);
                            }
                            break;

                        case JsonTokenType.Number:
                            if (currentPropertyName == "price" && reader.TryGetDecimal(out decimal price))
                            {
                                if (products.Count > 0)
                                {
                                    products.Last().Price = price;
                                }
                            }
                            break;
                    }
                }
            }
        }
        return products;
    }

    private void ValidateAndAddProduct(List<Product> products, string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            throw new ArgumentNullException(nameof(title), "Title cannot be null or empty.");
        }
        else
        {
            products.Add(new Product { Title = title });
        }
    }
}