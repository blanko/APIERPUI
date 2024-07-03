using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace APIERP.Entidades;

public partial class Store
{
    [Key]
    public int StoreId { get; set; }
    public string Name { get; set; } = null!;

    [InverseProperty("Store")]
    public virtual ICollection<Raincheck> Rainchecks { get; set; } = new List<Raincheck>();
}
