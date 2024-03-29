using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatWebApplication.Share
{
    public class MessageBody
    {
        private static int messageBodyID = 0;
        public MessageBody() {
            messageBodyID++;
        }
        public int MessageId { get; } = messageBodyID;
        public string MessageText { get; set; } = string.Empty;
        public DateTime UTCDateTime { get; set; } 
        public string AdditionalText { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } = false;
        public MessageBodyType MessageType { get; set; } = MessageBodyType.Undefined;
        public enum MessageBodyType
        { 
            Undefined,
            ServerMessage,
            ClientMessage
        }  
    }
}
