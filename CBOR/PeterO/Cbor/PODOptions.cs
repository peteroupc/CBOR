using System;

namespace PeterO.Cbor {
  /// <summary>Options for controlling how certain DotNET or Java
  /// objects, such as so-called "plain old data" objects (better known
  /// as POCOs in DotNET or POJOs in Java), are converted to CBOR
  /// objects.</summary>
  public class PODOptions {
    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.PODOptions'/> class with all the default
    /// options.</summary>
    public PODOptions() : this(String.Empty) {
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.PODOptions'/> class.</summary>
    /// <param name='removeIsPrefix'>The parameter is not used.</param>
    /// <param name='useCamelCase'>The value of the "UseCamelCase"
    /// property.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Usage",
      "CA1801",
      Justification = "'removeIsPrefix' is present for backward compatibility.")]
    [Obsolete("Use the more readable string constructor instead.")]
    public PODOptions(bool removeIsPrefix, bool useCamelCase) {
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
    /// where the letters can be any combination of basic upper-case and/or
    /// basic lower-case letters: <c>usecamelcase</c>. Other keys are
    /// ignored in this version of the CBOR library. (Keys are compared
    /// using a basic case-insensitive comparison, in which two strings are
    /// equal if they match after converting the basic upper-case letters A
    /// to Z (U+0041 to U+005A) in both strings to basic lower-case
    /// letters.) If two or more key/value pairs have equal keys (in a
    /// basic case-insensitive comparison), the value given for the last
    /// such key is used. The key just given can have a value of <c>1</c>,
    /// <c>true</c>, <c>yes</c>, or <c>on</c> (where the letters can be
    /// any combination of basic upper-case and/or basic lower-case
    /// letters), which means true, and any other value meaning false. For
    /// example, <c>usecamelcase=Yes</c> and <c>usecamelcase=1</c> both set
    /// the <c>UseCamelCase</c> property to true. In the future, this class
    /// may allow other keys to store other kinds of values, not just true
    /// or false.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='paramString'/> is null.</exception>
    public PODOptions(string paramString) {
      if (paramString == null) {
        throw new ArgumentNullException(nameof(paramString));
      }
      var parser = new OptionsParser(paramString);
      this.UseCamelCase = parser.GetBoolean("usecamelcase", true);
    }

    /// <summary>Gets the values of this options object's properties in
    /// text form.</summary>
    /// <returns>A text string containing the values of this options
    /// object's properties. The format of the string is the same as the
    /// one described in the String constructor for this class.</returns>
    public override string ToString() {
      return new System.Text.StringBuilder()
        .Append("usecamelcase=").Append(this.UseCamelCase ? "true" :
"false")
        .ToString();
    }

    /// <summary>The default settings for "plain old data"
    /// options.</summary>
    public static readonly PODOptions Default = new PODOptions();

    /// <summary>
    /// <para>Gets a value indicating whether property, field, and method
    /// names are converted to camel case before they are used as keys.
    /// This option changes the behavior of key name serialization as
    /// follows. If "useCamelCase" is <c>false</c> :</para>
    /// <list>
    /// <item>In the .NET version, all key names are capitalized, meaning
    /// the first letter in the name is converted to a basic upper-case
    /// letter if it's a basic lower-case letter ("a" to "z"). (For
    /// example, "Name" and "IsName" both remain unchanged.)</item>
    /// <item>In the Java version, all field names are capitalized, and for
    /// each eligible method name, the word "get" or "set" is removed from
    /// the name if the name starts with that word, then the name is
    /// capitalized. (For example, "getName" and "setName" both become
    /// "Name", and "isName" becomes "IsName".)</item></list>
    /// <para>If "useCamelCase" is <c>true</c> :</para>
    /// <list>
    /// <item>In the .NET version, for each eligible property or field
    /// name, the word "Is" is removed from the name if the name starts
    /// with that word, then the name is converted to camel case, meaning
    /// the first letter in the name is converted to a basic lower-case
    /// letter if it's a basic upper-case letter ("A" to "Z"). (For
    /// example, "Name" and "IsName" both become "name", and "IsIsName"
    /// becomes "isName".)</item>
    /// <item>In the Java version: For each eligible method name, the word
    /// "get", "set", or "is" is removed from the name if the name starts
    /// with that word, then the name is converted to camel case. (For
    /// example, "getName", "setName", and "isName" all become "name".) For
    /// each eligible field name, the word "is" is removed from the name if
    /// the name starts with that word, then the name is converted to camel
    /// case. (For example, "name" and "isName" both become
    /// "name".)</item></list>
    /// <para>In the description above, a name "starts with" a word if that
    /// word begins the name and is followed by a character other than a
    /// basic digit or basic lower-case letter, that is, other than "a" to
    /// "z" or "0" to "9".</para></summary>
    /// <value><c>true</c> If the names are converted to camel case;
    /// otherwise, <c>false</c>. This property is <c>true</c> by
    /// default.</value>
    public bool UseCamelCase
    {
      get;
      private set;
    }
  }
}
