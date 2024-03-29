using ChatWebApplication.Share;
using Microsoft.AspNetCore.SignalR;

namespace ChatWebApplication.Server
{
    internal class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;
        public ChatHub(ILogger<ChatHub> logger) 
        { 
            _logger = logger;
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            MessageBody message = new()
            {
                UserId = Context.ConnectionId,
                MessageText = $"User \'{Context.ConnectionId}\' left.",
                IsAdmin = true,
                UTCDateTime = DateTime.UtcNow,
                MessageType = MessageBody.MessageBodyType.ServerMessage
            };
            await Clients.All.SendAsync(MessageEventName.RECEIVEDISCONNECTED, Context.ConnectionId);
        }
        [HubMethodName(MessageEventName.SENDMESSAGE)]
        public async Task SendMessage(MessageBody message)
        {
            LogInformation(message);
            await Clients.All.SendAsync(MessageEventName.RECEIVEMESSAGE, message);
        }
        [HubMethodName(MessageEventName.SENDCHANGENAME)]
        public async Task ChangeName(MessageBody message)
        {
            LogInformation(message);
            await Clients.All.SendAsync(MessageEventName.RECEIVECHANGENAME, message);
        }
        [HubMethodName(MessageEventName.SENDCONNECTED)]
        public async Task ConnectedUser(MessageBody message)
        {
            LogInformation(message);
            await Clients.All.SendAsync(MessageEventName.RECEIVECONNECTED, message);
        }
        private void LogInformation(MessageBody message)
        {
            _logger.LogInformation("{0:dd/MM/yyyy hh:mm:ss.FFFF}, received message \'{1}\' with additional message \'{2}\' from \'{3}\'. " +
                "Message send at {4:dd/MM/yyyy hh:mm:ss.FFFF}, message id {5}.",
                DateTime.UtcNow, message.MessageText, message.AdditionalText, message.UserId, message.UTCDateTime, message.MessageId);
        }
    }
}