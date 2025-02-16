using System;
using System.Collections.Generic;
using Models;
using UnityEngine;

namespace Services
{
    public class MessageService
    {
        readonly FirebaseService _firebaseService;

        public MessageService(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }
    
        public void SendMessage(string text)
        {
            var message = new Message
            {
                text = text,
                timestamp = System.DateTime.UtcNow.AddHours(9).ToString("yyyy-MM-dd HH:mm:ss")
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
    }
}