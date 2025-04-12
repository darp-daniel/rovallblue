using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class Altura : MonoBehaviour
{
    public float targetPosition;
    public Rigidbody rb;

    public float rb.mass;

    public float currentPosition;

    private ROSConnection ros;
    private string Sub = "/controleAltura";
    private string Pub = "/unityPublisherAltura";

    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<Float32Msg>(Sub);
        ros.Subscribe<Float32Msg>(Pub, changeMass);
        Debug.Log("Conectado ao ROS");
    }

    // Update is called once per frame
    void Update()
    {
        float movimento_vertical = Input.GetAxis("Vertical");
        targetPosition += movimento_vertical * Time.deltaTime * 0.1f;
        currentPosition = transform.position.y;
    }
    void SendPosition()
    {
        Float32Msg msg = new Float32Msg();
        msg.data = new float[]{
            targetPosition,
            currentPosition
        }
        ros.Publish(Sub, msg);
}
    void changeMass(Float32Msg mass){
        rb.mass = mass.data;
    }
