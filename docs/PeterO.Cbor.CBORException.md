## PeterO.Cbor.CBORException

    public class CBORException :
        System.Exception,
        System.Runtime.Serialization.ISerializable,
        System.Runtime.InteropServices._Exception

Exception thrown for errors involving CBOR data.

### CBORException Constructor

    public CBORException();

Initializes a new instance of the CBORException class.

### CBORException Constructor

    public CBORException(
        string message);

Initializes a new instance of the CBORException class.

<b>Parameters:</b>

 * <i>message</i>: A string object.

### CBORException Constructor

    public CBORException(
        string message,
        System.Exception innerException);

Initializes a new instance of the CBORException class. Uses the given message and inner exception.

<b>Parameters:</b>

 * <i>message</i>: A string object.

 * <i>innerException</i>: An Exception object.
