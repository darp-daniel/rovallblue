#ifndef MOTOR_H
#include <Arduino.h>

#define MOTOR_H

class Motor {
    public:
        Motor(int outA, int outB, int pwmPin, int pwmChannel);

        void begin();

        void setSpeed(int speed);
        void stop();
    private:
        int _outA;
        int _outB;
        int _pwmPin;
        int _pwmChannel;
        int _isStopped;

        void _setupPwm();
};
#endif