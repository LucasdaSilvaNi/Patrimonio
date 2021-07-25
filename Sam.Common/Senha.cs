using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Common
{
    public class Senha
    {
        public static string GerarSenha() 
        {
            string carac = "1234567890";
            //!@#$%&*
            // converte em uma matriz de caracteres
            char[] letras = carac.ToCharArray();

            // vamos embaralhar 5 vezes
            Embaralhar(ref letras, 1000);

            // senha de 4 caracteres
            string senha = new String(letras).Substring(0, 4);

            // resultado
            return senha;
        }


        private static void Embaralhar(ref char[] array, int vezes)
        {
            Random rand = new Random(DateTime.Now.Millisecond);

            for (int i = 1; i <= vezes; i++)
            {
                for (int x = 1; x <= array.Length; x++)
                {
                    Trocar(ref array[rand.Next(0, array.Length)],
                      ref array[rand.Next(0, array.Length)]);
                }
            }
        }

        private static void Trocar(ref char arg1, ref char arg2)
        {
            char strTemp = arg1;
            arg1 = arg2;
            arg2 = strTemp;
        }
    }
}
