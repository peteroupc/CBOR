/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/24/2014
 * Time: 10:26 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PeterO.Mail
{
    /// <summary>A generic interface for reading data one byte at a time.</summary>
  internal interface ITransform {
    /// <summary>Reads a byte from the data source.</summary>
    /// <returns>The byte read, or -1 if the end of the source is reached.</returns>
    int ReadByte();
  }
}
