## PeterO.Cbor.CBORException

    public class CBORException :
        System.Exception,
        System.Runtime.InteropServices._Exception,
        System.Runtime.Serialization.ISerializable

Exception thrown for errors involving CBOR data.

### CBORException Constructor

    public CBORException(
        string message);

Initializes a new instance of the [PeterO.Cbor.CBORException](PeterO.Cbor.CBORException.md) class.

<b>Parameters:</b>

 * <i>message</i>: A text string.

### CBORException Constructor

    public CBORException(
        string message,
        System.Exception innerException);

Initializes a new instance of the [PeterO.Cbor.CBORException](PeterO.Cbor.CBORException.md) class. Uses the given message and inner exception.

<b>Parameters:</b>

 * <i>message</i>: A text string.

 * <i>innerException</i>: An Exception object.

### CBORException Constructor

    public CBORException();

Initializes a new instance of the [PeterO.Cbor.CBORException](PeterO.Cbor.CBORException.md) class.
