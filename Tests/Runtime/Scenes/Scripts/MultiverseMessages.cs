namespace Multiverse.Tests.Scenes.Scripts
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
    
    public struct ServerRequestMessage : IMvNetworkMessage
    {
        public ServerRequestType RequestType { get; set; }
        public int Id { get; set; }
    }

    public struct ServerResponseMessage : IMvNetworkMessage
    {
        public int Id { get; set; }
    }
    
    public struct ClientRequestMessage : IMvNetworkMessage
    {
        public ClientRequestType RequestType { get; set; }
        public int Id { get; set; }
    }
    
    public struct ClientResponseMessage : IMvNetworkMessage
    {
        public int Id { get; set; }
    }
}