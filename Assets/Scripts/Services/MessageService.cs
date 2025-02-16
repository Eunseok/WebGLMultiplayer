using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Models;
using UnityEngine;

namespace Services
{
    public class MessageService
    {
        readonly FirebaseService _firebaseService;
        string lastMessageKey = ""; // 마지막으로 받은 메시지 Key
        bool isCheckingForUpdates; // 중복 호출 방지

        public MessageService(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }
    
        public void SendMessage(string text)
        {
            var message = new Message
            {
                text = text,
                timestamp = DateTime.UtcNow.AddHours(9).ToString("yyyy-MM-dd HH:mm:ss")
            };
            
            _firebaseService.Post("messages", message);
        }

        public void GetMessages(Action<Dictionary<string, Message>> onMessagesReceived)
        {
            _firebaseService.Get<Dictionary<string, Message>>("messages",
                messages =>
                {
                    if (messages is { Count: > 0 })
                    {
                        Debug.Log($"메시지 불러오기 성공: 총 {messages.Count}개의 메시지");

                        //마지막 메세지 key저장
                        lastMessageKey = messages.Keys.Last();
                        
                        onMessagesReceived?.Invoke(messages); // 메시지 콜백 호출
                    }
                    else
                    {
                        Debug.LogWarning("메시지가 없습니다.");
                        onMessagesReceived?.Invoke(new Dictionary<string, Message>());
                    }
                },
                error => { Debug.LogError($"메시지 불러오기 실패: {error.Message}"); });
        }
        
        // Long-Polling 방식: 새로운 메시지가 있는지 감지하여 업데이트
        public void StartListeningForNewMessages(Action<Dictionary<string, Message>> onMessagesReceived)
        {
            if (isCheckingForUpdates) return; // 이미 실행 중이면 중복 실행 방지

            isCheckingForUpdates = true;
            Debug.Log("Firebase 메시지 업데이트 감지 시작...");

            CheckForNewMessages(onMessagesReceived);
        }
        
        private void CheckForNewMessages(Action<Dictionary<string, Message>> onMessagesReceived)
        {
            _firebaseService.Get<Dictionary<string, Message>>("messages",
                messages =>
                {
                    if (messages is { Count: > 0 })
                    {
                        string latestKey = "";
                        if (messages.Count > 0)
                        {
                            latestKey = messages.Keys.Last(); // 가장 마지막 메시지의 Key 직접 가져오기
                        }

                        // 새로운 메시지가 감지된 경우에만 UI 업데이트
                        if (latestKey != lastMessageKey)
                        {
                            lastMessageKey = latestKey;
                            onMessagesReceived?.Invoke(messages);
                        }
                    }

                    // 일정 시간 후 다시 실행 (Long-Polling)
                    DatabaseManager.Instance.StartCoroutine(DelayedCheck(onMessagesReceived));
                },
                error =>
                {
                    Debug.LogError($"메시지 업데이트 확인 실패: {error.Message}");
                    DatabaseManager.Instance.StartCoroutine(DelayedCheck(onMessagesReceived));
                });
        }

        private IEnumerator DelayedCheck(Action<Dictionary<string, Message>> onMessagesReceived)
        {
            yield return new WaitForSeconds(2f); // 2초마다 Firebase 확인
            CheckForNewMessages(onMessagesReceived);
        }
    }
}