using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;  // Pacote para conexão ROS no Unity
using RosMessageTypes.Std;             // Tipos de mensagens padrão do ROS
using RosMessageTypes.Geometry;        // Mensagens geométricas do ROS

public class Altura : MonoBehaviour
{
    // Variáveis públicas ajustáveis no Inspector do Unity
    public float targetPosition;       // Posição vertical desejada (altura alvo)
    public Rigidbody rb;               // Referência ao componente Rigidbody do objeto
    public float currentPosition;      // Posição vertical atual do objeto

    // Configuração de comunicação ROS
    private ROSConnection ros;         // Instância da conexão ROS
    private string Pub = "/dadosAltura";     // Tópico para PUBLICAR dados de altura
    private string Sub = "/controleAltura"; // Tópico para RECEBER comandos de massa

    // Inicialização (chamada uma vez no início)
    void Start()
    {
        // Configura a conexão ROS
        ros = ROSConnection.GetOrCreateInstance();
        // Registra os tópicos
        ros.RegisterPublisher<Float32MultiArrayMsg>(Pub);  // Publica no tópico de controle
        ros.Subscribe<Float32Msg>(Sub, changeMass);        // Inscreve para receber massa
        Debug.Log("Conectado ao ROS");  // Confirmação no console
    }

    // Atualização a cada frame (chamada constantemente)
    void Update()
    {
        // Captura input do teclado (setas ou W/S)
        float movimento_vertical = Input.GetAxis("Vertical");
        // Atualiza a posição alvo com base no input
        targetPosition += movimento_vertical * Time.deltaTime * 0.1f;
        // Atualiza a posição atual com a posição Y do objeto
        currentPosition = transform.position.y;
    }

    // Método para enviar posições para o ROS
    void SendPosition()
    {
        // Cria mensagem ROS com array de floats
        Float32MultiArrayMsg msg = new Float32MultiArrayMsg();
        // Preenche com posição alvo e atual
        msg.data = new float[]{
            targetPosition,
            currentPosition
        };
        // Publica no tópico ROS
        ros.Publish(Sub, msg);
    }

    // Callback para receber comandos de massa do ROS
    void changeMass(Float32Msg mass)
    {
        // Aplica a massa recebida ao Rigidbody
        rb.mass = mass.data;
    }
}