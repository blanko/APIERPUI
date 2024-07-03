using APIERP.DTOs;
using APIERP.Entidades;
using APIERP.Utilidades;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace APIERP.Endpoints
{
    public static class CartItemsEndpoints
    {
        public static RouteGroupBuilder MapCartItems(this RouteGroupBuilder group)
        {
            //group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("cartitems-get"));
            //group.MapGet("/{id:int}", GetById);
            //group.MapGet("/byguid/{guid:guid}", GetByGuid);
            group.MapGet("/mycart/{cartId}", GetMyCart);
            //group.MapPost("/", Add);
            //group.MapPut("/{id:int}", Update);
            //group.MapDelete("/{id:int}", Delete);
            group.MapPost("/multiple", AddMultipleCartItems);
            //group.MapGet("/cartid", GetCartId);
            return group;
        }

        static async Task<Results<Ok<List<CartItemDTO>>, NotFound>> GetAll([AsParameters] ComunParams p)
        {
            var cartItems = await p.Db.CartItems.OrderBy(c => c.CartItemId).ToListAsync();
            return cartItems.Count != 0
                ? TypedResults.Ok(p.Mapper.Map<List<CartItemDTO>>(cartItems))
                : TypedResults.NotFound();
        }

        static async Task<Results<Ok<CartItemDTO>, NotFound>> GetById(int id, [AsParameters] ComunParams p)
        {
            var cartItem = await p.Db.CartItems.FirstOrDefaultAsync(x => x.CartItemId == id);
            return cartItem is not null
                ? TypedResults.Ok(p.Mapper.Map<CartItemDTO>(cartItem))
                : TypedResults.NotFound();
        }

        //static async Task<Results<Ok<CartItemDTO>, NotFound>> GetByGuid(Guid guid, [AsParameters] ComunParams p)
        //{
        //    var cartItem = await p.Db.CartItems.FirstOrDefaultAsync(x => x.Guid == guid);
        //    return cartItem is not null
        //        ? TypedResults.Ok(p.Mapper.Map<CartItemDTO>(cartItem))
        //        : TypedResults.NotFound();
        //}

        static async Task<Results<Ok<List<CartItemWithPriceDTO>>, NotFound>> GetMyCart(string cartId, [AsParameters] ComunParams p)
        {
            var cartItems = await p.Db.CartItems
                .Where(x => x.CartId == cartId)
                .Include(x => x.Product)
                .OrderBy(x => x.CartItemId)
                .ToListAsync();

            if (cartItems.Count == 0)
            {
                return TypedResults.NotFound();
            }

            var result = cartItems.Select(ci => new CartItemWithPriceDTO
            {
                CartItemId = ci.CartItemId,
                ProductId = ci.ProductId,
                Quantity = ci.Count,
                UnitPrice = ci.Product.Price,
                ProductTitle = ci.Product.Title
            }).ToList();

            return TypedResults.Ok(result);
        }

        static async Task<Results<Created<CartItemDTO>, ValidationProblem>> Add(CartItemDTOAdd cartItemDTOAdd, [AsParameters] ComunParams p, IOutputCacheStore outputCacheStore)
        {
            var cartItem = p.Mapper.Map<CartItem>(cartItemDTOAdd);
            p.Db.Add(cartItem);
            await p.Db.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("cartitems-get", default);
            return TypedResults.Created($"/cartitems/{cartItem.CartItemId}", p.Mapper.Map<CartItemDTO>(cartItem));
        }

        static async Task<Results<NoContent, NotFound>> Update(int id, CartItemDTOAdd cartItemDTOAdd, [AsParameters] ComunParams p, IOutputCacheStore outputCacheStore)
        {
            var cartItemDB = await p.Db.CartItems.AsNoTracking().FirstOrDefaultAsync(x => x.CartItemId == id);

            if (cartItemDB is null) return TypedResults.NotFound();

            var cartItem = p.Mapper.Map<CartItem>(cartItemDTOAdd);
            cartItem.CartItemId = id;

            p.Db.Update(cartItem);
            await p.Db.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("cartitems-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Delete(int id, [AsParameters] ComunParams p, IOutputCacheStore outputCacheStore)
        {
            var cartItem = await p.Db.CartItems.FirstOrDefaultAsync(x => x.CartItemId == id);

            if (cartItem is null)
            {
                return TypedResults.NotFound();
            }

            p.Db.CartItems.Remove(cartItem);
            await p.Db.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("cartitems-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<Created<List<CartItemDTO>>, ValidationProblem>> AddMultipleCartItems(List<CartItemDTOAdd> cartItemDTOAdds, [AsParameters] ComunParams p, IOutputCacheStore outputCacheStore)
        {
            var cartItems = p.Mapper.Map<List<CartItem>>(cartItemDTOAdds);
            p.Db.CartItems.AddRange(cartItems);
            await p.Db.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("cartitems-get", default);

            var cartItemDTOs = p.Mapper.Map<List<CartItemDTO>>(cartItems);
            return TypedResults.Created($"/cartitems/multiple", cartItemDTOs);
        }
    }
}
