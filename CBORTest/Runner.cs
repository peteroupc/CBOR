/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

using System;
using System.Reflection;
using NUnit.Framework;

namespace PeterO
{
	/// <summary>
	/// Description of Runner.
	/// </summary>
	public class Runner
	{
		private static bool HasAttribute(Type mi, Type t){
			foreach(object a in mi.GetCustomAttributes(false)){
				if(t.IsAssignableFrom(a.GetType())){
					return true;
				}
			}
			return false;
		}
		private static bool HasAttribute(MethodInfo mi, Type t){
			foreach(object a in mi.GetCustomAttributes(false)){
				if(t.IsAssignableFrom(a.GetType())){
					return true;
				}
			}
			return false;
		}
		
		public static void Main(){
			new Test.CBORTest().TestRandomData();
			return;
			// Run all the NUnit tests in this assembly
			foreach(var type in Assembly.GetExecutingAssembly().GetTypes()){
				if(!HasAttribute(type,typeof(TestFixtureAttribute)))continue;
				Console.WriteLine("-------");
				Console.WriteLine(type.FullName);
				Console.WriteLine("-------");
				object test=Activator.CreateInstance(type);
				var setup=type.GetMethod("SetUp");
				if(setup!=null){
					setup.Invoke(test,new object[]{});
				}
				foreach(var method in test.GetType().GetMethods()){
					if(!HasAttribute(method,typeof(TestAttribute)))continue;
					Console.WriteLine(method.Name);
					Type exctype=null;
					foreach(var a in method.GetCustomAttributes(false)){
						if(a is ExpectedExceptionAttribute){
							exctype=((ExpectedExceptionAttribute)a).ExpectedException;
							break;
						}
					}
					try {
						method.Invoke(test,new object[]{});
					} catch(System.Reflection.TargetInvocationException e){
						if(exctype==null || !e.InnerException.GetType().Equals(exctype)){
							Console.WriteLine(e.GetType().FullName);
							string message=e.Message;
							if(message.Length>140)
								message=message.Substring(0,140);
							Console.WriteLine(message);
						}
					}
				}
			}
		}

	}
}
