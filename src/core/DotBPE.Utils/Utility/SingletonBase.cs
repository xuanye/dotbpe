using System;
using System.Diagnostics;

namespace DotBPE.Utils.Utility {
    public abstract class SingletonBase<T> where T: class {
        protected SingletonBase() { }

        private static readonly Lazy<T> _instance = new Lazy<T>(() => {
            var instance = (T)Activator.CreateInstance(typeof(T), true);
            if (instance is IInitializable)
                ((IInitializable)instance).Initialize();

            return instance;
        });

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [DebuggerNonUserCode]
        public static T Current => _instance.Value;
    }

    public interface IInitializable {
        void Initialize();
    }
}
