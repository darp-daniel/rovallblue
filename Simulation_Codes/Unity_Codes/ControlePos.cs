using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class VelocityController : MonoBehaviour
{
    // [Header] cria seções organizadas no Inspector do Unity
    
    [Header("Configurações ROS")]
    [SerializeField] private string topicName = "/cmd_vel";  // Tópico ROS para receber comandos
    
    [Header("Configurações de Movimento")]
    [SerializeField] private float maxSpeed = 5f;  // Velocidade máxima em m/s
    [SerializeField] private float maxRotationSpeed = 180f; // Velocidade angular máxima em graus/s
    [SerializeField] private float acceleration = 2f;  // Taxa de aceleração
    [SerializeField] private float deceleration = 5f; // Taxa de desaceleração
    [SerializeField] private float deadZone = 0.01f;   // Zona morta para ignorar pequenos inputs
    
    [Header("Limites de Posição")]
    [SerializeField] private float minXPosition = -10f; // Limite mínimo no eixo X
    [SerializeField] private float maxXPosition = 10f;  // Limite máximo no eixo X

    // Variáveis privadas de controle
    private ROSConnection ros;  // Conexão ROS
    private float currentVelocity = 0f;  // Velocidade linear atual
    private float currentRotationVelocity = 0f;  // Velocidade angular atual
    private float lastMessageTime = 0f;  // Último tempo de recebimento de mensagem
    private float messageTimeout = 0.5f; // Tempo (segundos) para parada automática

    void Start()
    {
        // Inicializa a conexão ROS e se inscreve no tópico
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<TwistMsg>(topicName, HandleVelocityCommand);
    }

    // Callback para comandos de velocidade recebidos via ROS
    void HandleVelocityCommand(TwistMsg msg)
    {
        lastMessageTime = Time.time;  // Atualiza o tempo da última mensagem
        
        // Converte os valores double do ROS para float do Unity
        float linearX = (float)msg.linear.x;
        float angularY = (float)msg.angular.z;
        
        // Aplica limites às velocidades
        float targetVelocity = Mathf.Clamp(linearX, -maxSpeed, maxSpeed);
        float targetRotationVelocity = Mathf.Clamp(angularY, -maxRotationSpeed, maxRotationSpeed);
        
        // Atualiza velocidades apenas se o input estiver fora da zona morta
        if (Mathf.Abs(linearX) > deadZone)
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, targetVelocity, 
                                              acceleration * Time.deltaTime);
        }
        
        if (Mathf.Abs(angularY) > deadZone)
        {
            currentRotationVelocity = Mathf.MoveTowards(currentRotationVelocity, targetRotationVelocity, 
                                                      acceleration * Time.deltaTime);
        }
    }

    void Update()
    {
        // Lógica de parada automática por timeout
        if (Time.time - lastMessageTime > messageTimeout)
        {
            currentVelocity = 0f;
            currentRotationVelocity = 0f;
        }
        else
        {
            // Desaceleração natural quando não há input significativo
            if (Mathf.Abs(currentVelocity) > deadZone)
            {
                currentVelocity = Mathf.MoveTowards(currentVelocity, 0, 
                                                   deceleration * Time.deltaTime);
            }
            
            if (Mathf.Abs(currentRotationVelocity) > deadZone)
            {
                currentRotationVelocity = Mathf.MoveTowards(currentRotationVelocity, 0, 
                                                          deceleration * Time.deltaTime);
            }
        }
        
        // Aplica movimento linear
        Vector3 newPosition = transform.position + 
                            transform.right * currentVelocity * Time.deltaTime;
        
        // Restringe posição dentro dos limites definidos
        newPosition.x = Mathf.Clamp(newPosition.x, minXPosition, maxXPosition);
        transform.position = newPosition;
        
        // Aplica rotação (convertendo de graus para radianos)
        transform.Rotate(0, currentRotationVelocity * Time.deltaTime * 180 * Mathf.PI, 0);
    }

    void OnDestroy()
    {
        // Limpeza: cancela a inscrição no tópico ao destruir o objeto
        if (ros != null)
        {
            ros.Unsubscribe(topicName);
        }
    }
}