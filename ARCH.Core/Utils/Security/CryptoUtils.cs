﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ARCH.CoreLibrary.Utils.Security
{
    public class CryptoUtils
    {
        // Change these keys
        private readonly byte[] _key = { 123, 217, 19, 11, 24, 26, 85, 45, 114, 184, 27, 162, 37, 112, 222, 209, 241, 24, 175, 144, 173, 53, 196, 29, 24, 26, 17, 218, 131, 236, 53, 209 };
        private readonly byte[] _vector = { 146, 64, 191, 111, 23, 3, 113, 119, 231, 121, 221, 112, 79, 32, 114, 156 };
        private readonly ICryptoTransform _encryptorTransform;
        private readonly ICryptoTransform _decryptorTransform;
        private readonly UTF8Encoding _utfEncoder;

        public CryptoUtils()
        {
            //This is our encryption method
            var rm = new RijndaelManaged();

            //Create an encryptor and a decryptor using our encryption method, key, and vector.
            _encryptorTransform = rm.CreateEncryptor(this._key, this._vector);
            _decryptorTransform = rm.CreateDecryptor(this._key, this._vector);

            //Used to translate bytes to text and vice versa
            _utfEncoder = new UTF8Encoding();
        }

        /// <summary>
        /// -------------- Two Utility Methods (not used but may be useful) -----------
        /// Generates an encryption key.
        /// </summary>
        static public byte[] GenerateEncryptionKey()
        {
            //Generate a Key.
            RijndaelManaged rm = new RijndaelManaged();
            rm.GenerateKey();
            return rm.Key;
        }

        /// <summary>
        /// Generates a unique encryption vector
        /// </summary>
        static public byte[] GenerateEncryptionVector()
        {
            //Generate a Vector
            RijndaelManaged rm = new RijndaelManaged();
            rm.GenerateIV();
            return rm.IV;
        }

        /// <summary>
        /// ----------- The commonly used methods ------------------------------    
        /// Encrypt some text and return a string suitable for passing in a URL.
        /// </summary>
        public string EncryptToString(string textValue)
        {
            return ByteArrToString(Encrypt(textValue));
        }

        /// <summary>
        /// Encrypt some text and return an encrypted byte array.
        /// </summary>
        public byte[] Encrypt(string textValue)
        {
            //Translates our text value into a byte array.
            Byte[] bytes = _utfEncoder.GetBytes(textValue);

            //Used to stream the data in and out of the CryptoStream.
            MemoryStream memoryStream = new MemoryStream();

            /*
             * We will have to write the unencrypted bytes to the stream,
             * then read the encrypted result back from the stream.
             */
            #region Write the decrypted value to the encryption stream
            CryptoStream cs = new CryptoStream(memoryStream, _encryptorTransform, CryptoStreamMode.Write);
            cs.Write(bytes, 0, bytes.Length);
            cs.FlushFinalBlock();
            #endregion

            #region Read encrypted value back out of the stream
            memoryStream.Position = 0;
            byte[] encrypted = new byte[memoryStream.Length];
            memoryStream.Read(encrypted, 0, encrypted.Length);
            #endregion

            //Clean up.
            cs.Close();
            memoryStream.Close();

            return encrypted;
        }

        /// <summary>
        /// The other side: Decryption methods
        /// </summary>
        public string DecryptString(string encryptedString)
        {
            try
            {
                return Decrypt(StrToByteArray(encryptedString));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Decryption when working with byte arrays.
        /// </summary>
        public string Decrypt(byte[] encryptedValue)
        {
            #region Write the encrypted value to the decryption stream
            MemoryStream encryptedStream = new MemoryStream();
            CryptoStream decryptStream = new CryptoStream(encryptedStream, _decryptorTransform, CryptoStreamMode.Write);
            decryptStream.Write(encryptedValue, 0, encryptedValue.Length);
            decryptStream.FlushFinalBlock();
            #endregion

            #region Read the decrypted value from the stream.
            encryptedStream.Position = 0;
            Byte[] decryptedBytes = new Byte[encryptedStream.Length];
            encryptedStream.Read(decryptedBytes, 0, decryptedBytes.Length);
            encryptedStream.Close();
            #endregion
            return _utfEncoder.GetString(decryptedBytes);
        }

        /// <summary>
        /// Convert a string to a byte array.  NOTE: Normally we'd create a Byte Array from a string using an ASCII encoding (like so).
        /// </summary>
        //      System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
        //      return encoding.GetBytes(str);
        // However, this results in character values that cannot be passed in a URL.  So, instead, I just
        // lay out all of the byte values in a long string of numbers (three per - must pad numbers less than 100).
        public byte[] StrToByteArray(string str)
        {
            if (str.Length == 0)
                throw new Exception("Invalid string value in StrToByteArray");

            byte[] byteArr = new byte[str.Length / 3];
            int i = 0;
            int j = 0;
            do
            {
                var val = byte.Parse(str.Substring(i, 3));
                byteArr[j++] = val;
                i += 3;
            }
            while (i < str.Length);
            return byteArr;
        }

        // Same comment as above.  Normally the conversion would use an ASCII encoding in the other direction:
        //      System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
        //      return enc.GetString(byteArr);    
        public string ByteArrToString(byte[] byteArr)
        {
            string tempStr = "";
            for (int i = 0; i <= byteArr.GetUpperBound(0); i++)
            {
                var val = byteArr[i];
                if (val < 10)
                    tempStr += "00" + val;
                else if (val < 100)
                    tempStr += "0" + val;
                else
                    tempStr += val.ToString();
            }
            return tempStr;
        }
    }
}
