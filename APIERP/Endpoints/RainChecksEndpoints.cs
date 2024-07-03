using APIERP.DTOs;
using APIERP.Entidades;
using APIERP.Utilidades;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace APIERP.Endpoints
{
    public static class RainChecksEndpoints
    {
        public static RouteGroupBuilder MapRainChecks(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("rainchecks-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Add);
            group.MapPut("/{id:int}", Update);
            group.MapDelete("/{id:int}", Delete);
            return group;
        }

        static async Task<Results<Ok<List<RaincheckDTO>>, NotFound>> GetAll([AsParameters] ComunParams p, int pagina = 1, int recordsPorPagina = 10)
        {
            var paginacion = new PaginacionDTO { Pagina = pagina, RecordsPorPagina = recordsPorPagina };
            var queryable = p.Db.Rainchecks.AsQueryable();
            await p.HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var rainChecks = await queryable.OrderBy(p => p.Name).Paginar(paginacion).ToListAsync();
            return rainChecks.Count != 0
                ? TypedResults.Ok(p.Mapper.Map<List<RaincheckDTO>>(rainChecks))
                : TypedResults.NotFound();
        }

        static async Task<Results<Ok<RaincheckDTO>, NotFound>> GetById(int id, [AsParameters] ComunParams p)
        {
            var rainCheck = await p.Db.Rainchecks.FirstOrDefaultAsync(x => x.RaincheckId == id);
            return rainCheck is not null
                ? TypedResults.Ok(p.Mapper.Map<RaincheckDTO>(rainCheck))
                : TypedResults.NotFound();
        }

        static async Task<Results<Created<RaincheckDTO>, ValidationProblem>> Add(RaincheckDTOAdd RaincheckDTOAdd, [AsParameters] ComunParams p, IOutputCacheStore outputCacheStore)
        {
            var rainCheck = p.Mapper.Map<Raincheck>(RaincheckDTOAdd);
            p.Db.Add(rainCheck);
            await p.Db.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("rainchecks-get", default);
            return TypedResults.Created($"/rainchecks/{rainCheck.RaincheckId}", p.Mapper.Map<RaincheckDTO>(rainCheck));
        }

        static async Task<Results<NoContent, NotFound>> Update(int id, RaincheckDTOAdd RaincheckDTOAdd, [AsParameters] ComunParams p, IOutputCacheStore outputCacheStore)
        {
            var rainCheckDB = await p.Db.Rainchecks.AsNoTracking().FirstOrDefaultAsync(x => x.RaincheckId == id);

            if (rainCheckDB is null) return TypedResults.NotFound();

            var rainCheck = p.Mapper.Map<Raincheck>(RaincheckDTOAdd);
            rainCheck.RaincheckId = id;

            p.Db.Update(rainCheck);
            await p.Db.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("rainchecks-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Delete(int id, [AsParameters] ComunParams p, IOutputCacheStore outputCacheStore)
        {
            var rainCheck = await p.Db.Rainchecks.FirstOrDefaultAsync(x => x.RaincheckId == id);

            if (rainCheck is null)
            {
                return TypedResults.NotFound();
            }

            p.Db.Rainchecks.Remove(rainCheck);
            await p.Db.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("rainchecks-get", default);
            return TypedResults.NoContent();
        }
    }
}
