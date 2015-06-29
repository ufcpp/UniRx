using System;
using System.Collections;
using System.Collections.Generic;
using IteratorTasks;

namespace UniRx
{
    public static class ObservableEx
    {
        public static IDisposable Subscribe<T>(this IObservable<T> source, Action onNext) => source.Subscribe(arg => onNext());

        /// <summary>
        /// 非同期ハンドラーでイベントを処理する。
        /// ハンドラー完了前に次のイベントが来た時は、キューに溜めておいて、前の処理が完了し次第次の処理を始める(という意味で同期的(synchronously))。
        /// </summary>
        public static IDisposable SubscribeSynchronously<T>(this IObservable<T> source, AsyncAction<T> onNext)
        {
            var cts = new CancellationTokenSource();
            var queue = new Queue<T>();
            Task.Run(ct => ProcessSynchronouslyIterator(queue, onNext, ct), cts);

            return new CompositeDisposable
            {
                source.Subscribe(t => queue.Enqueue(t)),
                Disposable.Create(() => cts.Cancel())
            };
        }

        private static IEnumerator ProcessSynchronouslyIterator<T>(Queue<T> queue , AsyncAction<T> onNext, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                if (queue.Count == 0)
                {
                    yield return Task.Delay(TimeSpan.FromSeconds(1), ct);
                    continue;
                }

                var item = queue.Dequeue();
                var task = onNext(item, ct);
                yield return task;
                task.ThrowIfException();
            }
        }

        public static void OnNext<T>(this IObserver<T> source)
        {
            source.OnNext(default(T));
        }

        /// <summary>
        /// イベントの中継。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnNext<T>(this IObserver<EventPattern<T>> source, object sender, T e)
        {
            source.OnNext(new EventPattern<T>(sender, e));
        }

        /// <summary>
        /// 複数のイベントを1つに併合。
        /// 型違い版。
        /// </summary>
        public static IObservable<object> Merge<T1, T2>(IObservable<T1> source1, IObservable<T2> source2) => Observable.Merge(
                source1.Select(x => (object)x),
                source2.Select(x => (object)x));

        /// <summary>
        /// 複数のイベントを1つに併合。
        /// 型違い版。
        /// </summary>
        public static IObservable<object> Merge<T1, T2, T3>(IObservable<T1> source1, IObservable<T2> source2, IObservable<T3> source3) => Observable.Merge(
                source1.Select(x => (object)x),
                source2.Select(x => (object)x),
                source3.Select(x => (object)x));

        /// <summary>
        /// 複数のイベントを1つに併合。
        /// 型違い版。
        /// </summary>
        public static IObservable<object> Merge<T1, T2, T3, T4>(IObservable<T1> source1, IObservable<T2> source2, IObservable<T3> source3, IObservable<T4> source4) => Observable.Merge(
                source1.Select(x => (object)x),
                source2.Select(x => (object)x),
                source3.Select(x => (object)x),
                source4.Select(x => (object)x));

        /// <summary>
        /// 複数のイベントを1つに併合。
        /// 型違い版。
        /// </summary>
        public static IObservable<object> Merge<T1, T2, T3, T4, T5>(IObservable<T1> source1, IObservable<T2> source2, IObservable<T3> source3, IObservable<T4> source4, IObservable<T5> source5) => Observable.Merge(
                source1.Select(x => (object)x),
                source2.Select(x => (object)x),
                source3.Select(x => (object)x),
                source4.Select(x => (object)x),
                source5.Select(x => (object)x));

        /// <summary>
        /// 複数のイベントを1つに併合。
        /// 型違い版。
        /// </summary>
        public static IObservable<object> Merge<T1, T2, T3, T4, T5, T6>(IObservable<T1> source1, IObservable<T2> source2, IObservable<T3> source3, IObservable<T4> source4, IObservable<T5> source5, IObservable<T6> source6) => Observable.Merge(
                source1.Select(x => (object)x),
                source2.Select(x => (object)x),
                source3.Select(x => (object)x),
                source4.Select(x => (object)x),
                source5.Select(x => (object)x),
                source6.Select(x => (object)x));
    }
}
