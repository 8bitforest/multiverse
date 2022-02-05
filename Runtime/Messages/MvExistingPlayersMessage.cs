namespace Multiverse.Messages
{
    internal struct MvExistingPlayersMessage : IMvMessage
    {
        public MvPlayerConnectedMessage[] Players { get; set; }
    }
}