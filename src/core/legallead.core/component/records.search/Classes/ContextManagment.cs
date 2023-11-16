﻿using System.Reflection;

namespace legallead.records.search.Classes
{
    /// <summary>
    /// Class definition providing access to application metadata
    /// </summary>
    public static class ContextManagment
    {
        /// <summary>
        /// The application directory
        /// </summary>
        private static string _appDirectory;

        /// <summary>
        /// Gets the application directory.
        /// </summary>
        /// <value>
        /// The application directory.
        /// </value>
        public static string AppDirectory
        {
            get
            {
                return _appDirectory ??= GetAppDirectory();
            }
        }

        /// <summary>
        /// Gets the application directory.
        /// </summary>
        /// <returns></returns>
        private static string GetAppDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}