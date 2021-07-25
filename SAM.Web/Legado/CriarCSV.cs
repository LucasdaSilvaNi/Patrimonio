using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace SAM.Web.Legado
{
    internal class CriarCSV
    {
        private StringBuilder builder = null;
        private CriarCSV() { }
        private CriarCSV(DataTable table, bool printHeaders)
        {
            conveterTableParaCsvString(table, printHeaders);
        }

        internal static CriarCSV createInstance(DataTable table, bool printHeaders)
        {
            return new CriarCSV(table, printHeaders);
        }

        internal string getCSV()
        {
            return this.builder.ToString();
        }
        private void conveterTableParaCsvString(DataTable table, bool printHeaders
)
        {
            builder = new StringBuilder();

            if (printHeaders)
            {
                //Grava os cabeçalhos.
                for (int colCount = 0;
                     colCount < table.Columns.Count; colCount++)
                {
                    builder.Append(table.Columns[colCount].ColumnName);
                    if (colCount != table.Columns.Count - 1)
                    {
                        builder.Append(",");
                    }
                    else
                    {
                        builder.AppendLine();
                    }
                }
            }

            // Grava todas as linhas.
            for (int rowCount = 0;
                 rowCount < table.Rows.Count; rowCount++)
            {
                for (int colCount = 0;
                     colCount < table.Columns.Count; colCount++)
                {
                    builder.Append(table.Rows[rowCount][colCount]);
                    if (colCount != table.Columns.Count - 1)
                    {
                        builder.Append(",");
                    }
                }
                if (rowCount != table.Rows.Count - 1)
                {
                    builder.AppendLine();
                }
            }

            
        }
    }
}