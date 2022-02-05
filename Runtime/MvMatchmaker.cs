using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Multiverse.LibraryInterfaces;
using Multiverse.Serialization;
using Multiverse.Utils;
using Reaction;
using UnityEngine;

namespace Multiverse
{
    public class MvMatchmaker
    {
        public RxnValue<bool> Connected { get; }
        public RxnDictionary<uint, MvMatch> Matches { get; }

        private readonly MvNetworkManager _networkManager;
        private readonly IMvLibraryMatchmaker _matchmaker;

        private Task _lookForMatchesTask;
        private bool _lookingForMatches;

        private readonly Dictionary<int, MvMatch> _libIdMatches = new Dictionary<int, MvMatch>();
        private readonly MvBinaryReader _reader = new MvBinaryReader();
        private readonly MvBinaryWriter _writer = new MvBinaryWriter();

        private Type _matchDataType;

        public MvMatchmaker(IMvLibraryMatchmaker matchmaker)
        {
            Connected = new RxnValue<bool>();
            Matches = new RxnDictionary<uint, MvMatch>();

            _networkManager = MvNetworkManager.I;
            _matchmaker = matchmaker;

            _matchmaker.Connected = () => Connected.AsOwner.Set(true);
            _matchmaker.Disconnected = () => Connected.AsOwner.Set(false);
            _matchmaker.MatchesUpdated = MatchesUpdated;
        }

        private async Task Connect()
        {
            _matchmaker.Connect();
            await Connected.WaitUntil(true, Multiverse.Timeout);
            StartLookingForMatches();
        }

        public async Task Connect<T>() where T : IMvMatchData
        {
            Debug.Log("Connecting to matchmaker");
            _matchDataType = typeof(T);
            await Connect();
        }

        public async Task Disconnect()
        {
            Debug.Log("Disconnecting from matchmaker");
            StopLookingForMatches();
            _matchmaker.Disconnect();
            await Connected.WaitUntil(false, Multiverse.Timeout);
        }

        private async Task HostMatch(MvMatch match)
        {
            Debug.Log($"Hosting match {match}");
            var errorTsc = new TaskCompletionSource<string>();
            var connectedTsc = new TaskCompletionSource();
            _matchmaker.HostMatchError = msg => errorTsc.SetResult(msg);
            _matchmaker.ConnectedToMatch = () => connectedTsc.SetResult();
            StopLookingForMatches();

            _matchmaker.HostMatch(WriteMatch(match));

            if (await Task.WhenAny(errorTsc.Task, connectedTsc.Task) == errorTsc.Task)
                throw new MvException(errorTsc.Task.Result);

            _networkManager.JoinedMatch(true, false);
            await _networkManager.Connected.WaitUntil(true, Multiverse.Timeout);
        }

        public async Task HostMatch(string matchName, int maxPlayers)
        {
            await HostMatch(new MvMatch(MvIdManager.AllocateMatchId(), -1, new MultiverseMatchData
            {
                Name = matchName,
                MaxPlayers = maxPlayers
            }, null));
        }

        public async Task HostMatch<T>(string matchName, int maxPlayers, T customData) where T : IMvMatchData
        {
            await HostMatch(new MvMatch(MvIdManager.AllocateMatchId(), -1, new MultiverseMatchData
            {
                Name = matchName,
                MaxPlayers = maxPlayers
            }, customData));
        }

        public async Task JoinMatch(MvMatch match)
        {
            Debug.Log($"Joining match {match}");
            var errorTsc = new TaskCompletionSource<string>();
            var connectedTsc = new TaskCompletionSource();
            _matchmaker.JoinMatchError = msg => errorTsc.SetResult(msg);
            _matchmaker.ConnectedToMatch = () => connectedTsc.SetResult();
            StopLookingForMatches();
            _matchmaker.JoinMatch(match.LibId);

            if (await Task.WhenAny(errorTsc.Task, connectedTsc.Task) == errorTsc.Task)
                throw new MvException(errorTsc.Task.Result);

            _networkManager.JoinedMatch(false, true);
            await _networkManager.Connected.WaitUntil(true, Multiverse.Timeout);
        }

        private void MatchesUpdated(IEnumerable<(int libId, byte[] data)> matches)
        {
            var removedMatches = new HashSet<uint>(Matches.Keys);
            foreach (var (libId, data) in matches)
            {
                var match = ReadMatch(libId, data ?? new byte[] { });
                _libIdMatches[libId] = match;
                Matches.AsOwner[match.Id] = match;
                removedMatches.Remove(_libIdMatches[libId].Id);
            }

            foreach (var removedMatch in removedMatches)
            {
                _libIdMatches.Remove(Matches[removedMatch].Value.LibId);
                Matches.AsOwner.Remove(removedMatch);
            }
        }

        private void StartLookingForMatches()
        {
            _lookingForMatches = true;
            if (_lookForMatchesTask?.IsCompleted ?? true)
                _lookForMatchesTask = LookForMatches();
        }

        private void StopLookingForMatches()
        {
            _lookingForMatches = false;
            Matches.AsOwner.Clear();
        }

        private async Task LookForMatches()
        {
            while (_lookingForMatches && Connected)
            {
                Debug.Log("Looking for matches...");
                _matchmaker.UpdateMatchList();
                await Task.Delay(3000);
            }
        }

        private MvMatch ReadMatch(int libId, byte[] data)
        {
            _reader.Reset(new ArraySegment<byte>(data));
            var matchData = _reader.ReadSerializable<MultiverseMatchData>();
            IMvMatchData customData = null;
            if (_reader.ReadBool())
            {
                var readMethod = typeof(MvSerializableTypes).GetMethod(nameof(MvSerializableTypes.ReadSerializable))!
                    .MakeGenericMethod(_matchDataType);
                customData = (IMvMatchData) readMethod.Invoke(null, new object[] {_reader});
            }

            return new MvMatch(MvIdManager.AllocateMatchId(), libId, matchData, customData);
        }

        private byte[] WriteMatch(MvMatch match)
        {
            _writer.Reset();
            _writer.WriteSerializable(match.Data);
            _writer.WriteBool(match.CustomData != null);
            if (match.CustomData != null)
            {
                var writeMethod = typeof(MvSerializableTypes).GetMethod(nameof(MvSerializableTypes.WriteSerializable))!
                    .MakeGenericMethod(_matchDataType);
                writeMethod.Invoke(null, new object[] {_writer, match.CustomData});
            }

            var data = _writer.GetData();
            var bytes = new byte[data.Count];
            Array.Copy(data.Array!, data.Offset, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}