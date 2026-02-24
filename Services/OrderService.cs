using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Cloud9_2.Models;
using Cloud9_2.Data;
using System.ComponentModel.DataAnnotations;

namespace Cloud9_2.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<OrderService> _logger;

        public OrderService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<OrderService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // Get an order by ID
public async Task<Order?> GetOrderByIdAsync(int orderId)
{
    return await _context.Orders
        .Include(o => o.Partner) // csak egyszer!
        .Include(o => o.Site)
        .Include(o => o.Currency)
        .Include(o => o.ShippingMethod)
        .Include(o => o.OrderStatusType)
        .Include(o => o.PaymentTerm)
        .Include(o => o.Contact)
        .Include(o => o.Quote)
        .FirstOrDefaultAsync(o => o.OrderId == orderId);
}

        // Get all orders
public async Task<List<Order>> GetAllOrdersAsync()
{
    return await _context.Orders
        .Include(o => o.Partner)
        .Include(o => o.Site)
        .Include(o => o.Currency)
        .Include(o => o.ShippingMethod)
        .Include(o => o.OrderStatusType)
        .Include(o => o.PaymentTerm)
        .Include(o => o.Contact)
        .Include(o => o.Quote)
        .ToListAsync();
}


        public async Task<Order?> UpdateOrderAsync(OrderUpdateDTO orderDto, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var userName = user?.UserName ?? "System";

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderDto.OrderId);

            if (order == null)
            {
                return null;
            }

            // Update order properties
            order.OrderNumber = orderDto.OrderNumber;
            order.OrderDate = orderDto.OrderDate;
            order.Deadline = orderDto.Deadline;
            order.Description = orderDto.Description;
            order.SalesPerson = orderDto.SalesPerson;
            order.DeliveryDate = orderDto.DeliveryDate;
            order.PlannedDelivery = orderDto.PlannedDelivery;
            order.DiscountPercentage = orderDto.DiscountPercentage;
            order.DiscountAmount = orderDto.DiscountAmount;
            order.CompanyName = orderDto.CompanyName;
            order.Subject = orderDto.Subject;
            order.DetailedDescription = orderDto.DetailedDescription;
            order.Status = orderDto.Status;
            order.PartnerId = orderDto.PartnerId;
            order.SiteId = orderDto.SiteId;
            order.CurrencyId = orderDto.CurrencyId;
            order.ShippingMethodId = orderDto.ShippingMethodId;
            order.PaymentTermId = orderDto.PaymentTermId;
            order.ContactId = orderDto.ContactId;
            order.ReferenceNumber = orderDto.ReferenceNumber;
            order.QuoteId = orderDto.QuoteId;
            order.ModifiedBy = userName;
            order.ModifiedDate = DateTime.UtcNow;
            order.OrderStatusTypes = orderDto.OrderStatusTypes ?? 1;


            await _context.SaveChangesAsync();
            return order;
        }

        // Delete an order
        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return false;
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }
        
    }
}