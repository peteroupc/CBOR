Older versions release notes
---------------------

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
