using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace BlueNoiseSampling
{
    /// <summary>
    /// A simple implementation of <see cref="IRng"/> that uses buffer to reduce ammount of calls to <see cref="RNGCryptoServiceProvider"/>
    /// </summary>
    public sealed class CachingCryptoStableRNG : IRng
    {
        private static readonly RNGCryptoServiceProvider CRYPTO_RNG_PROVIDER = new RNGCryptoServiceProvider();

        private const int ELEMENT_BYTE_SIZE = sizeof(uint)/sizeof(byte);

        private const int BUFFER_SIZE = 4096;

        private byte[] _buffer;

        private int _taken;

        public CachingCryptoStableRNG()
        {
            _buffer =  new byte[BUFFER_SIZE * ELEMENT_BYTE_SIZE];
            InitializeBuffer();
        }
        /// <inheritdoc/>
        public uint Next(uint maxvalue)
        {
            if (_taken >= _buffer.Length)  InitializeBuffer(); 
            var value = BitConverter.ToUInt32(_buffer, _taken);
            _taken += ELEMENT_BYTE_SIZE;
            return value;
        }

        /// <summary>
        /// A convinience method that refills the buffer and resets the index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InitializeBuffer()
        {
            CRYPTO_RNG_PROVIDER.GetBytes(_buffer, 0, _buffer.Length);
            _taken = 0;
        }
    }
}
