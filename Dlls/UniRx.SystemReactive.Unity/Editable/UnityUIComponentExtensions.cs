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
    /// UniRx.UnityUIComponentExtensionsを、エラーのない範囲でしぼったもの。
    /// </summary>
    public static partial class UnityUIComponentExtensions
    {
        /// <summary>Observe onClick event.</summary>
        public static IObservable<Unit> OnClickAsObservable(this Button button)
        {
            return button.onClick.AsObservable();
        }

        /// <summary>Observe onEndEdit(Submit) event.</summary>
        public static IObservable<string> OnEndEditAsObservable(this InputField inputField)
        {
            return inputField.onEndEdit.AsObservable();
        }
    }
}

#endif