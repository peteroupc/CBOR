## PeterO.TrapException

    public class TrapException :
        System.ArithmeticException,
        System.Runtime.Serialization.ISerializable,
        System.Runtime.InteropServices._Exception

<b>Deprecated.</b> Use ETrapException from PeterO.Numbers/com.upokecenter.numbers.

 <b>This class is obsolete. It will be replaced by a new version of this class in a different namespace/package and library, called  `PeterO.Numbers.ETrapException`  in the <a href="https://www.nuget.org/packages/PeterO.Numbers">  `PeterO.Numbers`  </a> library (in .NET), or  `com.upokecenter.numbers.ETrapException`  in the <a href="https://github.com/peteroupc/numbers-java">  `com.github.peteroupc/numbers`  </a> artifact (in Java).</b>

 Exception thrown for arithmetic trap errors.

### Member Summary
* <code>[Context](#Context)</code> - Gets the precision context used during the operation that triggered the trap.
* <code>[Error](#Error)</code> - Gets the flag that specifies the kind of error (PrecisionContext.
* <code>[Result](#Result)</code> - Gets the defined result of the operation that caused the trap.

<a id="Void_ctor_Int32_PeterO_PrecisionContext_System_Object"></a>
### TrapException Constructor

    public TrapException(
        int flag,
        PeterO.PrecisionContext ctx,
        object result);

 Initializes a new instance of the [PeterO.TrapException](PeterO.TrapException.md) class.

    <b>Parameters:</b>

 * <i>flag</i>: A flag that specifies the kind of error (PrecisionContext.FlagXXX). This will only be one flag, such as FlagInexact or FlagSubnormal.

 * <i>ctx</i>: A context object for arbitrary-precision arithmetic settings.

 * <i>result</i>: The desired result of the operation that caused the trap, such as an  `ExtendedDecimal`  or  `ExtendedFloat` .

<a id="Context"></a>
### Context

    public PeterO.PrecisionContext Context { get; }

 Gets the precision context used during the operation that triggered the trap. May be null.

 <b>Returns:</b>

The precision context used during the operation that triggered the trap. May be null.

<a id="Error"></a>
### Error

    public int Error { get; }

 Gets the flag that specifies the kind of error (PrecisionContext.FlagXXX). This will only be one flag, such as FlagInexact or FlagSubnormal.

 <b>Returns:</b>

The flag that specifies the kind of error (PrecisionContext.FlagXXX). This will only be one flag, such as FlagInexact or FlagSubnormal.

<a id="Result"></a>
### Result

    public object Result { get; }

 Gets the defined result of the operation that caused the trap.

 <b>Returns:</b>

The defined result of the operation that caused the trap.
