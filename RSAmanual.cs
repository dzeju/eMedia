using System;
using System.Collections;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading;

namespace e_media0_2
{
    class RSAmanual
    {
        BigInteger q = 0;

        public void GenerateKeys()
        {
            Thread t = new Thread(new ThreadStart(ThreadProc));
            t.Start();
            Thread.Sleep(0);
            BigInteger p = GetProbablyPrimeNumber();
            t.Join();

            BigInteger n = BigInteger.Multiply(p, q);
            BigInteger phi = BigInteger.Multiply(p - 1, q - 1);

            int e = ChooseE(phi, n);
            BigInteger d = GetD(phi, e);
            string[] lines = { "n: " + n.ToString(), "d: " + d.ToString(), "e: " + e.ToString() };
            System.IO.File.WriteAllLines(@"C:\Users\Kuba\source\repos\emedia0_1\key.txt", lines);
        }

        public byte[] EncryptMe(byte[] myFile, int begin, string pubKey, string privKey)
        {
            BigInteger n = BigInteger.Parse(pubKey);
            int e = int.Parse(privKey);
            try 
            {
                byte[] encryptedFile = FileEncryption(myFile, e, n, begin);

                return encryptedFile;
            }
            catch (Exception en)
            {
                System.Windows.MessageBox.Show(en.Message);
            }


            return null;
        }

        public byte[] DecryptMe(byte[] myFile, int begin, string pubKey, string privKey)
        {
            BigInteger publicKey = BigInteger.Parse(pubKey);
            BigInteger privateKey = BigInteger.Parse(privKey);
            //BigInteger.Parse("94318557139602195448158125342475306092189185571396021954481581253424753060921829235002025535738496980427195448158125342475306092036401701382828528111237014985247894763679799234720673890150365998238256646536185571396021954481581253424753060921885685407491343323498596509119829317223718801371295913828285281112370149852478947636797992347206738");
            BigInteger inv2 = 0;
            byte[] decryptedData = new byte[begin];
            Array.Copy(myFile, 0, decryptedData, 0, begin);
            int len = 384;
            byte[] fragment = new byte[len];
            
            BitArray a = new BitArray(new byte[len]);
            BigInteger inv = new BigInteger(new byte[len]);
            BitArray b = new BitArray(new byte[len]);
            BitArray c = new BitArray(new byte[len]);

            if (((myFile.Length - begin) % len) != 0)
                myFile = AddDataToArray(myFile, new byte[len - ((myFile.Length - begin) % len)]);

            int times = (myFile.Length - begin) / len;
            for (int i = 0; i < times; i++)
            {
                Array.Copy(myFile, begin + (i * len), fragment, 0, fragment.Length);
                //c = new BitArray(fragment);
                BigInteger tmp = new BigInteger(fragment);
                inv2 = tmp;
                /*if (tmp > publicKey)
                    throw new Exception("message bigger than n, big no no");*/
                tmp = BigInteger.ModPow(tmp, privateKey, publicKey);
                tmp = tmp ^ inv;
                inv = inv2;
                byte[] decryptedChunk = tmp.ToByteArray();
                
                int desLength = 384;
                /*if (decryptedChunk.Length > desLength)
                    throw new Exception((string)("Decrypted Chunk longer than " + desLength.ToString()));*/

                if (decryptedChunk.Length != desLength && decryptedChunk.Length < desLength)
                    decryptedChunk = AddDataToArray(decryptedChunk, new byte[desLength - decryptedChunk.Length]);//tu

                /*a = new BitArray(decryptedChunk);
                
                a.Xor(b);
                b = c;
                decryptedChunk = ConvertToByte(a);*/


                
                decryptedData = AddDataToArray(decryptedData, decryptedChunk);
            }
            System.Windows.MessageBox.Show("File size is (if successful): " + myFile.Length.ToString() +
                "and actually is: " + decryptedData.Length.ToString());
            if (myFile.Length > decryptedData.Length)
                decryptedData = AddDataToArray(decryptedData, new byte[myFile.Length - decryptedData.Length]);

            return decryptedData;
        }

        private byte[] FileEncryption(byte[] myFile, int e, BigInteger n, int begin)
        {
            byte[] encryptedData = new byte[begin];
            Array.Copy(myFile, 0, encryptedData, 0, begin);
            //BigInteger inv = BigInteger.Parse("94318557139602195448158125342475306092189185571396021954481581253424753060921829235002025535738496980427195448158125342475306092036401701382828528111237014985247894763679799234720673890150365998238256646536185571396021954481581253424753060921885685407491343323498596509119829317223718801371295913828285281112370149852478947636797992347206738");
            int len = 384;
            byte[] fragment = new byte[len];
            BitArray a = new BitArray(new byte[len]);
            BigInteger inv = new BigInteger(new byte[len]);
            BitArray b = new BitArray(new byte[len]);

            if (((myFile.Length - begin) % len) != 0)
                myFile = AddDataToArray(myFile, new byte[len - ((myFile.Length - begin) % len)]);

            int times = (myFile.Length - begin) / len;
            for (int i = 0; i < times; i++)
            {
                Array.Copy(myFile, begin + (i * len), fragment, 0, fragment.Length);
                /*a = new BitArray(fragment);
                a.Xor(b);
                fragment = ConvertToByte(a);*/
                BigInteger tmp = new BigInteger(fragment);
                tmp = inv ^ tmp;
                if (tmp > n)
                    throw new Exception("message bigger than n, big no no");
                tmp = BigInteger.ModPow(tmp, e, n);
                inv = tmp;

                byte[] encryptedChunk = tmp.ToByteArray();
                
                if (encryptedChunk.Length != len && encryptedChunk.Length < len)
                    encryptedChunk = AddDataToArray(new byte[len - encryptedChunk.Length], encryptedChunk);
                //b = new BitArray(encryptedChunk);
                /*if (encryptedChunk.Length != 384)
                    throw new Exception("will fail at: " + i.ToString() + " out of: " + times.ToString() +
                        "\nsize: " + encryptedChunk.Length.ToString() +
                        "\ngiven fragment: " + BitConverter.ToString(fragment));*/
                encryptedData = AddDataToArray(encryptedData, encryptedChunk);
            }
            System.Windows.MessageBox.Show("File size should be: " + myFile.Length.ToString() +
                "and actually is: " + encryptedData.Length.ToString());
            if (myFile.Length > encryptedData.Length)
                encryptedData = AddDataToArray(encryptedData, new byte[myFile.Length - encryptedData.Length]);
            return encryptedData;
        }

        private static byte[] ConvertToByte(BitArray bits)
        {
            if (bits.Count != 3072)
            {
                throw new ArgumentException("bits");
            }
            byte[] bytes = new byte[384];
            bits.CopyTo(bytes, 0);
            return bytes;
        }

        private static byte[] AddDataToArray(byte[] array, byte[] addedArray)
        {
            byte[] sum = new byte[array.Length + addedArray.Length];
            Array.Copy(array, 0, sum, 0, array.Length);
            Array.Copy(addedArray, 0, sum, array.Length, addedArray.Length);

            return sum;
        }

        private BigInteger GetD(BigInteger phi, int e)//euclid
        {
            BigInteger d = 1;
            BigInteger phi1 = phi;
            BigInteger phi2 = phi;
            BigInteger e_tmp = e;

            while (true)
            {
                BigInteger tmp = BigInteger.Divide(phi1, e_tmp);

                BigInteger e2_tmp = BigInteger.Multiply(tmp, e_tmp);
                BigInteger d_tmp = BigInteger.Multiply(tmp, d);

                e2_tmp = phi1 - e2_tmp;
                d_tmp = phi2 - d_tmp;

                phi1 = e_tmp;
                phi2 = d;

                e_tmp = e2_tmp;
                if (d_tmp < 0)
                    d = d_tmp + phi;
                else
                    d = d_tmp;
                //System.Windows.MessageBox.Show(phi1.ToString() + "  " + phi2.ToString() + "\n" + e_tmp.ToString() + "  " + d.ToString());
                if (e_tmp == 1)
                    return d;
            }
            

            throw new NotImplementedException();
        }

        private int ChooseE(BigInteger phi, BigInteger n)
        {
            for(int e = 65537; e < phi; e++)
            {
                if(e % 2 != 0)
                {
                    if (phi % e != 0)
                        if (n % e != 0)
                            return e;
                }
            }
            throw new Exception();
        }

        private void ThreadProc()
        {
            q = GetProbablyPrimeNumber();
            //System.Windows.MessageBox.Show("Call to join from thread");
        }

        public BigInteger GetProbablyPrimeNumber()
        {
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();

            byte[] randomNumber = new byte[192];
            rngCsp.GetNonZeroBytes(randomNumber);
            BigInteger n = BigInteger.Abs(new BigInteger(randomNumber));
            while (!IsItProbablyPrime(n))
            {
                rngCsp.GetNonZeroBytes(randomNumber);
                n = BigInteger.Abs(new BigInteger(randomNumber));
            }
            rngCsp.Dispose();
            return n;
        }

        private bool IsItProbablyPrime(BigInteger num)
        {
            if (num % 2 == 0)
                return false;
            if (num % 3 == 0)
                return false;
            if (num % 5 == 0)
                return false;
            if (num % 7 == 0)
                return false;

            BigInteger n = num - 1;
            int k = 0;
            
            while (n % 2 == 0)
            {
                n /= 2;
                k += 1;
            }
            BigInteger m = (num - 1) / BigInteger.Pow(2, k);
            
            BigInteger b = BigInteger.ModPow(2, m, num);
            
            if (b.IsOne || b == BigInteger.MinusOne)
                return true;
            else
            {
                for(int i = 1; i < k; i++)
                {
                    b = BigInteger.ModPow(b, 2, num);
                    if (b.IsOne)
                        return false;
                    /*if (b == BigInteger.MinusOne)
                        return true;*/
                    if (b == num - 1)
                        break;
                }
                if (b != num - 1)
                    return false;
            }
            return true;
        }
    }
}
