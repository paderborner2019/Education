using EducationApp.BusinessLogicLayer.Extention.PrintingEditionFilterState;
using EducationApp.BusinessLogicLayer.Helpers.Mapping;
using EducationApp.BusinessLogicLayer.Helpers.Mapping.AuthorInPrintingEditionMapper;
using EducationApp.BusinessLogicLayer.Helpers.Mapping.PrintingEditions;
using EducationApp.BusinessLogicLayer.Models.Base;
using EducationApp.BusinessLogicLayer.Models.PrintingEditions;
using EducationApp.BusinessLogicLayer.Services.Interfaces;
using EducationApp.DataAccessLayer.Ropositories.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using errors = EducationApp.BusinessLogicLayer.Common.Consts.Constants.Errors;
using EducationApp.BusinessLogicLayer.Extention.Mapper.PrintingEditionsMapper;
using EducationApp.BusinessLogicLayer.Helpers;

namespace EducationApp.BusinessLogicLayer.Services
{
    public class PrintingEditionService : IPrintingEditionService
    {
        private readonly IPrintingEditionRepository _printingEditionRepository;
        private readonly IAuthorInPrintingEditionRepository _authorInPrintingEditionRepository;

        public PrintingEditionService(IPrintingEditionRepository printingEditionRepository, IAuthorInPrintingEditionRepository authorInPrintingEditionRepository)
        {
            _authorInPrintingEditionRepository = authorInPrintingEditionRepository;
            _printingEditionRepository = printingEditionRepository;
        }

        public async Task<BaseModel> CreateAsync(PrintingEditionModelItem model)
        {
            var resultModel = new BaseModel();

            var printingEdition = model.Map();

            var printingEditionId = await _printingEditionRepository.CreateAsync(printingEdition);

            if (printingEditionId < 1)
            {
                resultModel.Errors.Add(errors.Create);
                return resultModel;
            }

            var authorInPrintingEditions = AuthorInPrintingEditionMapper.Map(printingEditionId, model.Authors.Items.ToList());

            var hasCreated = await _authorInPrintingEditionRepository.CreateRangeAsync(authorInPrintingEditions);

            if (!hasCreated)
            {
                resultModel.Errors.Add(errors.Create);
            }

            return resultModel;
        }

        public async Task<BaseModel> RemoveAsync(long id)
        {
            var resultModel = new BaseModel();

            var printingEdition = await _printingEditionRepository.GetByIdAsync(id);

            if (printingEdition == null)
            {
                resultModel.Errors.Add(errors.PINotFound);
                return resultModel;
            }

            var result = await _printingEditionRepository.MarkRemoveAsync(printingEdition);

            if (!result)
            {
                resultModel.Errors.Add(errors.PIRemove);
                return resultModel;
            }

            var wasRemoveAuthorInPrintingEdition = await _authorInPrintingEditionRepository.RemoveRangeAsync(x => x.PrintingEditionId == printingEdition.Id);

            if (!wasRemoveAuthorInPrintingEdition)
            {
                resultModel.Errors.Add(errors.AuthorInPERemove);
            }

            return resultModel;
        }

        public async Task<BaseModel> UpdateAsync(PrintingEditionModelItem printingEditionModelItem)
        {
            var resultModel = new BaseModel();

            var printingEdition = await _printingEditionRepository.GetByIdAsync(printingEditionModelItem.Id);

            if (printingEdition == null || printingEdition.IsRemoved)
            {
                resultModel.Errors.Add(errors.PINotFound);
                return resultModel;
            }

            var wasRemoveAuthorInPrintingEdition = await _authorInPrintingEditionRepository.RemoveRangeAsync(x => x.PrintingEditionId == printingEdition.Id);

            printingEdition = printingEditionModelItem.Map(printingEdition);

            var wasUpdatePrintingEdition = await _printingEditionRepository.UpdateAsync(printingEdition);

            if (!wasUpdatePrintingEdition)
            {
                resultModel.Errors.Add(errors.PIUpdate);
                return resultModel;
            }

            var newAuthorInPE = AuthorInPrintingEditionMapper.Map(printingEdition.Id, printingEditionModelItem.Authors.Items);

            var wasCreateAuthorInPrintingEdition = await _authorInPrintingEditionRepository.CreateRangeAsync(newAuthorInPE);

            if (!wasCreateAuthorInPrintingEdition)
            {
                resultModel.Errors.Add(errors.AuthorInPECreate);
            }

            return resultModel;
        }

        public async Task<PrintingEditionModel> GetPrintingEditionsAsync(PrintingEditionFilterState state)
        {

            var filterModel = state.Map();

            var printingEdition = await _printingEditionRepository.GetFiltredAsync(filterModel);

            var modelItems = new PrintingEditionModel();

            if (printingEdition == null)
            {
                modelItems.Errors.Add(errors.PINotFound);
                return modelItems;
            }

            for (int i = 0; i < printingEdition.Data.Count(); i++)
            {
                modelItems.Items.Add(printingEdition.Data[i].Map(state.CurrencyType));
            }

            modelItems.Count = printingEdition.Count;

            return modelItems;
        }
    }
}




