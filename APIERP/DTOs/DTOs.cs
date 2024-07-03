namespace APIERP.DTOs
{
    public record OrderDTOAdd
    {
        public DateTime OrderDate { get; set; }
        public string Username { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public decimal Total { get; set; }
    }

    public record OrderDTO
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Username { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public decimal Total { get; set; }
    }

    public record CategoryDTOAdd
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
    }

    public record CategoryDTO
    {
        public int CategoryId { get; set; }

        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }

    public record RaincheckDTOAdd
    {
        public string? Name { get; set; }
        public int Count { get; set; }
        public double SalePrice { get; set; }
        public int StoreId { get; set; }
        public int ProductId { get; set; }
    }

    public record RaincheckDTO
    {
        public int RaincheckId { get; set; }

        public string? Name { get; set; }
        public int Count { get; set; }
        public double SalePrice { get; set; }
        public StoreDTO? Store { get; set; }
        public ProductDTO? Product { get; set; }
    }

    public record StoreDTOAdd
    {
        public string? Name { get; set; }
    }

    public record StoreDTO
    {
        public int StoreId { get; set; }

        public string? Name { get; set; }
    }

    public record ProductDTOAdd
    {
        public string SkuNumber { get; set; } = null!;
        public int RecommendationId { get; set; }
        public string Title { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public string? ProductArtUrl { get; set; }
        public string Description { get; set; } = null!;
        public DateTime Created { get; set; }
        public string ProductDetails { get; set; } = null!;
        public int Inventory { get; set; }
        public int LeadTime { get; set; }
        public int CategoryId { get; set; }
    }

    public record ProductDTO
    {
        public int ProductId { get; set; }

        public string SkuNumber { get; set; } = null!;
        public int RecommendationId { get; set; }
        public string Title { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public string? ProductArtUrl { get; set; }
        public string Description { get; set; } = null!;
        public DateTime Created { get; set; }
        public string ProductDetails { get; set; } = null!;
        public int Inventory { get; set; }
        public int LeadTime { get; set; }
        public CategoryDTO? Category { get; set; }
    }

    public record CartItemDTOAdd
    {
        public string CartId { get; set; } = null!;
        public int Count { get; set; }
        public DateTime DateCreated { get; set; }
        public int ProductId { get; set; }
    }

    public record CartItemDTO
    {
        public int CartItemId { get; set; }

        public string CartId { get; set; } = null!;
        public int Count { get; set; }
        public DateTime DateCreated { get; set; }
        public ProductDTO? Product { get; set; }
    }

    public class CartItemWithPriceDTO
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } // Precio del producto
        public string ProductTitle { get; set; } = null!; // Titulo del producto
    }

    public record OrderDetailDTOAdd
    {
        public int Count { get; set; }
        public decimal UnitPrice { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
    }

    public record OrderDetailDTOMultiAdd
    {
        public int Count { get; set; }
        public decimal UnitPrice { get; set; }
        public int ProductId { get; set; }
    }

    public record OrderDetailDTO
    {
        public int OrderDetailId { get; set; }

        public int Count { get; set; }
        public decimal UnitPrice { get; set; }
        public OrderDTO? Order { get; set; }
        public ProductDTO? Product { get; set; }
    }
}
