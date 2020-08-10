#include <DHT.h>
#define DHTTYPE DHT11

int dhtPin = 10;
int dhtVcc = 9;
int dhtGrd = 8;

int photoPin = A0;
int photoGnd = A1;
int photoVcc = A2;
int iP = 0;
float fT = 0;
float fH = 0;
DHT dht(dhtPin, DHTTYPE);

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  
  pinMode(dhtPin, INPUT);
  pinMode(dhtVcc, OUTPUT);
  pinMode(dhtGrd, OUTPUT);

  digitalWrite(dhtVcc, HIGH);
  digitalWrite(dhtGrd, LOW);

  pinMode(photoPin, INPUT);
  pinMode(photoVcc, OUTPUT);
  pinMode(photoGnd, OUTPUT);

  digitalWrite(photoVcc, HIGH);
  digitalWrite(photoGnd, LOW);

  dht.begin();
}

void loop() {
  iP = analogRead(photoPin);
  fT = dht.readTemperature();
  fH = dht.readHumidity();

  Serial.print(iP);
  if(isnan(fT) || isnan(fH))
  {
    Serial.print("/");
    Serial.print("0");
    Serial.print("/");
    Serial.println("0");
  }
  else
  {
    Serial.print("/");
    Serial.print(fT);
    Serial.print("/");
    Serial.println(fH);
  }
  
  
  
  
  delay(1000);
}
