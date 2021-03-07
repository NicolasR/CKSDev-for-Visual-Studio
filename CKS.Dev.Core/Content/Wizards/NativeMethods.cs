using System;
using System.Runtime.InteropServices;

namespace CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards
{
    /// <summary>
    /// Native Methods to assist with wizards
    /// </summary>
    static class NativeMethods
    {
        #region Methods

        /// <summary>
        /// Strongs the name key gen.
        /// </summary>
        /// <param name="wszKeyContainer">The WSZ key container.</param>
        /// <param name="dwFlags">The dw flags.</param>
        /// <param name="KeyBlob">The key BLOB.</param>
        /// <param name="KeyBlobSize">Size of the key BLOB.</param>
        /// <returns></returns>
        [DllImport("mscoree.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        internal static extern int StrongNameKeyGen(IntPtr wszKeyContainer, uint dwFlags, out IntPtr KeyBlob, out uint KeyBlobSize);

        /// <summary>
        /// Strongs the name token from public key.
        /// </summary>
        /// <param name="publicKeyBlob">The public key BLOB.</param>
        /// <param name="publicKeyBlobCount">The public key BLOB count.</param>
        /// <param name="strongNameTokenArray">The strong name token array.</param>
        /// <param name="strongNameTokenCount">The strong name token count.</param>
        [DllImport("mscoree.dll", ExactSpelling = true, PreserveSig = false)]
        internal static extern void StrongNameTokenFromPublicKey(byte[] publicKeyBlob, int publicKeyBlobCount, out IntPtr strongNameTokenArray, out int strongNameTokenCount);

        /// <summary>
        /// Strongs the name get public key.
        /// </summary>
        /// <param name="wszKeyContainer">The WSZ key container.</param>
        /// <param name="KeyBlob">The key BLOB.</param>
        /// <param name="KeyBlobSize">Size of the key BLOB.</param>
        /// <param name="PublicKeyBlob">The public key BLOB.</param>
        /// <param name="PublicKeyBlobSize">Size of the public key BLOB.</param>
        /// <returns></returns>
        [DllImport("mscoree.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int StrongNameGetPublicKey(string wszKeyContainer, [In] byte[] KeyBlob, [In] uint KeyBlobSize, out IntPtr PublicKeyBlob, out uint PublicKeyBlobSize);

        /// <summary>
        /// Strongs the name free buffer.
        /// </summary>
        /// <param name="pbMemory">The pb memory.</param>
        /// <returns></returns>
        [DllImport("mscoree.dll")]
        internal static extern int StrongNameFreeBuffer(IntPtr pbMemory);

        /// <summary>
        /// Strongs the name error info.
        /// </summary>
        /// <returns></returns>
        [DllImport("mscoree.dll", CharSet = CharSet.Unicode)]
        internal static extern int StrongNameErrorInfo();

        /// <summary>
        /// Hs the result from win32.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <returns></returns>
        internal static int HResultFromWin32(int errorCode)
        {
            if (errorCode > 0)
            {
                return (((errorCode & 0xffff) | 0x70000) | -2147483648);
            }
            return errorCode;
        }

        #endregion
    }
}
