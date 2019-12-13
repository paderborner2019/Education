﻿using EducationApp.BusinessLogicLayer.Models.Base;
using EducationApp.BusinessLogicLayer.Models.Users;
using System.Threading.Tasks;

namespace EducationApp.BusinessLogicLayer.Services.Interfaces
{
    public interface IAccountService
    {
        Task<BaseModel> ConfirmEmailAsync(string email);
        Task<BaseModel> CreateUserAsync(UserModelItem userItemModel, string password);
        Task<UserModelItem> GetByIdAsync(long Id);
        Task<UserModelItem> GetByEmailAsync(string email);
        Task<BaseModel> SignIn(string email, string password);
        Task SignOutAsync();
    }
}
