using System;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using ReportsLibrary.Db;

namespace ReportsLibrary.Util
{
	public static class DataTableHelper
	{
		/// <summary>
		/// Загрузка базы данных в DataTable
		/// </summary>
		public static DataTable LoadDataTableByQuery(string dbFolder, string query, string tableName)
		{
			var dataTable = new DataTable();
			using (var connection = DbControl.GetConnection(dbFolder))
			{
				connection.Open();
				using (var command = new OleDbCommand(query, connection))
				{
					using (var reader = command.ExecuteReader())
					{
						if (reader != null)
						{
							dataTable.Load(reader);
							dataTable.TableName = tableName;
						}
					}
				}
			}
			return dataTable;
		}

		/// <summary>
		/// Объеденение двух DataTable по одному полю
		/// </summary>
		public static DataTable JoinTwoDataTablesOnOneColumn(DataTable dtblLeft, string leftColumnName, DataTable dtblRight,
			string rightColumnName, int joinType)
		{
			//Change column name to a temp name so the LINQ for getting row data will work properly.
			if (dtblRight.Columns.Contains(leftColumnName))
				dtblRight.Columns[leftColumnName].ColumnName = rightColumnName;

			//Get columns from dtblLeft
			DataTable dtblResult = dtblLeft.Clone();

			//Get columns from dtblRight
			var dt2Columns = dtblRight.Columns.OfType<DataColumn>()
				.Select(dc => new DataColumn(dc.ColumnName, dc.DataType, dc.Expression, dc.ColumnMapping));

			//Get columns from dtblRight that are not in dtblLeft
			var dt2FinalColumns = from dc in dt2Columns.AsEnumerable()
								  where !dtblResult.Columns.Contains(dc.ColumnName)
								  select dc;

			//Add the rest of the columns to dtblResult
			dtblResult.Columns.AddRange(dt2FinalColumns.ToArray());

			//No reason to continue if the colToJoinOn does not exist in both DataTables.
			if (!dtblLeft.Columns.Contains(leftColumnName) ||
				(!dtblRight.Columns.Contains(leftColumnName) && !dtblRight.Columns.Contains(rightColumnName)))
			{
				if (!dtblResult.Columns.Contains(leftColumnName))
					dtblResult.Columns.Add(leftColumnName);
				return dtblResult;
			}

			switch (joinType)
			{
				case 0:
					#region Inner
					//get row data
					//To use the DataTable.AsEnumerable() extension method you need to add a reference to the System.Data.DataSetExtension assembly in your project. 
					var rowDataLeftInner = from rowLeft in dtblLeft.AsEnumerable()
										   join rowRight in dtblRight.AsEnumerable() on rowLeft[leftColumnName] equals rowRight[rightColumnName]
										   select rowLeft.ItemArray.Concat(rowRight.ItemArray).ToArray();


					//Add row data to dtblResult
					foreach (object[] values in rowDataLeftInner)
						dtblResult.Rows.Add(values);
					#endregion
					break;
				case 1:
					#region Left
					var rowDataLeftOuter = from rowLeft in dtblLeft.AsEnumerable()
										   join rowRight in dtblRight.AsEnumerable() on rowLeft[leftColumnName] equals rowRight[rightColumnName] into gj
										   from subRight in gj.DefaultIfEmpty()
										   select rowLeft.ItemArray.Concat((subRight == null) ? (dtblRight.NewRow().ItemArray) : subRight.ItemArray)
											   .ToArray();


					//Add row data to dtblResult
					foreach (object[] values in rowDataLeftOuter)
						dtblResult.Rows.Add(values);
					#endregion
					break;
			}

			//Change column name back to original
			dtblRight.Columns[rightColumnName].ColumnName = leftColumnName;

			//Remove extra column from result
			dtblResult.Columns.Remove(rightColumnName);

			return dtblResult;
		}

		/// <summary>
		/// Объеденение двух DataTable по двум полям
		/// </summary>
		public static DataTable LeftJoin_TwoTable_By_TwoFields<T1, T2>(DataTable table1, string t1Column1,
			string t1Column2, DataTable table2, string t2Column1, string t2Column2)
		{
			var query = from t1 in table1.AsEnumerable()
						join t2 in table2.AsEnumerable()
							on new { x1 = t1.Field<T1>(t1Column1), x2 = t1.Field<T2>(t1Column2) }
							equals new { x1 = t2.Field<T1>(t2Column1), x2 = t2.Field<T2>(t2Column2) } into tb
						from t2 in tb.DefaultIfEmpty()
						select new { t1, t2 };

			var result = new DataTable();
			foreach (DataColumn col in table1.Columns)
			{
				if (result.Columns[col.ColumnName] == null)
				{
					result.Columns.Add(col.ColumnName, col.DataType);
				}
				else
				{
					throw new ApplicationException();
				}
			}
			foreach (DataColumn col in table2.Columns)
			{
				if (result.Columns[col.ColumnName] == null)
				{
					result.Columns.Add(col.ColumnName, col.DataType);
				}
				else
				{
					throw new ApplicationException();
				}
			}
			foreach (var element in query)
			{
				var insertRow = result.NewRow();
				foreach (DataColumn column in element.t1.Table.Columns)
				{
					insertRow[column.ColumnName] = element.t1[column.ColumnName];
				}
				if (element.t2 != null)
				{
					foreach (DataColumn column in element.t2.Table.Columns)
					{
						insertRow[column.ColumnName] = element.t2[column.ColumnName];
					}
				}
				result.Rows.Add(insertRow);
			}
			return result;
		}
	}
}
