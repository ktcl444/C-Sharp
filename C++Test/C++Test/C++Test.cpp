// C++Test.cpp : 定义控制台应用程序的入口点。
//
#include "stdio.h"
#include "stdafx.h"

int n[]={0x48, 0x65,0x6C,0x6C,  
0x6F,0x2C,0x20,  
0x77,0x6F,0x72,  
0x6C,0x64,0x21,  
0x0A,0x00},*m=n; 
void _tmain(int* argc)
{
if(putchar (*m)!='\0') 
        _tmain(m++); 
      
        
        printf("%s:%c%d", "pipi", 41, 88); 
          getchar();


}




