## Interop Notes

### Writing CBOR objects using System.Formats.Cbor.CborWriter

Assumes that the `System.Formats.Cbor` namespace was declared and the `System.Formats.Cbor` package was installed.

```
    public CborWriter WriteUsingCborWriter(CBORObject obj, CborWriter writer, int depth) {
      if (depth>50) {
        // Fail for unusual depth
        throw new InvalidOperationException("too deep");
      }
      foreach(var tag in obj.GetAllTags()){
        writer.WriteTag((CborTag)(ulong)tag);
      }
      obj=obj.Untag();
      switch(obj.Type) {
         case CBORType.Integer:
            if(obj.CanValueFitInInt64())
               writer.WriteInt64(obj.AsInt64Value());
            else {
               var ei=obj.AsEIntegerValue();
               if(ei<0) {
                 ei=ei.Abs()-1;
                 writer.WriteCborNegativeIntegerRepresentation((ulong)ei);
               } else {
                 writer.WriteUInt64((ulong)ei);
               }
            }
            break;
         case CBORType.FloatingPoint:
            writer.WriteDouble(obj.AsDouble());
            break;
         case CBORType.Boolean:
            if(obj.IsTrue)writer.WriteSimpleValue(CborSimpleValue.True);
            else writer.WriteSimpleValue(CborSimpleValue.False);
            break;
         case CBORType.SimpleValue:
            writer.WriteSimpleValue((CborSimpleValue)obj.SimpleValue);
            break;
         case CBORType.ByteString:
            writer.WriteByteString(obj.GetByteString());
            break;
         case CBORType.TextString:
            writer.WriteTextString(obj.AsString());
            break;
         case CBORType.Array:
            writer.WriteStartArray(obj.Count);
            foreach(var o in obj.Values){
               WriteUsingCborWriter(o, writer, depth+1);
            }
            writer.WriteEndArray();
            break;
         case CBORType.Map:
            writer.WriteStartMap(obj.Count);
            foreach(var o in obj.Keys){
               WriteUsingCborWriter(o, writer, depth+1);
               WriteUsingCborWriter(obj[o], writer, depth+1);
            }
            writer.WriteEndMap();
            break;
      }
      return writer;
    }
```
