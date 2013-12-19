if(typeof require!="undefined"){
var sys=require("sys");
var console={};
console.log=sys.puts
}

var Assert={
AreEqual:function(x,y,ln){
 if(typeof ln!="undefined")ln="\n"+ln
 if(x==null || y==null){
  if((x==null)!=(y==null)){
    console.log("Expected "+(x||"[]")+", was "+(y||"[]")+" "+(ln||""))
  }
 } else if(x.equals ? !x.equals(y) : x!=y){
  console.log("Expected "+x+", was "+y+" "+(ln||""))
 }
},
Fail:function(x){
 console.log("Failed: "+x)
}
}

if(typeof module!="undefined")
module.exports=Assert;