#!/usr/bin/env python3
# Importações de bibliotecas ROS e matemáticas
import rospy
from std_msgs.msg import Float32MultiArray, Float32
from math import pi
from numpy import clip as clamp

class Controle_Profundidade:
    def __init__(self):
        # Inicialização das variáveis de estado do controlador PID
        self.integral = 0.0
        self.prev_erro = 0.0
        self.prev_derivada = 0.0
        self.last_time = rospy.Time.now()
        
        # Configuração dos tópicos ROS
        self.sub = rospy.Subscriber('/dadosAltura', Float32MultiArray, self.controleAltura)
        self.pub = rospy.Publisher('/controleAltura', Float32, queue_size=10)

        # Ganhos do controlador PID
        self.Kp = 4.0  # Proporcional
        self.Ki = 2    # Integral
        self.Kd = 0.05 # Derivativo
        
        # Limites de saturação para a massa
        self.min_mass = 1
        self.max_mass = 5

    def controleAltura(self, data):
        msg = Float32()
        # Cálculo do delta de tempo desde a última iteração
        current_time = rospy.Time.now()
        dt = (current_time - self.last_time).to_sec()
        self.last_time = current_time

        # Extrai os dados de posição (alvo e atual)
        targetPosition, currentPosition = data.data

        print(f"Posição desejada: {targetPosition}")
        print(f"Posição atual: {currentPosition}")
        erro = targetPosition - currentPosition
        
        # Lógica do controlador PID (só atua se o erro for significativo)
        if abs(erro) > 0.03:  
            # Termo Integral (acumula o erro ao longo do tempo)
            self.integral += erro * dt
            
            # Termo Derivativo (suavizado com filtro de primeira ordem)
            derivada = (erro - self.prev_erro) / dt if dt > 0 else 0
            derivada = 0.05 * derivada + 0.95 * self.prev_derivada
            
            # Cálculo do sinal PID completo
            pid = self.Kp * erro + self.Ki * self.integral + self.Kd * derivada
            print(f"PID: {pid}")
            
            # Conversão do PID para massa (com saturação)
            massa = clamp(1 + pid, self.min_mass, self.max_mass)
            
            # Publica o comando de controle
            msg.data = massa
            self.pub.publish(msg)
            
            # Atualiza estados para próxima iteração
            self.prev_erro = erro
            self.prev_derivada = derivada
        else:
            # Reset dos termos integrativo/derivativo quando o erro é pequeno
            self.integral = 0.0
            self.prev_derivada = 0.0
            self.prev_erro = 0.0

if __name__ == '__main__':
    # Inicialização do nó ROS
    rospy.init_node('altura_controller')
    try:
        controller = Controle_Profundidade()
        rospy.spin()  # Mantém o programa rodando
    except rospy.ROSInterruptException:
        pass