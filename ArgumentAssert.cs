/*
Written by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace PeterO
{
	[DebuggerStepThrough]
	internal static class ArgumentAssertInternal {
		public static void Fail(object value, string message){
			throw new ArgumentException(
				String.Format(CultureInfo.CurrentCulture,"{0} [Value={1}]",message,value));
		}
		public static void Fail(object value, string message, params object[] items){
			throw new ArgumentException(
				String.Format(CultureInfo.CurrentCulture,"{0} [Value={1}]",
				                    String.Format(CultureInfo.CurrentCulture,message,items),value));
		}
		public static void NotEmpty(string value, string paramName){
			if(value==null)
				throw new ArgumentNullException(paramName);
			else if(value.Length==0)
				throw new ArgumentException(value,
				                            String.Format(CultureInfo.CurrentCulture,"'{0}' is null or empty.",paramName));
		}
		public static void NotEmpty<T>(ICollection<T> collection, string paramName){
			if(collection==null)
				throw new ArgumentNullException(paramName);
			if(collection.Count==0)
				throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,"'{0}' is empty.",paramName));
		}
		private static void ArgumentOutOfRange(string paramName, object value, string message){
			throw new ArgumentOutOfRangeException(
				String.Format(CultureInfo.CurrentCulture,"{0} [Parameter name: {1}, value: {2}]",message,paramName,value));
		}
		public static void Less<T>(T value1, T value2, string paramName) where T : IComparable<T>
	        {
			if(Comparer<T>.Default.Compare(value1,value2)>=0)
				ArgumentOutOfRange(paramName,value1,String.Format(CultureInfo.CurrentCulture,"'{0}' must be less than {1}.",paramName,value2));
	        }
		public static void LessOrEqual<T>(T value1, T value2, string paramName) where T : IComparable<T>
	        {
			if(Comparer<T>.Default.Compare(value1,value2)>0)
	            ArgumentOutOfRange(paramName,value1,String.Format(CultureInfo.CurrentCulture,"'{0}' must be less than or equal to {1}.",paramName,value2));
	        }
		public static void Greater<T>(T value1, T value2, string paramName) where T : IComparable<T>
	        {
			if(Comparer<T>.Default.Compare(value1,value2)<=0)
	            ArgumentOutOfRange(paramName,value1,String.Format(CultureInfo.CurrentCulture,"'{0}' must be greater than {1}.",paramName,value2));
	        }
		public static void GreaterOrEqual<T>(T value1, T value2, string paramName) where T : IComparable<T>
	        {
			if(Comparer<T>.Default.Compare(value1,value2)<0)
	            ArgumentOutOfRange(paramName,value1,String.Format(CultureInfo.CurrentCulture,"'{0}' must be greater than or equal to {1}.",paramName,value2));
	        }
		public static void NotNull(object str, string paramName){
			if(str==null)
				throw new ArgumentNullException(paramName);
		}
		public static void IsTrue(bool condition, string message, params object[] options){
			if(!condition)
				throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,message,options));
		}
		public static void CheckIndex(string buffer, int index,
		                              string bufferName, string offsetName){
			ArgumentAssertInternal.NotNull(buffer,bufferName);
			ArgumentAssertInternal.GreaterOrEqual(index,0,offsetName);
			if(index>=buffer.Length){
				throw new ArgumentOutOfRangeException(
					String.Format(CultureInfo.CurrentCulture,
						"'{0}' ({1}) must be less than the length of '{2}' ({3}).",
						offsetName,index,bufferName,buffer.Length)
				);
			}
		}
		public static void CheckIndex<T>(IList<T> buffer, int index,
		                                  string bufferName, string offsetName){
			ArgumentAssertInternal.NotNull(buffer,bufferName);
			ArgumentAssertInternal.GreaterOrEqual(index,0,offsetName);
			if(index>=buffer.Count){
				throw new ArgumentOutOfRangeException(
					String.Format(CultureInfo.CurrentCulture,
						"'{0}' ({1}) must be less than the length of '{2}' ({3}).",
						offsetName,index,bufferName,buffer.Count)
				);
			}
		}
		public static void CheckBuffer<T>(IList<T> buffer, int offset, int count,
		                                  string bufferName, string offsetName, string countName){
			ArgumentAssertInternal.NotNull(buffer,bufferName);
			ArgumentAssertInternal.GreaterOrEqual(offset,0,offsetName);
			ArgumentAssertInternal.GreaterOrEqual(count,0,countName);
			if(offset+count>buffer.Count){
				throw new ArgumentOutOfRangeException(
					String.Format(CultureInfo.CurrentCulture,
						"'{0}' ({1}) plus '{2}' ({3}) must not exceed length of '{4}' ({5}).",
						countName,count,offsetName,offset,bufferName,buffer.Count)
				);
			}			
		}
		public static void CheckRange(int value, int minValue, int maxValue, string paramName){
			if(value<minValue||value>maxValue)
				ArgumentOutOfRange(paramName,value,String.Format(CultureInfo.CurrentCulture,"'{0}' must be from {1} through {2}.",paramName,minValue,maxValue));
		}
		public static void CheckRange(long value, long minValue, long maxValue, string paramName){
			if(value<minValue||value>maxValue)
				ArgumentOutOfRange(paramName,value,String.Format(CultureInfo.CurrentCulture,"'{0}' must be from {1} through {2}.",paramName,minValue,maxValue));
		}
	}
}
