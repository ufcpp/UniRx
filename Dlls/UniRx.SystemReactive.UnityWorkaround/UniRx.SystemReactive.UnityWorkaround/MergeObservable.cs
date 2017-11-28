using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Threading;

namespace UniRx.SystemReactive.UnityWorkaround
{
    /// <summary>
    /// <see cref="System.Reactive.Linq.Observable.Merge{TSource}(IObservable{IObservable{TSource}})"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class MergeObservable<T> : IObservable<T>
    {
        private readonly IObservable<IObservable<T>> _sources;
        private readonly IScheduler _scheduler;
        private readonly int _maxConcurrent;

        public MergeObservable(IObservable<IObservable<T>> sources, IScheduler scheduler)
        {
            _sources = sources;
            _scheduler = scheduler;
        }

        public MergeObservable(IObservable<IObservable<T>> sources, int maxConcurrent, IScheduler scheduler)
        {
            _sources = sources;
            _scheduler = scheduler;
            _maxConcurrent = maxConcurrent;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (_maxConcurrent > 0)
            {
                return new MergeConcurrentObserver(_sources, observer, _scheduler, _maxConcurrent);
            }
            else
            {
                return new MergeObserver(_sources, observer, _scheduler);
            }
        }

        /// <summary>
        /// Merge
        /// </summary>
        class MergeObserver : IObserver<IObservable<T>>, IDisposable
        {
            private readonly IObservable<IObservable<T>> _sources;
            private readonly IObserver<T> _observer;
            private readonly IScheduler _scheduler;
            private readonly CompositeDisposable _disposables = new CompositeDisposable();

            private int _activeCount = 0;
            private bool _completed = false;

            public MergeObserver(IObservable<IObservable<T>> sources, IObserver<T> observer, IScheduler scheduler)
            {
                _sources = sources;
                _observer = observer;
                _scheduler = scheduler;
                _disposables.Add(sources.Subscribe(this));
            }

            void IObserver<IObservable<T>>.OnNext(IObservable<T> value)
            {
                Interlocked.Increment(ref _activeCount);
                _disposables.Add(
                    value.Subscribe(
                        OnNext,
                        _observer.OnError,
                        OnCompleted
                    )
                );
            }

#if NET35
            private void OnNext(T value)
            {
                _scheduler.Schedule(() => OnNextSchedule(value));
            }

            private void OnNextSchedule(T value)
            {
                _observer.OnNext(value);
            }
#else
            private void OnNext(T value)
            {
                _scheduler.Schedule(value, OnNextSchedule);
            }

            private IDisposable OnNextSchedule(IScheduler s, T value)
            {
                _observer.OnNext(value);
                return Disposable.Empty;
            }
#endif

            private void OnCompleted()
            {
                var c = Interlocked.Decrement(ref _activeCount);
                if (_completed && c == 0)
                {
                    _observer.OnCompleted();
                }
            }

            void IObserver<IObservable<T>>.OnCompleted() => _completed = true;

            void IObserver<IObservable<T>>.OnError(Exception error) => _observer.OnError(error);

            public void Dispose() => _disposables.Dispose();
        }

        /// <summary>
        /// 同時実行数制限付きMerge
        /// </summary>
        class MergeConcurrentObserver : IObserver<IObservable<T>>, IDisposable
        {
            private readonly IObservable<IObservable<T>> _sources;
            private readonly int _maxConcurrent;
            private readonly IObserver<T> _observer;
            private readonly IScheduler _scheduler;
            private readonly CompositeDisposable _disposables = new CompositeDisposable();

            private readonly object _gate = new object();
            private readonly Queue<IObservable<T>> _queue = new Queue<IObservable<T>>();
            private int _activeCount = 0;
            private bool _completed = false;

            public MergeConcurrentObserver(IObservable<IObservable<T>> sources, IObserver<T> observer, IScheduler scheduler, int maxConcurrent)
            {
                _sources = sources;
                _observer = observer;
                _maxConcurrent = maxConcurrent;
                _scheduler = scheduler;
                _disposables.Add(sources.Subscribe(this));
            }

            void IObserver<IObservable<T>>.OnNext(IObservable<T> value)
            {
                lock (_gate)
                {
                    if (_activeCount < _maxConcurrent)
                    {
                        _activeCount++;
                        _disposables.Add(
                            value.Subscribe(
                                OnNext,
                                _observer.OnError,
                                OnCompleted
                            )
                        );
                    }
                    else
                    {
                        _queue.Enqueue(value);
                    }
                }
            }

#if NET35
            private void OnNext(T value)
            {
                _scheduler.Schedule(() => OnSchedule(value));
            }

            private void OnSchedule(T value)
            {
                _observer.OnNext(value);
            }
#else
            private void OnNext(T value)
            {
                _scheduler.Schedule(value, OnSchedule);
            }

            private IDisposable OnSchedule(IScheduler s, T value)
            {
                _observer.OnNext(value);
                return Disposable.Empty;
            }
#endif

            private void OnCompleted()
            {
                lock (_gate)
                {
                    _activeCount--;
                    if (_queue.Count > 0)
                    {
                        ((IObserver<IObservable<T>>)this).OnNext(_queue.Dequeue());
                    }
                    else if (_completed && _activeCount == 0)
                    {
                        _observer.OnCompleted();
                    }
                }
            }

            void IObserver<IObservable<T>>.OnCompleted() => _completed = true;

            void IObserver<IObservable<T>>.OnError(Exception error) => _observer.OnError(error);

            public void Dispose() => _disposables.Dispose();
        }
    }
}
