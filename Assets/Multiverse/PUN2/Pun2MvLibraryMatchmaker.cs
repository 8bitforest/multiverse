using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Multiverse.Common;
using Photon.Pun;
using Photon.Realtime;

namespace Multiverse.PUN2
{
    public class Pun2MvLibraryMatchmaker : MonoBehaviourPunCallbacks, IMvLibraryMatchmaker
    {
        private TaskCompletionSource _connectTask;
        private TaskCompletionSource _disconnectTask;
        private TaskCompletionSource _createMatchTask;
        private TaskCompletionSource _joinMatchTask;

        private IEnumerable<IMvMatch> _matchListCache = new List<IMvMatch>();

        public bool Connected => PhotonNetwork.IsConnected;

        public async Task Connect()
        {
            if (PhotonNetwork.IsConnected)
                return;

            _connectTask = new TaskCompletionSource();
            PhotonNetwork.ConnectUsingSettings();
            await _connectTask.Task;
        }

        public override void OnConnectedToMaster()
        {
            _connectTask?.SetResult();
            _connectTask = null;
        }

        public async Task Disconnect()
        {
            if (!PhotonNetwork.IsConnected)
                return;

            _disconnectTask = new TaskCompletionSource();
            PhotonNetwork.Disconnect();
            await _disconnectTask.Task;
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            _disconnectTask?.SetResult();
            _disconnectTask = null;
        }

        public async Task CreateMatch(string matchName, int maxPlayers)
        {
            _createMatchTask = new TaskCompletionSource();
            PhotonNetwork.CreateRoom(matchName, new RoomOptions
            {
                MaxPlayers = (byte) maxPlayers
            });
            await _createMatchTask.Task;
        }

        public override void OnCreatedRoom()
        {
            _createMatchTask?.SetResult();
            _createMatchTask = null;
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            _createMatchTask?.SetException(new MvException(message));
            _createMatchTask = null;
        }

        public async Task JoinMatch(IMvMatch match)
        {
            _joinMatchTask = new TaskCompletionSource();
            PhotonNetwork.JoinRoom(match.Id);
            await _joinMatchTask.Task;
        }

        public override void OnJoinedRoom()
        {
            _joinMatchTask?.SetResult();
            _joinMatchTask = null;
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            _joinMatchTask?.SetException(new MvException(message));
            _joinMatchTask = null;
        }

        public async Task<IEnumerable<IMvMatch>> GetMatchList()
        {
            return _matchListCache;
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            _matchListCache = roomList.Select(r => new DefaultMvMatch
            {
                Id = r.Name,
                Name = r.Name,
                MaxPlayers = r.MaxPlayers
            });
        }
    }
}