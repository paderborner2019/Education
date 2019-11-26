﻿using EducationApp.BusinessLogicLayer.Extention.PrintingEditionFilterState;
using EducationApp.DataAccessLayer.Helpers.PrintingEditionFilter;
using System.Collections.Generic;
using static EducationApp.DataAccessLayer.Entities.Enums.Enums;

namespace EducationApp.BusinessLogicLayer.Helpers.Mapping.PrintingEditions
{
    public class PrintingEditionFilterStateMapping
    {
        public static PrintingEditionFilterModel Map(PrintingEditionFilterState printingEditionFilterState)
        {
            var printingEditionFilter = new PrintingEditionFilterModel
            {
                PageNumber = printingEditionFilterState.PageNumber,
                PageSize = printingEditionFilterState.PageSize,
                SearchString = printingEditionFilterState.SearchString,
                Price = (Price)printingEditionFilterState.Price,
            };
            printingEditionFilter.TypeProduct = MapList(printingEditionFilterState);
            return printingEditionFilter;
        }

        public static List<TypeProduct> MapList(PrintingEditionFilterState printingEditionFilterState)
        {
            var typeProduct = new TypeProduct();
            List<TypeProduct> result = new List<TypeProduct>();
            foreach (var item in printingEditionFilterState.TypeProduct)
            {
                typeProduct = (DataAccessLayer.Entities.Enums.Enums.TypeProduct)item;
                result.Add(typeProduct);
            }
            return result;
        }
    }
}
