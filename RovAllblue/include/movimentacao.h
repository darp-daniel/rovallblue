#include <motor.h>
#include <servo.h>
#include <data.h>
#include <imu.h>

class Movimentacao{
    private:
        Data data;
        double thetaT;
        double alturaT;
        float Kp;
        float Ki;
        float Kd;
        void start(){
            data.start();
        };
    public:
        Movimentacao() {
            std::vector<double> dados = data.getDados();
            alturaT = dados[0];
        };
        double getAltura(Data d);
        void controleAlt(Data d);
        void controleF(Data d, imu mpu, Motor motor1, Motor motor2);
};