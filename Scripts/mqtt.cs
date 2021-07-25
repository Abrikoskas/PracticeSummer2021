//libraries to make all the code work
using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using static System.Text.Encoding;
//Unyti must have libraries
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//MQTT libraries
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt;


public class mqtt : MonoBehaviour
{
    private MqttClient client;
    // The connection information
    public string brokerHostname = "127.0.0.1";
    public int brokerPort = 1883;
    public string userName = "Abrikos";
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
            Debug.Log("connecting to " + brokerHostname + ":" + brokerPort);
            Connect();
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE };
            client.Subscribe(new string[] { subTopic }, qosLevels);
        }
    }

    private void Connect()
    {
        Debug.Log("about to connect on '" + brokerHostname + "'");
        // Forming a certificate based on a TextAsset
        X509Certificate cert = new X509Certificate();
        cert.Import(certificate.bytes);
        Debug.Log("Using the certificate '" + cert + "'");
        string clientId = Guid.NewGuid().ToString();
        client = new MqttClient(brokerHostname, MqttSettings.MQTT_BROKER_DEFAULT_PORT, false, null, null, MqttSslProtocols.None);
        Debug.Log("About to connect using '" + userName + "' / '" + password + "'");
        try
        {
            client.Connect(clientId, userName, password);
        }
        catch (Exception e)
        {
            Debug.LogError("Connection error: " + e);
        }
    }

    public static bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string msg = System.Text.Encoding.UTF8.GetString(e.Message);
        Debug.Log("Received message from " + e.Topic + " : " + msg);
    }

    private void Publish(string _topic, string msg)
    {
        client.Publish(_topic, System.Text.Encoding.UTF8.GetBytes(msg), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
