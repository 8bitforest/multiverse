using System.Threading.Tasks;

namespace Multiverse.Utils
{
    public class TaskCompletionSource : TaskCompletionSource<object>
    {
        public void SetResult()
        {
            SetResult(null);
        }

        public bool TrySetResult()
        {
            return TrySetResult(null);
        }
    }
}