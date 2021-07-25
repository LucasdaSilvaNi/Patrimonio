using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Sam.Common
{
    public class FillSourceGridView
    {
        private ObjectDataSource _objectDataSource;
        public ObjectDataSource ObjectDataSource
        {
            get { return _objectDataSource; }
        }

        public FillSourceGridView() { }

        public FillSourceGridView(string typeName, string selectMethod, string objectDataSourceId)
        {
            _objectDataSource = new ObjectDataSource(typeName, selectMethod);
            _objectDataSource.ID = objectDataSourceId;
        }

        public FillSourceGridView(string typeName, string objectDataSourceId, string startRowIndexParameterName, string maximumRowsParameterName, string selectCountMethod, string oldValuesParameterFormatString, bool enablePaging)
        {
            _objectDataSource = new ObjectDataSource();
            _objectDataSource.TypeName = typeName;
            _objectDataSource.ID = objectDataSourceId;
            _objectDataSource.StartRowIndexParameterName = startRowIndexParameterName;
            _objectDataSource.MaximumRowsParameterName = maximumRowsParameterName;
            _objectDataSource.SelectCountMethod = selectCountMethod;
            _objectDataSource.OldValuesParameterFormatString = oldValuesParameterFormatString;
            _objectDataSource.EnablePaging = enablePaging;
        }

        public void SetSelectParameters(Parameter parameters)
        {
            _objectDataSource.SelectParameters.Add(parameters);
        }

        public void SetSelectMethod(string selectMethod)
        {
            _objectDataSource.SelectMethod = selectMethod;
        }

        public void SetObjectDataSourceSelectingEventHandler(ObjectDataSourceSelectingEventHandler objectDataSourceSelectingEventHandler)
        {
            _objectDataSource.Selecting += new ObjectDataSourceSelectingEventHandler(objectDataSourceSelectingEventHandler);
        }

        public void SetBind()
        {
            _objectDataSource.DataBind();
        }
    }
}
