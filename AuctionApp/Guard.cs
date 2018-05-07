using System;
using System.Collections;
using System.Collections.Generic;

namespace AuctionApp
{
    public static class Guard
    {
        /// <summary>
        /// Throws an <see cref="KeyNotFoundException"/> if the value for the given key is not found.
        /// </summary>
        public static void KeyNotFound(IDictionary dictionary, object key)
        {
            if (!dictionary.Contains(key))
            {
                throw new KeyNotFoundException();
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the given value is null.
        /// </summary>
        /// <param name="value">The value to check for nullity.</param>
        /// <param name="name">The name to use when throwing an exception, if necessary.</param>
        public static T NotNull<T>(T value, string name) where T : class
        {
            if (value == null)
                throw new ArgumentNullException(name);
            return value;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the given value is null or
        /// <see cref="ArgumentException"/> if value is empty.
        /// </summary>
        /// <param name="value">The value to check for nullity or empty.</param>
        /// <param name="name">The name to use when throwing an exception, if necessary.</param>
        public static string NotNullOrEmpty(string value, string name)
        {
            if (value == null)
                throw new ArgumentNullException(name);
            if (value == string.Empty)
                throw new ArgumentException("Value is empty", name);
            return value;
        }
    }
}
