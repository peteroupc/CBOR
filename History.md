Older versions release notes
---------------------

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
