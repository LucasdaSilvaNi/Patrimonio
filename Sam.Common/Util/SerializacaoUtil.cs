using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;

namespace Sam.Common.Util
{
    public static class SerializacaoUtil<T>
    {
        public static byte[] Serializar(T objeto)
        {
            MemoryStream memoryStream = new MemoryStream();
            IFormatter binaryFormatter = new BinaryFormatter();

            try
            {
                binaryFormatter.Serialize(memoryStream, objeto);

                byte[] byteObj = memoryStream.GetBuffer();
                return byteObj;
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao serializar o objeto.", e);
            }
            finally
            {
                memoryStream.Close();
            }            
        }

        public static T Deserializar(byte[] byteObj)
        {
            MemoryStream memoryStream = new MemoryStream();
            IFormatter binaryFormatter = new BinaryFormatter();

            try
            {
                memoryStream.Write(byteObj, 0, byteObj.Length);                
                memoryStream.Seek(0, SeekOrigin.Begin);

                object objeto = binaryFormatter.Deserialize(memoryStream);
                return (T)objeto;
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao deserializar o objeto.", e);
            }
            finally
            {
                memoryStream.Close();
            }
        }

    }
}
