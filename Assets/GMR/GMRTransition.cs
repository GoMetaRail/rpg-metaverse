using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Networking;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using TMPro;

public class GMRTransition : MonoBehaviour
{
    public VideoPlayer GMRTunnelVideo;
    public TextMeshProUGUI StatusText; 

    private const string BASE_URL = "https://api.prototype.gometarail.io";
    private const string API_KEY = "nHaAqbNu4oG4Qwy6cgCgiWnErzp9P1sD";
    private const string ORIGIN = "b046d01a-c736-11ec-9d64-0242ac120002";

    // This will be pulled from the purchased NFT ticket
    private const string EXAMPLE_DESTINATION = "d63e629c-c736-11ec-9d64-0242ac120002";

    // Start is called before the first frame update
    void Start()
    {
        // Wait until the video has loaded to begin travel
        GMRTunnelVideo.prepareCompleted += VideoPrepared;

        StatusText.SetText("Get ready...");
    }

    void VideoPrepared(VideoPlayer vp)
    {
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        // Pause just for fun ;) 
        yield return new WaitForSeconds(2);

        StartCoroutine(SendTraveler());
    }
    
    IEnumerator SendTraveler()
    {
        // Post the traveler to receive their ID and destination information
        UnityWebRequest request = new UnityWebRequest(BASE_URL + "/traveler", UnityWebRequest.kHttpVerbPOST);
        request.SetRequestHeader("x-api-key", API_KEY);
        request.SetRequestHeader("x-origin", ORIGIN);

        var body = "{\"destination\": \"" + EXAMPLE_DESTINATION + "\"}";
        byte[] bytes = Encoding.UTF8.GetBytes(body);
        UploadHandlerRaw dataUpload = new UploadHandlerRaw(bytes);
        request.uploadHandler = dataUpload;
        request.SetRequestHeader("Content-Type", "application/json");
        DownloadHandler dataDownload = new DownloadHandlerBuffer();
        request.downloadHandler = dataDownload;
        
        yield return request.SendWebRequest();

        if (request.isNetworkError)
        {
            Debug.Log("Error While Sending: " + request.error);
        }
        else
        {
            var data = JsonUtility.FromJson<GMRTravelerResponse>(request.downloadHandler.text);
            var destinationUri = data.destination.uri + "?gmr_traveler_id=" + HttpUtility.UrlEncode(data.traveler);

            Debug.Log("Initiating travel");
            StatusText.SetText("Let's Go!");
            Application.ExternalEval("window.location.href = \"" + destinationUri + "\"");
        }
    }
}
