CBOR
====

A C# and Java implementation of Concise Binary Object Representation, a general-purpose binary data format defined in RFC 7049. According to that RFC, CBOR's data model "is an extended version of the JSON data model", supporting many more types of data than JSON. "CBOR was inspired by MessagePack", but "is not intended as a version of or replacement for MessagePack."

This implementation was written by Peter O. and is released to the Public Domain under the CC0 Declaration.

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
as an artifact in the Central Repository. To add this library to a Maven
project, add the following to the `dependencies` section in your `pom.xml` file:

    <dependency>
      <groupId>com.upokecenter</groupId>
      <artifactId>cbor</artifactId>
      <version>0.23.0</version>
    </dependency>

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

Example
----------

This code is in C#, but the Java version of the code would be very similar.

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