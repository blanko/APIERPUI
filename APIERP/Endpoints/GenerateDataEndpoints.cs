using APIERP.Entidades;
using AutoMapper;

namespace APIERP.Endpoints
{
    public static class GenerateDataEndpoints
    {
        public static RouteGroupBuilder AddDataToDB(this RouteGroupBuilder group)
        {
            group.MapGet("/", async (
                IMapper mapper, ApplicationDbContext db) =>
            {
                //await GenerateStores(db);
                await GenerateCategories(db);
                await GenerateProducts(db);
                //await GenerateOrders(db);
                //await GenerateOrderDetails(db);
                //await GenerateRainChecks(db);

                return TypedResults.Ok();
            });

            return group;
        }

        static async Task GenerateStores(ApplicationDbContext db)
        {
            for (int i = 1; i <= 20; i++)
            {
                db.Add(new Store
                {
                    Name = $"Tienda {i}"
                });
                await db.SaveChangesAsync();
            }
        }

        static async Task GenerateCategories(ApplicationDbContext db)
        {
            for (int i = 1; i <= 20; i++)
            {
                db.Add(new Category
                {
                    Name = $"Categoria {i}"
                });
            }
            await db.SaveChangesAsync();
        }

        static async Task GenerateProducts(ApplicationDbContext db)
        {
            Random random = new Random();

            for (int i = 1; i <= 200; i++)
            {
                db.Add(new Product
                {
                    SkuNumber = $"SKU{i}",
                    CategoryId = random.Next(1, 21),
                    RecommendationId = 0,
                    Title = $"Producto {i}.{random.Next(3, 21)}",
                    Price = GetRandomDecimal(3, 120, 2),
                    SalePrice = 0,
                    Created = DateTime.Now,
                    ProductArtUrl = "string",
                    Description = $"Descripción {i}",
                    ProductDetails = "string",
                    Inventory = random.Next(3, 21),
                    LeadTime = 0
                });
            }
            await db.SaveChangesAsync();
        }

        static async Task GenerateOrders(ApplicationDbContext db)
        {
            Random random = new Random();

            for (int i = 1; i <= 20; i++)
            {
                db.Add(new Order
                {
                    OrderDate = DateTime.Now,
                    Username = $"user{i}",
                    Name = $"Nombre {i}",
                    Address = $"direccion{i}",
                    City = "Málaga",
                    State = "Málaga",
                    PostalCode = "29000",
                    Country = "pais",
                    Phone = $"{i}{i}{i}{i}{i}{i}{i}{i}{i}",
                    Email = $"user{i}@localhost.com",
                    Total = 0
                });
            }
            await db.SaveChangesAsync();
        }

        static async Task GenerateOrderDetails(ApplicationDbContext db)
        {
            Random random = new Random();

            for (int i = 1; i <= 20; i++)
            {
                db.Add(new OrderDetail
                {
                    OrderId = random.Next(1, 21),
                    ProductId = random.Next(2, 11),
                    Count = random.Next(1, 3),
                    UnitPrice = GetRandomDecimal(3, 120, 2)
                });
            }
            await db.SaveChangesAsync();
        }

        static async Task GenerateRainChecks(ApplicationDbContext db)
        {
            Random random = new Random();

            for (int i = 1; i <= 20; i++)
            {
                db.Add(new Raincheck
                {
                    Name = $"RainName {i}",
                    Count = random.Next(1, 3),
                    SalePrice = (double)GetRandomDecimal(3, 120, 2),
                    StoreId = random.Next(2, 11),
                    ProductId = random.Next(2, 11)
                });
            }
            await db.SaveChangesAsync();
        }

        public static decimal GetRandomDecimal(int minValue, int maxValue, int decimalPlaces)
        {
            Random random = new Random();
            int integerPart = random.Next(minValue, maxValue);
            decimal fractionalPart = (decimal)random.NextDouble();

            // Combina las partes entera y fraccionaria
            decimal result = integerPart + fractionalPart;

            // Redondea a los decimales especificados
            return Math.Round(result, decimalPlaces);
        }
    }
}
