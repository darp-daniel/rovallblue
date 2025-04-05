#include <movimentacao.h>
#include <vector>

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


