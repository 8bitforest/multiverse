namespace Multiverse.LibraryInterfaces
{
    public interface IMvLibraryMatchmaker
    {
        Connected Connected { set; }
        Disconnected Disconnected { set; }
        Connected ConnectedToMatch { set; }
        
        ErrorHandler HostMatchError { set; }
        ErrorHandler JoinMatchError { set; }
        
        MatchesUpdated MatchesUpdated { set; }
        
        void Connect();
        void Disconnect();
        void HostMatch(byte[] data);
        void JoinMatch(int libId);
        void UpdateMatchList();
    }
}