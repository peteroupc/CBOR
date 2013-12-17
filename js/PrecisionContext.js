
var PrecisionContext = 

function(precision, rounding, exponentMinSmall, exponentMaxSmall, clampNormalExponents) {

    if ((precision) < 0) throw ("precision" + " not greater or equal to " + "0" + " (" + (precision) + ")");
    if ((exponentMinSmall) > exponentMaxSmall) throw ("exponentMinSmall" + " not less or equal to " + (exponentMaxSmall) + " (" + (exponentMinSmall) + ")");
    this.bigintPrecision = BigInteger.valueOf(precision);
    this.rounding = rounding;
    this.clampNormalExponents = clampNormalExponents;
    this.hasExponentRange = true;
    this.exponentMax = BigInteger.valueOf(exponentMaxSmall);
    this.exponentMin = BigInteger.valueOf(exponentMinSmall);
};
(function(constructor,prototype){
    prototype.exponentMax = null;
    prototype.getEMax = function() {
        return this.hasExponentRange ? this.exponentMax : BigInteger.ZERO;
    };
    prototype.exponentMin = null;
    prototype.hasExponentRange = null;
    prototype.getHasExponentRange = function() {
        return this.hasExponentRange;
    };
    prototype.getEMin = function() {
        return this.hasExponentRange ? this.exponentMin : BigInteger.ZERO;
    };
    prototype.bigintPrecision = null;
    prototype.getPrecision = function() {
        return this.bigintPrecision;
    };
    prototype.rounding = null;
    prototype.clampNormalExponents = null;
    prototype.getClampNormalExponents = function() {
        return this.hasExponentRange ? this.clampNormalExponents : false;
    };
    prototype.getRounding = function() {
        return this.rounding;
    };
    prototype.flags = null;
    prototype.hasFlags = null;
    prototype.getHasFlags = function() {
        return this.hasFlags;
    };
    constructor.FlagInexact = 1;
    constructor.FlagRounded = 2;
    constructor.FlagSubnormal = 4;
    constructor.FlagUnderflow = 8;
    constructor.FlagOverflow = 16;
    constructor.FlagClamped = 32;
    prototype.getFlags = function() {
        return this.flags;
    };
    prototype.setFlags = function(value) {
        if (!this.getHasFlags()) throw ("Can't set flags");
        this.flags = value;
    };
    prototype.ExponentWithinRange = function(exponent) {
        if ((exponent) == null) throw ("exponent");
        if (!this.getHasExponentRange()) return true;
        if (this.bigintPrecision.signum() == 0) {
            return exponent.compareTo(this.getEMax()) <= 0;
        } else {
            var bigint = exponent;
            bigint = bigint.add(this.bigintPrecision);
            bigint = bigint.subtract(BigInteger.ONE);
            if (bigint.compareTo(this.getEMin()) < 0) return false;
            if (exponent.compareTo(this.getEMax()) > 0) return false;
            return true;
        }
    };
    prototype.WithRounding = function(rounding) {
        var pc = this.Copy();
        pc.rounding = rounding;
        return pc;
    };
    prototype.WithBlankFlags = function() {
        var pc = this.Copy();
        pc.hasFlags = true;
        pc.flags = 0;
        return pc;
    };
    prototype.WithExponentClamp = function(clamp) {
        var pc = this.Copy();
        pc.clampNormalExponents = clamp;
        return pc;
    };
    prototype.WithExponentRange = function(exponentMin, exponentMax) {
        if ((exponentMin) == null) throw ("exponentMin");
        if (exponentMin.compareTo(exponentMax) > 0) throw ("exponentMin greater than exponentMax");
        var pc = this.Copy();
        pc.hasExponentRange = true;
        pc.exponentMin = exponentMin;
        pc.exponentMax = exponentMax;
        return pc;
    };
    prototype.WithNoFlags = function() {
        var pc = this.Copy();
        pc.hasFlags = false;
        pc.flags = 0;
        return pc;
    };
    prototype.WithUnlimitedExponents = function() {
        var pc = this.Copy();
        pc.hasExponentRange = false;
        return pc;
    };
    prototype.WithPrecision = function(precision) {
        if (precision < 0) throw ("precision" + " not greater or equal to " + "0" + " (" + (precision) + ")");
        var pc = this.Copy();
        pc.bigintPrecision = BigInteger.valueOf(precision);
        return pc;
    };
    prototype.WithBigPrecision = function(bigintPrecision) {
        if ((bigintPrecision) == null) throw ("bigintPrecision");
        if (bigintPrecision.signum() < 0) throw ("precision" + " not greater or equal to " + "0" + " (" + bigintPrecision + ")");
        var pc = this.Copy();
        pc.bigintPrecision = bigintPrecision;
        return pc;
    };
    prototype.Copy = function() {
        var pcnew = new PrecisionContext(0, this.rounding, 0, 0, this.clampNormalExponents);
        pcnew.hasFlags = this.hasFlags;
        pcnew.flags = this.flags;
        pcnew.exponentMax = this.exponentMax;
        pcnew.exponentMin = this.exponentMin;
        pcnew.hasExponentRange = this.hasExponentRange;
        pcnew.bigintPrecision = this.bigintPrecision;
        pcnew.rounding = this.rounding;
        pcnew.clampNormalExponents = this.clampNormalExponents;
        return pcnew;
    };
    constructor.ForPrecision = function(precision) {
        return new PrecisionContext(precision, Rounding.HalfUp, 0, 0, false).WithUnlimitedExponents();
    };
    constructor.ForRounding = function(rounding) {
        return new PrecisionContext(0, rounding, 0, 0, false).WithUnlimitedExponents();
    };
    constructor.ForPrecisionAndRounding = function(precision, rounding) {
        return new PrecisionContext(precision, rounding, 0, 0, false).WithUnlimitedExponents();
    };
    constructor.Unlimited = PrecisionContext.ForPrecision(0);
    constructor.Decimal32 = new PrecisionContext(7, Rounding.HalfEven, -95, 96, true);
    constructor.Decimal64 = new PrecisionContext(16, Rounding.HalfEven, -383, 384, true);
    constructor.Decimal128 = new PrecisionContext(34, Rounding.HalfEven, -6143, 6144, true);
    constructor.CliDecimal = new PrecisionContext(96, Rounding.HalfEven, 0, 28, true);
})(PrecisionContext,PrecisionContext.prototype);


if(typeof exports!=="undefined")exports.PrecisionContext=PrecisionContext;