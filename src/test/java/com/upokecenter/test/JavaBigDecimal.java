package com.upokecenter.test;

import java.math.MathContext;
import java.math.RoundingMode;

import com.upokecenter.util.*;


public class JavaBigDecimal {

	ExtendedDecimal edec;

	public static final JavaBigDecimal ZERO = new JavaBigDecimal(ExtendedDecimal.Zero);
	public static final JavaBigDecimal ONE = new JavaBigDecimal(ExtendedDecimal.One);
	public static final JavaBigDecimal TEN = new JavaBigDecimal(ExtendedDecimal.Ten);

	private static final PrecisionContext CONTEXT = 
			PrecisionContext.BigDecimalJava
			.WithTraps(PrecisionContext.FlagUnderflow);

	public static final int ROUND_UP = 0;
	public static final int ROUND_DOWN = 1;
	public static final int ROUND_CEILING = 2;
	public static final int ROUND_FLOOR = 3;
	public static final int ROUND_HALF_UP = 4;
	public static final int ROUND_HALF_DOWN = 5;
	public static final int ROUND_HALF_EVEN = 6;
	public static final int ROUND_UNNECESSARY = 7;


	private static RoundingMode RoundingNumberToRoundingMode(int mode){
		if(mode==ROUND_HALF_EVEN){
			return RoundingMode.HALF_EVEN;
		}
		if(mode==ROUND_HALF_UP){
			return RoundingMode.HALF_UP;
		}
		if(mode==ROUND_HALF_DOWN){
			return RoundingMode.HALF_DOWN;
		}
		if(mode==ROUND_DOWN){
			return RoundingMode.DOWN;
		}
		if(mode==ROUND_UP){
			return RoundingMode.UP;
		}
		if(mode==ROUND_CEILING){
			return RoundingMode.CEILING;
		}
		if(mode==ROUND_FLOOR){
			return RoundingMode.FLOOR;
		}
		if(mode==ROUND_UNNECESSARY){
			return RoundingMode.UNNECESSARY;
		}
		throw new ArithmeticException("Invalid rounding mode");
	}

	private static Rounding RoundingModeToRounding(RoundingMode mode){
		if(mode==RoundingMode.HALF_EVEN){
			return Rounding.HalfEven;
		}
		if(mode==RoundingMode.HALF_UP){
			return Rounding.HalfUp;
		}
		if(mode==RoundingMode.HALF_DOWN){
			return Rounding.HalfDown;
		}
		if(mode==RoundingMode.DOWN){
			return Rounding.Down;
		}
		if(mode==RoundingMode.UP){
			return Rounding.Up;
		}
		if(mode==RoundingMode.CEILING){
			return Rounding.Ceiling;
		}
		if(mode==RoundingMode.FLOOR){
			return Rounding.Floor;
		}
		return Rounding.Unnecessary;
	}

	private static PrecisionContext GetContext(MathContext ctx){
		return CONTEXT
				.WithPrecision(ctx.getPrecision())
				.WithRounding(RoundingModeToRounding(ctx.getRoundingMode()));
	}
	private static PrecisionContext GetContext(RoundingMode mode){
		return CONTEXT
				.WithRounding(RoundingModeToRounding(mode));
	}

	private JavaBigDecimal(ExtendedDecimal e) {
		this.edec=e;
	}

	private static JavaBigDecimal GetBigDecimal(ExtendedDecimal e){
		if(!e.isFinite()){
			throw new ArithmeticException();
		}
		if(e.isZero()){
			// Ensure positive zero
			return new JavaBigDecimal(e.Abs());
		}
		return new JavaBigDecimal(e);
	}

	public JavaBigDecimal add(JavaBigDecimal augend) {
		return GetBigDecimal(edec.Add(augend.edec,CONTEXT));
	}

	public JavaBigDecimal add(JavaBigDecimal augend, MathContext mc) {
		return GetBigDecimal(edec.Add(augend.edec,GetContext(mc)));
	}

	public JavaBigDecimal subtract(JavaBigDecimal subtrahend) {
		return GetBigDecimal(edec.Subtract(subtrahend.edec,CONTEXT));
	}

	public JavaBigDecimal subtract(JavaBigDecimal subtrahend, MathContext mc) {
		return GetBigDecimal(edec.Subtract(subtrahend.edec,GetContext(mc)));
	}

	public JavaBigDecimal multiply(JavaBigDecimal multiplicand) {
		return GetBigDecimal(edec.Multiply(multiplicand.edec,CONTEXT));
	}

	public JavaBigDecimal multiply(JavaBigDecimal multiplicand, MathContext mc) {
		return GetBigDecimal(edec.Multiply(multiplicand.edec,GetContext(mc)));
	}

	public JavaBigDecimal divide(JavaBigDecimal divisor, int scale,
			RoundingMode roundingMode) {
		return GetBigDecimal(edec.DivideToExponent(divisor.edec, -((long)scale), GetContext(roundingMode)));
	}

	public JavaBigDecimal divide(JavaBigDecimal divisor, RoundingMode roundingMode) {
		return GetBigDecimal(edec.Divide(divisor.edec, GetContext(roundingMode)));
	}

	public JavaBigDecimal divide(JavaBigDecimal divisor) {
		return GetBigDecimal(edec.Divide(divisor.edec));
	}

	public JavaBigDecimal divide(JavaBigDecimal divisor, MathContext mc) {
		return GetBigDecimal(edec.Divide(divisor.edec, GetContext(mc)));
	}

	public JavaBigDecimal divideToIntegralValue(JavaBigDecimal divisor) {
		return GetBigDecimal(edec.DivideToIntegerNaturalScale(divisor.edec));
	}

	public JavaBigDecimal divideToIntegralValue(JavaBigDecimal divisor, MathContext mc) {
		return GetBigDecimal(edec.DivideToIntegerNaturalScale(divisor.edec, GetContext(mc)));
	}

	public JavaBigDecimal remainder(JavaBigDecimal divisor) {
		return GetBigDecimal(edec.RemainderNaturalScale(divisor.edec));
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#remainder(java.math.BigDecimal, java.math.MathContext)
	 */
	public JavaBigDecimal remainder(JavaBigDecimal divisor, MathContext mc) {
		return GetBigDecimal(edec.RemainderNaturalScale(divisor.edec, GetContext(mc)));
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#divideAndRemainder(java.math.BigDecimal)
	 */
	public JavaBigDecimal[] divideAndRemainder(JavaBigDecimal divisor) {
		return new JavaBigDecimal[] {
				this.divideToIntegralValue(divisor),
				this.remainder(divisor)
		};
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#divideAndRemainder(java.math.BigDecimal, java.math.MathContext)
	 */
	public JavaBigDecimal[] divideAndRemainder(JavaBigDecimal divisor, MathContext mc) {
		return new JavaBigDecimal[] {
				this.divideToIntegralValue(divisor,mc),
				this.remainder(divisor,mc)
		};
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#pow(int)
	 */
	public JavaBigDecimal pow(int n) {
		if(n<0 || n>999999999){
			throw new IllegalArgumentException("n is out of range");
		}
		if(n==0){
			return ONE;
		}
		return GetBigDecimal(edec.Pow(n,CONTEXT));
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#pow(int, java.math.MathContext)
	 */
	public JavaBigDecimal pow(int n, MathContext mc) {
		if(n<0 || n>999999999){
			throw new IllegalArgumentException("n is out of range");
		}
		if(n==0){
			return ONE;
		}
		return GetBigDecimal(edec.Pow(n,GetContext(mc)));
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#abs()
	 */
	public JavaBigDecimal abs() {
		return edec.isNegative() ? negate() : this;
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#abs(java.math.MathContext)
	 */
	public JavaBigDecimal abs(MathContext mc) {
		return GetBigDecimal(edec.Abs(GetContext(mc)));
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#negate()
	 */
	public JavaBigDecimal negate() {
		return GetBigDecimal(edec.Negate());
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#negate(java.math.MathContext)
	 */
	public JavaBigDecimal negate(MathContext mc) {
		return GetBigDecimal(edec.Negate(GetContext(mc)));
	}

	public JavaBigDecimal plus() {
		return this;
	}

	public JavaBigDecimal plus(MathContext mc) {
		return GetBigDecimal(edec.Plus(GetContext(mc)));
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#signum()
	 */
	public int signum() {
		return edec.signum();
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#scale()
	 */
	public int scale() {
		return edec.getExponent().negate().intValue();
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#precision()
	 */
	public int precision() {
		return edec.getMantissa().getDigitCount();
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#unscaledValue()
	 */
	public com.upokecenter.util.BigInteger unscaledValue() {
		return edec.getMantissa();
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#round(java.math.MathContext)
	 */
	public JavaBigDecimal round(MathContext mc) {
		return plus(mc);
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#setScale(int, java.math.RoundingMode)
	 */
	public JavaBigDecimal setScale(int newScale, RoundingMode roundingMode) {
		BigInteger bigScale = BigInteger.valueOf(newScale).negate();
		return GetBigDecimal(edec.Quantize(bigScale, GetContext(roundingMode)));
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#setScale(int)
	 */
	public JavaBigDecimal setScale(int newScale) {
		return setScale(newScale, RoundingMode.UNNECESSARY);
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#movePointLeft(int)
	 */
	public JavaBigDecimal movePointLeft(int n) {
		if(n==0)return this;
		int currentScale=this.scale();
		if ((currentScale < 0 && n < Integer.MIN_VALUE - currentScale) ||
                (currentScale > 0 && n > Integer.MAX_VALUE - currentScale)) {
			throw new ArithmeticException("Scale overflow");
		} else {
			currentScale+=n;
			JavaBigDecimal ret=new JavaBigDecimal(this.unscaledValue(), currentScale);
			if(currentScale<0){
				ret=ret.setScale(0);
			}
			return ret;
		}
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#movePointRight(int)
	 */
	public JavaBigDecimal movePointRight(int n) {
		int currentScale=this.scale();
		if ((n < 0 && Integer.MAX_VALUE + n < currentScale) ||
                (n > 0 && Integer.MIN_VALUE + n > currentScale)) {
			throw new ArithmeticException("Scale overflow");
		} else {
			currentScale-=n;
			JavaBigDecimal ret=new JavaBigDecimal(this.unscaledValue(), currentScale);
			if(currentScale<0){
				ret=ret.setScale(0);
			}
			return ret;
		}
   	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#scaleByPowerOfTen(int)
	 */
	public JavaBigDecimal scaleByPowerOfTen(int n) {
		int currentScale=this.scale();
		if ((n < 0 && Integer.MAX_VALUE + n < currentScale) ||
                (n > 0 && Integer.MIN_VALUE + n > currentScale)) {
			throw new ArithmeticException("Scale overflow");
		} else {
			currentScale-=n;
			return new JavaBigDecimal(this.unscaledValue(), currentScale);
		}
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#stripTrailingZeros()
	 */
	public JavaBigDecimal stripTrailingZeros() {
		if(edec.isZero()){
			// The reduce operation would make all zeros
			// have an exponent of 0; by contrast,
			// this method preserves the exponents of zeros
			return this;
		}
		return GetBigDecimal(edec.Reduce(CONTEXT));
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#compareTo(java.math.BigDecimal)
	 */
	public int compareTo(JavaBigDecimal val) {
		return edec.compareTo(val.edec);
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#equals(java.lang.Object)
	 */
	@Override
	public boolean equals(Object x) {
		if(x==null || !(x instanceof JavaBigDecimal)){
			return false;
		}
		return edec.equals(((JavaBigDecimal)x).edec);
	}

	public JavaBigDecimal min(JavaBigDecimal val) {
		return compareTo(val)<=0 ? this : val;
	}

	public JavaBigDecimal max(JavaBigDecimal val) {
		return compareTo(val)>=0 ? this : val;
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#hashCode()
	 */
	@Override
	public int hashCode() {
		return edec.hashCode();
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#toString()
	 */
	@Override
	public String toString() {
		return edec.toString();
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#toEngineeringString()
	 */
	public String toEngineeringString() {
		return edec.ToEngineeringString();
	}

	/* (non-Javadoc)
	 * @inheritDoc java.math.BigDecimal#toPlainString()
	 */
	public String toPlainString() {
		return edec.ToPlainString();
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#toBigInteger()
	 */
	public com.upokecenter.util.BigInteger toBigInteger() {
		return edec.ToBigInteger();
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#toBigIntegerExact()
	 */
	public com.upokecenter.util.BigInteger toBigIntegerExact() {
		return edec.ToBigIntegerExact();
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#longValue()
	 */
	public long longValue() {
		return toBigInteger().longValueUnchecked();
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#longValueExact()
	 */
	public long longValueExact() {
		return toBigIntegerExact().longValueChecked();
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#intValue()
	 */
	public int intValue() {
		return toBigInteger().intValueUnchecked();
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#intValueExact()
	 */
	public int intValueExact() {
		return toBigIntegerExact().intValueChecked();
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#shortValueExact()
	 */
	public short shortValueExact() {
		int val=intValueExact();
		if((val>>16)!=0){
			throw new ArithmeticException("Too big for a short");
		}
		return (short)val;
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#byteValueExact()
	 */
	public byte byteValueExact() {
		int val=intValueExact();
		if((val>>8)!=0){
			throw new ArithmeticException("Too big for a byte");
		}
		return (byte)val;
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#floatValue()
	 */
	public float floatValue() {
		return edec.ToSingle();
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#doubleValue()
	 */
	public double doubleValue() {
		return edec.ToDouble();
	}

	/* (non-Javadoc)
	 * @see java.math.BigDecimal#ulp()
	 */
	public JavaBigDecimal ulp() {
		return new JavaBigDecimal(ExtendedDecimal.Create(BigInteger.ONE, edec.getExponent()));
	}

	private static ExtendedDecimal CheckExtendedDecimal(ExtendedDecimal edec){
		if(!edec.isFinite()){
			throw new ArithmeticException();
		}
		if(edec.isZero()){
			// Ensure positive zero
			return edec.Abs();
		}	
		return edec;
	}

	public JavaBigDecimal(char[] value, int offset, int len){
		if(offset<0 || offset>value.length ||
				len<0 || len>value.length || value.length - offset < len){
			throw new NumberFormatException();
		}
		try {
		edec=CheckExtendedDecimal(ExtendedDecimal.FromString(
			new String(value,offset,len),CONTEXT));
		} catch(ArithmeticException ex){
			throw (NumberFormatException)new NumberFormatException(ex.getMessage()).initCause(ex);
		}
	}

	public JavaBigDecimal(String value, int offset, int len){
		try {
		edec=CheckExtendedDecimal(ExtendedDecimal.FromString(
			value, offset, len,CONTEXT));
		} catch(ArithmeticException ex){
			throw (NumberFormatException)new NumberFormatException(ex.getMessage()).initCause(ex);
		}
	}

	public JavaBigDecimal(String value, int offset, int len, MathContext mc){
		try {
		edec=CheckExtendedDecimal(ExtendedDecimal.FromString(
			value, offset, len,GetContext(mc)));
		} catch(ArithmeticException ex){
			throw (NumberFormatException)new NumberFormatException(ex.getMessage()).initCause(ex);
		}
	}

	public static JavaBigDecimal valueOf(String str){
		return new JavaBigDecimal(str);
	}
	public static JavaBigDecimal valueOf(double value){
		return valueOf(Double.toHexString(value));
	}
	public static JavaBigDecimal valueOf(int value){
		return GetBigDecimal(ExtendedDecimal.FromInt32(value));
	}
	public static JavaBigDecimal valueOf(long value){
		return GetBigDecimal(ExtendedDecimal.FromInt64(value));
	}

	public JavaBigDecimal(BigInteger val) {
		this(val, 0);
	}

	public JavaBigDecimal(String value) {
		this(value, 0, value.length());
	}

	public JavaBigDecimal(BigInteger fromString, int aScale) {
		edec=CheckExtendedDecimal(ExtendedDecimal.Create(fromString, BigInteger.valueOf(aScale).negate()));		
	}

	public JavaBigDecimal(int a, MathContext mc) {
		edec=CheckExtendedDecimal(ExtendedDecimal.Create(a, 0).RoundToPrecision(GetContext(mc)));
	}		

	public JavaBigDecimal(long a, MathContext mc) {
		edec=CheckExtendedDecimal(ExtendedDecimal.Create(BigInteger.valueOf(a), BigInteger.ZERO).RoundToPrecision(GetContext(mc)));
	}

	public JavaBigDecimal(int a) {
		edec=CheckExtendedDecimal(ExtendedDecimal.Create(a, 0).RoundToPrecision(CONTEXT));
	}		

	public JavaBigDecimal(long a) {
		edec=CheckExtendedDecimal(ExtendedDecimal.Create(BigInteger.valueOf(a), BigInteger.ZERO).RoundToPrecision(CONTEXT));
	}

	public JavaBigDecimal(char[] value, int offset, int len, MathContext mc) {
		if(offset<0 || offset>value.length ||
				len<0 || len>value.length || value.length - offset < len){
			throw new NumberFormatException();
		}
		try {
		edec=CheckExtendedDecimal(ExtendedDecimal.FromString(
			new String(value,offset,len),GetContext(mc)));
		} catch(ArithmeticException ex){
			throw (NumberFormatException)new NumberFormatException(ex.getMessage()).initCause(ex);
		}
	}

	public JavaBigDecimal(String a, MathContext mc) {
		this(a,0,a.length(),mc);
	}

	public JavaBigDecimal(char[] cs) {
		this(cs, 0, cs.length);
	}

	public JavaBigDecimal(char[] cs, MathContext mc) {
		this(cs, 0, cs.length, mc);
	}

	public JavaBigDecimal(BigInteger bA, MathContext mc) {
		this(bA, 0, mc);
	}

	public JavaBigDecimal(BigInteger bA, int aScale, MathContext mc) {
		edec=CheckExtendedDecimal(ExtendedDecimal.Create(bA, BigInteger.valueOf(aScale).negate()).RoundToPrecision(GetContext(mc)));		
	}

	public JavaBigDecimal(double a) {
		if(Double.isInfinite(a) || Double.isNaN(a)){
			throw new NumberFormatException();
		}
		edec=CheckExtendedDecimal(ExtendedDecimal.FromDouble(a));
	}

	public JavaBigDecimal(double a, MathContext mc) {
		if(Double.isInfinite(a) || Double.isNaN(a)){
			throw new NumberFormatException();
		}
		edec=CheckExtendedDecimal(ExtendedDecimal.FromDouble(a).RoundToPrecision(GetContext(mc)));
	}

	public JavaBigDecimal setScale(int newScale, int roundingMode) {
		return setScale(newScale, RoundingNumberToRoundingMode(roundingMode));
	}		

	public JavaBigDecimal divide(JavaBigDecimal divisor, int roundingMode) {
		return divide(divisor, RoundingNumberToRoundingMode(roundingMode));
	}

	public JavaBigDecimal divide(JavaBigDecimal divisor, int scale, int roundingMode) {
		return divide(divisor, scale, RoundingNumberToRoundingMode(roundingMode));
	}
}
