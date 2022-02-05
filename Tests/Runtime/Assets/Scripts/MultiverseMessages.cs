namespace Multiverse.Tests.Assets.Scripts
{
    public enum ServerRequestType
    {
        RespondToSender,
        RespondToAll,
        RequestOtherClientToRequestServer
    }

    public enum ClientRequestType
    {
        RespondToServer,
        RequestServer
    }

    public struct ServerRequestMessage : IMvMessage
    {
        public ServerRequestType RequestType { get; set; }
        public int Id { get; set; }
    }

    public struct ServerResponseMessage : IMvMessage
    {
        public int Id { get; set; }
    }

    public struct ClientRequestMessage : IMvMessage
    {
        public ClientRequestType RequestType { get; set; }
        public int Id { get; set; }
    }

    public struct ClientResponseMessage : IMvMessage
    {
        public int Id { get; set; }
    }

    public struct StopServerMessage : IMvMessage { }
    public struct StopClientMessage : IMvMessage { }
}