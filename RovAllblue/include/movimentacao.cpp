#include <movimentacao.h>
#include <vector>
#include <iostream>
#include <array>
#include <imu.h>
#include <motor.h>
#include <servo.h>

template<typename T>
T clamp(T valor, T minimo, T maximo) {
    return std::max(minimo, std::min(valor, maximo));
}

double Movimentacao::getAltura(Data d){
    std::vector<double> dados = d.getDados();
    double altura = dados[0];
    return altura;
};

void Movimentacao::controleAlt(Data d){
    float integral = 0;
    float derivada = 0;
    float prevErro = 0;
    float prevDerivada = 0;
    long tempo = millis();
    double altura = getAltura(d);
    float erro = alturaT - altura;
    while (abs(erro) > 0.01){
        long deltaT = millis() - tempo;
        integral = integral + (erro * deltaT);
        derivada = (erro - prevErro) / deltaT;
        derivada = 0.05 * derivada + 0.95 * prevDerivada;
        float PID = Kp * erro + Ki * integral + Kd * derivada;
        //servo.theta = servo.neutro + pid;
        prevErro = erro;
        prevDerivada = derivada;
        erro = alturaT - getAltura(d);
        tempo = millis();
    };
};

void Movimentacao::controleF(Data d, imu mpu, Motor motor1, Motor motor2){
    float integral = 0;
    float prevErro = 0;
    long tempoAnterior = 0;

    
    double theta = mpu.getZAngle(); 
    double erro = thetaT - theta;

    
    long tempoAtual = millis();
    double dt = (tempoAnterior > 0) ? (tempoAtual - tempoAnterior) / 1000.0 : 0.01;
    tempoAnterior = tempoAtual;

    
    double Kp;
    double Ki;
    double Kd;

    
    integral += erro * dt;
    double derivada = (erro - prevErro) / dt;
    prevErro = erro;

    double T_theta = Kp * erro + Ki * integral + Kd * derivada;

    
    double f = 0.0;

    
    double Y[2][2] = {
        {0.5,  1.0},
        {0.5, -1.0}
    };

    
    double u1 = Y[0][0] * f + Y[0][1] * T_theta;
    double u2 = Y[1][0] * f + Y[1][1] * T_theta;

    
    u1 = clamp(u1, -255.0, 255.0);
    u2 = clamp(u2, -255.0, 255.0);

    motor1.setSpeed(u1);
    motor2.setSpeed(u2);
    
}



