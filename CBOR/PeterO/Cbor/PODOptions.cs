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
      this.UseCamelCase = useCamelCase;
    }

   ///
   /// <summary>The default settings for "plain old data" options.
   /// </summary>
   ///
    public static readonly PODOptions Default = new PODOptions();

   /// <include file='../../docs.xml'
  /// path='docs/doc[@name="P:PeterO.Cbor.PODOptions.UseCamelCase"]/*'/>
    public bool UseCamelCase { get; private set; }
    }
}
