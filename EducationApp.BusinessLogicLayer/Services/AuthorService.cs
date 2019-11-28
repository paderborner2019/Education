﻿using EducationApp.BusinessLogicLayer.Extention.Author;
using EducationApp.BusinessLogicLayer.Helpers.Mapping.Authors;
using EducationApp.BusinessLogicLayer.Models.Authors;
using EducationApp.BusinessLogicLayer.Models.Base;
using EducationApp.BusinessLogicLayer.Services.Interfaces;
using EducationApp.DataAccessLayer.Ropositories.Interfaces;
using System.Threading.Tasks;
using errors = EducationApp.BusinessLogicLayer.Common.Consts.Consts.Errors;

namespace EducationApp.BusinessLogicLayer.Services
{
    public class AuthorService : IAuthorService
    {

        private readonly IAuthorRepository _authorRepository;
        private readonly IAuthorInPrintingEditionRepository _authorInPrintingEditionRepository;
        public AuthorService(IAuthorRepository authorRepository,IAuthorInPrintingEditionRepository authorInPrintingEditionRepository)
        {
            _authorRepository = authorRepository;
            _authorInPrintingEditionRepository = authorInPrintingEditionRepository;
        }

        public async Task<BaseModel> CreateAsync(AuthorModelItem authorModelItem)
        {
            var resultModel = new BaseModel();
            var author = AuthorsMapper.Map(authorModelItem);
            var resultCreate = await _authorRepository.CreateAsync(author);
            if (resultCreate < 1)
            {
                resultModel.Errors.Add(errors.AuthorCreate);
            }
            return resultModel;
        }

        public async Task<BaseModel> UpdateAsync(long id)
        {
            var resultModel = new BaseModel();
            var author = await _authorRepository.FindByIdAsync(id);
            if (author == null)
            {
                resultModel.Errors.Add(errors.AuthorNotFound);
                return resultModel;
            }
            var wasUpdateAuthor = await _authorRepository.UpdateAsync(author);
            if (!wasUpdateAuthor)
            {
                resultModel.Errors.Add(errors.AuthorUpdate);
            }
            return resultModel;
        }

        public async Task<BaseModel> RemoveAsync(long id)
        {
            var resultModel = new BaseModel();
            var author = await _authorRepository.FindByIdAsync(id);
            if (author == null)
            {
                resultModel.Errors.Add(errors.AuthorNotFound);
                return resultModel;
            }
            var wasRemoveAuthor = await _authorRepository.RemoveAsync(author); //todo rename to result
            if (!wasRemoveAuthor)
            {
                resultModel.Errors.Add(errors.AuthorRemove);
                return resultModel;
            }
            var wasRemoveAuthorInPrintingEdition = await _authorInPrintingEditionRepository.RemoveAuthorInPrintingEditionAsync(x=>x.AuthorId == author.Id ); //todo rename
            if (!wasRemoveAuthorInPrintingEdition)
            {
                resultModel.Errors.Add(errors.PIRemove);
            }
            return resultModel;
        }

        public async Task<AuthorModel> GetAuthorsAsync(AuthorFilterModel authorFilterModel)
        {
            var filter = AuthorsMapper.Map(authorFilterModel);
            var authors = await _authorRepository.GetAuthorsAsync(filter);
            var authorsModel = new AuthorModel();
            if (authors == null)
            {
                authorsModel.Errors.Add(errors.AuthorNotFound);
                return authorsModel;
            }
            for (int i = 0; i < authors.Data.Count; i++)
            {
                authorsModel.Items.Add(AuthorsMapper.Map(authors.Data[i]));
            }
            authorsModel.Count = authors.Count;
            return authorsModel;
        }
    }
}
