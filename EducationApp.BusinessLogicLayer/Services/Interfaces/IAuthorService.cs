﻿using EducationApp.BusinessLogicLayer.Extention.Author;
using EducationApp.BusinessLogicLayer.Models.Authors;
using EducationApp.BusinessLogicLayer.Models.Base;
using System.Threading.Tasks;

namespace EducationApp.BusinessLogicLayer.Services.Interfaces
{
    public interface IAuthorService 
    {
        Task<BaseModel> CreateAsync(AuthorModelItem authorModelItem);
        Task<BaseModel> UpdateAsync(long id,string name);
        Task<BaseModel> RemoveAsync(long id);
        Task<AuthorModel> GetAuthorsAsync(AuthorFilterModel authorFilterModel);
    }
}
