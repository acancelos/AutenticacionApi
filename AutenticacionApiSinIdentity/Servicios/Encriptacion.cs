using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AutenticacionApiSinIdentity.Servicios
{
    public class Encriptacion
    {
        private readonly IConfiguration configuration;

        public Encriptacion(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string Encriptar(string Cadena)
        {
            try
            {
                string key = configuration["Llave"]; //llave para encriptar datos
                byte[] keyArray;
                byte[] Arreglo_a_Cifrar = UTF8Encoding.UTF8.GetBytes(Cadena);

                //Se utilizan las clases de encriptación MD5
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();

                //Algoritmo TripleDES
                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = tdes.CreateEncryptor();
                byte[] ArrayResultado = cTransform.TransformFinalBlock(Arreglo_a_Cifrar, 0, Arreglo_a_Cifrar.Length);
                tdes.Clear();

                //se regresa el resultado en forma de una cadena
                Cadena = Convert.ToBase64String(ArrayResultado, 0, ArrayResultado.Length);
            }
            catch (Exception) { }
            return Cadena;
        }
    }
}
