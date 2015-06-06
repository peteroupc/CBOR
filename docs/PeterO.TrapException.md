## PeterO.TrapException

    public class TrapException :
        System.ArithmeticException,
        System.Runtime.Serialization.ISerializable,
        System.Runtime.InteropServices._Exception

Exception thrown for arithmetic trap errors.

### TrapException Constructor

    public TrapException(
        int flag,
        PeterO.PrecisionContext ctx,
        object result);

Initializes a new instance of the TrapException class.

<b>Parameters:</b>

 * <i>flag</i>: A 32-bit signed integer.

 * <i>ctx</i>: A PrecisionContext object.

 * <i>result</i>: An arbitrary object.

### Context

    public PeterO.PrecisionContext Context { get; }

Gets the precision context used during the operation that triggered the trap. May be null.

<b>Returns:</b>

The precision context used during the operation that triggered the trap. May be null.

### Error

    public int Error { get; }

Gets the flag that specifies the kind of error (PrecisionContext.FlagXXX). This will only be one flag, such as FlagInexact or FlagSubnormal.

<b>Returns:</b>

The flag that specifies the kind of error (PrecisionContext.FlagXXX). This will only be one flag, such as FlagInexact or FlagSubnormal.

### Result

    public object Result { get; }

Gets the defined result of the operation that caused the trap.

<b>Returns:</b>

The defined result of the operation that caused the trap.


