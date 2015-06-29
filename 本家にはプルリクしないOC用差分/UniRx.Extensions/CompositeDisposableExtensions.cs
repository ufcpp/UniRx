using System;

namespace UniRx
{
    /// <summary>
    /// CompositeDisposable's Extension methods.
    /// </summary>
    public static class CompositeDisposableExtensions
    {
        /// <summary>
        /// <see cref="Disposable.Create"/> を使って <see cref="IDisposable.Dispose"/> 時に実行される <see cref="Action"/> を追加する。
        /// </summary>
        public static void Add(this CompositeDisposable disposable, Action disposeAction)
        {
            disposable.Add(Disposable.Create(disposeAction));
        }

        /// <summary>
        /// Adds a disposable to the CompositeDisposable or disposes the disposable if the CompositeDisposable is disposed.
        /// </summary>
        public static void Add(this CompositeDisposable self, IDisposable disposable1, IDisposable disposable2)
        {
            self.Add(disposable1);
            self.Add(disposable2);
        }
        /// <summary>
        /// Adds a disposable to the CompositeDisposable or disposes the disposable if the CompositeDisposable is disposed.
        /// </summary>
        public static void Add(this CompositeDisposable self, IDisposable disposable1, IDisposable disposable2, IDisposable disposable3)
        {
            self.Add(disposable1);
            self.Add(disposable2);
            self.Add(disposable3);
        }
        /// <summary>
        /// Adds a disposable to the CompositeDisposable or disposes the disposable if the CompositeDisposable is disposed.
        /// </summary>
        public static void Add(this CompositeDisposable self, IDisposable disposable1, IDisposable disposable2, IDisposable disposable3, IDisposable disposable4)
        {
            self.Add(disposable1);
            self.Add(disposable2);
            self.Add(disposable3);
            self.Add(disposable4);
        }
        /// <summary>
        /// Adds a disposable to the CompositeDisposable or disposes the disposable if the CompositeDisposable is disposed.
        /// </summary>
        public static void Add(this CompositeDisposable self, params IDisposable[] disposables)
        {
            foreach (var disposable in disposables)
                self.Add(disposable);
        }
    }
}
