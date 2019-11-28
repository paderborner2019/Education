﻿using BookStore.DataAccess.AppContext;
using EducationApp.DataAccessLayer.Entities;
using EducationApp.DataAccessLayer.Helpers.PrintingEditionFilter;
using EducationApp.DataAccessLayer.Models.Base;
using EducationApp.DataAccessLayer.Ropositories.Base;
using EducationApp.DataAccessLayer.Ropositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EducationApp.DataAccessLayer.Entities.Enums.Enums;

namespace EducationApp.DataAccessLayer.Ropositories.EFRepositories
{
    public class PrintingEditionRepository : BaseEFRepository<PrintingEdition>, IPrintingEditionRepository
    {
        public PrintingEditionRepository(ApplicationContext applicationContext) : base(applicationContext)
        {

        }

        public async Task<ResponseModel<PrintingEdition>> GetPrintingEditionsAsync(PrintingEditionFilterModel printingEditionFilter) //toso rename ...s
        {
            var printingEditions = (from printingEdition in _applicationContext.PrintingEditions
                                    select new PrintingEdition
                                    {
                                        Id = printingEdition.Id,
                                        Title = printingEdition.Title,
                                        ProductType = printingEdition.ProductType,
                                        Price = printingEdition.Price,
                                        Date = printingEdition.Date,
                                        Authors = (from authorPrintingEdition in _applicationContext.AuthorInPrintingEditions
                                                   join author in _applicationContext.Authors on authorPrintingEdition.AuthorId equals author.Id
                                                   where (authorPrintingEdition.PrintingEditionId == printingEdition.Id)
                                                   select new Author
                                                   {
                                                       Name = author.Name,
                                                       Id = author.Id
                                                   }).ToList()
                                    }).AsEnumerable();

            if (!string.IsNullOrWhiteSpace(printingEditionFilter.SearchString))
            {
                printingEditions = printingEditions.Where(l => l.Title == printingEditionFilter.SearchString).Concat(printingEditions.Where(k => k.Authors.Any(l => l.Name == printingEditionFilter.SearchString)));
            }
            if (printingEditions.Count() == 0)
            {
                return null;
            }
            List<TypeProduct> types = Enum.GetValues(typeof(TypeProduct))
                .OfType<TypeProduct>()
                .Except(printingEditionFilter.TypeProduct)
                .ToList();
            foreach (var item in types)
            {
                printingEditions = printingEditions.Where(k => k.ProductType != item);
            }
            //todo may be you can update sort logic withour If (use Reflection)?
            var propertyInfo = printingEditions.First().GetType().GetProperty(printingEditionFilter.PrintingEditionSortType.ToString());
            printingEditions = printingEditionFilter.SortType == SortType.Decrease ? printingEditions
                .OrderBy(e => propertyInfo.GetValue(e, null)) : printingEditions.OrderByDescending(e => propertyInfo.GetValue(e, null));

            var count = printingEditions.Count();
            printingEditions = printingEditions
                .Skip((printingEditionFilter.PageNumber - 1) * printingEditionFilter.PageSize)
                .Take(printingEditionFilter.PageSize);
            var result = new ResponseModel<PrintingEdition>() { Data = printingEditions.ToList(), Count = count };
            return result;
        }
    }
}


