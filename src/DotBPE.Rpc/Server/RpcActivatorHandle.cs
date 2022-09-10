namespace DotBPE.Rpc.Server
{
    /// <summary>
    /// Handle to the activator instance.
    /// </summary>
    /// <typeparam name="T">The instance type.</typeparam>
    public readonly struct RpcActivatorHandle<T>
    {
        /// <summary>
        /// Gets the activated instance.
        /// </summary>
        public T Instance { get; }

        /// <summary>
        /// Gets a value indicating whether the instanced was created by the activator.
        /// </summary>
        public bool Created { get; }

        /// <summary>
        /// Gets state related to the instance.
        /// </summary>
        public object State { get; }

        /// <summary>
        /// Creates a new instance of RpcActivatorHandle
        /// </summary>
        /// <param name="instance">The activated instance.</param>
        /// <param name="created">A value indicating whether the instanced was created by the activator.</param>
        /// <param name="state">State related to the instance.</param>
        public RpcActivatorHandle (T instance, bool created, object state)
        {
            Instance = instance;
            Created = created;
            State = state;
        }
    }
}
