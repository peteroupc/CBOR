CBOR
====

A C# and Java implementation of Concise Binary Object Representation (RFC 7049).  
CBOR is a general-purpose binary data format designed by Carsten 
Bormann, and its data model "is an extended version of the JSON data model",
supporting many more types of data than JSON. "CBOR was inspired by 
MessagePack", but "is not intended as a version of or replacement for 
MessagePack."

Documentation
------------

This library defines one class, called CBORObject, that allows you to read and
write CBOR objects to and from data streams and byte arrays, and to convert JSON
text to CBOR objects.

See the [Wiki](https://github.com/peteroupc/CBOR/wiki) for API documentation.

The Different Versions
-----------

This repository contains code in three languages: C#, Java, and JavaScript.
C# is the main language of the project, and has the most features.  The Java
and JavaScript versions are translations from the C# version. 

The Java version contains almost as many features as the C# version
and has all the important ones, such as reading and writing CBOR objects,
CBOR/JSON conversion, and support for decimal fractions and bigfloats.

The JavaScript version currently only contains the code for big integers,
decimal fractions, and bigfloats.  It currently doesn't support converting
singles and doubles to big numbers and its support for converting other
objects, except strings, to big numbers is limited. 

About
-----------

Written in 2013 by Peter O.

Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/

Acknowledgments
-----------

* Carsten Bormann reviewed this library and gave helpful suggestions.
* Anders Gustafsson converted this library to a Portable Class Library.