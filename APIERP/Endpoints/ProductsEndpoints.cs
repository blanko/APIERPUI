using APIERP.DTOs;
using APIERP.Entidades;
using APIERP.Utilidades;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace APIERP.Endpoints
{
    public static class ProductsEndpoints
    {
        public static RouteGroupBuilder MapProducts(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("products-get"));
            //group.MapGet("/{id:int}", GetById);
            group.MapGet("/bycategory/{id:int}", GetByCategories);
            //group.MapPost("/", Add);
            //group.MapPut("/{id:int}", Update);
            //group.MapDelete("/{id:int}", Delete);
            return group;
        }

        static async Task<Results<Ok<List<ProductDTO>>, NotFound>> GetAll([AsParameters] ComunParams p, int pagina = 1, int recordsPorPagina = 10)
        {
            var paginacion = new PaginacionDTO { Pagina = pagina, RecordsPorPagina = recordsPorPagina };
            var queryable = p.Db.Products.AsQueryable();
            await p.HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var products = await queryable.OrderBy(p => p.Title).Paginar(paginacion).ToListAsync();
            return products.Count != 0
                ? TypedResults.Ok(p.Mapper.Map<List<ProductDTO>>(products))
                : TypedResults.NotFound();
        }

        static async Task<Results<Ok<ProductDTO>, NotFound>> GetById(int id, [AsParameters] ComunParams p)
        {
            var product = await p.Db.Products.FirstOrDefaultAsync(x => x.ProductId == id);
            return product is not null
                ? TypedResults.Ok(p.Mapper.Map<ProductDTO>(product))
                : TypedResults.NotFound();
        }

        static async Task<Results<Ok<List<ProductDTO>>, NotFound>> GetByCategories(int id, [AsParameters] ComunParams p, int pagina = 1, int recordsPorPagina = 10)
        {
            var paginacion = new PaginacionDTO { Pagina = pagina, RecordsPorPagina = recordsPorPagina };
            var queryable = p.Db.Products.Where(x => x.CategoryId.Equals(id)).AsQueryable();
            await p.HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var products = await queryable.Where(x => x.CategoryId.Equals(id))
                .OrderBy(p => p.Title).Paginar(paginacion).ToListAsync();
            return products.Count != 0
                ? TypedResults.Ok(p.Mapper.Map<List<ProductDTO>>(products))
                : TypedResults.NotFound();
        }

        static async Task<Results<Created<ProductDTO>, ValidationProblem>> Add(ProductDTOAdd ProductDTOAdd, [AsParameters] ComunParams p, IOutputCacheStore outputCacheStore)
        {
            var product = p.Mapper.Map<Product>(ProductDTOAdd);
            product.Created = DateTime.Now;
            p.Db.Add(product);
            await p.Db.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("products-get", default);
            return TypedResults.Created($"/products/{product.ProductId}", p.Mapper.Map<ProductDTO>(product));
        }

        static async Task<Results<NoContent, NotFound>> Update(int id, ProductDTOAdd ProductDTOAdd, [AsParameters] ComunParams p, IOutputCacheStore outputCacheStore)
        {
            var productDB = await p.Db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == id);

            if (productDB is null) return TypedResults.NotFound();

            var product = p.Mapper.Map<Product>(ProductDTOAdd);
            product.ProductId = id;

            p.Db.Update(product);
            await p.Db.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("products-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Delete(int id, [AsParameters] ComunParams p, IOutputCacheStore outputCacheStore)
        {
            var product = await p.Db.Products.FirstOrDefaultAsync(x => x.ProductId == id);

            if (product is null)
            {
                return TypedResults.NotFound();
            }

            p.Db.Products.Remove(product);
            await p.Db.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("products-get", default);
            return TypedResults.NoContent();
        }
    }
}
