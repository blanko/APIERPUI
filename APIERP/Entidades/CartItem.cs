using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using APIERP.Entidades;
using Microsoft.EntityFrameworkCore;

namespace APIERP.Entidades
{

    [Index("ProductId", Name = "IX_ProductId")]
    public partial class CartItem
    {
        [Key]
        public int CartItemId { get; set; }
        public string CartId { get; set; } = Guid.NewGuid().ToString();
        public int ProductId { get; set; }
        public int Count { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime DateCreated { get; set; }

        [ForeignKey("ProductId")]
        [InverseProperty("CartItems")]
        public virtual Product Product { get; set; } = null!;
    }
}
