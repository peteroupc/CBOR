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

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.PODOptions'/> class.</summary>
    /// <param name='paramString'>A string setting forth the options to
    /// use. This is a semicolon-separated list of options, each of which
    /// has a key and a value separated by an equal sign ("="). Whitespace
    /// and line separators are not allowed to appear between the
    /// semicolons or between the equal signs, nor may the string begin or
    /// end with whitespace. The string can be empty, but cannot be null.
    /// The following is an example of this parameter:
    /// <c>usecamelcase=true</c>. The key can be any one of the following
    /// in any combination of case: <c>usecamelcase</c>. Other keys are
    /// ignored. If the same key appears more than once, the value given
    /// for the last such key is used. The key just given can have a value
    /// of <c>1</c>, <c>true</c>, <c>yes</c>, or <c>on</c> (in any
    /// combination of case), which means true, and any other value meaning
    /// false. For example, <c>usecamelcase=Yes</c> and
    /// <c>usecamelcase=1</c> both set the <c>UseCamelCase</c> property to
    /// true.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='paramString'/> is null.</exception>
    public PODOptions(string paramString) {
      if (paramString == null) {
        throw new ArgumentNullException(nameof(paramString));
      }
      var parser = new OptionsParser(paramString);
      this.UseCamelCase = parser.GetBoolean("usecamelcase", true);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.PODOptions.ToString"]/*'/>
    public override string ToString() {
      return new System.Text.StringBuilder()
           .Append("usecamelcase=")
           .Append(this.UseCamelCase ? "true" : "false")
           .ToString();
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
