#include <motor.h>

Motor::Motor(int outA, int outB, int pwmPin, int pwmChannel){
    _outA = outA;
    _outB = outB;
    _pwmPin = pwmPin;
    _pwmChannel = pwmChannel;
    _isStopped = true;
}

void Motor::begin(){
    pinMode(_outA, OUTPUT);
    pinMode(_outB, OUTPUT);
    _setupPwm();
    stop();
}

void Motor::setSpeed(int speed){
    if (speed == 0){
        stop();
        return;
    }
    bool isFoward = speed > 0;
    digitalWrite(_outA, isFoward ? HIGH : LOW);
    digitalWrite(_outB, isFoward ? LOW : HIGH);

    ledcWrite(_pwmChannel, abs(speed));
    _isStopped = false;
}

void Motor::stop(){
    digitalWrite(_outA, LOW);
    digitalWrite(_outB, LOW);
    ledcWrite(_pwmChannel, 0);
    _isStopped = true;
}

void Motor::_setupPwm(){
    const int pwmFreq = 5000;
    const int pwmReso = 8;
    ledcSetup(_pwmChannel, pwmFreq, pwmReso);
    ledcAttachPin(_pwmPin, _pwmChannel);
}

