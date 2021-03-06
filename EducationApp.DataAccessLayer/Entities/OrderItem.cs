﻿using EducationApp.DataAccessLayer.Entities.Base;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static EducationApp.DataAccessLayer.Entities.Enums.Enums;

namespace EducationApp.DataAccessLayer.Entities
{
    public class OrderItem : BaseEntity
    {
        public decimal Amount { get; set; }
        public CurrencyType Currency { get; set; }
        public long PrintingEditionId { get; set; }
        [ForeignKey("Order")]
        public long OrderId { get; set; }
        public int Count { get; set; }
        public Order Order { get; set; }
        public PrintingEdition PrintingEdition { get; set; }
        [NotMapped]
        public string PrintingEditionTitle { get; set; }
        [NotMapped]
        public string TypeProduct { get; set; }
    }
}
