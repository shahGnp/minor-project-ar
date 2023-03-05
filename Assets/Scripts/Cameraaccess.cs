using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Net.Http;
using System.IO;
using System.Drawing;
using System.Net;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UnityEngine.SceneManagement;
public class serverRes
{
    public string classType;
}

public class Cameraaccess : MonoBehaviour
{
    /*public SnapshotCamera snapCam;*/
    public string fileName = "webcam_image.png";
    public int width = 1920;
    public int height = 1080;

    private static readonly HttpClient client = new HttpClient();

    private UnityEngine.WebCamTexture camTexture;
    private UnityEngine.Texture defaultbackground;

    public RawImage backround; //To display our image
    private bool camAviable;
    public AspectRatioFitter fit;
    public GameObject test;
    public Text textField;
    public Button btn;

    void Start()
    {

        defaultbackground = backround.texture;

        // Get all available cameras
        WebCamDevice[] devices = WebCamTexture.devices;


        if (devices.Length == 0)
        {
            Debug.Log("No camera detected");
            camAviable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing)
            {
                camTexture = new WebCamTexture(devices[0].name, Screen.width, Screen.height);
            }
        }

        if (camTexture == null)
        {
            Debug.Log("Unable to log back camera");
            return;
        }


        // Assign the camera texture to a material
        /*GetComponent<Renderer>().material.mainTexture = camTexture;*/

        // Start the camera
        camTexture.Play();
        backround.texture = camTexture;
        camAviable = true;


        btn.onClick.AddListener(SavePhoto);

    }

    void Update()
    {
        if (!camAviable)
            return;

        float ratio = (float)camTexture.width / (float)camTexture.height;
        fit.aspectRatio = ratio;

        float scaleY = camTexture.videoVerticallyMirrored ? -1f : 1f;
        backround.rectTransform.localScale = new Vector3(1f, scaleY, 1f);


    }


    public void SavePhoto()
    {
        string send = "button click vayo";
        Texture2D photo = new Texture2D(camTexture.width, camTexture.height);
        photo.SetPixels(camTexture.GetPixels());
        photo.Apply();

        //Encode to a PNG
        byte[] bytes = photo.EncodeToJPG();
        StartCoroutine(
        //Write out the PNG. Of course you have to substitute your_path for something sensible
        /*        System.IO.File.WriteAllBytes(Application.dataPath + "/photo.png", bytes);
        */        predictImage(bytes));
    }

    IEnumerator predictImage(byte[] bytes)
    {
        string send = "";

        
            WWWForm form = new WWWForm();
            form.AddField("getlist", "all");

            string base64Image = Convert.ToBase64String(bytes);

            string url = "http://74.235.83.70:80/predict/";
/*            string url = "http://localhost/predict/";*/

            UnityWebRequest request = new UnityWebRequest(url, "POST");
            request.SetRequestHeader("Content-Type", "application/json");

            send = "thikcha";


            JObject jsonobjectReq = new JObject();

            jsonobjectReq["imgstr"] = base64Image;

            byte[] bodyRaw = new System.Text.UTF8Encoding(true).GetBytes(jsonobjectReq.ToString());

/*            byte[] bodyRaw = Encoding.UTF8.GetBytes((string)jsonobjectReq);
*/
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            Debug.Log("Sending request");


            yield return request.SendWebRequest();
            Debug.Log("Sending request complete");
        
            try
            {
                string jsonResponse = request.downloadHandler.text;
                serverRes output = new serverRes();
                output = JsonUtility.FromJson<serverRes>(jsonResponse);
                Debug.Log("response start: ");
                string classType = output.classType;
                Debug.Log("What i got?: "+classType);
                Debug.Log("response end: ");
                print("Response block complete");
                
            switch (classType)
            {

                case "frog":
                    SceneManager.LoadScene("Frog");
                    break;
                case "airplane":
                    SceneManager.LoadScene("Airplane");
                    break;
                case "car":
                    SceneManager.LoadScene("Car");
                    break;
                case "bird":
                    SceneManager.LoadScene("Bird");
                    break;
                case "cat":
                    SceneManager.LoadScene("Cat");
                    break;
                case "deer":
                    SceneManager.LoadScene("Deer");
                    break;
                case "dog":
                    SceneManager.LoadScene("Dog");
                    break;
                case "horse":
                    SceneManager.LoadScene("Horse");
                    break;
                case "ship":
                    SceneManager.LoadScene("Ship");
                    break;
                case "truck":
                    SceneManager.LoadScene("Truck");
                    break;
                default:
                    Debug.Log("kuch nai");
                    break;
            }

            }
            catch
            {
                print("Response from the server not recevied");
            }
            
           







        /*if (request.result == UnityWebRequest.Result.Success)
        {

        }
        else
        {
            Debug.LogError("Error sending POST request: " + request.error);
        }*/

    }

        /*catch (System.Exception e)
        {
            print(e.ToString());
            send = e.ToString();
            send = "thikchaina";

        }

        finally
        {
            Debug.Log("La sakkyo");
        }

    }*/

}