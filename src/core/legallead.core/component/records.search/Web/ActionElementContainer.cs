﻿using StructureMap;

namespace legallead.records.search.Web
{
    /// <summary>
    /// Class definition to supply DI container needed
    /// to access action element automation objects
    /// </summary>
    public static class ActionElementContainer
    {
        private static Container _container;

        /// <summary>
        /// Gets the get container.
        /// </summary>
        /// <value>
        /// The container of registered interfaces.
        /// </value>
        public static Container GetContainer
        {
            get
            {
                return _container ??=
                  new Container(new ActionElementRegistry());
            }
        }
    }
}