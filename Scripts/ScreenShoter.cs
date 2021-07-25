using UnityEngine;
using UnityEngine.SceneManagement;
using System.Drawing;
using System.Drawing.Imaging;

using System.Collections;
using System.Collections.Generic;
using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using static System.Text.Encoding;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt;

public class ScreenShoter : MonoBehaviour
{
	public Camera mainCamera;

	int counter = 1;

    private MqttClient client;
    // The connection information
    public string brokerHostname = "127.0.0.1";
    public int brokerPort = 1883;
    public string userName = "Robot";
    public string password = "1234";
    public TextAsset certificate; //using ssl certificate
    // listen on test/camera/ Topic
    static string subTopic = "test/camera/";

    private string myflag = ""; //flag of condition of the camera
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

    private void Connect()//connecting to the brocker
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
            Debug.Log("Camera connected");
        }
        catch (Exception e)
        {
            Debug.LogError("Connection error: " + e);
        }
    }

    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) //Event on message recieved
    {
        string msg = System.Text.Encoding.UTF8.GetString(e.Message);
        Debug.Log("Received message from " + e.Topic + " : " + msg);
        myflag = msg;
    }

    private void Publish(string _topic, string msg)//definition of publish function
    {
        client.Publish(_topic, System.Text.Encoding.UTF8.GetBytes(msg), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
    }

    private void ConvertToBitmap()
    {
        Bitmap BT = new Bitmap("Assets/Screenshots/Sreenshot" + counter.ToString("00") + "_" + mainCamera.pixelWidth + "x" + mainCamera.pixelHeight + "_" + "_SceneID" + SceneManager.GetActiveScene().name + "." + "png"); //Reading png image as bitmap
        Bitmap WithoutAlpha = new Bitmap(mainCamera.pixelWidth, mainCamera.pixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppRgb);//New bitmap with 32 bytes per pixel 
        Image img = BT;

        try
        {

            for (var i = 0; i < BT.Width; i++)
            {
                for (var j = 0; j < BT.Height; j++)
                {
                    var originalColor = BT.GetPixel(i, j);
                    var grayScale = (int)((originalColor.R + originalColor.G + originalColor.B) / 3); //Calculating every pixel to grayscale
                    //var grayScale = (int)((originalColor.R * 0.3) + (originalColor.G * 0.59) + (originalColor.B * 0.11));
                    var gsPixels = System.Drawing.Color.FromArgb(grayScale, grayScale, grayScale); //Creating new grayscale pixels
                    WithoutAlpha.SetPixel(i, j, gsPixels); //Writing new pixels to bitmap
                    //BT.SetPixel(i, j, gsPixels);
                }
            }
        }
        catch
        {

        }
        WithoutAlpha.Save("Assets/Screenshots/Sreenshot" + counter.ToString("00") + "_" + mainCamera.pixelWidth + "x" + mainCamera.pixelHeight + "_" + "." + "bmp", System.Drawing.Imaging.ImageFormat.Bmp); // Saving 32bpp bitmap as bmp image
    }

    private void FixedUpdate()
    {
        if (myflag == "Snap") 
        {
            ScreenCapture.CaptureScreenshot("Assets/Screenshots/Sreenshot" + counter.ToString("00") + "_" + mainCamera.pixelWidth + "x" + mainCamera.pixelHeight + "_" + "_SceneID" + SceneManager.GetActiveScene().name + "." + "png");//saving screenshot
            ConvertToBitmap();
            counter++;
        }
        myflag = "";
    }
}
