package com.upokecenter.cbor;
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import java.util.UUID;

    /**
     * Description of CBORTag37.
     */
  class CBORTag37 implements ICBORTag, ICBORConverter<UUID> {

    /**
     * Not documented yet.
     *
     * @return A CBORTypeFilter object.
     */
    public CBORTypeFilter GetTypeFilter() {
      return CBORTypeFilter.ByteString;
    }

    /**
     * {@inheritDoc}
     *
     * Not documented yet.
     */
    public CBORObject ValidateObject(final CBORObject obj) {
      if (obj.getType() != CBORType.ByteString) {
        throw new CBORException("UUID must be a byte string");
      }
      if (obj.GetByteString().length != 16) {
        throw new CBORException("UUID must be 16 bytes long");
      }
      return obj;
    }

    static void AddConverter() {
      CBORObject.AddConverter(UUID.class, new CBORTag37());
    }

    /**
     * <p>ToCBORObject.</p>
     *
     * @param obj a {@link java.util.UUID} object.
     * @return a {@link com.upokecenter.cbor.CBORObject} object.
     */
    public CBORObject ToCBORObject(final UUID obj) {
      byte[] bytes2 = new byte[16];
      long lsb = obj.getLeastSignificantBits();
      long msb = obj.getMostSignificantBits();
      bytes2[0] = (byte)((msb >> 56) & 0xFFL);
      bytes2[1] = (byte)((msb >> 48) & 0xFFL);
      bytes2[2] = (byte)((msb >> 40) & 0xFFL);
      bytes2[3] = (byte)((msb >> 32) & 0xFFL);
      bytes2[4] = (byte)((msb >> 24) & 0xFFL);
      bytes2[5] = (byte)((msb >> 16) & 0xFFL);
      bytes2[6] = (byte)((msb >> 8) & 0xFFL);
      bytes2[7] = (byte)((msb) & 0xFFL);
      bytes2[8] = (byte)((lsb >> 56) & 0xFFL);
      bytes2[9] = (byte)((lsb >> 48) & 0xFFL);
      bytes2[10] = (byte)((lsb >> 40) & 0xFFL);
      bytes2[11] = (byte)((lsb >> 32) & 0xFFL);
      bytes2[12] = (byte)((lsb >> 24) & 0xFFL);
      bytes2[13] = (byte)((lsb >> 16) & 0xFFL);
      bytes2[14] = (byte)((lsb >> 8) & 0xFFL);
      bytes2[15] = (byte)((lsb) & 0xFFL);
      return CBORObject.FromObjectAndTag(bytes2, 37);
    }

  }
