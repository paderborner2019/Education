﻿using System.Threading.Tasks;
using EducationApp.BusinessLogicLayer.Extention.User;
using EducationApp.BusinessLogicLayer.Helpers;
using EducationApp.BusinessLogicLayer.Helpers.Mapping;
using EducationApp.BusinessLogicLayer.Helpers.Mapping.User;
using EducationApp.BusinessLogicLayer.Models.Base;
using EducationApp.BusinessLogicLayer.Models.Users;
using EducationApp.DataAccessLayer.Ropositories.Interfaces;
using errors = EducationApp.BusinessLogicLayer.Common.Consts.Consts.Errors;
using emailConsts = EducationApp.BusinessLogicLayer.Common.Consts.Consts.EmailConsts;
using EducationApp.BusinessLogicLayer.Extention.Mapper.UserMapper;

namespace EducationApp.BusinessLogicLayer.Services
{
    class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailSender _emailSender;
        public UserService(IUserRepository userRepository, IEmailSender emailSender)
        {
            _userRepository = userRepository;
            _emailSender = emailSender;
        }

        public async Task<BaseModel> EditProfileAsync(UserProfileEditModel model)
        {
            var resultModel = new BaseModel();

            if (model == null)
            {
                resultModel.Errors.Add(errors.EmptyField);
                return resultModel;
            }

            var user = await _userRepository.GetByIdAsync(model.Id);

            if (user == null)
            {
                resultModel.Errors.Add(errors.NotFound);
                return resultModel;
            }

            var resultChangeEmail = await _userRepository.ChangeEmailAsync(user, model.Email);

            if (!resultChangeEmail)
            {
                resultModel.Errors.Add(errors.ChangeEmailFailure);
                return resultModel;
            }

            var result = await _userRepository.EditAsync(user.Map(model));

            if (!result)
            {
                resultModel.Errors.Add(errors.EditUserFailure);
                return resultModel;
            }

            return resultModel;
        }

        public async Task<BaseModel> RemoveUserAsync(long id)
        {
            var resultModel = new UserModelItem();
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                resultModel.Errors.Add(errors.NotFound);
                return resultModel;
            }

            var result = await _userRepository.RemoveAsync(user);
            
            if (!result)
            {
                resultModel.Errors.Add(errors.Remove);
            }

            return resultModel;
        }

        public async Task<BaseModel> BlockUserAsync(long id)
        {
            var resultModel = new BaseModel();

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                resultModel.Errors.Add(errors.NotFound);
                return resultModel;
            }

            var result = await _userRepository.BlockUserAsync(user);

            if (!result)
            {
                resultModel.Errors.Add(errors.Blok);
            }

            return resultModel;
        }

        public async Task<BaseModel> ChangePassword(long id, string oldPassword,string newPassword)
        {
            var resultModel = new BaseModel();

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                resultModel.Errors.Add(errors.NotFound);
                return resultModel;
            }

            var result = await _userRepository.ResetPasswordAsync(user, newPassword);

            if (!result)
            {

                resultModel.Errors.Add(errors.ChangePasswordFailure);
            }
            return resultModel;

        }


        public async Task<UserModelItem> GetProfileAsync(long id)
        {
            var resultModel = new UserModelItem();

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                resultModel.Errors.Add(errors.NotFound);
                return resultModel;
            }

            resultModel = user.Map();

            return resultModel;
        }

        public async Task<BaseModel> RestorePasswordAsync(string email)
        {
            var resultModel = new BaseModel();

            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
            {
                resultModel.Errors.Add(errors.NotFound);
                return resultModel;
            }
            var newPassword = GeneratePassword.CreateRandomPassword();

            var result = await _userRepository.ResetPasswordAsync(user, newPassword);

            if (!result)
            {
                resultModel.Errors.Add(errors.InvalidToken);
            }

            _emailSender.SendingEmailAsync(user.Email, emailConsts.ResetPassword, newPassword);

            return resultModel;
        }


        public async Task<UserModel> GetUsersAsync(UserFilterModel userFilter)
        {
            var filter = userFilter.Map();

            var users = await _userRepository.GetUsersAsync(filter);

            var usersModel = new UserModel();

            if (users == null)
            {
                usersModel.Errors.Add(errors.NotFound);
                return usersModel;
            }

            for (int i = 0; i < users.Data.Count; i++)
            {
                usersModel.Items.Add(users.Data[i].Map());
            }

            usersModel.Count = users.Count;

            return usersModel;
        }


    }
}
