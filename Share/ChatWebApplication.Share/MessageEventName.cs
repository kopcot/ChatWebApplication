namespace ChatWebApplication.Share
{
    public static class MessageEventName
    {
        /// <summary>
        /// Event name when a message is received
        /// </summary>
        public const string RECEIVEMESSAGE = "ReceiveMessage";

        /// <summary>
        /// Name of the Hub method to send a message
        /// </summary>
        public const string SENDMESSAGE = "SendMessage";

        /// <summary>
        /// Name of the Hub method to change name of user
        /// </summary>
        public const string SENDCHANGENAME = "SendChangeName";

        /// <summary>
        /// Name of the Hub method to change name of user
        /// </summary>
        public const string RECEIVECHANGENAME = "ReceiveChangeName";

        /// <summary>
        /// Name of the Hub method to change name of user
        /// </summary>
        public const string SENDCONNECTED = "SendConnected";

        /// <summary>
        /// Name of the Hub method to change name of user
        /// </summary>
        public const string RECEIVECONNECTED = "ReceiveConnected";
        /// <summary>
        /// Name of the Hub method to change name of user
        /// </summary>
        public const string RECEIVEDISCONNECTED = "ReceiveDisconnected";
    }
}
