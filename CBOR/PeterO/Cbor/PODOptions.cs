using System;

namespace PeterO.Cbor {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Cbor.PODOptions"]/*'/>
    public class PODOptions {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.PODOptions.#ctor"]/*'/>
    public PODOptions() : this(true, true) {
}

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.PODOptions.#ctor(System.Boolean,System.Boolean)"]/*'/>
    public PODOptions(bool removeIsPrefix, bool useCamelCase) {
      this.RemoveIsPrefix = removeIsPrefix;
      this.UseCamelCase = useCamelCase;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Cbor.PODOptions.Default"]/*'/>
    public static readonly PODOptions Default = new PODOptions();

    /// <summary>Gets a value indicating whether the "Is" prefix in
    /// property names is removed before they are used as keys.</summary>
    /// <value><c>true</c> If the prefix is removed; otherwise,.
    /// <c>false</c>.</value>
        public bool RemoveIsPrefix { get; private set; }

    /// <summary>Gets a value indicating whether property names are
    /// converted to camel case before they are used as keys.</summary>
    /// <value><c>true</c> If the names are converted to camel case;
    /// otherwise,. <c>false</c>.</value>
    public bool UseCamelCase { get; private set; }
    }
}
