using System.Collections;
using System.Threading;

namespace EspIot.Core.Collections
{
    public class ConcurrentQueue
    {
        private readonly ManualResetEvent _manualReset = new ManualResetEvent(false);

        private readonly Queue _messageQue = new Queue();
        public int Count => _messageQue.Count;

        public void Enqueue(object value)
        {
            lock (_messageQue.SyncRoot)
            {
                _messageQue.Enqueue(value);
                _manualReset.Set();
            }
        }

        public object Dequeue()
        {
            while (true)
            {
                _manualReset.WaitOne();
                lock (_messageQue.SyncRoot)
                {
                    if (Count > 0)
                    {
                        return _messageQue.Dequeue();
                    }
                }

                _manualReset.Reset();
            }
        }
    }
}