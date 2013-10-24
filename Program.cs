/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 10/24/2013
 * Time: 12:30 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace PeterO
{
	/// <summary>
	/// Description of Program.
	/// </summary>
	public class Program
	{

		public static void Test(String s, byte[] data){
			if(!CBORObject.Read(data).ToString().Equals(s)){
				Console.WriteLine(
					String.Format(
						System.Globalization.CultureInfo.InvariantCulture,
						"Expected {0}, got {1}",
						s,CBORObject.Read(data)));
				Console.ReadLine();
				Environment.Exit(1);
			}
		}
		
		public static void Main(){
		}
	}
}
