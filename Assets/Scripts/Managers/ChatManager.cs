using System.Collections.Generic;
using Models;
using UnityEngine;
using TMPro;

public class ChatManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private TextMeshProUGUI chatDisplay;
    private MessageService _messageService;

    void Start()
    {
        _messageService = DatabaseManager.Instance.MessageService;
        LoadMessages();
    }

    public void SendChatMessage()
    {
        string message = chatInput.text.Trim();
        if (string.IsNullOrEmpty(message)) return;

        _messageService.SendMessage(message);
        chatInput.text = "";
        Invoke(nameof(LoadMessages), 1f); // 1초 후 채팅 새로고침
    }

    void LoadMessages()
    {
        //_messageService.GetMessages(UpdateChatDisplay);
    }

    void UpdateChatDisplay(Dictionary<string, Message> messages)
    {
        chatDisplay.text = ""; // 기존 메시지 내용 초기화

        foreach (var msg in messages)
        {
            chatDisplay.text += $"{msg.Value.Timestamp}: {msg.Value.Text}\n"; // UI에 메시지 추가
        }
    }
}