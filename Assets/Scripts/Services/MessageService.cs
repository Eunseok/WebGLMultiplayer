using System.Collections.Generic;
using UnityEngine;

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
            timestamp = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        };

        _firebaseService.Post("messages", message);
    }

    public void GetMessages()
    {
        _firebaseService.Get<List<Message>>("messages",
            messages => { Debug.Log($"📜 메시지 불러오기 성공: 총 {messages?.Count}개의 메시지 🔍"); },
            error => { Debug.LogError($"❌ 메시지 불러오기 실패: {error.Message}"); });
    }
}