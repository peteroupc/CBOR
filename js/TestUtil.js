var Assert={
AreEqual:function(x,y,ln){
 if(typeof ln!="undefined")ln="\n"+ln
 if(x.equals ? !x.equals(y) : x!=y){
  console.log("Expected "+x+", was "+y+ln)
 }
},
Fail:function(x){
 console.log("Failed: "+x)
}
}