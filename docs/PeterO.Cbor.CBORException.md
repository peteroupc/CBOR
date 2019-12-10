## PeterO.Cbor.CBORException

    public sealed class CBORException :
        System.Exception,
        System.Runtime.InteropServices._Exception,
        System.Runtime.Serialization.ISerializable

Exception thrown for errors involving CBOR data. This library may throw exceptions of this type in certain cases, notably when errors occur, and may supply messages to those exceptions (the message can be accessed through the  `Message`  property in.NET or the  `getMessage()`  method in Java). These messages are intended to be read by humans to help diagnose the error (or other cause of the exception); they are not intended to be parsed by computer programs, and the exact text of the messages may change at any time between versions of this library.

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
