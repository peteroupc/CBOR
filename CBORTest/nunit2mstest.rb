#!/usr/bin/ruby
# Written in 2013 by Peter Occil.
# Any copyright to this work is released to the Public Domain.
# https://creativecommons.org/publicdomain/zero/1.0/
#
#

# Converts unit tests to the Microsoft Unit Testing Framework.

###################

# Reads a UTF-8 file to a string, ignoring the byte order mark.
def utf8read(x)
  File.open(x,"rb"){|f|
    if f.getbyte!=0xef || f.getbyte!=0xbb || f.getbyte!=0xbf
      f.pos=0 # skip UTF-8 byte order mark
    end
    data=f.read
    ec1=Encoding::Converter.new("utf-8","utf-16",{
      :undef=>:replace,
      :invalid=>:replace
    })
    ec2=Encoding::Converter.new("utf-16","utf-8",{
      :undef=>:replace,
      :invalid=>:replace,
      :replace=>"\uFFFD"
    })
    data=ec1.convert(data)
    data=ec2.convert(data)
    return data
  }
end

# Writes a UTF-8 string to a file
def utf8write(str,f)
  File.open(f,"wb:utf-8"){|ff|
    ff.write(str)
  }
end

def utf8edit(file,createIfNotFound=false)
  data=""
  found=false
  if !FileTest.exist?(file)
    return if !createIfNotFound
  else
    found=true
    data=utf8read(file)
  end
  return if !data
  data2=yield(data.clone)
  if (createIfNotFound && !found) ||
      (data2!=data && data2!=nil) # nil check for sanity
    utf8write(data2||"",file)
  end
end

##################

Dir.chdir(File.dirname(__FILE__))
Dir.glob("*.cs"){|f|
  utf8edit(f){|data|
    data=data.gsub(/\[Test\]/,"[TestMethod]")
    data=data.gsub(/\[TestFixture\]/,"[TestClass]")
    data=data.gsub(/\.ExpectedException/,".ExceptionType")
    data=data.gsub(/\bAssertionException\b/,"AssertFailedException")
    data=data.gsub(/\bTestAttribute\b/,"TestMethodAttribute")
    data=data.gsub(/\bTestFixtureAttribute\b/,"TestClassAttribute")
    data=data.gsub(/using\s+NUnit\.Framework\;/,"using Microsoft.VisualStudio.TestTools.UnitTesting;")
    next data
  }
}
Dir.glob("*.csproj"){|f|
  utf8edit(f){|data|
    data=data.gsub(/<Reference\s+Include\s*=\s*[\'\"]nunit\.framework[\'\"]\s*\/>/,
       " <Reference Include=\"Microsoft.VisualStudio.QualityTools.UnitTestFramework, "+
       "Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL\" />")
    next data
  }
}
