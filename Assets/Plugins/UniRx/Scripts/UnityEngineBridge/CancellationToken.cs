using System;

#if !SystemReactive

#if SystemReactive
using System.Reactive.Disposables;
#endif

namespace UniRx
{
    public struct CancellationToken
    {
        readonly ICancelable source;

        public static readonly CancellationToken Empty = new CancellationToken(null);

        public CancellationToken(ICancelable source)
        {
            this.source = source;
        }

        public bool IsCancellationRequested
        {
            get
            {
                return (source == null) ? false : source.IsDisposed;
            }
        }

        public void ThrowIfCancellationRequested()
        {
            if (IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }
        }
    }
}

#endif