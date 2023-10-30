using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ApiRequest : MonoBehaviour
{
    //private const string url = "https://promptchainer.io/flow/clobrynih038imc0o7sqru9e4";
    private const string url = "https://api.promptchainer.io/api/flows/run/clobrynih038imc0o7sqru9e4";
    private const string apiKey = "d0bca3b6-a646-424b-9989-3fc57b5af5b7";

    void Start()
    {
        StartCoroutine(PostRequest(url));
    }

    IEnumerator PostRequest(string uri)
    {
        // Create the data to send in the body of the request
        string postData = "{\"variables\": {\"Review\": \"The movie was a complete waste of time. The plot was predictable, and the acting was subpar.\"}}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(postData);

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
                    Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }
        }
    }
}