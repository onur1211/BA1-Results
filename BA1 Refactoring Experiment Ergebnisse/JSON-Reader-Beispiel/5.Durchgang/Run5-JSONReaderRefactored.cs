public class Product
{
    public string Title { get; set; }
    public decimal Price { get; set; }
}

public class CustomJSONReader
{
    public void UseReader(string json)
    {
        var products = ParseJson(json);

        Console.WriteLine("JSON File content:");

        foreach (var product in products)
        {
            Console.WriteLine($"{product.Title}, price: {product.Price:C}");
        }

        Console.ReadKey();
    }

    private List<Product> ParseJson(string json)
    {
        var products = new List<Product>();

        using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
        using (var reader = new Utf8JsonReader(memoryStream))
        {
            string currentProperty = "";

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName:
                        currentProperty = reader.GetString();
                        break;

                    case JsonTokenType.String when currentProperty == "title":
                        var title = reader.GetString();
                        ValidateAndAddTitle(products, title);
                        break;

                    case JsonTokenType.Number when currentProperty == "price":
                        if (reader.TryGetDecimal(out var price))
                        {
                            AddPriceToLastProduct(products, price);
                        }
                        break;
                }
            }
        }

        return products;
    }

    private void ValidateAndAddTitle(List<Product> products, string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            throw new ArgumentNullException(nameof(title), "Title cannot be null or empty.");
        }

        products.Add(new Product { Title = title });
    }

    private void AddPriceToLastProduct(List<Product> products, decimal price)
    {
        if (products.Count > 0)
        {
            products[^1].Price = price;
        }
    }
}