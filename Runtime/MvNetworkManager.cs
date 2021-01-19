using System;
using Reaction;
using UnityEngine;

namespace Multiverse
{
    [RequireComponent(typeof(IMvLibrary))]
    public class MvNetworkManager : Singleton<MvNetworkManager>
    {
        public MvMatchmaker Matchmaker { get; private set; }
        public MvHost Host { get; private set; }
        public MvServer Server { get; private set; }
        public MvClient Client { get; private set; }

        public RxnEvent OnConnected { get; private set; }
        public RxnEvent OnDisconnected { get; private set; }

        public bool IsConnected { get; private set; }
        public bool IsHost { get; private set; }
        public bool IsClient { get; private set; }

        private IMvLibrary _library;

        public void SetTimeout(float seconds) => _library.SetTimeout(seconds);

        private void Awake()
        {
            _library = GetComponent<IMvLibrary>();
            Matchmaker = new MvMatchmaker(_library.GetMatchmaker());
            Matchmaker.OnJoinedMatch.OnInvoked(gameObject, Connected);
            OnConnected = new RxnEvent();
            OnDisconnected = new RxnEvent();
        }
        
        private void Connected((bool isHost, bool isClient) args)
        {
            IsConnected = true;
            IsHost = args.isHost;
            IsClient = args.isClient;

            if (IsHost)
            {
                Server = new MvServer(_library.GetServer());
                Client = new MvClient(_library.GetClient());
                Host = new MvHost(Server, Client);
                Host.OnDisconnected.OnInvoked(gameObject, Disconnected);
            }
            else if (IsClient)
            {
                Client = new MvClient(_library.GetClient());
                Client.OnDisconnected.OnInvoked(gameObject, Disconnected);
            }

            OnConnected.AsOwner.Invoke();
        }

        private void Disconnected()
        {
            IsConnected = false;
            IsHost = false;
            IsClient = false;
            Server = null;
            Client = null;
            Host = null;
            Matchmaker.Disconnect();
            _library.CleanupAfterDisconnect();
            OnDisconnected.AsOwner.Invoke();
        }

        private void OnDestroy()
        {
            Disconnected();
        }

        private void OnApplicationQuit()
        {
            Disconnected();
        }
    }
}