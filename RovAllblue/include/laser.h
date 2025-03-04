#ifndef LASER_H
#define LASER_H

#include <Wire.h>
#include <VL53L0X.h>

class Laser {
private:
    VL53L0X sensor;

public:
    Laser() {}  // Construtor vazio

    void iniciar() {
        Wire.begin();
        sensor.setTimeout(500);
        
        if (!sensor.init()) {
            Serial.println("Falha ao detectar e inicializar o sensor!");
            while (1); // Trava o código se houver erro
        }

        sensor.startContinuous(); // Inicia modo contínuo
    }

    int getAltura() {
        int altura = sensor.readRangeContinuousMillimeters();

        return altura;
    }
};

#endif
