CBOR
====

A C# and Java implementation of Concise Binary Object Representation, a general-purpose binary data format defined in RFC 7049. According to that RFC, CBOR's data model "is an extended version of the JSON data model", supporting many more types of data than JSON. "CBOR was inspired by MessagePack", but "is not intended as a version of or replacement for MessagePack."

This implementation was written by Peter O. and is released to the Public Domain under the [CC0 Declaration](http://creativecommons.org/publicdomain/zero/1.0/).

This implementation also doubles as a reader and writer of JSON, and can convert data from JSON to CBOR and back.

Finally, this implementation supports arbitrary-precision binary and decimal floating-point numbers and rational numbers with arbitrary-precision components.

How to Install
---------
Starting with version 0.21.0, the C# implementation is available in the
NuGet Package Gallery under the name
[PeterO.Cbor](https://www.nuget.org/packages/PeterO.Cbor). To install
this library as a NuGet package, enter `Install-Package PeterO.Cbor` in the
NuGet Package Manager Console.

Starting with version 0.23.0, the Java implementation is available
as an [artifact](https://search.maven.org/#search|ga|1|g%3A%22com.upokecenter%22%20AND%20a%3A%22cbor%22) in the Central Repository. To add this library to a Maven
project, add the following to the `dependencies` section in your `pom.xml` file:

```xml
<dependency>
  <groupId>com.upokecenter</groupId>
  <artifactId>cbor</artifactId>
  <version>1.3.0</version>
</dependency>
```

In other Java-based environments, the library can be referred to by its
group ID (`com.upokecenter`), artifact ID (`cbor`), and version, as given above.

Documentation
------------

This library defines one class, called CBORObject, that allows you to read and
write CBOR objects to and from data streams and byte arrays, and to convert JSON
text to CBOR objects and back.

See the [Wiki](https://github.com/peteroupc/CBOR/wiki) for Java API documentation.

See [docs/APIDocs.md](https://github.com/peteroupc/CBOR/blob/master/docs/APIDocs.md) for C# (.NET) API documentation.

More About the Different Versions
-----------

This repository contains code in two languages: C# and Java.
C# is the main language of the project, and the C# implementation has the most features.  

The C# implementation is designed as a Portable Class Library, making it usable not only in the .NET
Framework, but also Silverlight 5 and Windows Phone 8.

The Java version is a translation from the C# version. It contains almost as many features as the C# version
and has all the important ones, such as reading and writing CBOR objects,
CBOR/JSON conversion, and support for decimal fractions and bigfloats.

Examples
----------

This code is in C#, but the Java version of the code would be very similar.

```c#
// The following creates a CBOR map and adds
// several kinds of objects to it
var cbor = CBORObject.NewMap()
   .Add("item", "any string")
   .Add("number", 42)
   .Add("map", CBORObject.NewMap().Add("number", 42))
   .Add("array", CBORObject.NewArray().Add(999f).Add("xyz"))
   .Add("bytes", new byte[] { 0, 1, 2 });
// The following converts the map to CBOR
byte[] bytes = cbor.EncodeToBytes();
// The following converts the map to JSON
string json = cbor.ToJSONString();
Console.WriteLine(json);
```

Reading data from a file (C#).  Note that all the examples for
reading and writing to files assume that the platform supports
file I/O; the portable class library doesn't make that assumption.

```c#
 // Read all the bytes from a file and decode the CBOR object
 // from it.  However, there are two disadvantages to this approach:
 // 1.  The byte array might be very huge, so a lot of memory to store
 // the array may be needed.
 // 2.  The decoding will succeed only if the entire array,
 // not just the start of the array, consists of a CBOR object.
 var cbor = CBORObject.DecodeFromBytes(File.ReadAllBytes("object.cbor"));
```

Another example of reading data from a file:

```c#
 // C#
 // Open the file stream
 using (var stream = new FileStream("object.cbor", FileMode.Open)) {
    // Read the CBOR object from the stream
    var cbor = CBORObject.Read(stream);
    // At this point, the object is read, but the file stream might
    // not have ended yet.  Here, the code may choose to read another
    // CBOR object, check for the end of the stream, or just ignore the
    // rest of the file.  The following is an example of checking for the
    // end of the stream.
    if (stream.Position != stream.Length) {
      // The end of the stream wasn't reached yet.
    } else {
      // The end of the stream was reached.
    }
 }
```

```java
 // Java
 // Open the file stream
 try (FileInputStream stream = new FileInputStream("object.cbor")) {
    // Read the CBOR object from the stream
    var cbor = CBORObject.Read(stream);
    // At this point, the object is read, but the file stream might
    // not have ended yet.  Here, the code may choose to read another
    // CBOR object, check for the end of the stream, or just ignore the
    // rest of the file.  The following is an example of checking for the
    // end of the stream.
    if (stream.getChannel().position() != stream.getChannel().size()) {
      // The end of the stream wasn't reached yet.
    } else {
      // The end of the stream was reached.
    }
 }
```

Writing CBOR data to a file (C#):

```c#
// This example assumes that the variable "cbor" refers
// to a CBORObject object.
using (var stream = new FileStream("object.cbor", FileMode.Create)) {
   cbor.WriteTo(stream);
}
```

Writing multiple objects to a file, including arbitrary objects:

```c#
// C#
// This example writes different kinds of objects in CBOR
// format to the same file.
using (var stream = new FileStream("object.cbor", FileMode.Create)) {
   CBORObject.Write(true, stream);
   CBORObject.Write(422.5, stream);
   CBORObject.Write("some string", stream);
   CBORObject.Write(CBORObject.Undefined, stream);
   CBORObject.NewArray().Add(42).WriteTo(stream);
}
```

```java
// Java
// This example uses the "try-with-resources" statement from Java 7.
// This example writes different kinds of objects in CBOR
// format to the same file.
try (FileOutputStream stream = new FileOutputStream("object.cbor")) {
   CBORObject.Write(true, stream);
   CBORObject.Write(422.5, stream);
   CBORObject.Write("some string", stream);
   CBORObject.Write(CBORObject.Undefined, stream);
   CBORObject.NewArray().Add(42).WriteTo(stream);
}
```

Reading JSON from a file:

```c#
 // Open the file stream
 using (var stream = new FileStream("object.json", FileMode.Open)) {
    // Read the JSON object from the stream
    // as a CBOR object
    var cbor = CBORObject.ReadJSON(stream);
 }
```

Writing a CBOR object as JSON:

```c#
// This example assumes that the variable "cbor" refers
// to a CBORObject object.
// NOTE: Specifying Encoding.UTF8 as the third parameter
// would add a byte order mark to the beginning of the text, 
// but conforming JSON implementations are forbidden from 
// adding it this way in JSON texts they generate.
File.WriteAllText(
  "object.json",
  cbor.ToJSONString(),
  new System.Text.Encoding.UTF8Encoding(false));

// This is an alternative way to write the CBOR object
// and is now supported in version 1.2.
using (var stream = new FileStream("object2.json", FileMode.Create)) {
    // Write the CBOR object as JSON; here, a byte order
    // mark won't be added
    cbor.WriteJSONTo(stream);
}
// Version 1.2 now supports a third way to write
// objects to JSON: the CBORObject.WriteJSON method
using (var stream = new FileStream("object3.json", FileMode.Create)) {
   CBORObject.WriteJSON("some string", stream);
}
using (var stream = new FileStream("object4.json", FileMode.Create)) {
   CBORObject.WriteJSON(cbor, stream);
}
using (var stream = new FileStream("object5.json", FileMode.Create)) {
   CBORObject.WriteJSON(true, stream);
}
using (var stream = new FileStream("object6.json", FileMode.Create)) {
   CBORObject.WriteJSON(42, stream);
}
```



NOTE: All code samples in this section are released to the Public Domain,
as explained in <http://creativecommons.org/publicdomain/zero/1.0/>.

Source Code
---------
Source code is available in the [project page](https://github.com/peteroupc/CBOR).

About
-----------

Written in 2013-2014 by Peter O.

Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/

Clarifications
------------------

The following are some clarifications to RFC 7049.

* Section 2.4.2 doesn't specify what happens if a bignum's byte
  string has a length of 0.  This implementation treats a positive
  bignum with length 0 as having a value of 0 and a negative
  bignum with length 0 as having a value of -1.
* Section 2.4.1 specifies the number of seconds since the start of 1970.  It is
  based on the POSIX definition of "seconds since the Epoch", which
  the RFC cites as a normative reference.  This definition does not
  count leap seconds.  When this implementation supports date
  conversion, it won't count leap seconds, either.  This implementation
  treats values of infinity and NaN as invalid.
* For tag 32, this implementation accepts strings that are valid
  Internationalized Resource Identifiers (IRIs) in addition to URIs.
  IRI are like URIs except that they also allow non-ASCII characters.

Release Notes
-----------

In version 1.3:

- Added a CompareToIgnoreTags method to CBORObject
- The BigInteger constructor in the C# version is deprecated
- Fixed bugs in converting from CBOR float and double to integers in some corner cases
- Fixed a bug where CBORObject's OutermostTag returns 0 instead of the correct -1 for untagged objects
- Fixed a bug where BigInteger's bitLength return value can be wrong in some corner cases

In version 1.2:

- The WriteJSON and WriteToJSON methods were added to CBORObject
- Bugs were fixed in the Set and Add methods of CBORObject

In version 1.1 there were many additions and bug fixes in arbitrary-precision
arithmetic, including:

- Added unchecked versions of intValue and longValue in BigInteger
- Added more overloads for FromString in ExtendedDecimal and ExtendedFloat
- Fixed bug where Pow doesn't compute the exact value in unlimited precision contexts
- Much added documentation

In version 1.0, the "adjust exponent" and "is precision in bits" flags were added to
the arbitrary-precision arithmetic contexts, and a new Set method that is similar
to Add but can replace a key's value in a CBOR map. Some bugs were also fixed.

Version 0.23.0 has no new features of note.

Version 0.22.0 adds CBOR support for decimal fractions and big floats with any 
exponent, even exponents higher than 65 bits, and implements well-formedness 
checking for tag 32 (URIs).  Several bugs were also fixed.   

The [commit history](https://github.com/peteroupc/CBOR/commits/master) 
contains details on code changes in previous versions.

Specifications
-----------
Here are specifications by this implementation's author on proposed
CBOR tags:

* Tag 30: [Rational numbers](http://peteroupc.github.io/CBOR/rational.html)
* Tag 257: [Binary MIME messages](http://peteroupc.github.io/CBOR/binarymime.html)
* Tag 38: [Language-tagged strings](http://peteroupc.github.io/CBOR/langtags.html)
* Tag 264 and 265: [Arbitrary-exponent numbers](http://peteroupc.github.io/CBOR/bigfrac.html)

Acknowledgments
-----------

* Carsten Bormann reviewed this library and gave helpful suggestions.
* Anders Gustafsson converted this library to a Portable Class Library.