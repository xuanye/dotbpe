// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

namespace DotBPE.Abstractions
{
    /// <summary>
    /// RPC result containing code information
    /// </summary>
    public class RpcResult
    {
        /// <summary>
        /// Gets or sets the result code (0 indicates success)
        /// </summary>
        public int Code { get; set; }
    }

    /// <summary>
    /// Generic RPC result containing code and data
    /// </summary>
    /// <typeparam name="T">The data type</typeparam>
    public class RpcResult<T> : RpcResult where T : class
    {
        /// <summary>
        /// Gets or sets the result data
        /// </summary>
        public T Data { get; set; }
    }
}
