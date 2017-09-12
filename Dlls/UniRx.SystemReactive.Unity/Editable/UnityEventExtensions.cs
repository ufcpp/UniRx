// for uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Events;


namespace UniRx
{
#if SystemReactive
    using System.Reactive;
    using System.Reactive.Linq;
    using Observable = System.Reactive.Linq.Observable;
#endif

    /// <summary>
    /// UnityEventExtensionsを、エラーのない範囲で絞ったもの。
    /// </summary>
    public static partial class UnityEventExtensions
    {
        public static IObservable<Unit> AsObservable(this UnityEngine.Events.UnityEvent unityEvent)
        {
            var dummy = 0;
            return Observable.FromEvent<UnityAction, Unit>(h =>
            {

                dummy.GetHashCode();
                return new UnityAction(() => h(Unit.Default));
            }, h => unityEvent.AddListener(h), h => unityEvent.RemoveListener(h));
        }

        public static IObservable<T> AsObservable<T>(this UnityEngine.Events.UnityEvent<T> unityEvent)
        {
            return Observable.FromEvent<UnityAction<T>, T>(h => new UnityAction<T>(h), h => unityEvent.AddListener(h), h => unityEvent.RemoveListener(h));
        }
    }
}

#endif