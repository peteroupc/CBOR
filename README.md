CBOR
====

[![NuGet Status](http://img.shields.io/nuget/v/PeterO.Cbor.svg?style=flat)](https://www.nuget.org/packages/PeterO.Cbor)

**Download source code: [ZIP file](https://github.com/peteroupc/CBOR/archive/master.zip)**

If you like this software, consider donating to me at this link: [http://peteroupc.github.io/](http://peteroupc.github.io/)

----

A C# implementation of Concise Binary Object Representation, a general-purpose binary data format defined in RFC 7049. According to that RFC, CBOR's data model "is an extended version of the JSON data model", supporting many more types of data than JSON. "CBOR was inspired by MessagePack", but "is not intended as a version of or replacement for MessagePack."

This implementation was written by Peter O. and is released to the Public Domain under the [CC0 Declaration](http://creativecommons.org/publicdomain/zero/1.0/).

This implementation also doubles as a reader and writer of JSON, and can convert data from JSON to CBOR and back.

Finally, this implementation supports arbitrary-precision binary and decimal floating-point numbers and rational numbers with arbitrary-precision components.

Source code is available in the [project page](https://github.com/peteroupc/CBOR).

How to Install
---------
Starting with version 0.21.0, the C# implementation is available in the
NuGet Package Gallery under the name
[PeterO.Cbor](https://www.nuget.org/packages/PeterO.Cbor). To install
this library as a NuGet package, enter `Install-Package PeterO.Cbor` in the
NuGet Package Manager Console.

Documentation
------------

This library defines one class, called CBORObject, that allows you to read and
write CBOR objects to and from data streams and byte arrays, and to convert JSON
text to CBOR objects and back.

**See the [C# (.NET) API documentation](https://peteroupc.github.io/CBOR/docs/).**

The C# implementation is designed as a Portable Class Library.

Other Sites
----------

* CodePlex: [https://peterocbor.codeplex.com/](https://peterocbor.codeplex.com/)
* Code Project: [http://www.codeproject.com/Tips/897294/Concise-Binary-Object-Representation-CBOR-in-Cshar](http://www.codeproject.com/Tips/897294/Concise-Binary-Object-Representation-CBOR-in-Cshar)
* SourceForge: [https://sourceforge.net/p/petero-cbor](https://sourceforge.net/p/petero-cbor)

Examples
----------

The following shows certain use examples of this library.  Additional examples can be found in the API documentation.

Creating a map and converting that map to CBOR bytes
and a JSON string:

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
byte[] bytes = cbor.EncodeToBytes(CBOREncodeOptions.Default);
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
 var cbor = CBORObject.DecodeFromBytes(
    File.ReadAllBytes("object.cbor"), CBOREncodeOptions.Default);
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

If a byte array contains multiple CBOR objects, the byte array should
be wrapped in a MemoryStream and the stream used to read the objects,
as DecodeFromBytes assumes the array contains only one CBOR object.
Here is an example.

```c#
 // C#
 // Create a memory stream with a view of the byte array
 using (var stream = new MemoryStream(byteArray)) {
    // Read the CBOR object from the stream
    var cbor = CBORObject.Read(stream);
    // The rest of the example follows the one given above.
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
// and is supported since version 1.2.
using (var stream = new FileStream("object2.json", FileMode.Create)) {
    // Write the CBOR object as JSON; here, a byte order
    // mark won't be added
    cbor.WriteJSONTo(stream);
}
// Version 1.2 and later support a third way to write
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

Demo
----------

Go to [https://github.com/peteroupc/Calculator](https://github.com/peteroupc/Calculator) for source code to a demo of the
CBOR library in the form of a calculator.

About
-----------

Written in 2013-2017 by Peter O.

Any copyright is dedicated to the Public Domain.
[http://creativecommons.org/publicdomain/zero/1.0/](http://creativecommons.org/publicdomain/zero/1.0/)

If you like this, you should donate to Peter O.
at: [http://peteroupc.github.io/CBOR/](http://peteroupc.github.io/CBOR/)

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
  IRIs are like URIs except that they also allow non-ASCII characters.

Release Notes
-----------

### Version 3.4.0-alpha1

- Add ToObject method for deserializing CBOR objects.
- Add ICBORObjectConverter interface.
- Add HasMostOuterTag method to CBORObject class.
- Add CTAP2 canonicalization support to CBOR object encoding.
- Added examples in several places in documentation.

### Version 3.3

- Added Clear, RemoveAt and Remove(object) methods to CBORObject class.  Formerly, it was very hard with existing methods to remove items from CBOR maps and arrays.
- Added CodePointLength and ToUpperCaseAscii methods to DataUtilities class.
- Added WriteValue family of methods to CBORObject class.  This can be used for lower-level encoding of CBOR objects.  Examples on its use were included in the documentation.
- Bug fixes.

### Version 3.2.1

- Add .NET Framework 4.0 targeted assembly to avoid compiler warnings that can appear when this package is added to a project that targets .NET Framework 4.0 or later.

### Version 3.2

- Added build targeting the .NET Framework 2.0
- Obsoleted much of the existing API in CBOREncodeOptions and added new APIs to replace it.
- Documentation for some CBORObject methods now points to the use of CBOREncodeOptions.Default
- Documentation edited in other places

### Version 3.1

- Add options to control property name generation in CBORObject.FromObject.
- Add option to control base64 padding write-out in CBORObject.ToJSONString and CBORObject.WriteJSONTo.

### Version 3.0.3

- Fix issue "Encode options not honored for some nested objects".

### Version 3.0.2

- Really strong-name sign the assembly, which (probably) was inadvertently delay-signed in version 3.0.

### Version 3.0.0

- Moved from .NET Portable to .NET Standard 1.0.
- Deprecated arbitrary-precision classes in PeterO namespace; use the classes from the "PeterO.Numbers" library and namespace instead.  In particular, methods that used the former classes were deprecated and often replaced with versions that use the newer classes.
- Change JSON output behavior slightly, including preserving negative zero
- Hash code calculation was changed in this version
- Deprecated OutermostTag in favor of MostOuterTag in CBORObject
- Deprecated InnermostTag in favor of MostInnerTag in CBORObject
- Bug fixes

See [History.md](https://github.com/peteroupc/CBOR/tree/master/History.md)
for release notes for older versions.

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

I thank all users who sent issues to this repository.
