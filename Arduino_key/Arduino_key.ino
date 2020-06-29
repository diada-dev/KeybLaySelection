void setup() {
  // put your setup code here, to run once:
Serial.begin(9600);
pinMode(2, INPUT);
pinMode(3, INPUT);
pinMode(4, INPUT);

}

void loop() {
   if (digitalRead(2)==LOW){
      Serial.write(170);
      Serial.write(1);  
      delay(1000);    
   }

   if (digitalRead(3)==LOW){
      Serial.write(170);
      Serial.write(2);   
      delay(1000);    
   }

   if (digitalRead(4)==LOW){
      Serial.write(170);
      Serial.write(3);  
      delay(1000);    
   }

}
