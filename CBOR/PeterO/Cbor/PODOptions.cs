using System;

namespace PeterO.Cbor {
   /// <include file='../../docs.xml'
   /// path='docs/doc[@name="T:PeterO.Cbor.PODOptions"]/*'/>
    public class PODOptions {
    ///
    /// <summary>Initializes a new instance of the <see cref='PODOptions'/> class.</summary>
    ///
    public PODOptions() : this(true, true) {
}

   ///
   /// <summary>Initializes a new instance of the <see cref='PODOptions'/> class.</summary><param name='removeIsPrefix'>If set to
   /// <c>true</c> remove is prefix.
   /// </param><param name='useCamelCase'>If set to
   /// <c>true</c> use camel case.
   /// </param>
   ///
    public PODOptions(bool removeIsPrefix, bool useCamelCase) {
      this.UseCamelCase = useCamelCase;
    }

   /// <summary>The default settings for "plain old data" options.</summary>
    public static readonly PODOptions Default = new PODOptions();

   /// <include file='../../docs.xml'
   /// path='docs/doc[@name="P:PeterO.Cbor.PODOptions.UseCamelCase"]/*'/>
    public bool UseCamelCase { get; private set; }
    }
}
