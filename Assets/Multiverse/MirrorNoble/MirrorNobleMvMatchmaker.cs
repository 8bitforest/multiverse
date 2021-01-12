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
    public class MirrorNobleMvMatchmaker : MonoBehaviour, IMvMatchmaker
    {
        private Matchmaker _matchmaker;

        private TaskCompletionSource _connectTask;

        public bool Connected => _matchmaker.IsReady;

        private void Awake()
        {
            _matchmaker = GetComponent<Matchmaker>();
        }

        public async Task Connect()
        {
            if (_matchmaker.IsReady)
                return;

            _connectTask = new TaskCompletionSource();
            StartCoroutine(ConnectCoroutine());
            await _connectTask.Task;
        }

        private IEnumerator ConnectCoroutine()
        {
            yield return _matchmaker.ConnectToMatchmaker();
            yield return new WaitUntilTimeout(() => _matchmaker.IsReady, 5);
            _connectTask?.SetResult();
        }

        public async Task CreateMatch(int maxClients)
        {
            var task = new TaskCompletionSource();
            _matchmaker.CreateMatch(maxClients, onCreateMatch: (success, _) =>
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
                    task.SetResult(matches.Select(m => new DefaultMvMatch(m.id.ToString())));
                else
                    task.SetException(new MvException());
            });
            return await task.Task;
        }
    }
}