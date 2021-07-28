using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DLS.Helper.Patterns
{
    public class MultiPipeLine<T> : PipeLine<T>
    {
        private IEnumerable<PipeLine<T>> _pipeLines;

        public MultiPipeLine(Action<T> job, int pipeCount, int maxCount = -1, bool islastAccess = false)
        {
            var list = new List<PipeLine<T>>();

            for (int i = 0; i < pipeCount; i++)
                list.Add(new SinglePipeLine<T>(job, maxCount, islastAccess));

            _pipeLines = list;
        }

        public override void Run(CancellationToken token)
        {
            foreach (var pipeLine in _pipeLines)
                pipeLine.Run(token);
        }

        public override void Enqueue(T data)
        {
            var ordered = _pipeLines.OrderBy(p => p.Count);
            ordered.Last().Enqueue(data);
        }
    }
}
