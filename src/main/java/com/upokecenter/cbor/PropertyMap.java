package com.upokecenter.cbor;
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */

import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.lang.reflect.Array;
import java.util.AbstractMap;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

/**
 * Description of PropertyMap.
 */
class PropertyMap {

  private static class MethodData {
    public String name;
    public Method method;
  }

  private static Map<Class<?>, List<MethodData>> propertyLists =
      new HashMap<Class<?>, List<MethodData>>();

  private static List<MethodData> GetPropertyList(final Class<?> t) {
    synchronized(propertyLists) {
      List<MethodData> ret;
      ret = propertyLists.get(t);
      if (ret != null) {
        return ret;
      }
      ret = new ArrayList<MethodData>();
      for(Method pi : t.getMethods()) {
        if(pi.getParameterTypes().length == 0) {
          String methodName = pi.getName();
          if(methodName.startsWith("get") && methodName.length() > 3 &&
              methodName.charAt(3) >= 'A' && methodName.charAt(3) <= 'Z' &&
              !methodName.equals("getClass")
              ) {
            MethodData md = new MethodData();
            md.name = methodName.substring(3);
            if(md.name.charAt(0) >= 'A' && md.name.charAt(0) <= 'Z') {
              StringBuilder sb = new StringBuilder();
              sb.append((char)(md.name.charAt(0) + 0x20));
              sb.append(md.name.substring(1));
              md.name = sb.toString();
            }
            md.method = pi;
            ret.add(md);
          } else if(methodName.startsWith("is") && methodName.length() > 2 &&
              methodName.charAt(2) >= 'A' && methodName.charAt(2) <= 'Z') {
            MethodData md = new MethodData();
            md.name = methodName.substring(2);
            if(md.name.charAt(0) >= 'A' && md.name.charAt(0) <= 'Z') {
              StringBuilder sb = new StringBuilder();
              sb.append((char)(md.name.charAt(0) + 0x20));
              sb.append(md.name.substring(1));
              md.name = sb.toString();
            }
            md.method = pi;
            ret.add(md);
          }
        }
      }
      propertyLists.put(t, ret);
      return ret;
    }
  }

  /**
   * <p>FromArray.</p>
   *
   * @param arr a {@link java.lang.Object} object.
   * @return a {@link com.upokecenter.cbor.CBORObject} object.
   */
  public static CBORObject FromArray(final Object arr) {
   int length = Array.getLength(arr);
   CBORObject obj = CBORObject.NewArray();
   for(int i = 0;i < length;i++) {
    obj.Add(CBORObject.FromObject(Array.get(arr,i)));
   }
   return obj;
  }

  /**
   * <p>EnumToObject.</p>
   *
   * @param value a {@link java.lang.Enum} object.
   * @return a {@link java.lang.Object} object.
   */
  public static Object EnumToObject(final Enum<?> value) {
    return value.name();
  }

  /**
   * <p>GetProperties.</p>
   *
   * @param o a {@link java.lang.Object} object.
   * @return a {@link java.lang.Iterable} object.
   */
  public static Iterable<Map.Entry<String, Object>> GetProperties(final Object o) {
    List<Map.Entry<String, Object>> ret =
        new ArrayList<Map.Entry<String, Object>>();
    try {
      for(MethodData key : GetPropertyList(o.getClass())) {
        ret.add(new AbstractMap.SimpleEntry<String, Object>(key.name,
            key.method.invoke(o)));
      }
      return ret;
    } catch(InvocationTargetException ex) {
      throw (RuntimeException)new RuntimeException("").initCause(ex);
    } catch (IllegalAccessException ex) {
      throw (RuntimeException)new RuntimeException("").initCause(ex);
    }
  }

  /**
   * <p>FindOneArgumentMethod.</p>
   *
   * @param obj a {@link java.lang.Object} object.
   * @param name a {@link java.lang.String} object.
   * @param argtype a {@link java.lang.Class} object.
   * @return a {@link java.lang.Object} object.
   */
  public static Object FindOneArgumentMethod(final Object obj, final String name, final Class<?> argtype) {
    try {
      return obj.getClass().getMethod(name, argtype);
    } catch (SecurityException e) {
      return null;
    } catch (NoSuchMethodException e) {
      return null;
    }
  }

  /**
   * <p>InvokeOneArgumentMethod.</p>
   *
   * @param method a {@link java.lang.Object} object.
   * @param obj a {@link java.lang.Object} object.
   * @param argument a {@link java.lang.Object} object.
   * @return a {@link java.lang.Object} object.
   */
  public static Object InvokeOneArgumentMethod(final Object method,
      final Object obj, final Object argument) {
    if(method == null) {
      throw new NullPointerException("method");
    }
    Method m = (Method)method;
    try {
      return m.invoke(obj, argument);
    } catch (IllegalAccessException e) {
      throw new RuntimeException(e);
    } catch (InvocationTargetException e) {
      throw new RuntimeException(e);
    }
  }
}
