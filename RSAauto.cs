using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace e_media0_2
{
    class RSAauto
    {
        public byte[] EncryptMe(byte[] myFile)
        {
            try
            {
                //Create a UnicodeEncoder to convert between byte array and string.
                UnicodeEncoding ByteConverter = new UnicodeEncoding();

                //Create byte arrays to hold original, encrypted, and decrypted data.
                byte[] dataToEncrypt = myFile;
                byte[] encryptedData;
                byte[] decryptedData;

                //Create a new instance of RSACryptoServiceProvider to generate
                //public and private key data.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {

                    //Pass the data to ENCRYPT, the public key information 
                    //(using RSACryptoServiceProvider.ExportParameters(false),
                    //and a boolean flag specifying no OAEP padding.
                    encryptedData = RSAEncrypt(dataToEncrypt, RSA.ExportParameters(false), false);

                    //Pass the data to DECRYPT, the private key information 
                    //(using RSACryptoServiceProvider.ExportParameters(true),
                    //and a boolean flag specifying no OAEP padding.
                    decryptedData = RSADecrypt(encryptedData, RSA.ExportParameters(true), false);

                    //Display the decrypted plaintext to the console. 
                    //Console.WriteLine("Decrypted plaintext: {0}", ByteConverter.GetString(decryptedData));
                    return decryptedData;
                }
            }
            catch (ArgumentNullException) { return null; }
        }

        public static byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] encryptedData = new byte[DataToEncrypt.Length];
                byte[] fragment = new byte[100];
                byte[] encryptedChunk = new byte[100];
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {

                    //Import the RSA Key information. This only needs
                    //toinclude the public key information.
                    RSA.ImportParameters(RSAKeyInfo);

                    //Encrypt the passed byte array and specify OAEP padding.  
                    //OAEP padding is only available on Microsoft Windows XP or
                    //later. 
                    int times = DataToEncrypt.Length / 100;
                    for(int i = 0; i < times; i++)
                    {
                        Array.Copy(DataToEncrypt, i*100, fragment, 0, 100);
                        encryptedChunk = RSA.Encrypt(fragment, DoOAEPPadding);
                        Array.Copy(encryptedChunk, 0, encryptedData, i * 100, 100);
                    }
                }
                return encryptedData;
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                System.Windows.MessageBox.Show(e.Message.ToString(), "Error");
                return null;
            }
        }
        public static byte[] RSADecrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] decryptedData = new byte[DataToDecrypt.Length];
                byte[] fragment = new byte[100];
                byte[] decryptedChunk = new byte[100];
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    //Import the RSA Key information. This needs
                    //to include the private key information.
                    RSA.ImportParameters(RSAKeyInfo);

                    //Decrypt the passed byte array and specify OAEP padding.  
                    //OAEP padding is only available on Microsoft Windows XP or
                    //later.  
                    int times = DataToDecrypt.Length / 100;
                    for (int i = 0; i < times; i++)
                    {
                        Array.Copy(DataToDecrypt, i * 100, fragment, 0, 100);
                        decryptedChunk = RSA.Decrypt(fragment, DoOAEPPadding);
                        Array.Copy(decryptedChunk, 0, decryptedData, i * 100, 100);
                    }
                }
                return decryptedData;
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                System.Windows.MessageBox.Show(e.Message.ToString(), "Error");

                return null;
            }
        }
    }
}
