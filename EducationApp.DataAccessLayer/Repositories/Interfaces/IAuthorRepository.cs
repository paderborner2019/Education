﻿using EducationApp.DataAccessLayer.Entities;
using EducationApp.DataAccessLayer.Helpers.Author;
using EducationApp.DataAccessLayer.Models.Base;
using EducationApp.DataAccessLayer.Repositories.Interfaces;
using System.Threading.Tasks;

namespace EducationApp.DataAccessLayer.Ropositories.Interfaces
{
    public interface IAuthorRepository : IBaseEFRRepository<Author>
    {
        Task<ResponseModel<Author>> GetFiltredAsync(AuthorFilterModel authorFilterModel);
    }
}
