using System;
using Proyecto26;
using System.Threading.Tasks;
using UnityEngine;

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

    public void Get<T>(string path, System.Action<T> onSuccess, Action<Exception> onError)
    {
        RestClient.Get<T>($"{_baseUrl}/{path}.json")
            .Then(response =>
            {
                Debug.Log("✅ 데이터 불러오기 성공!");
                onSuccess?.Invoke(response);
            })
            .Catch(error =>
            {
                Debug.LogError($"❌ 데이터 불러오기 실패: {error.Message}");
                onError?.Invoke(error);
            });
    }
}