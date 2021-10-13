using System.Collections.Generic;
using System.Data;

namespace PowerPlant.Infrastructure.Services.AutoFillDataServices
{
    public class ProductionDataTableMemo
    {
        private Dictionary<int, DataTable> _memo = new Dictionary<int, DataTable>();

        public DataTable GetDataTableTemplate(int size)
        {
            if (!_memo.ContainsKey(size))
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Id");
                dataTable.Columns.Add("GeneratorId");
                dataTable.Columns.Add("Quantity");
                dataTable.Columns.Add("Date");
                dataTable.Columns.Add("Time");

                for (int i = 0; i < size; i++)
                    dataTable.Rows.Add(dataTable.NewRow());

                _memo.Add(size, dataTable);
            }

            return _memo[size];
        }
    }
}
