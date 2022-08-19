/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
https://creativecommons.org/publicdomain/zero/1.0/

 */
using System;

namespace PeterO.Cbor
{
    internal class CBORUuidConverter : ICBORToFromConverter<Guid>
    {
        private static CBORObject ValidateObject(CBORObject obj)
        {
            if (obj.Type != CBORType.ByteString)
            {
                throw new CBORException("UUID must be a byte string");
            }
            byte[] bytes = obj.GetByteString();
            if (bytes.Length != 16)
            {
                throw new CBORException("UUID must be 16 bytes long");
            }
            return obj;
        }

        /// <summary>Internal API.</summary>
        /// <param name='obj'>The parameter <paramref name='obj'/> is an
        /// internal parameter.</param>
        /// <returns>A CBORObject object.</returns>
        public CBORObject ToCBORObject(Guid obj)
        {
            byte[] bytes = PropertyMap.UUIDToBytes(obj);
            return CBORObject.FromObjectAndTag(bytes, 37);
        }

        public Guid FromCBORObject(CBORObject obj)
        {
            if (!obj.HasMostOuterTag(37))
            {
                throw new CBORException("Must have outermost tag 37");
            }
            ValidateObject(obj);
            byte[] bytes = obj.GetByteString();
            char[] guidChars = new char[36];
            string hex = "0123456789abcdef";
            int index = 0;
            for (int i = 0; i < 16; ++i)
            {
                if (i == 4 || i == 6 || i == 8 || i == 10)
                {
                    guidChars[index++] = '-';
                }
                guidChars[index++] = hex[(bytes[i] >> 4) & 15];
                guidChars[index++] = hex[bytes[i] & 15];
            }
            string guidString = new string(guidChars);
            return new Guid(guidString);
        }
    }
}