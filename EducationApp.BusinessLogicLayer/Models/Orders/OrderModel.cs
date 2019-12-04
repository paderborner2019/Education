﻿using EducationApp.BusinessLogicLayer.Models.Base;
using System.Collections.Generic;

namespace EducationApp.BusinessLogicLayer.Models.Orders
{
    public class OrderModel : BaseModel
    {
        public ICollection<OrderModelItem> Items = new List<OrderModelItem>();
        public int ItemsCount { get; set; }
    }
}
