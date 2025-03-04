#include <Arduino.h>

class Temperatura {
public:
    Temperatura(int sensorPin) : sensorPin(sensorPin) {}

    float getTemperatura() {
        int analogValue = analogRead(sensorPin);
        return convertToTemperature(analogValue);
    }

private:
    int sensorPin;

    float convertToTemperature(int analogValue) {
        // Example conversion logic (this would depend on the specific sensor)
        float voltage = analogValue * (5.0 / 1023.0); // Convert to voltage
        float temperatureCelsius = (voltage - 0.5) * 100.0; // Convert voltage to temperature
        return temperatureCelsius;
    }
};