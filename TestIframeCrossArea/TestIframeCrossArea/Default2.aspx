<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"> 
<html xmlns="http://www.w3.org/1999/xhtml"> 
 <head> 
  <title> new document </title> 
  <script>      document.domain = 'mysoft.com';
  </script>
 </head> 
 <body> 
<input type="button" value="测试二(自动创建IFRAME.并弹出其内容)" onclick="test2()"> 
<script type="text/javascript"> 
 
function test2(){
	var ifra;
	if((ifra = document.getElementById('test_iframe')) == null){
		ifra = document.createElement('iframe');
		ifra.src = 'http://testcroosarea.mysoft.com:800/Default.htm'; ;
		ifra.id  = 'test_iframe';
		document.body.appendChild(ifra);
}
setTimeout(function() {
var ifr = document.getElementById('test_iframe').contentWindow.document;
ifr.write("小马哥");
}, 5000);



}
 
</script> 
 </body> 
</html>