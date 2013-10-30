/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 10/29/2013
 * Time: 2:26 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NUnit.Framework;

namespace PeterO
{
	/// <summary>
	/// Description of Runner.
	/// </summary>
	public class Runner
	{
		public static void Main(){
			var test=new CBORTest();
			var setup=test.GetType().GetMethod("SetUp");
			if(setup!=null){
				setup.Invoke(test,new object[]{});
			}
			foreach(var method in test.GetType().GetMethods()){
				if(method.Name.StartsWith("Test")){
					Console.WriteLine(method.Name);
					Type exctype=null;
					foreach(var a in method.GetCustomAttributes(false)){
						if(a is ExpectedExceptionAttribute){
							exctype=((ExpectedExceptionAttribute)a).ExpectedException;
							break;
						}
					}
					if(exctype==null){
						method.Invoke(test,new object[]{});
					} else {
						try {
							method.Invoke(test,new object[]{});
						} catch(System.Reflection.TargetInvocationException e){
							if(!e.InnerException.GetType().Equals(exctype))
								throw;
						}
					}
				}
			}
		}

	}
}
