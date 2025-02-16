using System.Collections.Generic;
using Models;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Managers
{
    public class ChatManager : MonoBehaviour
    {
        [SerializeField] TMP_InputField chatInput;
        [SerializeField] TextMeshProUGUI chatDisplay;
        [SerializeField] ScrollRect scrollRect;
    
        private MessageService _messageService;
        
        void Start()
        {
            _messageService = DatabaseManager.Instance.MessageService;
            LoadMessages();
        
            // Long-Polling 방식으로 Firebase 실시간 채팅 감지 시작
            _messageService.StartListeningForNewMessages(UpdateChatDisplay);
        }

        private void Update()
        {
            HandleEnterKey();
        }

        private void HandleEnterKey()
        {
            if (!Input.GetKeyDown(KeyCode.Return)) return;

            if (EventSystem.current.currentSelectedGameObject != chatInput.gameObject)
            {
                ReFocusInputField();
            }
            else
            {
                SendChatMessage();
            }
        }
        private void ReFocusInputField()
        {
            chatInput.Select();
            chatInput.ActivateInputField();
        }
    
        public void SendChatMessage()
        {
            string message = chatInput.text.Trim();
            if (string.IsNullOrEmpty(message))
            {
                EventSystem.current.SetSelectedGameObject(null);
                return;
            }

            _messageService.SendMessage(message);
            chatInput.text = "";
            Invoke(nameof(LoadMessages), 0.1f); // 0.1초 후 채팅 새로고침
        
            chatInput.ActivateInputField(); // 계속 입력
        }

        void LoadMessages()
        {
            _messageService.GetMessages(UpdateChatDisplay);
        }

        void UpdateChatDisplay(Dictionary<string, Message> messages)
        {
            chatDisplay.text = ""; // 기존 메시지 내용 초기화

            foreach (var msg in messages)
            {
                // Timestamp에서 시간만 추출
                if (System.DateTime.TryParse(msg.Value.timestamp, out var dateTime))
                {
                    string timeOnly = dateTime.ToString("HH:mm"); // 시간만 변환
                    chatDisplay.text += $"{timeOnly}: {msg.Value.text}\n"; // 시간만 표시
                }
                else
                    chatDisplay.text += "??: ??:\n"; // 오류 시 기본 메시지 처리
            }
        
            // 스크롤을 최신 메시지로 이동
            Invoke(nameof(AutoScroll), 0.05f);
        }
    
        void AutoScroll()
        {
            scrollRect.verticalNormalizedPosition = 0f; // 아래로 스크롤 이동
        }
    }
}
