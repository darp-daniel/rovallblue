#!/usr/bin/env python3
# Importações para ROS, controle de terminal e mensagens
import rospy
from geometry_msgs.msg import Twist
import sys
import select
import tty
import termios
from sensor_msgs.msg import Joy

class JoystickControl:
    def __init__(self):
        # Configuração de publisher (cmd_vel) e subscriber (joy)
        self.pub = rospy.Publisher('/cmd_vel', Twist, queue_size=10)
        self.sub = rospy.Subscriber('/joy', Joy, self.joy_callback)
        self.last_publish_time = rospy.Time.now()
        
        # Parâmetros de velocidade e timeout
        self.max_linear_vel = 2.0  # Velocidade linear máxima em m/s
        self.max_angular_vel = 90.0  # Velocidade angular máxima em graus/s
        self.timeout = rospy.Duration(0.1)  # Tempo para zerar comandos sem input

    def joy_callback(self, data):
        msg = Twist()
        # Lógica de deadzone - só envia comandos se o joystick estiver sendo movido
        if abs(data.axes[1]) > 0.1 or abs(data.axes[0]) > 0.1:
            # Mapeia os eixos do joystick para velocidades
            msg.linear.x = data.axes[1] * self.max_linear_vel
            msg.angular.z = data.axes[0] * self.max_angular_vel
            self.last_publish_time = rospy.Time.now()
        else:
            # Zera os comandos se o joystick estiver centralizado e o timeout expirar
            if (rospy.Time.now() - self.last_publish_time) > self.timeout:
                msg.linear.x = 0.0
                msg.angular.z = 0.0
        
        self.pub.publish(msg)
        # Log throttled para evitar sobrecarga do terminal
        rospy.loginfo_throttle(0.1, f"Linear Vel: {msg.linear.x:.2f} m/s, Angular Vel: {msg.angular.z:.1f}°/s")

class KeyboardControl:
    def __init__(self):
        # Configuração do publisher e parâmetros do terminal
        self.pub = rospy.Publisher('/cmd_vel', Twist, queue_size=10)
        self.rate = rospy.Rate(10)  # 10Hz
        self.settings = termios.tcgetattr(sys.stdin)  # Salva configurações do terminal
        self.last_key_time = rospy.Time.now()
        self.timeout = rospy.Duration(0.2)  # Timeout para auto-zero

        # Parâmetros de controle incremental
        self.linear_vel = 0.0
        self.angular_vel = 0.0
        self.linear_step = 1  # Incremento de velocidade linear (m/s)
        self.angular_step = 30.0  # Incremento de velocidade angular (graus/s)
    
    def getKey(self):
        # Captura de tecla não-bloqueante
        tty.setraw(sys.stdin.fileno())
        rlist, _, _ = select.select([sys.stdin], [], [], 0.1)
        if rlist:
            key = sys.stdin.read(1)
        else:
            key = ''
        termios.tcsetattr(sys.stdin, termios.TCSADRAIN, self.settings)
        return key
    
    def run(self):
        # Interface de usuário
        print("Keyboard Velocity Control Instructions:")
        print("W/S: Increase/Decrease forward velocity")
        print("A/D: Increase/Decrease rotational velocity")
        print("Space: Immediate stop")
        print("Q: Quit")

        while not rospy.is_shutdown():
            key = self.getKey()
            current_time = rospy.Time.now()
            
            # Lógica de controle por teclado
            if key == 'w':
                self.linear_vel += self.linear_step
                self.last_key_time = current_time
            elif key == 's':
                self.linear_vel -= self.linear_step
                self.last_key_time = current_time
            elif key == 'a':
                self.angular_vel += self.angular_step
                self.last_key_time = current_time
            elif key == 'd':
                self.angular_vel -= self.angular_step
                self.last_key_time = current_time
            elif key == ' ':
                # Parada emergencial
                self.linear_vel = 0.0
                self.angular_vel = 0.0
                self.last_key_time = current_time
            elif key == 'q':
                break
            
            # Auto-zero após timeout
            if (current_time - self.last_key_time) > self.timeout:
                self.linear_vel = 0.0
                self.angular_vel = 0.0

            # Publica a mensagem de velocidade
            msg = Twist()
            msg.linear.x = self.linear_vel
            msg.angular.z = self.angular_vel
            self.pub.publish(msg)
            
            self.rate.sleep()
        
        print("\nShutting down...")

if __name__ == '__main__':
    rospy.init_node('velocity_controller')
    
    # Seletor de método de controle (altere para "joystick" se necessário)
    control_method = "keyboard"
    
    try:
        if control_method == "joystick":
            jc = JoystickControl()
            rospy.spin()
        else:
            kc = KeyboardControl()
            kc.run()
    except rospy.ROSInterruptException:
        pass
    finally:
        # Restaura configurações do terminal no modo keyboard
        if control_method == "keyboard":
            termios.tcsetattr(sys.stdin, termios.TCSADRAIN, kc.settings)