using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using static System.Text.Encoding;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt;

public class Controller : MonoBehaviour
{
    private MqttClient client;
    // The connection information
    public string brokerHostname = "127.0.0.1";
    public int brokerPort = 1883;
    public string userName = "Controller";
    public string password = "1234";
    public TextAsset certificate; //using ssl certificate
    // listen on all the Topic
    static string subTopic = "test/";
    // Start is called before the first frame update

    // Use this for initialization
    void Start()
    {
        if (brokerHostname != null && userName != null && password != null)
        {
            //Debug.Log("connecting to " + brokerHostname + ":" + brokerPort);
            Connect();
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE };
            client.Subscribe(new string[] { subTopic }, qosLevels);
        }
    }

    private void Connect()
    {
        //Debug.Log("about to connect on '" + brokerHostname + "'");
        // Forming a certificate based on a TextAsset
        X509Certificate cert = new X509Certificate();
        cert.Import(certificate.bytes);
        //Debug.Log("Using the certificate '" + cert + "'");
        string clientId = Guid.NewGuid().ToString();
        client = new MqttClient(brokerHostname, brokerPort, false, null, null, MqttSslProtocols.None);
        //Debug.Log("About to connect using '" + userName + "' / '" + password + "'");
        try
        {
            client.Connect(clientId, userName, password);
            Debug.Log("Controller connected");
        }
        catch (Exception e)
        {
            Debug.LogError("Connection error: " + e);
        }
    }

    private void Publish(string _topic, string msg)
    {
        client.Publish(_topic, System.Text.Encoding.UTF8.GetBytes(msg), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
    }

    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string msg = System.Text.Encoding.UTF8.GetString(e.Message);
        //Debug.Log("Received message from " + e.Topic + " : " + msg);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float accel = 0;
        float steer = 0;
        accel = Input.GetAxis("Vertical"); //var for control buttons. Int in range -1;1 
        steer = Input.GetAxis("Horizontal");

        if (Input.GetMouseButtonDown(0))
        {
            Publish("test/camera/", "Snap");
            Debug.Log("Sent message to robot to take screenshot");
        };

        RobotMove(accel, steer);
    }

    private void RobotMove(float accel, float steer)
    {
        if (steer != 0)
        {
            if (steer < 0)
            {
                Publish("test/", "Left");
            };
            if (steer > 0)
            {
                Publish("test/", "Right");
            };
        }

        if (accel != 0)
        {
            if (accel < 0)
            {
                Publish("test/", "Backward");
            };
            if (accel > 0)
            {
                Publish("test/", "Forward");
            };
        }
    }
}
