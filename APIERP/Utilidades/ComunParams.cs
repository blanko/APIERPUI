using AutoMapper;

namespace APIERP.Utilidades
{
    public record ComunParams(ApplicationDbContext Db, IMapper Mapper, HttpContext HttpContext);
}
