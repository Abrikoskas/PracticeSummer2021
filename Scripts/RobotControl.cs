using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using static System.Text.Encoding;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt;

public class RobotControl : MonoBehaviour
{
    public GameObject robot;
    public WheelCollider[] WColLeft;
    public WheelCollider[] WColRight;
    public float maxBrake = 50f;
    public float maxAccel = 10f;
    public float angle = 1f;

    private MqttClient client;
    // The connection information
    public string brokerHostname = "127.0.0.1";
    public int brokerPort = 1883;
    public string userName = "Robot";
    public string password = "1234";
    public TextAsset certificate; //using ssl certificate
    // listen on all the Topic
    static string subTopic = "test/";

    private string myflag = "";
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
        Debug.Log("Using the certificate '" + cert + "'");
        string clientId = Guid.NewGuid().ToString();
        client = new MqttClient(brokerHostname, brokerPort, false, null, null, MqttSslProtocols.None);
        //Debug.Log("About to connect using '" + userName + "' / '" + password + "'");
        try
        {
            client.Connect(clientId, userName, password);
            Debug.Log("Robot connected");
        }
        catch (Exception e)
        {
            Debug.LogError("Connection error: " + e);
        }
    }

    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string msg = System.Text.Encoding.UTF8.GetString(e.Message);
        Debug.Log("Received message from " + e.Topic + " : " + msg);
        myflag = msg;
    }

    private void Publish(string _topic, string msg)
    {
        client.Publish(_topic, System.Text.Encoding.UTF8.GetBytes(msg), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
    }

    private void RunOnMessage(string msg)//Robot move depends on message recieved
    {
        if (msg == "Forward")
        {
            foreach (WheelCollider col in WColLeft)
            {
                col.brakeTorque = 0;
                col.motorTorque = maxAccel;
            };
            foreach (WheelCollider col in WColRight)
            {
                col.brakeTorque = 0;
                col.motorTorque = maxAccel;
            };
            Publish("test/callback/", "Ok");
        }
        else if (msg == "Backward")
        {
            foreach (WheelCollider col in WColLeft)
            {
                col.brakeTorque = 0;
                col.motorTorque = -maxAccel;
            };
            foreach (WheelCollider col in WColRight)
            {
                col.brakeTorque = 0;
                col.motorTorque = -maxAccel;
            };
            Publish("test/callback/", "Ok");
        }
        else if (msg == "Right")
        {
            transform.Rotate(0, angle, 0);
            Publish("test/callback/", "Ok");
        }
        else if (msg == "Left")
        {
            transform.Rotate(0, -angle, 0);
            Publish("test/callback/", "Ok");
        }
        else
        {
            foreach (WheelCollider col in WColLeft)
            {
                col.brakeTorque = maxBrake;
            }; 
            foreach (WheelCollider col in WColRight)
            {
                col.brakeTorque = maxBrake;
            };
        };
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RunOnMessage(myflag);
        myflag = "";


        /*foreach (WheelCollider col in WColLeft)
        {
            col.brakeTorque = maxBrake;
        }
        foreach (WheelCollider col in WColRight)
        {
            col.brakeTorque = maxBrake;
        }*/
    }
}
