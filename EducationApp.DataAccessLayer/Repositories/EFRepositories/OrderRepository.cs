﻿using BookStore.DataAccess.AppContext;
using EducationApp.DataAccessLayer.Entities;
using EducationApp.DataAccessLayer.Helpers.OrderFilterModel;
using EducationApp.DataAccessLayer.Models;
using EducationApp.DataAccessLayer.Models.Base;
using EducationApp.DataAccessLayer.Ropositories.Base;
using EducationApp.DataAccessLayer.Ropositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EducationApp.DataAccessLayer.Entities.Enums.Enums;

namespace EducationApp.DataAccessLayer.Ropositories.EFRepositories
{
    public class OrderRepository : BaseEFRepository<Order>, IOrderRepository
    {

        public OrderRepository(ApplicationContext applicationContext) : base(applicationContext)
        {
        }
        public async Task<OrderModel> GetOrderAsync(OrderFilterModel orderFilterModel)
        {
            var orders = from order in _applicationContext.Orders
                         join user in _applicationContext.Users on order.UserId equals user.Id
                         join orderItem in _applicationContext.OrderItems on order.Id equals orderItem.OrderId
                         select new Order
                         {
                             Id = order.Id,
                             Date = orderItem.Date,
                             Status = order.OrderStatusType,
                             UserName = user.UserName,
                             UserEmail = user.Email,
                             CountOrdersModel = orderItem.Count,
                         };
            var test = orders.ToList();
            if (orderFilterModel.Id > 0)
            {
                orders = orders.Where(k => k.Id == orderFilterModel.Id).OrderBy(l => l.Date);
                var myOrders = new OrderModel { Data = await orders.ToListAsync(), Count = orders.Count() };
                return myOrders;
            }
            if (orderFilterModel.SortOrder == SortOrder.Id)
            {
                orders = orderFilterModel.SortType == SortType.Increase ? orders.OrderBy(k => k.Id) : orders.OrderByDescending(k => k.Id);
            }
            if (orderFilterModel.SortOrder == SortOrder.Data)
            {
                orders = orderFilterModel.SortType == SortType.Increase ? orders.OrderBy(k => k.Date) : orders.OrderByDescending(k => k.Date);
            }
            if (orderFilterModel.SortOrder == SortOrder.Amount)
            {
                orders = orderFilterModel.SortType == SortType.Increase ? orders.OrderBy(k => k.Amount) : orders.OrderByDescending(k => k.Amount);
            }

            List<OrderStatusType> types = Enum.GetValues(typeof(OrderStatusType))
               .OfType<OrderStatusType>()
               .Except(orderFilterModel.StatusOrder)
               .ToList();
            foreach (var item in orderFilterModel.StatusOrder)
            {
                orders.Where(k => k.Status != item);
            }
            //orders = orders.Skip((orderFilterModel.PageCount - 1) * orderFilterModel.PageSize).Take(orderFilterModel.PageSize);
            var result = new OrderModel { Data = await orders.ToListAsync(), Count = orders.Count() };
            return result;
        }

        public async Task<Order> Payment(long id)
        {
            var order = _applicationContext.Orders.Where(l => l.PaymentId == id).FirstOrDefault();
            order.OrderStatusType = OrderStatusType.Paid;
            return order;
        }
    }
}