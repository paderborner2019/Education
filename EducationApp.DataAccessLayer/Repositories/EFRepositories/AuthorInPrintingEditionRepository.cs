﻿using BookStore.DataAccess.AppContext;
using EducationApp.DataAccessLayer.Entities;
using EducationApp.DataAccessLayer.Ropositories.Base;
using EducationApp.DataAccessLayer.Ropositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EducationApp.DataAccessLayer.Ropositories.EFRepositories
{
    public class AuthorInPrintingEditionRepository : BaseEFRepository<AuthorInPrintingEdition>, IAuthorInPrintingEditionRepository
    {
        public AuthorInPrintingEditionRepository(ApplicationContext applicationContext) : base(applicationContext)
        {

        }
        //todo get Expression as param, remove copypaste
        //todo check for null
        public async Task<bool> RemoveAuthorInPrintingEditionAsync(Expression<Func<AuthorInPrintingEdition, bool>> predicate)
        {
            var authorInPrintingEdition = _applicationContext.AuthorInPrintingEditions.Where(predicate);
            if (authorInPrintingEdition.Count() == 0)
            {
                return false;
            }
            _applicationContext.RemoveRange(authorInPrintingEdition);
            var result = await _applicationContext.SaveChangesAsync();
            return result < 1 ? false : true;

        }

    }
}
