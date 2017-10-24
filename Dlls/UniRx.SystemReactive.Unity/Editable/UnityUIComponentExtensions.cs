// for uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using System;
using UnityEngine;
using UnityEngine.UI;

#if SystemReactive
using System.Reactive;
#endif

namespace UniRx
{
#if SystemReactive
    using Observable = System.Reactive.Linq.Observable;
#endif

    /// <summary>
    /// UniRx.UnityUIComponentExtensionsを、エラーのない範囲でしぼった・修正したもの。
    /// </summary>
    /// <remarks>
    /// NET35以外標準のSystem.Reactive.Linq.Observableを使用する。
    /// その場合、Observable.CreateWithState{T, TState}を利用できないため、
    /// 代替手段としてとしてObservable.Create{T}を利用する。
    /// </remarks>
    public static partial class UnityUIComponentExtensions
    {
        public static IObservable<Unit> OnClickAsObservable(this Button button)
        {
            return button.onClick.AsObservable();
        }

        public static IObservable<bool> OnValueChangedAsObservable(this Toggle toggle)
        {
#if NET35
            return Observable.CreateWithState<bool, Toggle>(toggle, (t, observer) =>
            {
                observer.OnNext(t.isOn);
                return t.onValueChanged.AsObservable().Subscribe(observer);
            });
#else
            return Observable.Create<bool>(observer =>
            {
                observer.OnNext(toggle.isOn);
                return toggle.onValueChanged.AsObservable().Subscribe(observer);
            });
#endif
        }

        public static IObservable<float> OnValueChangedAsObservable(this Scrollbar scrollbar)
        {
#if NET35
            return Observable.CreateWithState<float, Scrollbar>(scrollbar, (s, observer) =>
            {
                observer.OnNext(s.value);
                return s.onValueChanged.AsObservable().Subscribe(observer);
            });
#else
            return Observable.Create<float>(observer =>
            {
                observer.OnNext(scrollbar.value);
                return scrollbar.onValueChanged.AsObservable().Subscribe(observer);
            });
#endif
        }

        public static IObservable<Vector2> OnValueChangedAsObservable(this ScrollRect scrollRect)
        {
#if NET35
            return Observable.CreateWithState<Vector2, ScrollRect>(scrollRect, (s, observer) =>
            {
                observer.OnNext(s.normalizedPosition);
                return s.onValueChanged.AsObservable().Subscribe(observer);
            });
#else
            return Observable.Create<Vector2>(observer =>
            {
                observer.OnNext(scrollRect.normalizedPosition);
                return scrollRect.onValueChanged.AsObservable().Subscribe(observer);
            });
#endif
        }

        public static IObservable<float> OnValueChangedAsObservable(this Slider slider)
        {
#if NET35
            return Observable.CreateWithState<float, Slider>(slider, (s, observer) =>
            {
                observer.OnNext(s.value);
                return s.onValueChanged.AsObservable().Subscribe(observer);
            });
#else
            return Observable.Create<float>(observer =>
            {
                observer.OnNext(slider.value);
                return slider.onValueChanged.AsObservable().Subscribe(observer);
            });
#endif
        }

        /// <summary>Observe onEndEdit(Submit) event.</summary>
        public static IObservable<string> OnEndEditAsObservable(this InputField inputField)
        {
            return inputField.onEndEdit.AsObservable();
        }

        /// <summary>Observe onValueChanged with current `text` value on subscribe.</summary>
        public static IObservable<string> OnValueChangedAsObservable(this InputField inputField)
        {
#if NET35
            return Observable.CreateWithState<string, InputField>(inputField, (i, observer) =>
            {
                observer.OnNext(i.text);
                return i.onValueChanged.AsObservable().Subscribe(observer);
            });
#else
            return Observable.Create<string>(observer =>
            {
                observer.OnNext(inputField.text);
                return inputField.onValueChanged.AsObservable().Subscribe(observer);
            });
#endif
        }
    }
}

#endif