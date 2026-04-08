using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace ApiOAuthEmpleadosACV.Helpers
{
    public class HelperCifrado
    {
        //tenemos que cifrar los datos del usuario que van en el token con una clave en ida y en vuelta
        //clave en appsetings
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public HelperCifrado(IConfiguration config)
        {
            var key = config["ClavesCrypto:Key"];
            var iv = config["ClavesCrypto:IV"];

            _key = Encoding.UTF8.GetBytes(key);
            _iv = Encoding.UTF8.GetBytes(iv);
        }

        public string EncryptObject<T>(T objeto)
        {
            var json = JsonConvert.SerializeObject(objeto);
            return Encrypt(json);
        }

        public T DecryptObject<T>(string cipherText)
        {
            var json = Decrypt(cipherText);
            return JsonConvert.DeserializeObject<T>(json);
        }

        private string Encrypt(string textoPlano)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            using var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using var sw = new StreamWriter(cs);

            sw.Write(textoPlano);
            sw.Close();

            return Convert.ToBase64String(ms.ToArray());
        }

        private string Decrypt(string textoCifrado)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(Convert.FromBase64String(textoCifrado));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            return sr.ReadToEnd();
        }
    }
}