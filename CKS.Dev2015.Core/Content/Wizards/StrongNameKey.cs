using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards
{
    /// <summary>
    /// The Strong Name Key object
    /// </summary>
    internal sealed class StrongNameKey
    {
        #region Fields

        /// <summary>
        /// The _key buffer
        /// </summary>
        byte[] _keyBuffer;

        /// <summary>
        /// The _key container
        /// </summary>
        string _keyContainer;

        #endregion

        #region Methods

        /// <summary>
        /// Prevents a default instance of the <see cref="StrongNameKey" /> class from being created.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        StrongNameKey(byte[] buffer)
        {
            this._keyBuffer = buffer;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="StrongNameKey" /> class from being created.
        /// </summary>
        /// <param name="keyContainer">The key container.</param>
        StrongNameKey(string keyContainer)
        {
            this._keyContainer = keyContainer;
        }

        /// <summary>
        /// Creates the new key pair.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">StrongNameKeyGenerationFailed</exception>
        internal static StrongNameKey CreateNewKeyPair()
        {
            byte[] buffer;
            IntPtr zero = IntPtr.Zero;
            try
            {
                uint num;
                if (NativeMethods.StrongNameKeyGen(IntPtr.Zero, 0, out zero, out num) == 0)
                {
                    Marshal.ThrowExceptionForHR(NativeMethods.StrongNameErrorInfo());
                }
                if (zero == IntPtr.Zero)
                {
                    throw new InvalidOperationException("StrongNameKeyGenerationFailed");
                }
                buffer = new byte[num];
                Marshal.Copy(zero, buffer, 0, (int)num);
            }
            finally
            {
                NativeMethods.StrongNameFreeBuffer(zero);
            }
            return new StrongNameKey(buffer);
        }

        /// <summary>
        /// Gets the public key.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">StrongNameKeyGettingPublicKeyFailed</exception>
        private byte[] GetPublicKey()
        {
            byte[] buffer;
            IntPtr zero = IntPtr.Zero;
            uint publicKeyBlobSize = 0;
            try
            {
                uint keyBlobSize = 0;
                if (this._keyBuffer != null)
                {
                    keyBlobSize = (uint)this._keyBuffer.Length;
                }
                if (NativeMethods.StrongNameGetPublicKey(this._keyContainer, this._keyBuffer, keyBlobSize, out zero, out publicKeyBlobSize) == 0)
                {
                    throw Marshal.GetExceptionForHR(NativeMethods.StrongNameErrorInfo());
                }
                if (publicKeyBlobSize == 0)
                {
                    throw new InvalidOperationException("StrongNameKeyGettingPublicKeyFailed");
                }
                buffer = new byte[publicKeyBlobSize];
                Marshal.Copy(zero, buffer, 0, (int)publicKeyBlobSize);
            }
            finally
            {
                NativeMethods.StrongNameFreeBuffer(zero);
            }
            return buffer;
        }

        /// <summary>
        /// Gets the public key token.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">StrongNameKeyExtractingPublicKeyFailed</exception>
        internal string GetPublicKeyToken()
        {
            IntPtr zero = IntPtr.Zero;
            int strongNameTokenCount = 0;
            StringBuilder builder = new StringBuilder();
            try
            {
                byte[] publicKey = this.GetPublicKey();
                NativeMethods.StrongNameTokenFromPublicKey(publicKey, publicKey.Length, out zero, out strongNameTokenCount);
                if (strongNameTokenCount == 0)
                {
                    throw new InvalidOperationException("StrongNameKeyExtractingPublicKeyFailed");
                }
                for (int i = 0; i < strongNameTokenCount; i++)
                {
                    builder.Append(Marshal.ReadByte(zero, i).ToString("x02", CultureInfo.InvariantCulture));
                }
            }
            finally
            {
                NativeMethods.StrongNameFreeBuffer(zero);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Loads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        internal static StrongNameKey Load(string path)
        {
            return new StrongNameKey(File.ReadAllBytes(path));
        }

        /// <summary>
        /// Loads the container.
        /// </summary>
        /// <param name="keyContainer">The key container.</param>
        /// <returns></returns>
        internal static StrongNameKey LoadContainer(string keyContainer)
        {
            return new StrongNameKey(keyContainer);
        }

        /// <summary>
        /// Saves to.
        /// </summary>
        /// <param name="destinationFileName">Name of the destination file.</param>
        internal void SaveTo(string destinationFileName)
        {
            File.WriteAllBytes(destinationFileName, this._keyBuffer);
        }

        #endregion
    }
}


