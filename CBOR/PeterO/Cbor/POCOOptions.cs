using System;
using System.Collections.Generic;
using System.Text;

namespace PeterO.Cbor
{
    /// <summary>
    ///   Options for converting a plain old c# object to a <see cref="CBORObject"/>.
    /// </summary>
    public static class POCOOptions
    {
        /// <summary>
        ///   Remove leading "Is" from a property name.
        /// </summary>
        /// <value>
        ///   The default is <b>true</b>.
        /// </value>
        public static bool RemoveIsPrefix { get; set; } = true;

        /// <summary>
        ///   Use camelCase for the property name.
        /// </summary>
        public static bool UseCamelCase { get; set; } = true;
    }
}
