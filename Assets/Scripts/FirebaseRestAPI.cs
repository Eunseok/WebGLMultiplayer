using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class FirebaseRestAPI : MonoBehaviour
{
    private string firebaseURL = "https://multiplayergame-eec4e-default-rtdb.firebaseio.com/messages.json";

    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private TextMeshProUGUI chatDisplay;

    public void SendMessageToFirebase()
    {
        string message = chatInput.text.Trim();
        if (string.IsNullOrEmpty(message)) return;

        StartCoroutine(PostMessage(message));
        chatInput.text = "";
    }

    IEnumerator PostMessage(string message)
    {
        string json = "{\"text\":\"" + message + "\"}";
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(firebaseURL, "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ 메시지 전송 성공!");
            StartCoroutine(GetMessages());
        }
        else
        {
            Debug.LogError("❌ 메시지 전송 실패: " + request.error);
        }
    }

    IEnumerator GetMessages()
    {
        UnityWebRequest request = UnityWebRequest.Get(firebaseURL);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text;
            Debug.Log("📜 불러온 데이터: " + response);
            chatDisplay.text = response;
        }
        else
        {
            Debug.LogError("❌ 데이터 불러오기 실패: " + request.error);
        }
    }
}