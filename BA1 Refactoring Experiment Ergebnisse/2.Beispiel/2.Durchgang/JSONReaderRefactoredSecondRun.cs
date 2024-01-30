public class Product
{
    public string Title { get; set; }
    public decimal Price { get; set; }
}

public class CustomJSONReader
{
    public void UseReader(string json)
    {
        List<Product> products = DeserializeProducts(json);

        Console.WriteLine("JSON File content:");

        foreach (var product in products)
        {
            Console.WriteLine($"{product.Title}, price: {product.Price:C}");
        }

        Console.ReadKey();
    }

    private List<Product> DeserializeProducts(string json)
    {
        List<Product> products = new List<Product>();
        using (JsonDocument document = JsonDocument.Parse(json))
        {
            foreach (JsonProperty property in document.RootElement.EnumerateObject())
            {
                if (property.NameEquals("title") && property.Value.ValueKind == JsonValueKind.String)
                {
                    string title = property.Value.GetString();
                    if (string.IsNullOrEmpty(title))
                    {
                        throw new ArgumentNullException();
                    }

                    products.Add(new Product { Title = title });
                }
                else if (property.NameEquals("price") && property.Value.ValueKind == JsonValueKind.Number)
                {
                    decimal price = property.Value.GetDecimal();
                    products.Last().Price = price;
                }
            }
        }
        return products;
    }
}