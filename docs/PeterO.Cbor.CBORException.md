## PeterO.Cbor.CBORException

    public class CBORException :
        System.Exception,
        System.Runtime.Serialization.ISerializable,
        System.Runtime.InteropServices._Exception

 Exception thrown for errors involving CBOR data.

### Member Summary

<a id="Void_ctor_System_String"></a>
### CBORException Constructor

    public CBORException(
        string message);

 Initializes a new instance of the [PeterO.Cbor.CBORException](PeterO.Cbor.CBORException.md) class.

  <b>Parameters:</b>

 * <i>message</i>: The parameter  <i>message</i>
 is a text string.

<a id="Void_ctor_System_String_System_Exception"></a>
### CBORException Constructor

    public CBORException(
        string message,
        System.Exception innerException);

 Initializes a new instance of the [PeterO.Cbor.CBORException](PeterO.Cbor.CBORException.md) class. Uses the given message and inner exception.

  <b>Parameters:</b>

 * <i>message</i>: The parameter  <i>message</i>
 is a text string.

 * <i>innerException</i>: The parameter  <i>innerException</i>
 is an Exception object.

<a id="Void_ctor"></a>
### CBORException Constructor

    public CBORException();

 Initializes a new instance of the [PeterO.Cbor.CBORException](PeterO.Cbor.CBORException.md) class.
