using APIERP.DTOs;
using APIERP.Entidades;
using APIERP.Servicios;
using APIERP.Utilidades;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace APIERP.Endpoints
{
    public static class CategoriesEndpoints
    {
        private static readonly string container = "categories";
        public static RouteGroupBuilder MapCategories(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("categories-get"));
            //group.MapGet("/{id:int}", GetById);
            //group.MapGet("/search/{name}", GetByName);
            //group.MapPost("/", Add).DisableAntiforgery();
            //group.MapPut("/{id:int}", Update).DisableAntiforgery();
            //group.MapDelete("/{id:int}", Delete);
            return group;
        }

        static async Task<Results<Ok<List<CategoryDTO>>, NotFound>> GetAll([AsParameters] ComunParams p)
        {
            var categories = await p.Db.Categories.OrderBy(x => x.Name).ToListAsync();
            return categories.Count != 0
                ? TypedResults.Ok(p.Mapper.Map<List<CategoryDTO>>(categories))
                : TypedResults.NotFound();
        }

        static async Task<Results<Ok<CategoryDTO>, NotFound>> GetById(int id, [AsParameters] ComunParams p)
        {
            var category = await p.Db.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
            return category is not null
                ? TypedResults.Ok(p.Mapper.Map<CategoryDTO>(category))
                : TypedResults.NotFound();
        }

        static async Task<Results<Ok<List<CategoryDTO>>, NotFound>> GetByName(string name, [AsParameters] ComunParams p)
        {
            var categories = await p.Db.Categories.Where(x => x.Name.Contains(name)).OrderBy(x => x.Name).ToListAsync();
            return categories.Count != 0
                ? TypedResults.Ok(p.Mapper.Map<List<CategoryDTO>>(categories))
                : TypedResults.NotFound();
        }

        static async Task<Results<Created<CategoryDTO>, ValidationProblem>> Add(
            [FromForm] CategoryDTOAdd CategoryDTOAdd, [AsParameters] ComunParams p,
            IOutputCacheStore outputCacheStore, IAlmacenadorArchivos almacenadorArchivos)
        {
            var category = p.Mapper.Map<Category>(CategoryDTOAdd);

            if (CategoryDTOAdd.Image is not null)
            {
                var url = await almacenadorArchivos.Almacenar(container, CategoryDTOAdd.Image);
                category.ImageUrl = url;
            }

            p.Db.Add(category);
            await p.Db.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("categories-get", default);
            return TypedResults.Created($"/categories/{category.CategoryId}", p.Mapper.Map<CategoryDTO>(category));
        }

        static async Task<Results<NoContent, NotFound, ValidationProblem>> Update(int id,
            [FromForm] CategoryDTOAdd CategoryDTOAdd, [AsParameters] ComunParams p,
            IOutputCacheStore outputCacheStore, IAlmacenadorArchivos almacenadorArchivos)
        {
            var categoryDB = await p.Db.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.CategoryId == id);

            if (categoryDB is null)
            {
                return TypedResults.NotFound();
            }

            var category = p.Mapper.Map<Category>(CategoryDTOAdd);
            category.CategoryId = id;
            category.ImageUrl = categoryDB.ImageUrl;

            if (CategoryDTOAdd.Image is not null)
            {
                var url = await almacenadorArchivos.Editar(category.ImageUrl, container, CategoryDTOAdd.Image);
                category.ImageUrl = url;
            }

            p.Db.Update(category);
            await p.Db.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("categories-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Delete(int id, [AsParameters] ComunParams p,
            IOutputCacheStore outputCacheStore, IAlmacenadorArchivos almacenadorArchivos)
        {
            var categoryDB = await p.Db.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);

            if (categoryDB is null)
            {
                return TypedResults.NotFound();
            }

            p.Db.Categories.Remove(categoryDB);
            await p.Db.SaveChangesAsync();
            await almacenadorArchivos.Borrar(categoryDB.ImageUrl, container);
            await outputCacheStore.EvictByTagAsync("categories-get", default);
            return TypedResults.NoContent();
        }
    }
}