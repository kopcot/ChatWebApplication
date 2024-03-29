using ChatWebApplication.Share;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.JSInterop;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Web;

namespace ChatWebApplication.Client.Components
{
    public partial class ChatCompoment
    {
        [Inject]
        private ILogger<ChatCompoment> Logger { get; set; }
        [Parameter, EditorRequired]
        public bool IsAdmin { get; set; } = false;
        public bool ChangeableName { get; set; } = true;
        public string UserName { get; set; }
        private InputText ChatBoxTextElement { get; set; }
        private InputText UserNameElement { get; set; }
        private HubConnection Connection { get; set; }
        private string ConnectionUrl { get; set; }
        private string ChatBoxText { get; set; } = string.Empty;
        private string ReceivedMessage { get; set; } = string.Empty;
        private Timer? CheckConnection { get; set; }
        private HubConnectionState ConnectionStateBefore { get; set; }
        private string TextClass => ConnectionStateBefore != HubConnectionState.Connected ? "showntextdisconnected" : string.Empty;
        private string ConnectionClass => ConnectionStateBefore != HubConnectionState.Connected ? "notconnected" : "connected";
        public ElementReference Dummy { get; set; }
        protected override async Task OnInitializedAsync()
        {
            ConnectionUrl = Environment.GetEnvironmentVariable("ChatConnectionUrl");

            Connection = new HubConnectionBuilder()
                .WithAutomaticReconnect()
                .WithUrl(ConnectionUrl)
                .Build();
            await Connection.StartAsync();
            UserName = Connection.ConnectionId;
            await SendConnected();

            SubscribeReceiveEvents();

            CheckConnection = new Timer(async _ => await RecheckConnection(), null, 1000, 1000);

            Logger.LogInformation("Connection url = {0}; Connection ID = {1}", ConnectionUrl, Connection.ConnectionId);
            await base.OnInitializedAsync();
        }
        private async Task RecheckConnection()
        {
            if (Connection.State == ConnectionStateBefore)
                return;
            ConnectionStateBefore = Connection.State;
            await InvokeAsync(StateHasChanged);
        }
        ~ChatCompoment()
        {
            Logger.LogInformation("Disposing started for {0}", Connection.ConnectionId);
            CheckConnection?.Dispose();
            Connection?.StopAsync();
            Connection?.DisposeAsync();
            Logger.LogInformation("Disposing finished");
        }

        #region Send messages
        private async Task SendConnected()
        {
            try
            {
                MessageBody Message = new()
                {
                    UserId = Connection.ConnectionId ?? string.Empty,
                    UserName = UserName,
                    AdditionalText = string.Format("User connected \'{0}\'. Connection url: {1}", Connection.ConnectionId, ConnectionUrl),
                    IsAdmin = IsAdmin,
                    UTCDateTime = DateTime.UtcNow,
                };
                await Connection.InvokeAsync<MessageBody>(MessageEventName.SENDCONNECTED, Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
            }
        }
        private async Task HandleKeyUpUserName(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                if (!IsAdmin)
                    ChangeableName = false;
                try
                {
                    MessageBody Message = new()
                    {
                        UserId = Connection.ConnectionId ?? string.Empty,
                        UserName = UserName,
                        AdditionalText = string.Format("User changed name to \'{1}\' for \'{0}\'", Connection.ConnectionId, UserName),
                        IsAdmin = IsAdmin,
                        UTCDateTime = DateTime.UtcNow,
                    };
                    await Connection.InvokeAsync<MessageBody>(MessageEventName.SENDCHANGENAME, Message);
                    if (ChatBoxTextElement is not null &&
                        ChatBoxTextElement.Element is not null)
                        await ChatBoxTextElement.Element.Value.FocusAsync();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.Message, ex);
                }
            }
        }
        private async Task HandleKeyUpChatBox(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                try
                {
                    MessageBody Message = new()
                    {
                        UserId = Connection.ConnectionId ?? string.Empty,
                        UserName = UserName,
                        MessageText = ChatBoxText,
                        IsAdmin = IsAdmin,
                        UTCDateTime = DateTime.UtcNow,
                    };
                    await Connection.InvokeAsync<MessageBody>(MessageEventName.SENDMESSAGE, Message);
                    ChatBoxText = string.Empty;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.Message, ex);
                }
            }
        }
        #endregion
        #region Format receive messages
        private void SubscribeReceiveEvents()
        {
            Connection.On<MessageBody>(MessageEventName.RECEIVEMESSAGE, 
                async (receivedMessage) => await FormatReceiveMessage(receivedMessage));
            Connection.On<MessageBody>(MessageEventName.RECEIVECHANGENAME, 
                async (receivedMessage) => await FormatReceiveChangeUserName(receivedMessage));
            Connection.On<MessageBody>(MessageEventName.RECEIVECONNECTED, 
                async (receivedMessage) => await FormatReceiveConnectedUser(receivedMessage));
            Connection.On<MessageBody>(MessageEventName.RECEIVEDISCONNECTED,
                async (receivedMessage) => await FormatReceiveDisconnectedUser(receivedMessage));
        }
        private async Task FormatReceiveMessage(MessageBody receivedMessage)
        {
            string addedText = string.Empty;
            addedText += "<p> ";
            if (receivedMessage.UserId == Connection.ConnectionId)
                addedText += "<span class=\"showntextmyusername\"> ";
            else
                addedText += "<span class=\"showntextotherusername\"> ";
            addedText += receivedMessage.UserName;
            addedText += "&nbsp(";
            addedText += DateTime.Now;
            addedText += "):</span><br>";
            if (receivedMessage.UserId == Connection.ConnectionId)
                addedText += "<span class=\"showntextmytext\"> ";
            else
                addedText += "<span class=\"showntextothertext\"> ";
            addedText += HttpUtility.HtmlEncode(receivedMessage.MessageText);
            addedText += "</span></ p> ";

            ReceivedMessage = addedText + ReceivedMessage;
            await InvokeAsync(StateHasChanged);
        }
        private async Task FormatReceiveChangeUserName(MessageBody receivedMessage)
        {
            if (!IsAdmin)
                return;
            string addedText = string.Empty;
            addedText += "<p> ";
            if (receivedMessage.UserId == Connection.ConnectionId)
                addedText += "<span class=\"showntextmyusername\"> ";
            else
                addedText += "<span class=\"showntextotherusername\"> ";
            addedText += receivedMessage.UserName;
            addedText += "&nbsp(";
            addedText += DateTime.Now;
            addedText += "):</span><br><span class=\"showntextinformativetext\"> ";
            addedText += HttpUtility.HtmlEncode(receivedMessage.AdditionalText);
            addedText += "</span></ p> ";

            ReceivedMessage = addedText + ReceivedMessage;
            await InvokeAsync(StateHasChanged);
        }
        private async Task FormatReceiveConnectedUser(MessageBody receivedMessage)
        {
            if (!IsAdmin)
                return;
            string addedText = string.Empty;
            addedText += "<p> ";
            if (receivedMessage.UserId == Connection.ConnectionId)
                addedText += "<span class=\"showntextmyusername\"> ";
            else
                addedText += "<span class=\"showntextotherusername\"> ";
            addedText += receivedMessage.UserName;
            addedText += "&nbsp(";
            addedText += DateTime.Now;
            addedText += "):</span><br><span class=\"showntextinformativetext\"> ";
            addedText += HttpUtility.HtmlEncode(receivedMessage.AdditionalText);
            addedText += "</span></ p> ";

            ReceivedMessage = addedText + ReceivedMessage;
            await InvokeAsync(StateHasChanged);
        }
        private async Task FormatReceiveDisconnectedUser(MessageBody receivedMessage)
        {
            if (!IsAdmin)
                return;
            string addedText = string.Empty;
            addedText += "<p> ";
            if (receivedMessage.UserId == Connection.ConnectionId)
                addedText += "<span class=\"showntextmyusername\"> ";
            else
                addedText += "<span class=\"showntextotherusername\"> ";
            addedText += receivedMessage.UserName;
            addedText += "&nbsp(";
            addedText += DateTime.Now;
            addedText += "):</span><br><span class=\"showntextinformativetext\"> ";
            addedText += HttpUtility.HtmlEncode(receivedMessage.MessageText);
            addedText += "</span></ p> ";

            ReceivedMessage = addedText + ReceivedMessage;
            await InvokeAsync(StateHasChanged);
        }
        #endregion
    }
}