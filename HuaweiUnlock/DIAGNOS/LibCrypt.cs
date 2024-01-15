using Base62;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using static HuaweiUnlocker.LangProc;
namespace HuaweiUnlocker.DIAGNOS
{

    public static class LibCrypt
    {
        public static Base62Converter BS62 = new Base62Converter();
        public static AesManaged AESCrypter = new AesManaged();
        public static byte[] RSA_EncryptionOaepPkcs1(string pathcrt, byte[] data)
        {
            LOG(0, "RSA_EncryptionOaepPkcs1()");
            try
            {
                X509Certificate2 collection = new X509Certificate2();
                collection.Import(pathcrt);
                RSA csp = (RSA)collection.PublicKey.Key;
                return csp.Encrypt(data, RSAEncryptionPadding.Pkcs1);
            }
            catch (Exception ex)
            {
                LOG(2, ex.Message);
                return null;
            }
        }
        public static string RSA_DencryptionOaepPkcs1(string pathcrt, string Password, byte[] data)
        {
            LOG(0, "RSA_DencryptionOaepPkcs1()");
            try
            {
                var collection = new X509Certificate2();
                collection.Import(File.ReadAllBytes(pathcrt), Password, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
                if (collection.HasPrivateKey)
                {
                    LOG(0, "Decrypting");
                    RSA csp = (RSA)collection.PrivateKey;
                    var privateKey = collection.PrivateKey as RSACryptoServiceProvider;
                    var keys = Encoding.ASCII.GetString(csp.Decrypt(data, RSAEncryptionPadding.OaepSHA256));
                    return keys;
                }
                LOG(2, "No private");
            }
            catch (Exception ex) { LOG(2, ex.Message); }
            LOG(2, "WRONG CER: ");
            return null;
        }
        public static byte[] RSA_EncryptionOaepSha1(string publicKeyXml, byte[] plaintext)
        {
            RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider(2048);
            RSAalg.PersistKeyInCsp = false;
            RSAalg.FromXmlString(publicKeyXml);
            return RSAalg.Encrypt(plaintext, true);
        }

        public static byte[] RSA_DecryptionOaepSha1(string privateKeyXml, byte[] ciphertext)
        {
            RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider(2048);
            RSAalg.PersistKeyInCsp = false;
            RSAalg.FromXmlString(privateKeyXml);
            return RSAalg.Decrypt(ciphertext, true);
        }
        public static byte[] AesEncrypt(string data, string key)
        {
            byte[] initializationVector = Encoding.ASCII.GetBytes("abcdefghijklmnopqrstuvwxyz1234567890");
            AESCrypter.Key = Encoding.UTF8.GetBytes(key);
            AESCrypter.IV = initializationVector;
            var symmetricEncryptor = AESCrypter.CreateEncryptor(AESCrypter.Key, AESCrypter.IV);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricEncryptor, CryptoStreamMode.Write);
            StreamWriter streamWriter = new StreamWriter(cryptoStream);
            streamWriter.Write(data);
            return memoryStream.ToArray();
        }
        public static string AesDecrypt(string cipherText, string key)
        {
            byte[] initializationVector = Encoding.ASCII.GetBytes("abcede0123456789");
            byte[] buffer = Convert.FromBase64String(cipherText);
            AESCrypter.Key = Encoding.UTF8.GetBytes(key);
            AESCrypter.IV = initializationVector;
            var decryptor = AESCrypter.CreateDecryptor(AESCrypter.Key, AESCrypter.IV);
            MemoryStream memoryStream = new MemoryStream(buffer);
            CryptoStream cryptoStream = new CryptoStream(memoryStream as Stream, decryptor, CryptoStreamMode.Read);
            StreamReader streamReader = new StreamReader(cryptoStream as Stream);
            return streamReader.ReadToEnd();
        }
        public static byte[] Decrypt7Cisco(byte[] data)
        {
            try
            {
                LOG(0, "Triming and init....");
                string Encrypted = CRC.BytesToHexString(data);
                int[] Xlat = new int[] { 0x64, 0x73, 0x66, 0x64, 0x3b, 0x6b, 0x66, 0x6f, 0x41, 0x2c, 0x2e, 0x69, 0x79, 0x65, 0x77, 0x72, 0x6b, 0x6c, 0x64, 0x4a, 0x4b, 0x44, 0x48, 0x53, 0x55, 0x42, 0x73, 0x67, 0x76, 0x63, 0x61, 0x36, 0x39, 0x38, 0x33, 0x34, 0x6e, 0x63, 0x78, 0x76, 0x39, 0x38, 0x37, 0x33, 0x32, 0x35, 0x34, 0x6b, 0x3b, 0x66, 0x67, 0x38, 0x37 };
                LOG(0, "Converting XIdx");
                int XIdx = 0, Pos = 0;
                LOG(0, "Decrypting");
                List<byte> Decrypt = new List<byte>();
                while (Pos < Encrypted.Length)
                {
                    Decrypt.Add(Convert.ToByte(int.Parse(Encrypted.Substring(Pos, 2), NumberStyles.HexNumber) ^ Xlat[XIdx]));
                    Pos += 2;
                    XIdx = (XIdx + 1) < Xlat.Length ? XIdx + 1 : 0;
                }
                LOG(0, "HexTurn");
                return Decrypt.ToArray();
            }
            catch (Exception e)
            {
                LOG(2, e.Message);
            }
            return CRC.HexStringToBytes("00000000000000000000000000000000");
        }
        public static byte[] Encrypt7Cisco(string Decrypted)
        {
            try
            {
                Random rnd = new Random();
                LOG(0, "Triming and init....");
                Decrypted = Decrypted.Trim();
                int[] Xlat = new int[] { 0x64, 0x73, 0x66, 0x64, 0x3b, 0x6b, 0x66, 0x6f, 0x41, 0x2c, 0x2e, 0x69, 0x79, 0x65, 0x77, 0x72, 0x6b, 0x6c, 0x64, 0x4a, 0x4b, 0x44, 0x48, 0x53, 0x55, 0x42, 0x73, 0x67, 0x76, 0x63, 0x61, 0x36, 0x39, 0x38, 0x33, 0x34, 0x6e, 0x63, 0x78, 0x76, 0x39, 0x38, 0x37, 0x33, 0x32, 0x35, 0x34, 0x6b, 0x3b, 0x66, 0x67, 0x38, 0x37 };
                string Encrypt = string.Empty;
                LOG(0, "Converting XIdx");
                int XIdx = 0, Pos = 0;
                LOG(0, "Decrypting");
                while (Pos < Decrypted.Length)
                {
                    Encrypt += (char)int.Parse(Decrypted.Substring(Pos, 2), NumberStyles.HexNumber) ^ Xlat[rnd.Next(0, 15)];
                    Pos += 2;
                    XIdx = (XIdx + 1) < Xlat.Length ? XIdx + 1 : 0;
                }
                LOG(0, "HexTurn");
                return CRC.HexStringToBytes(Encrypt);
            }
            catch (Exception e)
            {
                LOG(2, e.Message);
            }
            return CRC.HexStringToBytes("00000000000000000000000000000000");
        }
        //Misc
        public static string MakeXMLRSA_PUBLIC(string rsapub, string exponent)
        {
            return "<RSAKeyValue><Modulus>" + rsapub + "</Modulus><Exponent>" + exponent + "</Exponent></RSAKeyValue>";
        }

        public static string MakeXMLRSA_PRIVATE(string rsapriv, string exponent)
        {
            return "<RSAKeyValue><Modulus>" + rsapriv + "</Modulus><Exponent>" + exponent + "</Exponent><P>/8atV5DmNxFrxF1PODDjdJPNb9pzNrDF03TiFBZWS4Q+2JazyLGjZzhg5Vv9RJ7VcIjPAbMy2Cy5BUffEFE+8ryKVWfdpPxpPYOwHCJSw4Bqqdj0Pmp/xw928ebrnUoCzdkUqYYpRWx0T7YVRoA9RiBfQiVHhuJBSDPYJPoP34k=</P><Q>8H9wLE5L8raUn4NYYRuUVMa+1k4Q1N3XBixm5cccc/Ja4LVvrnWqmFOmfFgpVd8BcTGaPSsqfA4j/oEQp7tmjZqggVFqiM2mJ2YEv18cY/5kiDUVYR7VWSkpqVOkgiX3lK3UkIngnVMGGFnoIBlfBFF9uo02rZpC5o5zebaDIms=</Q><DP>BPXecL9Pp6u/0kwY+DcCgkVHi67J4zqka4htxgP04nwLF/o8PF0tlRfj0S7qh4UpEIimsxq9lrGvWOne6psYxG5hpGxiQQvgIqBGLxV/U2lPKEIb4oYAOmUTYnefBCrmSQW3v93pOP50dwNKAFcGWTDRiB/e9j+3EmZm/7iVzDk=</DP><DQ>rBWkAC/uLDf01Ma5AJMpahfkCZhGdupdp68x2YzFkTmDSXLJ/P15GhIQ+Lxkp2swrvwdL1OpzKaZnsxfTIXNddmEq8PEBSuRjnNzRjQaLnqjGMtTBvF3G5tWkjClb/MW2q4fgWUG8cusetQqQn2k/YQKAOh2jXXqFOstOZQc9Q0=</DQ><InverseQ>BtiIiTnpBkd6hkqJnHLh6JxBLSxUopFvbhlR37Thw1JN94i65dmtgnjwluvR/OMgzcR8e8uCH2sBn5od78vzgiDXsqITF76rJgeO639ILTA4MO3Mz+O2umrJhrkmgSk8hpRKA+5Mf9aE7dwOzHrc8hbj8J102zyYJIE6pOehrGE=</InverseQ><D>hXGYfOMFzXX/vds8HYQZpISDlSF3NmbTCdyZkIsHjndcGoSOTyeEOxV93MggxIRUSjAeKNjPVzikyr2ixdHbp4fAKnjsAjvcfnOOjBp09WW4QCi3/GCfUh0w39uhRGZKPjiqIj8NzBitN06LaoYD6MPg/CtSXiezGIlFn/Hs+MuEzNFu8PFDj9DhOFhfCgQaIgEEr+IHdnl5HuUVrwTnIBrEzZA/08Q0Gv86qQZctZWoD9hPGzeAC+RSMyGVJw6Ls8zBFf0eysB4spsu4LUom/WnZMdS1ls4eqsAX+7AdqPKBRuUVpr8FNyRM3s8pJUiGns6KFsPThtJGuH6c6KVwQ==</D></RSAKeyValue>";
        }
        public static string ToBase64Encoding(byte[] input)
        {
            return Convert.ToBase64String(input);
        }
        public static string ToBase64Encoding(string input)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(input));
        }
        public static byte[] FromBase64Encoding(string input)
        {
            return Convert.FromBase64String(input);
        }
        public static byte[] ToBase62Encoding(string input)
        {
            return BS62.Encode(Encoding.ASCII.GetBytes(input));
        }
        public static byte[] FromBase62Encoding(byte[] input)
        {
            return BS62.Decode(input);
        }
    }
}
