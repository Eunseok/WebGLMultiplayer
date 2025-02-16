using System;
using Proyecto26;
using UnityEngine;
using Newtonsoft.Json; // 추가


public class FirebaseService
{
    private readonly string _baseUrl;

    public FirebaseService(string baseUrl)
    {
        _baseUrl = baseUrl;
    }

    public void Post<T>(string path, T data)
    {
        RestClient.Post<T>($"{_baseUrl}/{path}.json", data)
            .Then(response => { Debug.Log("✅ 데이터 전송 성공!"); })
            .Catch(error => { Debug.LogError($"❌ 데이터 전송 실패: {error.Message}"); });
    }

    public void Put<T>(string path, T data)
    {
        RestClient.Put<T>($"{_baseUrl}/{path}.json", data)
            .Then(response => { Debug.Log("✅ 데이터 업데이트 성공!"); })
            .Catch(error => { Debug.LogError($"❌ 데이터 업데이트 실패: {error.Message}"); });
    }

    public void Get<T>(string path, Action<T> onSuccess, Action<Exception> onError)
    {
        RequestHelper requestOptions = new RequestHelper
        {
            Uri = $"{_baseUrl}/{path}.json",
            EnableDebug = true // 디버깅 활성화
        };

        RestClient.Get(requestOptions)
            .Then(response =>
            {
                string jsonData = response.Text ?? response.ToString(); // JSON 데이터 가져오기
                Debug.Log($"Firebase 원본 JSON 데이터: {jsonData}");

                try
                {
                    var data = JsonConvert.DeserializeObject<T>(jsonData);

                    if (data != null)
                    {
                        Debug.Log($"📜 JSON 변환 성공! 변환된 데이터 타입: {typeof(T)}");
                        onSuccess?.Invoke(data);
                    }
                    else
                    {
                        Debug.LogWarning("JSON 변환은 성공했지만, 데이터가 null입니다.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"JSON 변환 실패: {ex.Message}");
                    onError?.Invoke(ex);
                }
            })
            .Catch(error =>
            {
                Debug.LogError($"Firebase 데이터 가져오기 실패: {error.Message}");
                onError?.Invoke(error);
            });
    }
}