# More Examples

## Number Conversion

Converting a hex string to a big integer:
```c#
public static BigInteger HexToBigInteger(string hexString){
  // Parse the hexadecimal string as a big integer.  Will
  // throw a FormatException if the parsing fails
  var bigInteger = BigInteger.fromRadixString(hexString, 16);
  // Optional: Check if the parsed integer is negative
  if(bigInteger.Sign < 0)
    throw new FormatException("negative hex string");
  return bigInteger;
}
```

Converting a big integer to a `double`:
```c#
public static double BigIntegerToDouble(BigInteger bigInteger){
 return ExtendedFloat.FromBigInteger(bigInteger).ToDouble();
}
```

Converting a number string to a `double`:
```c#
public static double StringToDouble(string str){
 return ExtendedFloat.FromString(str).ToDouble();
}
```
