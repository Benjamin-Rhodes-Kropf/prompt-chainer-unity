using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PromptChainerAPI : MonoBehaviour
{
    //put your url and private key here
    private const string url = "https://api.promptchainer.io/api/flows/run/clod44mjr03crmc0osluug9wo";
    private const string apiKey = "d0bca3b6-a646-424b-9989-3fc57b5af5b7";
    
    [Header("input space")]
    public Variables variables;
    public PostData postData;

    [Header("output space")] 
    public List<OutputObject> outputObjects;


    void Start()
    {
        StartCoroutine(PostRequest(url));
    }

    //input space
    [System.Serializable]
    public class Variables
    {
        //define all of your variables here in this instance I have "InputA" and "InputB" (make sure they match with your variables in prompt-chainer.io)
        public string InputA;
        public string InputB;
    }

    [System.Serializable]
    public class PostData
    {
        //this must be called variables to match prompt-chainer.io api request
        public Variables variables;
    }
    
    //output space this must not be changed as it is dependant on prompt-chainer.io api response
    [System.Serializable]
    public class OutputObjectList
    {
        public List<OutputObject> list;
    }
    
    [System.Serializable]
    public class OutputObject
    {
        public string name;
        public string type;
        public string output;
    }

    IEnumerator PostRequest(string uri)
    {
        Debug.Log("posting request at:" + url);

        // Create the object to send in the body of the request
        variables = new Variables
        {
            InputA = "input for A",
            InputB = "input for B"
        };

        postData = new PostData
        {
            variables = variables
        };

        // Create the data to send in the body of the request
        string json = JsonUtility.ToJson(postData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        // Create the UnityWebRequest object
        using (UnityWebRequest webRequest = new UnityWebRequest(uri, "POST"))
        {
            // Set up the request
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("x-api-key", apiKey);

            // Send the request and wait for a response
            yield return webRequest.SendWebRequest();

            // Handle the result
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("\nReceived: " + webRequest.downloadHandler.text);
                    ConvertJsonOutputIntoObject(webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    public void ConvertJsonOutputIntoObject(string jsonString)
    {
        outputObjects = JsonUtility.FromJson<OutputObjectList>("{\"list\":" + jsonString + "}").list;
        foreach (var outputObject in outputObjects)
        {
            Debug.Log("Name: " + outputObject.name + ", Type: " + outputObject.type + ", Output: " + outputObject.output);
        }
    }
}