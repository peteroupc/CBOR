CBOR
====

[![NuGet Status](http://img.shields.io/nuget/v/PeterO.Cbor.svg?style=flat)](https://www.nuget.org/packages/PeterO.Cbor)

**Download source code: [ZIP file](https://github.com/peteroupc/CBOR/archive/master.zip)**

----

A C# implementation of Concise Binary Object Representation, a general-purpose binary data format defined in RFC 8949. According to that RFC, CBOR's data model "is an extended version of the JSON data model", supporting many more types of data than JSON. "CBOR was inspired by MessagePack", but "is not intended as a version of or replacement for MessagePack."

This implementation was written by Peter O. and is released to the Public Domain under the [CC0 Declaration](https://creativecommons.org/publicdomain/zero/1.0/).

This implementation also doubles as a reader and writer of JSON, and can convert data from JSON to CBOR and back.

Finally, this implementation supports arbitrary-precision binary and decimal floating-point numbers and rational numbers with arbitrary-precision components.

Source code is available in the [project page](https://github.com/peteroupc/CBOR).

Note that after version 4.5x, the CBOR library's repository will stop including special projects for .NET 2.0 and .NET 4.0, leaving the .NET-Standard project for building the library.

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

Examples
----------

The following shows certain use examples of this library.  Additional examples can be found in the [API documentation](https://peteroupc.github.io/CBOR/docs/).

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
byte[] bytes = cbor.EncodeToBytes();
// The following converts the map to JSON
string json = cbor.ToJSONString();
Console.WriteLine(json);
```

Creating a map and converting that map to canonical CBOR
bytes (for WebAuthn and other purposes) and a .NET
dictionary:

```c#
// The following creates a CBOR map and adds
// several kinds of objects to it
var cbor = CBORObject.NewMap()
   .Add("item", "any string")
   .Add("foo", "another string")
   .Add("quux", "a third string");
// The following converts the map to canonical CBOR
byte[] bytes = cbor.EncodeToBytes(CBOREncodeOptions.DefaultCtap2Canonical);
// The following converts the map to a dictionary
var dict = cbor.ToObject<IDictionary<string,string>>();
Console.WriteLine(dict.Count);
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

Writing multiple objects to a file, including arbitrary objects (the resulting
file is also called a _CBOR sequence_):

```c#
// C#
// This example writes a sequence of objects in CBOR
// format to the same file.
using (var stream = new FileStream("object.cborseq", FileMode.Create)) {
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

There are several ways to check whether a CBOR object is a 32-bit integer.  Which
one to use depends on the application's needs.  Some of them follow (written in C#).

```c#
/* Accept any untagged CBOR integer
  that can fit the Int32 range */
if(!cbor.IsTagged && cbor.Type==CBORType.Integer &&
   cbor.CanValueFitInInt32()) {
  return cbor.AsInt32();
}
/* Accept any CBOR integer, tagged or not, that
 can fit the Int32 range */
if(cbor.Type==CBORType.Integer && cbor.CanValueFitInInt32()) {
  return cbor.AsInt32();
}
/* Accept any CBOR integer or floating-point number,
 tagged or not, that is an integer within the Int32 range */
if((cbor.Type==CBORType.Integer || cborType==CBORType.FloatingPoint) ||
   cbor.Untag().AsNumber().CanValueFitInInt32()) {
  return cbor.AsInt32();
}
/* Accept any CBOR object representing an integer number
   that can fit the Int32 range */
if(cbor.IsNumber && cbor.AsNumber().CanValueFitInInt32()) {
  return cbor.AsInt32();
}
```

The following example illustrates a custom strategy for converting objects
of a given class into CBOR objects.
```
    public class CPOD3 {
       public string Aa { get; set; }
       public string Bb { get; set; }
       public string Cc { get; set; }
    }

    private class CPOD3Converter : ICBORToFromConverter<CPOD3> {
       public CBORObject ToCBORObject(CPOD3 cpod) {
          return CBORObject.NewMap()
             .Add(0,cpod.Aa)
             .Add(1,cpod.Bb)
             .Add(2,cpod.Cc);
       }
       public CPOD3 FromCBORObject(CBORObject obj) {
          if (obj.Type!=CBORType.Map) {
             throw new CBORException();
          }
          var ret=new CPOD3();
          ret.Aa=obj[0].AsString();
          ret.Bb=obj[1].AsString();
          ret.Cc=obj[2].AsString();
          return ret;
       }
    }

    //////////
    //  And in the code...

       var cp2=new CPOD3();
       cp2.Aa="AA";
       cp2.Bb="BB";
       cp2.Cc="CC";
       var conv=new CPOD3Converter();
       // Serialize CBOR object, passing the type mapper
       var cbor=conv.ToCBORObject(cp2);
       // Deserialize CBOR object, passing the type mapper
       cp2=conv.FromCBORObject(cbor);
```

NOTE: All code samples in this section are released under the Unlicense.

Demo
----------

Go to [https://github.com/peteroupc/Calculator](https://github.com/peteroupc/Calculator) for source code to a demo of the
CBOR library in the form of a calculator.

About
-----------

Written by Peter O.

Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: [https://unlicense.org/](https://unlicense.org/)

Release Notes
-----------

See [History.md](https://github.com/peteroupc/CBOR/tree/master/History.md)
for release notes.

Specifications
-----------
Here are specifications by this implementation's author on certain CBOR tags:

* Tag 30: [Rational numbers](http://peteroupc.github.io/CBOR/rational.html)
* Tag 257: [Binary MIME messages](http://peteroupc.github.io/CBOR/binarymime.html)
* Tag 38: [Language-tagged strings](http://peteroupc.github.io/CBOR/langtags.html) (Expected to be superseded by an RFC; see the Internet Draft draft-ietf-core-problem-details).
* Tag 264 and 265: [Arbitrary-exponent numbers](http://peteroupc.github.io/CBOR/bigfrac.html)

Acknowledgments
-----------

* Carsten Bormann reviewed this library and gave helpful suggestions.
* Anders Gustafsson converted this library to a Portable Class Library.

I thank all users who sent issues to this repository.
