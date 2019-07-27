using System;
using System.Collections;
using System.Threading;

namespace EspIot.Core.Collections
{
    public class ConcurrentQueue
    {
        public int Count => _messageQue.Count;

        private Queue _messageQue = new Queue();
        private ManualResetEvent _manualReset = new ManualResetEvent(false);

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
                try
                {
                    _manualReset.WaitOne();
                    return _messageQue.Dequeue();
                }
                catch (InvalidOperationException e)
                {
                    _manualReset.Reset();
                }
            }
        }
    }
}
