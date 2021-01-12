using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchUp;
using Multiverse.Common;
using UnityEngine;

namespace Multiverse.MirrorNoble
{
    [RequireComponent(typeof(Matchmaker))]
    public class MirrorNobleMvLibraryMatchmaker : MonoBehaviour, IMvLibraryMatchmaker
    {
        private Matchmaker _matchmaker;

        public bool Connected => _matchmaker.IsReady;

        private void Awake()
        {
            _matchmaker = GetComponent<Matchmaker>();
        }

        public async Task Connect()
        {
            if (_matchmaker.IsReady)
                return;

            var connectTask = new TaskCompletionSource();
            StartCoroutine(ConnectCoroutine(connectTask));
            await connectTask.Task;
        }

        private IEnumerator ConnectCoroutine(TaskCompletionSource connectTask)
        {
            yield return _matchmaker.ConnectToMatchmaker();
            yield return new WaitUntilTimeout(() => _matchmaker.IsReady, 5);
            connectTask.SetResult();
        }

        public async Task Disconnect()
        {
            if (!_matchmaker.IsReady)
                return;

            var disconnectTask = new TaskCompletionSource();
            StartCoroutine(DisconnectCoroutine(disconnectTask));
            await disconnectTask.Task;
        }

        private IEnumerator DisconnectCoroutine(TaskCompletionSource disconnectTask)
        {
            _matchmaker.Disconnect();
            yield return new WaitUntilTimeout(() => !_matchmaker.IsReady, 5);
            disconnectTask.SetResult();
        }

        public async Task CreateMatch(string matchName, int maxPlayers)
        {
            var task = new TaskCompletionSource();
            _matchmaker.CreateMatch(maxPlayers, new Dictionary<string, MatchData>
            {
                {"Name", new MatchData(name)},
                {"MaxPlayers", new MatchData(maxPlayers)}
            }, (success, _) =>
            {
                if (success)
                    task.SetResult();
                else
                    task.SetException(new MvException());
            });
            await task.Task;
        }

        public async Task JoinMatch(IMvMatch match)
        {
            var task = new TaskCompletionSource();
            _matchmaker.JoinMatch(new Match(Convert.ToInt64(match.Id)), (success, _) =>
            {
                if (success)
                    task.SetResult();
                else
                    task.SetException(new MvException());
            });
            await task.Task;
        }

        public async Task<IEnumerable<IMvMatch>> GetMatchList()
        {
            var task = new TaskCompletionSource<IEnumerable<IMvMatch>>();
            _matchmaker.GetMatchList((success, matches) =>
            {
                if (success)
                    task.SetResult(matches.Select(m => new DefaultMvMatch
                    {
                        Id = m.id.ToString(),
                        Name = m.matchData["Name"].stringValue,
                        MaxPlayers = m.matchData["MaxPlayers"].intValue
                    }));
                else
                    task.SetException(new MvException());
            });
            return await task.Task;
        }
    }
}