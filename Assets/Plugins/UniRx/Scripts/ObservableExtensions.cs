using System;
using System.Collections.Generic;

#if SystemReactive
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading;
using UniRx;

namespace System.Reactive.Linq
#else
using UniRx.Operators;
using System.Threading;

namespace UniRx
#endif
{
    public static partial class Observable
    {
        /// <summary>
        /// observe on synchronizationContext
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="synchronizationContext"></param>
        /// <returns></returns>
        public static IObservable<T> ObserveOn<T>(this IObservable<T> source, SynchronizationContext synchronizationContext)
        {
            return Create<T>(x =>
            {
                return source.Subscribe(arg =>
                {
                    if (synchronizationContext == null || synchronizationContext == SynchronizationContext.Current)
                    {
                        x.OnNext(arg);
                    }
                    else
                    {
                        synchronizationContext.Post(y => x.OnNext((T)y), arg);
                    }
                });
            });
        }
    }
}