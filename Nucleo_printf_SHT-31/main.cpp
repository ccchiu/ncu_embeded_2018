#include "mbed.h"
#include "Sht31.h"
#include "pinmap.h"
void grafcet();
void action();
int gra_readTemp,gra_readHum,gra_start;
int X0,X1,X2;
Sht31 Sht31(I2C_SDA, I2C_SCL);
int main() {

	printf("Press blue button to start...\r\n");
	while(1)
	{
		grafcet();
		wait(1);
	}
	
}
void grafcet()
{
	X0=1;
	X1=1;
	X2=1;
   gra_readTemp =1;
	 gra_readHum =1;
   
	if(X0 ==1 )  X0=0,X1=1;
	else if (X1 ==1 && gra_readTemp ==0) X1=1,X0=0;
	else if (X1 ==1 && gra_readTemp == 1) X2=1,X1=0;
	else if (X2 ==1 && gra_readHum == 0) X1=0,X2=0;
	else if (X2 ==1 && gra_readHum == 1) X1=0,X2=1;
	//printf("X0 = %d , X1 = %d , X2 = %d \r\n",X0,X1,X2);
	
	action();
	
}

void action()
{
	if (X1 && gra_readTemp){
		    float t = Sht31.readTemperature();
        printf("Temperature=%3.2f,", t);
	}
	if (X2 && gra_readHum){
		 
        float h = Sht31.readHumidity();
        printf("Humidity=%3.2f %\r\n", h);    
	}
}
