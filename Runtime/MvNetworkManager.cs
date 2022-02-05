using Multiverse.LibraryInterfaces;
using Multiverse.Utils;
using Reaction;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Multiverse
{
    [RequireComponent(typeof(IMvLibrary))]
    [DisallowMultipleComponent]
    [AddComponentMenu("Multiverse/MvNetworkManager")]
    public class MvNetworkManager : Singleton<MvNetworkManager>
    {
        public MvMatchmaker Matchmaker { get; private set; }
        public MvHost Host { get; private set; }
        public MvServer Server { get; private set; }
        public MvClient Client { get; private set; }

        public MvUniverse Universe { get; private set; }

        public RxnValue<bool> Connected { get; private set; }
        public RxnEvent OnConnected { get; private set; }
        public RxnEvent OnDisconnected { get; private set; }

        public bool IsConnected { get; private set; }
        public bool IsHost { get; private set; }
        public bool IsServer { get; private set; } // This is always false, but leave it here for future proofing
        public bool IsClient { get; private set; }
        public bool HasServer { get; private set; }
        public bool HasClient { get; private set; } // This is always true, but leave it here for future proofing

        private IMvLibrary _library;

        public void SetTimeout(float seconds) => _library.SetTimeout(seconds);

        private void Awake()
        {
            _library = GetComponent<IMvLibrary>();
            Matchmaker = new MvMatchmaker(_library.GetMatchmaker());

            Connected = new RxnValue<bool>();
            OnConnected = new RxnEvent();
            OnDisconnected = new RxnEvent();
            Connected.OnChangedTo(true, gameObject, () => OnConnected.AsOwner.Invoke());
            Connected.OnChangedTo(false, gameObject, () => OnDisconnected.AsOwner.Invoke());

            SceneManager.sceneLoaded += (s, m) => MvIdManager.LoadCurrentIds();
            MvIdManager.LoadMvPrefabs();
            MvIdManager.LoadCurrentIds();
        }

        internal void JoinedMatch(bool isHost, bool isClient)
        {
            Debug.Log($"Joined match! isHost: {isHost} isClient: {isClient}");
            IsConnected = true;
            IsServer = false;
            IsHost = isHost;
            IsClient = isClient;
            HasServer = IsHost;
            HasClient = true;

            if (IsHost)
            {
                Server = new MvServer(_library.GetServer());
                Client = new MvClient(_library.GetClient());
                Host = new MvHost(_library.GetHost());
                Host.Connected.OnChangedTo(false, gameObject, SetDisconnected);
                Host.Connected.RelayChangedTo(true, gameObject, SetConnected);
            }
            else if (IsClient)
            {
                Client = new MvClient(_library.GetClient());
                Client.Connected.OnChangedTo(false, gameObject, SetDisconnected);
                Client.Connected.RelayChangedTo(true, gameObject, SetConnected);
            }
        }

        private void SetConnected()
        {
            Universe = new MvUniverse();
            Connected.AsOwner.Set(true);
        }

        private void SetDisconnected()
        {
            Server?.ClearMessageReceivers();
            Client?.ClearMessageReceivers();

            IsConnected = false;
            IsHost = false;
            IsClient = false;
            HasClient = false;
            HasServer = false;
            Server = null;
            Client = null;
            Universe = null;
            Host = null;
            Matchmaker.Disconnect();
            _library.CleanupAfterDisconnect();
            Connected.AsOwner.Set(false);
        }

        private void OnDestroy()
        {
            SetDisconnected();
        }

        private void OnApplicationQuit()
        {
            SetDisconnected();
        }
    }
}