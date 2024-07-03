using APIERP.DTOs;
using APIERP.Entidades;
using APIERP.Utilidades;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace APIERP.Endpoints
{
    public static class StoresEndpoints
    {
        public static RouteGroupBuilder MapStores(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("stores-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapGet("/search/{name}", GetByName);
            group.MapPost("/", Add);
            group.MapPut("/{id:int}", Update);
            group.MapDelete("/{id:int}", Delete);
            return group;
        }

        static async Task<Results<Ok<List<StoreDTO>>, NotFound>> GetAll([AsParameters] ComunParams p)
        {
            var stores = await p.Db.Stores.OrderBy(x => x.Name).ToListAsync();
            return stores.Count != 0
                ? TypedResults.Ok(p.Mapper.Map<List<StoreDTO>>(stores))
                : TypedResults.NotFound();
        }

        static async Task<Results<Ok<StoreDTO>, NotFound>> GetById(int id, [AsParameters] ComunParams p)
        {
            var store = await p.Db.Stores.FirstOrDefaultAsync(x => x.StoreId == id);
            return store is not null
                ? TypedResults.Ok(p.Mapper.Map<StoreDTO>(store))
                : TypedResults.NotFound();
        }

        static async Task<Results<Ok<List<StoreDTO>>, NotFound>> GetByName(string name, [AsParameters] ComunParams p)
        {
            var stores = await p.Db.Stores.Where(x => x.Name.Contains(name)).OrderBy(x => x.Name).ToListAsync();
            return stores.Count != 0
                ? TypedResults.Ok(p.Mapper.Map<List<StoreDTO>>(stores))
                : TypedResults.NotFound();
        }

        static async Task<Results<Created<StoreDTO>, ValidationProblem>> Add(
            StoreDTOAdd StoreDTOAdd, [AsParameters] ComunParams p,
            IOutputCacheStore outputCacheStore)
        {
            var store = p.Mapper.Map<Store>(StoreDTOAdd);
            p.Db.Add(store);
            await p.Db.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("stores-get", default);
            return TypedResults.Created($"/stores/{store.StoreId}", p.Mapper.Map<StoreDTO>(store));
        }

        static async Task<Results<NoContent, NotFound, ValidationProblem>> Update(int id,
            StoreDTOAdd StoreDTOAdd, [AsParameters] ComunParams p,
            IOutputCacheStore outputCacheStore)
        {
            var storeDB = await p.Db.Stores.AsNoTracking().FirstOrDefaultAsync(x => x.StoreId == id);

            if (storeDB is null)
            {
                return TypedResults.NotFound();
            }

            var store = p.Mapper.Map<Store>(StoreDTOAdd);
            store.StoreId = id;

            p.Db.Update(store);
            await p.Db.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("stores-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Delete(int id, [AsParameters] ComunParams p,
            IOutputCacheStore outputCacheStore)
        {
            var store = await p.Db.Stores.FirstOrDefaultAsync(x => x.StoreId == id);

            if (store is null)
            {
                return TypedResults.NotFound();
            }

            p.Db.Stores.Remove(store);
            await p.Db.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("stores-get", default);
            return TypedResults.NoContent();
        }
    }
}
