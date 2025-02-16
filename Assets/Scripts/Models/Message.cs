using System;

namespace Models
{
    [Serializable]
    public class Message
    {
        //public string Key { get; set; }  // Firebase JSON에 key 필드가 있으므로 추가
        public string Text { get; set; }
        public string Timestamp { get; set; }
    }
}