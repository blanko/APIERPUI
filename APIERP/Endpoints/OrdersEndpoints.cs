using APIERP.DTOs;
using APIERP.Entidades;
using APIERP.Utilidades;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace APIERP.Endpoints
{
    public static class OrdersEndpoints
    {
        public static RouteGroupBuilder MapOrders(this RouteGroupBuilder group)
        {
            //group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("orders-get"));
            //group.MapGet("/{id:int}", GetById);
            //group.MapGet("/byusername/{username}", GetByUsername);
            //group.MapGet("/bydate", GetAllByDate);
            group.MapPost("/", Add);
            //group.MapPut("/{id:int}", Update);
            //group.MapDelete("/{id:int}", Delete);
            //group.MapGet("/orderdetail/{id:int}", GetOrderDetailById);
            //group.MapGet("/orderdetails/order/{orderId:int}", GetAllOrderDetailsByOrderID);
            //group.MapPost("/orderdetail", AddOrderDetail);
            //group.MapPut("/orderdetail/{id:int}", UpdateOrderDetail);
            //group.MapDelete("/orderdetail/{id:int}", DeleteOrderDetail);
            group.MapPost("/orderdetails/{orderId:int}", AddMultipleOrderDetails);
            return group;
        }

        // Orders methods
        static async Task<Results<Ok<List<OrderDTO>>, NotFound>> GetAll([AsParameters] ComunParams p, int pagina = 1, int recordsPorPagina = 10)
        {
            var paginacion = new PaginacionDTO { Pagina = pagina, RecordsPorPagina = recordsPorPagina };
            var queryable = p.Db.Orders.AsQueryable();
            await p.HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var orders = await queryable.OrderBy(p => p.OrderId).Paginar(paginacion).ToListAsync();
            return orders.Count != 0
                ? TypedResults.Ok(p.Mapper.Map<List<OrderDTO>>(orders))
                : TypedResults.NotFound();
        }

        static async Task<Results<Ok<OrderDTO>, NotFound>> GetById(int id, [AsParameters] ComunParams p)
        {
            var order = await p.Db.Orders.FirstOrDefaultAsync(x => x.OrderId == id);
            return order is not null
                ? TypedResults.Ok(p.Mapper.Map<OrderDTO>(order))
                : TypedResults.NotFound();
        }

        static async Task<Results<Ok<List<OrderDTO>>, NotFound>> GetByUsername(string username, [AsParameters] ComunParams p)
        {
            var orders = await p.Db.Orders.Where(x => x.Username.Equals(username)).OrderBy(x => x.OrderDate).ToListAsync();
            return orders.Count != 0
                ? TypedResults.Ok(p.Mapper.Map<List<OrderDTO>>(orders))
                : TypedResults.NotFound();
        }

        static async Task<Results<Ok<List<OrderDTO>>, NotFound>> GetAllByDate(DateTime date, [AsParameters] ComunParams p, int pagina = 1, int recordsPorPagina = 10)
        {
            var paginacion = new PaginacionDTO { Pagina = pagina, RecordsPorPagina = recordsPorPagina };
            var queryable = p.Db.Orders.AsQueryable();
            await p.HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var orders = await queryable.Where(x => x.OrderDate.Date == date.Date).OrderBy(x => x.OrderDate).Paginar(paginacion).ToListAsync();
            return orders.Count != 0
                ? TypedResults.Ok(p.Mapper.Map<List<OrderDTO>>(orders))
                : TypedResults.NotFound();
        }

        static async Task<Results<Created<OrderDTO>, ValidationProblem>> Add(OrderDTOAdd OrderDTOAdd, [AsParameters] ComunParams p, IOutputCacheStore outputCacheStore)
        {
            var order = p.Mapper.Map<Order>(OrderDTOAdd);
            order.OrderDate = DateTime.Now;
            p.Db.Add(order);
            await p.Db.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("orders-get", default);
            return TypedResults.Created($"/orders/{order.OrderId}", p.Mapper.Map<OrderDTO>(order));
        }

        static async Task<Results<NoContent, NotFound>> Update(int id, OrderDTOAdd OrderDTOAdd, [AsParameters] ComunParams p, IOutputCacheStore outputCacheStore)
        {
            var orderDB = await p.Db.Orders.AsNoTracking().FirstOrDefaultAsync(x => x.OrderId == id);

            if (orderDB is null) return TypedResults.NotFound();

            var order = p.Mapper.Map<Order>(OrderDTOAdd);
            order.OrderId = id;

            p.Db.Update(order);
            await p.Db.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("orders-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Delete(int id, [AsParameters] ComunParams p, IOutputCacheStore outputCacheStore)
        {
            var order = await p.Db.Orders.FirstOrDefaultAsync(x => x.OrderId == id);

            if (order is null)
            {
                return TypedResults.NotFound();
            }

            p.Db.Orders.Remove(order);
            await p.Db.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("orders-get", default);
            return TypedResults.NoContent();
        }

        // OrderDetails methods
        static async Task<Results<Ok<OrderDetailDTO>, NotFound>> GetOrderDetailById(int id, [AsParameters] ComunParams p)
        {
            var orderDetail = await p.Db.OrderDetails.FirstOrDefaultAsync(x => x.OrderDetailId == id);
            return orderDetail is not null
                ? TypedResults.Ok(p.Mapper.Map<OrderDetailDTO>(orderDetail))
                : TypedResults.NotFound();
        }

        static async Task<Results<Ok<List<OrderDetailDTO>>, NotFound>> GetAllOrderDetailsByOrderID(int orderId, [AsParameters] ComunParams p)
        {
            var orderDetails = await p.Db.OrderDetails.Where(x => x.OrderId == orderId).OrderBy(x => x.OrderDetailId).ToListAsync();
            return orderDetails.Count != 0
                ? TypedResults.Ok(p.Mapper.Map<List<OrderDetailDTO>>(orderDetails))
                : TypedResults.NotFound();
        }

        static async Task<Results<Created<OrderDetailDTO>, ValidationProblem>> AddOrderDetail(OrderDetailDTOAdd OrderDetailDTOAdd, [AsParameters] ComunParams p, IOutputCacheStore outputCacheStore)
        {
            var orderDetail = p.Mapper.Map<OrderDetail>(OrderDetailDTOAdd);
            p.Db.Add(orderDetail);
            await p.Db.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("orderdetails-get", default);
            return TypedResults.Created($"/orderdetail/{orderDetail.OrderDetailId}", p.Mapper.Map<OrderDetailDTO>(orderDetail));
        }

        static async Task<Results<NoContent, NotFound>> UpdateOrderDetail(int id, OrderDetailDTOAdd OrderDetailDTOAdd, [AsParameters] ComunParams p, IOutputCacheStore outputCacheStore)
        {
            var orderDetailDB = await p.Db.OrderDetails.AsNoTracking().FirstOrDefaultAsync(x => x.OrderDetailId == id);

            if (orderDetailDB is null) return TypedResults.NotFound();

            var orderDetail = p.Mapper.Map<OrderDetail>(OrderDetailDTOAdd);
            orderDetail.OrderDetailId = id;

            p.Db.Update(orderDetail);
            await p.Db.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("orderdetails-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> DeleteOrderDetail(int id, [AsParameters] ComunParams p, IOutputCacheStore outputCacheStore)
        {
            var orderDetail = await p.Db.OrderDetails.FirstOrDefaultAsync(x => x.OrderDetailId == id);

            if (orderDetail is null)
            {
                return TypedResults.NotFound();
            }

            p.Db.OrderDetails.Remove(orderDetail);
            await p.Db.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("orderdetails-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<Created<List<OrderDetailDTO>>, NotFound, ValidationProblem>> AddMultipleOrderDetails(int orderId, List<OrderDetailDTOMultiAdd> orderDetailDTOAdds, [AsParameters] ComunParams p, IOutputCacheStore outputCacheStore)
        {
            var order = await p.Db.Orders.FirstOrDefaultAsync(x => x.OrderId == orderId);
            if (order is null)
            {
                return TypedResults.NotFound();
            }

            var orderDetails = p.Mapper.Map<List<OrderDetail>>(orderDetailDTOAdds);
            foreach (var orderDetail in orderDetails)
            {
                orderDetail.OrderId = orderId;
                p.Db.OrderDetails.Add(orderDetail);
            }

            await p.Db.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("orderdetails-get", default);

            var orderDetailDTOs = p.Mapper.Map<List<OrderDetailDTO>>(orderDetails);
            return TypedResults.Created($"/orderdetails/order/{orderId}", orderDetailDTOs);
        }
    }
}
