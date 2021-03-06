using BookStore.DataAccess.AppContext;
using EducationApp.DataAccessLayer.Entities;
using EducationApp.DataAccessLayer.Helpers;
using EducationApp.DataAccessLayer.Ropositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Linq.Dynamic.Core;
using System.Linq;
using System.Threading.Tasks;
using initialData = EducationApp.DataAccessLayer.Entities.Constants.Constants.InitialData;
using errors = EducationApp.DataAccessLayer.Entities.Constants.Constants.Errors;
using static EducationApp.DataAccessLayer.Entities.Enums.Enums;
using Microsoft.EntityFrameworkCore;
using EducationApp.DataAccessLayer.Models.Base;

namespace EducationApp.DataAccessLayer.Ropositories.EFRepositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationContext _applicationContext;

        public UserRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationContext applicationContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationContext = applicationContext;
        }
        public async Task<string> CreateUserAsync(ApplicationUser user, string password)
        {
            var excistUser = await _userManager.FindByEmailAsync(user.Email);

            if (excistUser != null)
            {
                return errors.EmailBusy;
            }
            
            var createUser = await _userManager.CreateAsync(user, password);
            if (!createUser.Succeeded)
            {
                var error = createUser.Errors.Select(k => k.Description).FirstOrDefault();
                return error;
            }
            
            var result = await _userManager.AddToRoleAsync(user, initialData.User);

            if (!result.Succeeded)
            {
                return errors.RoleNotAssigned;
            }

            return errors.Ok;
        }
        public async Task<bool> EditAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }

        public async Task<bool> RemoveAsync(ApplicationUser user)
        {
            user.IsRemoved = false;

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }
        public async Task<string> CheckRoleAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return null;
            }

            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            return userRole;
        }


        public async Task<ApplicationUser> GetByIdAsync(long id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            return user;
        }

        public async Task<ApplicationUser> GetByNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            return user;
        }

        public async Task<bool> ChangePasswordAsync(ApplicationUser user, string oldPassword, string newPassword)
        {
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

            return changePasswordResult.Succeeded;
        }

        public async Task<ApplicationUser> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            var result = await _userManager.GeneratePasswordResetTokenAsync(user);

            return result;
        }

        public async Task<ApplicationUser> SignInAsync(ApplicationUser user,string password)
        {
            await _signInManager.SignInAsync(user,isPersistent:false);

            return user;
        }


        public async Task<bool> ChangeEmailAsync(ApplicationUser user, string newEmail)
        {
            var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);

            var changeEmailResult = await _userManager.ChangeEmailAsync(user, newEmail, token);

            return changeEmailResult.Succeeded;
        }

        public async Task<bool> ConfirmEmailAsync(ApplicationUser user)
        {
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var result = await _userManager.ConfirmEmailAsync(user, token);

            return result.Succeeded;
        }

        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> ConfirmPasswordAsync(ApplicationUser user, string password)
        {
            var result = await _userManager.CheckPasswordAsync(user, password);

            return result;
        }


        public async Task SignOut()
        {
            await _signInManager.SignOutAsync();
        }
        public async Task<bool> ResetPasswordAsync(ApplicationUser user, string newPassword)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            return result.Succeeded;
        }

        public async Task<string> GetRoleAsync(ApplicationUser user)
        {
            var role = await _userManager.GetRolesAsync(user);

            return role.FirstOrDefault();
        }

        public async Task<bool> BlockAsync(ApplicationUser user)
        {
            user.LockoutEnabled = !user.LockoutEnabled;

            var result = await _applicationContext.SaveChangesAsync();

            return result < 1 ? false : true;
        }

        public async Task<ResponseModel<ApplicationUser>> GetFilteredAsync(UserFilterModel usersFilter)
        {
            var users = _applicationContext.Users.Where(k => k.IsRemoved == false);

            if (!string.IsNullOrWhiteSpace(usersFilter.SearchString))
            {
                users = users.Where(k => k.LastName.ToLower().Contains(usersFilter.SearchString.ToLower()) || k.FirstName.ToLower().Contains(usersFilter.SearchString.ToLower()));
                usersFilter.PageNumber = 1;
            }

           
            if (usersFilter.FilterType != UserFilterType.All)
            {
                users = usersFilter.FilterType == UserFilterType.Active ? users.Where(k => k.LockoutEnabled == true) : users.Where(k => k.LockoutEnabled == false);
            }

            var property = typeof(ApplicationUser).GetProperty(usersFilter.ColumnType.ToString());
            if (property != null)
            {
                users = usersFilter.SortType == SortType.Asc ? users.OrderBy(property.Name, usersFilter.ColumnType.ToString()) : users.OrderBy(property.Name + " descending");
            }


            var count = users.Count();

            users = users.Skip((usersFilter.PageNumber - 1) * usersFilter.PageSize).Take(usersFilter.PageSize);

            var presentationModel = new ResponseModel<ApplicationUser> { Data = await users.ToListAsync(), Count = count };

            return presentationModel;
        }


    }
}



