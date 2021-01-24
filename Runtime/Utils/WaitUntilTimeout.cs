using System;
using UnityEngine;

namespace Multiverse.Utils
{
    public class WaitUntilTimeout : CustomYieldInstruction
    {
        private readonly float _startTime;
        private readonly float _seconds;
        private readonly Func<bool> _predicate;

        public override bool keepWaiting
        {
            get
            {
                if (_predicate())
                    return false;

                if (Time.time - _startTime < _seconds)
                    return true;

                throw new Exception("WaitUntilTimeout timed out");
            }
        }

        public WaitUntilTimeout(Func<bool> predicate, float seconds = Multiverse.Timeout)
        {
            _startTime = Time.time;
            _seconds = seconds;
            _predicate = predicate;
        }
    }
}