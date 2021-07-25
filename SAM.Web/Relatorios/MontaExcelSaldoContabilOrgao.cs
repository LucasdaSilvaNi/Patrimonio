using System.Web.UI.WebControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace SAM.Web.Relatorios
{
    public class MontaExcelSaldoContabilOrgao
    {
        public StringWriter sw;
        public MontaExcelSaldoContabilOrgao(DataTable dados) {
            GridView gv = new GridView();

            gv.AutoGenerateColumns = false;
            gv.Columns.Add(new BoundField { HeaderText = "Orgão", DataField = "Orgao" });
            gv.Columns.Add(new BoundField { HeaderText = "UO", DataField = "UO" });
            gv.Columns.Add(new BoundField { HeaderText = "UGE", DataField = "UGE" });
            gv.Columns.Add(new BoundField { HeaderText = "Descrição UGE", DataField = "NomeUGE" });
            gv.Columns.Add(new BoundField { HeaderText = "Mês Referência", DataField = "MesRef" });
            gv.Columns.Add(new BoundField { HeaderText = "Conta Depreciação", DataField = "ContaContabilDepreciacao" });
            gv.Columns.Add(new BoundField { HeaderText = "Descrição Conta Depreciação", DataField = "ContaContabilDepreciacaoDescricao" });
            gv.Columns.Add(new BoundField { HeaderText = "Conta contábil", DataField = "ContaContabil" });
            gv.Columns.Add(new BoundField { HeaderText = "Descrição Conta contábil", DataField = "ContaContabilDescricao" });
            gv.Columns.Add(new BoundField { HeaderText = "Valor Contábil (R$)", DataField = "ValorContabil" });
            gv.Columns.Add(new BoundField { HeaderText = "Depreciação No Mês (R$)", DataField = "DepreciacaoNoMes" });
            gv.Columns.Add(new BoundField { HeaderText = "Depreciação Acumulada (R$)", DataField = "DepreciacaoAcumulada" });

            gv.DataSource = dados;
            gv.DataBind();

            sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
        }
    }
}