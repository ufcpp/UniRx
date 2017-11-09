// defined from .NET Framework 4.5 and NETFX_CORE

#if !NETFX_CORE

#if SystemReactive
namespace System
#else
using System;

namespace UniRx
#endif
{
    public interface IProgress<T>
    {
        void Report(T value);
    }

    public class Progress<T> : IProgress<T>
    {
        readonly Action<T> report;

        public Progress(Action<T> report)
        {
            this.report = report;
        }

        public void Report(T value)
        {
            report(value);
        }
    }
}

#endif