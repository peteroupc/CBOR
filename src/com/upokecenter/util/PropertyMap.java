package com.upokecenter.util;
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
class PropertyMap
{
	private static class MethodData {
	  public String name;
	  public Method method;
	}
	
	
	private static Map<Class<?>, List<MethodData>> propertyLists =
			new HashMap<Class<?>, List<MethodData>>();

	private static List<MethodData> GetPropertyList(Class<?> t) {
		synchronized(propertyLists) {
			List<MethodData> ret;
			ret=propertyLists.get(t);
			if (ret!=null) {
				return ret;
			}
			ret = new ArrayList<MethodData>();
			for(Method pi : t.getMethods()) {
				if(pi.getParameterTypes().length==0){
					String methodName=pi.getName();
					if(methodName.startsWith("get") && methodName.length()>3 && 
							methodName.charAt(3)>='A' && methodName.charAt(3)<='Z' &&
							!methodName.equals("getClass") &&
							!methodName.equals("getDeclaringClass")
							){
						MethodData md=new MethodData();
						md.name=methodName.substring(3);
						if(md.name.charAt(0)>='A' && md.name.charAt(0)<='Z'){
							StringBuilder sb=new StringBuilder();
							sb.append((char)(md.name.charAt(0)+0x20));
							sb.append(md.name.substring(1));
							md.name=sb.toString();
						}
						md.method=pi;
						ret.add(md);  
					} else if(methodName.startsWith("is") && methodName.length()>2 && 
							methodName.charAt(2)>='A' && methodName.charAt(2)<='Z'){
						MethodData md=new MethodData();
						md.name=methodName.substring(2);
						if(md.name.charAt(0)>='A' && md.name.charAt(0)<='Z'){
							StringBuilder sb=new StringBuilder();
							sb.append((char)(md.name.charAt(0)+0x20));
							sb.append(md.name.substring(1));
							md.name=sb.toString();
						}
						md.method=pi;
						ret.add(md);  
					}
				}
			}
			propertyLists.put(t, ret);
			return ret;
		}
	}
  
  private CBORObject FromArray(Object arr){
   int length=Array.getLength(arr);
   CBORObject obj=CBORObject.NewArray();
   for(int i=0;i<length;i++){
    obj.Add(CBORObject.FromObject(Array.get(arr,i));
   }
   return obj;
  }
	
	public static Object EnumToObject(Enum<?> value){
		return value.toString();
	}

	public static Iterable<Map.Entry<String, Object>> GetProperties(Object o) {
		List<Map.Entry<String, Object>> ret = 
				new ArrayList<Map.Entry<String, Object>>();
		try {
			for(MethodData key : GetPropertyList(o.getClass())) {
				ret.add(new AbstractMap.SimpleEntry<String, Object>(key.name, 
						key.method.invoke(o)));
			}
			return ret;
		} catch(InvocationTargetException ex){
			throw (RuntimeException)new RuntimeException("").initCause(ex);
		} catch (IllegalAccessException ex) {
			throw (RuntimeException)new RuntimeException("").initCause(ex);
		}
	}
}
