int SensorPin = A0;
int Gnd = A1;
int Vcc = A2;
int value = 0;

void setup() {
  Serial.begin(9600);

  pinMode(SensorPin, INPUT);
  pinMode(Vcc, OUTPUT);
  pinMode(Gnd, OUTPUT);

  digitalWrite(Vcc, HIGH);
  digitalWrite(Gnd, LOW);
}

void loop() {
  value = analogRead(SensorPin);
  Serial.println(value);
  delay(1000);
}
