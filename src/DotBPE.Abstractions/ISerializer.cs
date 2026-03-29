// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using System;

namespace DotBPE.Abstractions
{
    /// <summary>
    /// Serializer interface for serializing and deserializing objects
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Deserializes data to the specified type
        /// </summary>
        /// <typeparam name="T">The target type</typeparam>
        /// <param name="data">The byte array to deserialize</param>
        /// <returns>The deserialized object</returns>
        T Deserialize<T>(byte[] data);

        /// <summary>
        /// Serializes an object to byte array
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="item">The object to serialize</param>
        /// <returns>The serialized byte array</returns>
        byte[] Serialize<T>(T item);

        /// <summary>
        /// Deserializes data to the specified type
        /// </summary>
        /// <param name="data">The byte array to deserialize</param>
        /// <param name="type">The target type</param>
        /// <returns>The deserialized object</returns>
        object Deserialize(byte[] data, Type type);

        /// <summary>
        /// Serializes an object to byte array
        /// </summary>
        /// <param name="item">The object to serialize</param>
        /// <returns>The serialized byte array</returns>
        byte[] Serialize(object item);

        /// <summary>
        /// Gets the codec type identifier
        /// </summary>
        byte CodecType { get; }
    }
}
