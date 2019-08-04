using System;

namespace PeterO.Cbor {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Cbor.PODOptions"]/*'/>
    public class PODOptions {
    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.PODOptions'/> class.</summary>
    public PODOptions() : this(true, true) {
}

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.PODOptions'/> class.</summary>
    /// <param name='removeIsPrefix'>If set to <c>true</c> remove is
    /// prefix. NOTE: May be ignored in future versions of this
    /// library.</param>
    /// <param name='useCamelCase'>If set to <c>true</c> use camel
    /// case.</param>
    public PODOptions(bool removeIsPrefix, bool useCamelCase) {
        #pragma warning disable 618
      this.RemoveIsPrefix = removeIsPrefix;
        #pragma warning restore 618
      this.UseCamelCase = useCamelCase;
    }

    /// <summary>The default settings for "plain old data"
    /// options.</summary>
    public static readonly PODOptions Default = new PODOptions();

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="P:PeterO.Cbor.PODOptions.RemoveIsPrefix"]/*'/>
    [Obsolete("Property name conversion may change, making this property" +
"\u0020obsolete.")]
public bool RemoveIsPrefix { get; private set; }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.PODOptions.UseCamelCase"]/*'/>
    public bool UseCamelCase { get; private set; }
    }
}
