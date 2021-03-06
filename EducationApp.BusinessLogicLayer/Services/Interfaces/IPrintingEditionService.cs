﻿using EducationApp.BusinessLogicLayer.Extention.PrintingEditionFilterState;
using EducationApp.BusinessLogicLayer.Models.Base;
using EducationApp.BusinessLogicLayer.Models.PrintingEditions;
using System.Threading.Tasks;

namespace EducationApp.BusinessLogicLayer.Services.Interfaces
{
    public interface IPrintingEditionService
    {
        Task<BaseModel> CreateAsync(PrintingEditionModelItem model);
        Task<BaseModel> RemoveAsync(long id);
        Task<BaseModel> UpdateAsync(PrintingEditionModelItem printingEditionModelItem);
        Task<PrintingEditionModel> GetPrintingEditionsAsync(PrintingEditionFilterState state);
      
    }
}
