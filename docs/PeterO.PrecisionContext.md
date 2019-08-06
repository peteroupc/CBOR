## PeterO.PrecisionContext

    public class PrecisionContext

<b>Deprecated.</b> Use EContext from PeterO.Numbers/com.upokecenter.numbers.

 A precision context.

### Member Summary
* <code>[ToString()](#ToString)</code> - Gets a string representation of this object.

<a id="Void_ctor_Int32_PeterO_Rounding_Int32_Int32_Boolean"></a>
### PrecisionContext Constructor

    public PrecisionContext(
        int precision,
        PeterO.Rounding rounding,
        int exponentMinSmall,
        int exponentMaxSmall,
        bool clampNormalExponents);

 Initializes a new instance of the [PeterO.PrecisionContext](PeterO.PrecisionContext.md) class. HasFlags will be set to false.

       <b>Parameters:</b>

 * <i>precision</i>: The maximum number of digits a number can have, or 0 for an unlimited number of digits.

 * <i>rounding</i>: The rounding mode to use when a number can't fit the given precision.

 * <i>exponentMinSmall</i>: The minimum exponent.

 * <i>exponentMaxSmall</i>: The maximum exponent.

 * <i>clampNormalExponents</i>: Whether to clamp a number's significand to the given maximum precision (if it isn't zero) while remaining within the exponent range.

<a id="ToString"></a>
### ToString

    public override string ToString();

 Gets a string representation of this object. Note that the format is not intended to be parsed and may change at any time.

 <b>Return Value:</b>

A string representation of this object.
