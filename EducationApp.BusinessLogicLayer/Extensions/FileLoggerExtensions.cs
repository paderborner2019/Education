﻿using Microsoft.Extensions.Logging;

namespace EducationApp.BusinessLogicLayer.Common
{
    public static class FileLoggerExtensions
    {
        public static ILoggerFactory AddFile(this ILoggerFactory factory, string filePath)
        {
            factory.AddProvider(new FileLoggerProvider(filePath));
            return factory;
        }
    }
}