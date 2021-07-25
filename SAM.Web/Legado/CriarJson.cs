using Microsoft.ReportingServices.DataProcessing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace SAM.Web.Legado
{
    public class CriarJson
    {
        private String jsonReturno;
        private CriarJson()
        {
            jsonReturno = String.Empty;
        }

        public static CriarJson createInstance()
        {
            return new CriarJson();
        }
        public String converterDataReaderParaJson(SqlDataReader reader)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            IList<IDictionary<String, String>> rows = new List<IDictionary<String, String>>();
            IDictionary<String, String> row = null;
            
            while (reader.Read())
            {

                int fields = reader.FieldCount;
                row = new Dictionary<String, String>();
                for (int i = 0; i < fields; i++)
                {

                    row.Add(reader.GetName(i), reader[i].ToString());

                }
                rows.Add(row);

            }
            
            return new CriarJson().jsonReturno = serializer.Serialize(rows);
        }
              
    }
}