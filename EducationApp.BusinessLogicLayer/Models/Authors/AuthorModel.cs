﻿using EducationApp.BusinessLogicLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace EducationApp.BusinessLogicLayer.Models.Authors
{
    public class AuthorModel : BaseModel
    {
        public ICollection<AuthorModelItem> Items { get; set; } = new List<AuthorModelItem>();
        public int Count { get; set; }
    }
}
