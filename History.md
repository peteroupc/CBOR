Release notes
---------------------

### Version 4.5.3

- More bug fixes, including a fix to a problem that can occur when reading from compressed or network streams
- Fix issue that floating-point positive and negative zero are not encoded in the shortest form by default

### Version 4.5.2

- Bug and regression fixes

### Version 4.5.1

- Fix reported security issue

### Version 4.5:

- Add support for JSON Pointers and JSON Patches
- Add option to keep map key order when decoding CBOR and JSON
- Add option to write JSON using only ASCII characters
- CBORObject.ToString renders strings as ASCII
- Add support for deserializing CBOR objects to IReadOnlyList, IReadOnlyCollection, and ReadOnlyDictionary

Note that after version 4.5x, the CBOR library's repository will stop including special projects for .NET 2.0 and .NET 4.0, leaving the .NET-Standard project for building the library.

### Version 4.4.2

- Performance improvements in some cases, especially involving date/time conversions
- Error checks in DateTimeFieldsToCBORObject method in CBORDateConverter

### Version 4.4.1

- Fix bugs when parsing JSON with the JSON option 'numberconversion=double'

### Version 4.4

- Boolean constructors of PODOptions and CBOREncodeOptions were obsolete
- Float64 option of CBOREncodeOptions for encoding floating-point values as 64-bit only
- CBORDateConverter made public and expanded to enable conversion between various
  date/time formats and CBOR objects
- Added CanFitInUInt64 and CanTruncatedIntFitInUInt64 methods
- Bug fixes

### Version 4.3

- Fixed bugs in DateTime support
- Added CompareTo overloads in CBORNumber class
- Add NewOrderedMap method based on a suggestion by a GitHub user
- Other bug fixes

### Version 4.2

- Some arithmetic methods in CBORNumber do basic overflow checks.
- Add char array and byte array overloads to ParseJSONNumber
- Support implementations of IList in CBORObject deserialization
- Internally, the code avoids storing doubles (64-bit floating-point numbers) directly in CBORNumbers, uses sorted maps rather than hash tables in some CBOR objects, and can now store text strings as UTF-8 byte arrays.  This could help avoid unnecessary string conversions in many cases.
- Bug fixes and performance improvements
- Now uses Numbers library version 1.7.3

### Version 4.1.1

- Fix issue where some non-basic characters in JSON strings encoded in UTF-8 were read incorrectly by the CBORObject.FromJSONBytes method

### Version 4.1

- Added the following to the CBORObject class: Entries property; ToJSONBytes, CalcEncodedSize, WithTag, and FromJSONBytes methods.
- Added overload to From JSONString, allowing only a portion of a string to be used
- Added support for reading JSON text sequences (FromJSONSequenceBytes, ReadJSONSequence).
- F# types are supported better by CBORObject.FromObject
- JSON writer now checks circular references.
- Improved performance when reading JSON numbers, thanks in part to an upgrade of the Numbers library used by the CBOR library.
- Added number conversion options and PreserveNegativeZero property to JSONOptions.
- Added ParseJSONDouble method, other methods, and additional ParseJSONNumber overloads to CBORDataUtilities
- Deprecated some existing overloads of ParseJSONNumber
- Deprecated many CBORObject properties and methods, including the following: CanFitInInt32, CanFitInInt64, IsInfinity, IsNaN, AsDecimal, AsEInteger, AsEFloat, AsERational, AsUInt16, AsUInt32, AsUInt64, AsSByte, AsByte, Abs, Negate, Sign, IsPositiveInfinity, IsNegativeInfinity, FromJSONString(string, CBOREncodeOptions)
- Added several methods and properties to CBORNumber (including certain methods deprecated in CBORObject), and exposed the kind of number stored in the class.
- JSONOptions string constructor now sets ReplaceSurrogates to false by default (previously, it was inadvertently true).
- Bug fixes

### Version 4.0.1:

- Fix issue with unexpected CBORObject#ToString result for True and False.

### Version 4.0.0:

- Fix issues with CTAP2 Canonical CBOR form
- Support field serialization and deserialization in ToObject and FromObject

### Version 4.0.0-beta2:

The features in this version include:

- The CBOR library no longer stores numbers in a special form beyond the CBOR data model, which represents all "65-bit" signed integers and all "double" values.  This means the CBOR library no longer stores certain numbers as EDecimal, EInteger, EFloat, etc., rather than as tagged CBOR objects.
- CBORObject.CompareTo now compares objects using the default deterministic encoding comparison in the draft revision of the CBOR specification, and no longer treats numbers (objects with the former type CBORType.Number) as a special class.
- CBORType.Number is deprecated; CBORObjects no longer have this type.  In its place, certain numbers now have new CBORTypes Integer or FloatingPoint.
- CBORObject now stores floating-point numbers internally as the bits that make them up, rather than as `double`s, to avoid data loss in conversions.
- Methods were added to CBORObject to read and write floating-point numbers in terms of their bit patterns rather than as `double`s or `float`s.
- Ctap2Canonical was made more strict and now works when decoding CBOR objects.
- Added ReadSequence and DecodeSequence to CBORObject for reading CBOR sequences.
- New CBORNumber class for storing numbers representable in CBOR.  The new CBORObject.IsNumber property checks whether a CBOR object represents a number.
- Bug fixes

### Version 4.0.0-beta1:

- Support nullable types in CBORObject.ToObject.
- Update Numbers library to newer version
- JSONOptions.Base64Padding now has no effect.  The library will now write padding as necessary when
  writing traditional base64 to JSON and write no padding when writing base64url to JSON.
- JSONOptions.ReplaceSurrogates property added.
- Restrict valid shared reference indices to integers 0 or greater.
- Reject writing JSON where CBOR maps have two keys with the same string equivalent.
- Improve performance of CBOR object comparisons involving big floats.

### Version 4.0.0-alpha2:

- Support CBOR tags for IRIs and IRI references.
- Add CBOREncodeOptions.DefaultCtap2Canonical field.

### Version 4.0.0-alpha1:

- Remove all APIs obsoleted since version 3.4.  This
  includes the BigInteger, ExtendedDecimal, and ExtendedFloat APIs,
  which were renamed and moved to a different library, as well as the
  ICBORTag and CBORTypeFilter APIs.
- Changed implementation of FromObject, including imposing a nesting depth
  limit and supporting a CBORTypeMapper parameter.
- Property name conversion rules (in PODOptions) were changed
  in this version with respect to FromObject.  In this sense,
  PODOptions.RemoveIsPrefix was removed.
- Certain other changes in CBOR object reading and validation were
  made; they are largely compatible with previous versions but may be
  backwards-incompatible in certain rare cases

### Version 3.6.0

- Add new string constructors to CBOREncodeOptions, JSONOptions, and PODOptions
- Implement options to disable resolving shared references and allow empty streams when decoding CBOR objects
- Add IsNumber property to CBORObject to check whether a CBOR object stores a number; CBORType.Number is deprecated

### Version 3.5.2

- Update Numbers library used by this library to a version that doesn't depend on StyleCop.Analyzers in the package.

### Version 3.5.1

- Update Numbers library used by this library.

### Version 3.5.0

- Update Numbers library used by this library.
- Deprecate Base64Padding property of JSONOptions.

### Version 3.4.0

No significant changes from beta1.

### Version 3.4.0-beta1

- Improved implementation of new ToObject method.
- Bugs in multidimensional array serialization by FromObject were fixed.
- URI parsing restored to 3.0 version for backward compatibility.
- Remove method disallows null for backward compatibility.
- ICBORObjectConverter renamed to ICBORToFromConverter.
- Several APIs were obsoleted.

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

### Version 2.5.2

* Unlike version 2.4.2, signed CBOR assembly with a strong name key.
* Unlike version 2.4.2, library uses strong-named version of  [`PeterO.Numbers`](https://www.nuget.org/packages/PeterO.Numbers), version 0.4.0

### Version 2.5.1

* Release was erroneous.

### Version 2.5

* Release was erroneous.

### Version 2.4.2

* Really use 0.2.2 of
  [`PeterO.Numbers`](https://www.nuget.org/packages/PeterO.Numbers)
  as dependency in NuGet package

### Version 2.4.1

* C# implementation now uses version 0.2.2 of
  [`PeterO.Numbers`](https://www.nuget.org/packages/PeterO.Numbers)

### Version 2.4.0

* The arbitrary-precision classes in this library are being replaced
 with a new library (called [`PeterO.Numbers`](https://www.nuget.org/packages/PeterO.Numbers) in C#).  As a result, most
 of the methods in the existing classes are obsolete.  This affects the
 classes `BigInteger`, `ExtendedDecimal`, `ExtendedFloat`, `ExtendedRational`,
 `Rounding`, `PrecisionContext`, and `TrapException`.  Changes were made
 to those classes to call the new classes, and the behavior is mostly
 compatible with the previous behavior (with the notable exception
 of a new dependency in the CBOR library).
* After version 2.3.1, the classes in the new library were greatly changed
 from what they were in version 2.3.1.  Version 2.4.0 currently uses
 version 0.2 of the new library, but this may change in future versions.  See the
 [new library's release notes](https://github.com/peteroupc/Numbers),
 and this repository's commit history (from "version 2.3.1"
 to "move big number library...") for details.
* The FromObject method of the CBORObject class can now convert
 arbitrary-precision number objects from the new library
 appropriately, such as `EInteger` and `EDecimal`.  However, there are
 no methods that directly take or return one of those classes, for
 compatibility with version 2.0.
* Added Zero field and IsNegative property to the CBORObject class
* Added overloads to ReadJSON and FromJSONString in CBORObject class
* Added parameter in the ParseJSONNumber method of CBORDataUtilities
 to preserve negative zeros
* Added CBOR decoding option to disable duplicate keys
* Fixed JSON parsing bugs

### Older Versions

In version 2.3.1:

- Fixed NuGet package
- No changes of note in the Java version

In version 2.3:

- The C# version of the library now also targets "dotnet", which should make it compatible with platform .NET runtime
environments such as the upcoming cross-platform "coreclr" runtime.
- Added GetUtf8Bytes overload to DataUtilities
- Fixed line break bug when setting lenientLineBreaks to true in the PeterO.Cbor.DataUtilities.WriteUtf8 method
- In BigInteger, fixed divideAndRemainder method, added certain methods and made other methods obsolete
- Many additions to the documentation
- Other bug fixes

In version 2.2:
- Portable Class Library compatibility expanded
- Add option to always use definite length string encoding when generating CBOR objects

Known issue in version 2.2:
- Setting `lenientLineBreaks` to `true` in the `PeterO.Cbor.DataUtilities.WriteUtf8` method
 doesn't correctly write CR/LF line breaks.  This will be fixed in a future version of the library.

In version 2.1:

- Added Ulp, Precision, MovePointLeft, MovePointRight, and ScaleToPowerOfTwo/-Ten methods to
  ExtendedDecimal and ExtendedFloat
- Fixed double-rounding issue with ToDouble and ToFloat methods
  of ExtendedDecimal
- Added Odd and OddOrZeroFiveUp rounding modes
- Added non-decimal base conversion features to BigInteger
- Other bug fixes

In version 2.0:

- Several very special characters are escaped in JSON output, such as line and paragraph
  separators, and byte order marks.
- BigInteger's longValue method was fixed
- BigInteger was changed to have no public constructors
- ReadJSON now supports UTF-16 and UTF-32 in addition to UTF-8
- PrecisionContext's JavaBigDecimal object was corrected.
- Fixed bugs in parsing JSON numbers in some cases
- CBORObject's one-argument Add method now adds CBORObject.Null if passed null,
  rather than throwing an exception.

**NOTE**: In the "2.0" tag for this repository, the file `DataUtilities.cs` should be copied
from the `PeterO` directory to the root directory for this repository, in order for the CBORDocs
and CBORDocs2 projects to build.  This issue may probably exist in other older versions
as well.

In version 1.3:

- Added a CompareToIgnoreTags method to CBORObject
- The BigInteger constructor in the C# version is deprecated
- Fixed bugs in converting from CBOR float and double to integers in some corner cases
- Fixed a bug where CBORObject's OutermostTag returns 0 instead of the correct -1 for untagged objects
- Fixed a bug where BigInteger's bitLength return value can be wrong in some corner cases

In version 1.2:

- The WriteJSON and WriteJSONTo methods were added to CBORObject
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
