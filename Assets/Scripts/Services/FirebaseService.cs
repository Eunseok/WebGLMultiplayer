using System;
using Models;
using Newtonsoft.Json;
using Proyecto26;
using UnityEngine;

namespace Services
{
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
                .Then(response => { Debug.Log("데이터 전송 성공!"); })
                .Catch(error => { Debug.LogError($"데이터 전송 실패: {error.Message}"); });
        }

        public void Put<T>(string path, T data)
        {
            RestClient.Put<T>($"{_baseUrl}/{path}.json", data)
                .Then(response => { Debug.Log("데이터 업데이트 성공!"); })
                .Catch(error => { Debug.LogError($"데이터 업데이트 실패: {error.Message}"); });
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
                    var data = JsonConvert.DeserializeObject<T>(response.Text);

                    if (data != null)
                    {
                        Debug.Log($"Firebase 데이터 가져오기 성공!");
                        onSuccess?.Invoke(data);
                    }
                    else
                    {
                        Debug.LogWarning("데이터가 null입니다.");
                    }
                })
                .Catch(error =>
                {
                    Debug.LogError($"Firebase 데이터 가져오기 실패: {error.Message}");
                    onError?.Invoke(error);
                });
        }
    }
}