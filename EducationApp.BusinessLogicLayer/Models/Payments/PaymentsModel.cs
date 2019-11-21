﻿using EducationApp.BusinessLogicLayer.Models.Base;
using EducationApp.BusinessLogicLayer.Models.OrderItemModelItem;
using System;
using System.Collections.Generic;
using System.Text;

namespace EducationApp.BusinessLogicLayer.Models.Payments
{
    public class PaymentsModel : BaseModel
    {
        public OrderItemModel OrderItemModel { get; set; }
        public long TransactionId { get; set; }
        public string Description { get; set; }
        public long UserId { get; set; }
    }
}
