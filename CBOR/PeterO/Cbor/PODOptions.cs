using System;

namespace PeterO.Cbor {
    /// <summary>Options for converting "plain old data" objects to CBOR objects.</summary>
    public class PODOptions {
    /// <summary>Initializes a new instance of the <see cref='PeterO.Cbor.PODOptions'/> class.</summary>
    public PODOptions() : this(true, true) {
}

    /// <summary>Initializes a new instance of the <see cref='PeterO.Cbor.PODOptions'/> class.</summary><param name='removeIsPrefix'>If set to
    /// <c>true
    /// </c>
    /// remove is prefix. NOTE: May be ignored in future versions of this
    /// library.
    /// </param><param name='useCamelCase'>If set to
    /// <c>true
    /// </c>
    /// use camel case.
    /// </param>
    public PODOptions(bool removeIsPrefix, bool useCamelCase) {
        #pragma warning disable 618
      this.RemoveIsPrefix = removeIsPrefix;
        #pragma warning restore 618
      this.UseCamelCase = useCamelCase;
    }

    /// <summary>The default settings for "plain old data" options.</summary>
    public static readonly PODOptions Default = new PODOptions();

    /// <summary>Gets a value indicating whether the "Is" prefix in property names is
    /// removed before they are used as keys.</summary><value><c>true
    /// </c>
    /// If the prefix is removed; otherwise, .
    /// <c>false
    /// </c>
    /// .
    /// </value>
    [Obsolete("Property name conversion may change, making this property" +
"\u0020obsolete.")]
public bool RemoveIsPrefix { get; private set; }

    /// <summary>Gets a value indicating whether property names are converted to camel case
    /// before they are used as keys.</summary><value><c>true
    /// </c>
    /// If the names are converted to camel case; otherwise, .
    /// <c>false
    /// </c>
    /// .
    /// </value>
    public bool UseCamelCase { get; private set; }
    }
}
