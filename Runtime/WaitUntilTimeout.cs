using System;
using UnityEngine;

namespace Multiverse
{
    public class WaitUntilTimeout : CustomYieldInstruction
    {
        private readonly float _startTime;
        private readonly float _seconds;
        private readonly Func<bool> _predicate;

        public override bool keepWaiting => !_predicate() && Time.time - _startTime < _seconds;

        public WaitUntilTimeout(Func<bool> predicate, float seconds)
        {
            _startTime = Time.time;
            _seconds = seconds;
            _predicate = predicate;
        }
    }
}