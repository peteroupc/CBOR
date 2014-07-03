package com.upokecenter.test;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.cbor.*;

  public class CBORExceptionTest {
    @Test(expected = CBORException.class)
    public void TestConstructor() {
      throw new CBORException("Test exception");
    }
  }
