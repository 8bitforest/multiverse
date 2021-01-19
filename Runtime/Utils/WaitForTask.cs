using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Multiverse
{
    public class WaitForTask : CustomYieldInstruction
    {
        private readonly Task _task;

        public override bool keepWaiting
        {
            get
            {
                if (_task.IsFaulted)
                    throw _task.Exception ?? new Exception();
                return !_task.IsCompleted && !_task.IsCanceled;
            }
        }

        public WaitForTask(Task task)
        {
            _task = task;
        }

        public WaitForTask(Func<Task> task)
        {
            _task = task();
        }
    }
}