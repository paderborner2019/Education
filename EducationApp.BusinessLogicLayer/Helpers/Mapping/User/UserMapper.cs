﻿using EducationApp.BusinessLogicLayer.Extention.User;
using EducationApp.BusinessLogicLayer.Models.Users;
using EducationApp.DataAccessLayer.Entities;
using EducationApp.DataAccessLayer.Helpers;
using static EducationApp.DataAccessLayer.Entities.Enums.Enums;

namespace EducationApp.BusinessLogicLayer.Helpers.Mapping
{
    public static class UserMapper
    {
        public static ApplicationUser Map(this UserItemModel model)
        {
            ApplicationUser user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Id = model.Id,
                SecurityStamp = model.SecurityStamp,
                AccessFailedCount = model.AccessFailedCount,
                ConcurrencyStamp = model.ConcurrencyStamp,
                IsRemoved = model.IsRemoved,
                LockoutEnabled = model.LockoutEnabled,
                UserName = model.UserName
            };
            return user;
        }

        public static UserItemModel Map(this ApplicationUser user)
        {
            UserItemModel userItemModel = new UserItemModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Id = user.Id,
                SecurityStamp = user.SecurityStamp,
                AccessFailedCount = user.AccessFailedCount,
                ConcurrencyStamp = user.ConcurrencyStamp,
                IsRemoved = user.IsRemoved,
                LockoutEnabled = user.LockoutEnabled,
                UserName = user.UserName
            };
            return userItemModel;
        }
        public static UsersFilter Map(this UserFilterModel filterUser)
        {
            var userFilter = new UsersFilter
            {
                UsersSortType = (DataAccessLayer.Entities.Enums.Enums.UsersSortType)filterUser.UserSortType,
                UsersFilterType = (DataAccessLayer.Entities.Enums.Enums.UsersFilterType)filterUser.UsersFilterStatus
            };
            return userFilter;
        }
    }
}