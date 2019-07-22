using System;

namespace PeterO.Cbor {
    /// <summary>Options for converting "plain old data" objects (better known as POCOs in
    /// .NET or POJOs in Java) to CBOR objects.</summary>
    public class PODOptions {
    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.PODOptions'/> class.</summary>
    public PODOptions() : this(true, true) {
}

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.PODOptions'/> class.</summary>
    /// <param name='removeIsPrefix'>If set to <c>true</c> remove is
    /// prefix.</param>
    /// <param name='useCamelCase'>If set to <c>true</c> use camel
    /// case.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Usage",
      "CA1801",
      Justification = "'removeIsPrefix' is present for backward compatibility.")]
    public PODOptions(bool removeIsPrefix, bool useCamelCase) {
      this.UseCamelCase = useCamelCase;
    }

    /// <summary>The default settings for "plain old data"
    /// options.</summary>
    public static readonly PODOptions Default = new PODOptions();

    /// <summary><para>Gets a value indicating whether property names are converted to camel
    /// case before they are used as keys. This option changes the behavior of
    /// key name serialization as follows. If "useCamelCase" is
    /// <c>false</c> :
    /// </para>
    /// <list>
    /// <item>In the .NET version, all key names are capitalized, meaning the first
    /// letter in the name is converted to upper case if it's a basic
    /// lower-case letter ("a" to "z"). (For example, "Name" and "IsName" both
    /// remain unchanged.)
    /// </item>
    /// <item>In the Java version, for each eligible method name, the word "get" or
    /// "set" is removed from the name if the name starts with that word, then
    /// the name is capitalized. (For example, "getName" and "setName" both
    /// become "Name", and "isName" becomes "IsName".)
    /// </item>
    /// </list>
    /// <para>If "useCamelCase" is
    /// <c>true</c> :
    /// </para>
    /// <list>
    /// <item>In the .NET version, for each eligible property name, the word "Is" is
    /// removed from the name if the name starts with that word, then the name
    /// is converted to camel case, meaning the first letter in the name is
    /// converted to lower case if it's a basic upper-case letter ("A" to
    /// "Z"). (For example, "Name" and "IsName" both become "name".)
    /// </item>
    /// <item>In the Java version, for each eligible method name, the word "get",
    /// "set", or "is" is removed from the name if the name starts with that
    /// word, then the name is converted to camel case. (For example,
    /// "getName", "setName", and "isName" all become "name".)
    /// </item>
    /// </list>
    /// <para>In the description above, a name "starts with" a word if that word
    /// begins the name and is followed by a character other than a basic digit
    /// or lower-case letter, that is, other than "a" to "z" or "0" to "9".
    /// </para></summary><value><c>true</c> If the names are converted to camel case; otherwise,
    /// <c>false</c> . This property is
    /// <c>true</c> by default.
    /// </value>
    public bool UseCamelCase { get; private set; }
    }
}
