using System;
using System.Collections;

#if SystemReactive
using System.Threading;
using System.Reactive.Disposables;
using System.Reactive.Linq;
#endif

namespace UniRx.Operators
{
    internal class FromCoroutineObservable<T> : OperatorObservableBase<T>
    {
        readonly Func<IObserver<T>, CancellationToken, IEnumerator> coroutine;

        public FromCoroutineObservable(Func<IObserver<T>, CancellationToken, IEnumerator> coroutine)
            : base(false)
        {
            this.coroutine = coroutine;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            var fromCoroutineObserver = new FromCoroutine(observer, cancel);

#if SystemReactive
            var cts = new CancellationTokenSource();
            var cancelable = Disposable.Create(() => cts.Cancel());
            MainThreadDispatcher.SendStartCoroutine(coroutine(fromCoroutineObserver, cts.Token));
            return cancelable;
#else
            var moreCancel = new BooleanDisposable();
            MainThreadDispatcher.SendStartCoroutine(coroutine(fromCoroutineObserver, new CancellationToken(moreCancel)));
            return moreCancel;
#endif

        }

        class FromCoroutine : OperatorObserverBase<T, T>
        {
            public FromCoroutine(IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
            }

            public override void OnNext(T value)
            {
                try
                {
                    base.observer.OnNext(value);
                }
                catch
                {
                    Dispose();
                    throw;
                }
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); }
                finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                try { observer.OnCompleted(); }
                finally { Dispose(); }
            }
        }
    }

    internal class FromMicroCoroutineObservable<T> : OperatorObservableBase<T>
    {
        readonly Func<IObserver<T>, CancellationToken, IEnumerator> coroutine;
        readonly FrameCountType frameCountType;

        public FromMicroCoroutineObservable(Func<IObserver<T>, CancellationToken, IEnumerator> coroutine, FrameCountType frameCountType)
            : base(false)
        {
            this.coroutine = coroutine;
            this.frameCountType = frameCountType;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            var microCoroutineObserver = new FromMicroCoroutine(observer, cancel);

#if SystemReactive
            var cts = new CancellationTokenSource();
            var moreCancel = Disposable.Create(() => cts.Cancel());

            switch (frameCountType)
            {
                case FrameCountType.Update:
                    MainThreadDispatcher.StartUpdateMicroCoroutine(coroutine(microCoroutineObserver, cts.Token));
                    break;
                case FrameCountType.FixedUpdate:
                    MainThreadDispatcher.StartFixedUpdateMicroCoroutine(coroutine(microCoroutineObserver, cts.Token));
                    break;
                case FrameCountType.EndOfFrame:
                    MainThreadDispatcher.StartEndOfFrameMicroCoroutine(coroutine(microCoroutineObserver, cts.Token));
                    break;
                default:
                    throw new ArgumentException("Invalid FrameCountType:" + frameCountType);
            }

            return moreCancel;
#else
            var moreCancel = new BooleanDisposable();

            switch (frameCountType)
            {
                case FrameCountType.Update:
                    MainThreadDispatcher.StartUpdateMicroCoroutine(coroutine(microCoroutineObserver, new CancellationToken(moreCancel)));
                    break;
                case FrameCountType.FixedUpdate:
                    MainThreadDispatcher.StartFixedUpdateMicroCoroutine(coroutine(microCoroutineObserver, new CancellationToken(moreCancel)));
                    break;
                case FrameCountType.EndOfFrame:
                    MainThreadDispatcher.StartEndOfFrameMicroCoroutine(coroutine(microCoroutineObserver, new CancellationToken(moreCancel)));
                    break;
                default:
                    throw new ArgumentException("Invalid FrameCountType:" + frameCountType);
            }

            return moreCancel;
#endif
        }

        class FromMicroCoroutine : OperatorObserverBase<T, T>
        {
            public FromMicroCoroutine(IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
            }

            public override void OnNext(T value)
            {
                try
                {
                    base.observer.OnNext(value);
                }
                catch
                {
                    Dispose();
                    throw;
                }
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); }
                finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                try { observer.OnCompleted(); }
                finally { Dispose(); }
            }
        }
    }
}