#include <SFE_BMP180.h>
#include <Wire.h>

class Pressao {
private:
    SFE_BMP180 sensor;
    double baseline;

    double getPressure() {
        char status;
        double T, P;
        
        status = sensor.startTemperature();
        if (status != 0) {
            delay(status);
            status = sensor.getTemperature(T);
            if (status != 0) {
                status = sensor.startPressure(3);
                if (status != 0) {
                    delay(status);
                    status = sensor.getPressure(P, T);
                    if (status != 0) {
                        return P;
                    }
                }
            }
        }
        return -1; // Retorna -1 em caso de erro
    }

public:
    Pressao() {}

    bool iniciar() {
        if (sensor.begin()) {
            baseline = getPressure();
            return true;
        }
        return false;
    }

    double getAltura() {
        double P = getPressure();
        if (P != -1) {
            return sensor.altitude(P, baseline);
        }
        return -1; // Retorna -1 em caso de erro na leitura
    }
};