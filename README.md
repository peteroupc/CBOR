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
text to CBOR objects.  It defines the following methods.

See the [Wiki](https://github.com/peteroupc/CBOR/wiki) for API documentation.

About
-----------

Written in 2013 by Peter O.

Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/

Acknowledgments
-----------

Carsten Bormann reviewed this library and gave helpful suggestions.
