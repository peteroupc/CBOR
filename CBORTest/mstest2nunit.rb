#!/usr/bin/ruby
# Written in 2013 by Peter Occil.
# Any copyright to this work is released to the Public Domain.
# https://creativecommons.org/publicdomain/zero/1.0/
#
#

# Converts unit tests to the NUnit testing framework.

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
    data=data.gsub(/\[TestMethod\]/,"[Test]")
    data=data.gsub(/\[TestClass\]/,"[TestFixture]")
    data=data.gsub(/\.ExceptionType/,".ExpectedException")
    data=data.gsub(/\bAssertFailedException\b/,"AssertionException")
    data=data.gsub(/\bTestMethodAttribute\b/,"TestAttribute")
    data=data.gsub(/\bTestClassAttribute\b/,"TestFixtureAttribute")
    data=data.gsub(/using\s+Microsoft\.VisualStudio\.TestTools\.UnitTesting\;/,"using NUnit.Framework;")
    next data
  }
}
Dir.glob("*.csproj"){|f|
  utf8edit(f){|data|
    data=data.gsub(/<Reference\s+Include\s*=\s*[\'\"]Microsoft\.VisualStudio\.QualityTools\.UnitTestFramework[^\'\"]*[\'\"]\s*\/>/,
       " <Reference Include=\"nunit.framework\" />")
    next data
  }
}
