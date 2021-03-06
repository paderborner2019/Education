using EducationApp.BusinessLogicLayer.Helpers;
using EducationApp.BusinessLogicLayer.Helpers.Mapping;
using EducationApp.BusinessLogicLayer.Models.Base;
using EducationApp.BusinessLogicLayer.Models.Users;
using EducationApp.BusinessLogicLayer.Services.Interfaces;
using EducationApp.DataAccessLayer.Ropositories.Interfaces;
using System.Threading.Tasks;
using error = EducationApp.BusinessLogicLayer.Common.Consts.Constants.Errors;
using EducationApp.BusinessLogicLayer.Extention.Mapper.UserMapper;
using password = EducationApp.BusinessLogicLayer.Common.Consts.Constants.RandomPassword;
using EducationApp.BusinessLogicLayer.Helpers.GeneratePassword;

namespace EducationApp.BusinessLogicLayer.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailSender _emailSender;
        public AccountService(IUserRepository userRepository, IEmailSender emailSender)
        {
            _userRepository = userRepository;
            _emailSender = emailSender;
            
        }

        public async Task<BaseModel> CreateUserAsync(UserModelItem userModelItem)
        {
            var userModel = new BaseModel();

            if (userModelItem == null)
            {
                userModel.Errors.Add(error.EmptyField);
                return userModel;
            }

            var userEntity = userModelItem.Map();

            var result = await _userRepository.CreateUserAsync(userEntity,userModelItem.Password);

            if (result != error.Ok)
            {
                userModel.Errors.Add(result);
                return userModel;
            }

           // _emailSender.SendingEmailAsync(userModelItem.Email);    

            return userModel;
        }

        public async Task<BaseModel> RestorePasswordAsync(string email)
        {
            var resultModel = new BaseModel();

            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
            {
                resultModel.Errors.Add(error.Email + email + error.NotFound);
                return resultModel;
            }
            var newPassword = GeneratePassword.CreateRandomPassword(password.PasswordLength);

            var result = await _userRepository.ResetPasswordAsync(user, newPassword);

            if (!result)
            {
                resultModel.Errors.Add(error.InvalidToken);
            }

            _emailSender.SendingEmailAsync(user.Email);

            return resultModel;
        }

        public async Task<BaseModel> ConfirmEmailAsync(string email)
        {
            var userModel = new BaseModel();

            var user = await _userRepository.FindByEmailAsync(email);
            
            if (user == null)
            {
                userModel.Errors.Add(error.UserNotFound);
                return userModel;
            
            }
            //
            
            var result = await _userRepository.ConfirmEmailAsync(user);
            
            if (!result)
            {
                userModel.Errors.Add(error.ConfirmEmail);
            
            }
            return userModel;
        }

        public async Task<UserModelItem> GetByIdAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            var role = await _userRepository.GetRoleAsync(user);

            var userModel = user.Map();

            userModel.Role = role;

            return userModel;
        }

        public async Task<UserModelItem> GetByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }
            var user = await _userRepository.FindByEmailAsync(email);
            var userModel = user.Map();
            userModel.Role = await _userRepository.GetRoleAsync(user);
            return userModel;
        }

        public async Task<UserModelItem> SignInAsync(string email, string password)
        {
            var userModel = new UserModelItem();
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                userModel.Errors.Add(error.EmptyField);
                return userModel;
            }
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                userModel.Errors.Add(error.UserNotFound);
                return userModel;
            }

            var wasPasswordValid = await _userRepository.ConfirmPasswordAsync(user, password);
            if (!wasPasswordValid)
            {
                userModel.Errors.Add(error.NotValidPassword);
                return userModel;
            }
            var result = await _userRepository.SignInAsync(user, password);
            if (result == null)
            {
                userModel.Errors.Add(error.NotValidPassword);
            }
            userModel = result.Map();
            return userModel;
        }

        public async Task SignOutAsync()
        {
            await _userRepository.SignOut();
        }
    }
}
