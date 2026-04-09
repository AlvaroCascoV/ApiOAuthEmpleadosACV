using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace ApiOAuthEmpleadosACV.Helpers
{
    public class HelperCifrado
    {
        private static string KeyCifrado;

        public static void Initialize(IConfiguration configuration)
        {
            KeyCifrado = configuration.GetValue<string>("ClavesCrypto:Key");
        }

        public static string CifrarString(string data)
        {
            //CONVERTIMOS A BYTES LA KEY
            byte[] keyData = Encoding.UTF8.GetBytes(KeyCifrado);
            string res = EncryptString(keyData, data);
            return res;
        }

        public static string DescifrarString(string data)
        {
            //CONVERTIMOS A BYTES LA KEY
            byte[] keyData = Encoding.UTF8.GetBytes(KeyCifrado);
            string res = DecryptString(keyData, data);
            return res;
        }

        private static string EncryptString(byte[] key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        private static string DecryptString(byte[] key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }


        //Mi manera--------------------------------------------------------------------------------------
        ////tenemos que cifrar los datos del usuario que van en el token con una clave en ida y en vuelta
        ////clave en appsetings
        //private readonly byte[] _key;
        //private readonly byte[] _iv;

        //public HelperCifrado(IConfiguration config)
        //{
        //    var key = config["ClavesCrypto:Key"];
        //    var iv = config["ClavesCrypto:IV"];

        //    _key = Encoding.UTF8.GetBytes(key);
        //    _iv = Encoding.UTF8.GetBytes(iv);
        //}

        //public string EncryptObject<T>(T objeto)
        //{
        //    var json = JsonConvert.SerializeObject(objeto);
        //    return Encrypt(json);
        //}

        //public T DecryptObject<T>(string cipherText)
        //{
        //    var json = Decrypt(cipherText);
        //    return JsonConvert.DeserializeObject<T>(json);
        //}

        //private string Encrypt(string textoPlano)
        //{
        //    using var aes = Aes.Create();
        //    aes.Key = _key;
        //    aes.IV = _iv;

        //    using var encryptor = aes.CreateEncryptor();
        //    using var ms = new MemoryStream();
        //    using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        //    using var sw = new StreamWriter(cs);

        //    sw.Write(textoPlano);
        //    sw.Close();

        //    return Convert.ToBase64String(ms.ToArray());
        //}

        //private string Decrypt(string textoCifrado)
        //{
        //    using var aes = Aes.Create();
        //    aes.Key = _key;
        //    aes.IV = _iv;

        //    using var decryptor = aes.CreateDecryptor();
        //    using var ms = new MemoryStream(Convert.FromBase64String(textoCifrado));
        //    using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        //    using var sr = new StreamReader(cs);

        //    return sr.ReadToEnd();
        //}
    }
}