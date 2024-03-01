using InternationalWaterWebApp.Library.Common;
using Npgsql;
using System.Data;

namespace InternationalWaterWebApp.Library.DatabaseConnection
{
    public class WaterDBContext
    {
        private static readonly string connectionString;
        static WaterDBContext()
        {
            //Get connection string for appsettings.json file and set into global variable.
            connectionString = DatabaseConnection.connectionString;
        }

        public static DataSet GetData(string SqlQuery, int timeOut = 3000)
        {
            DataSet dataSet = new DataSet();
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    using (NpgsqlCommand command = new NpgsqlCommand(SqlQuery, conn))
                    {
                        NpgsqlTransaction tran = conn.BeginTransaction();
                        command.CommandText = SqlQuery;
                        command.CommandTimeout = timeOut;
                        command.Connection = conn;
                        command.CommandType = CommandType.Text;
                        command.Transaction = tran;
                        string cursorName = (string)command.ExecuteScalar();

                        dataSet = FetchResultFromCursorDataset(cursorName, conn);
                        tran.Commit();
                    }
                }
                return dataSet;
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                Console.WriteLine($"Error throw this function:{SqlQuery}");
            }
            return dataSet;
        }

        public static DataSet GetDataWithoutCursor(string SqlQuery, int timeOut = 3000)
        {
            DataSet dataSet = new DataSet();
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(SqlQuery, connection);
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = timeOut;
                    NpgsqlDataAdapter adp = new NpgsqlDataAdapter();
                    adp.SelectCommand = cmd;
                    adp.Fill(dataSet);

                    connection.Close();
                }
                return dataSet;
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                Console.WriteLine($"Error throw this function:{SqlQuery}");
            }
            return dataSet;
        }

        public static long? InsertUpdateData(string SqlQuery, int timeOut = 3000)
        {
            long? id = null;
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    using (NpgsqlCommand command = new NpgsqlCommand(SqlQuery, conn))
                    {
                        NpgsqlTransaction tran = conn.BeginTransaction();
                        command.CommandText = SqlQuery;
                        command.CommandTimeout = timeOut;
                        command.Connection = conn;
                        command.CommandType = CommandType.Text;
                        command.Transaction = tran;
                        string cursorName = (string)command.ExecuteScalar();

                        DataSet dataSet = FetchResultFromCursorDataset(cursorName, conn);
                        id = long.Parse(dataSet.Tables[0].Rows[0][0].ToString());
                        tran.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                Console.WriteLine($"Error throw this function:{SqlQuery}");
            }
            return id;
        }

        public static DataSet GetSelectItemDataset(string queryText, int timeOut = 3000)
        {
            DataSet resultList = new DataSet();
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    using (NpgsqlCommand command = new NpgsqlCommand(queryText, conn))
                    {
                        NpgsqlTransaction tran = conn.BeginTransaction();
                        command.CommandText = queryText;
                        command.CommandTimeout = timeOut;
                        command.Connection = conn;
                        command.CommandType = CommandType.Text;
                        command.Transaction = tran;
                        string cursorName = (string)command.ExecuteScalar();

                        resultList = FetchResultFromCursorDataset(cursorName, conn);
                        tran.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return resultList;
        }

        static DataSet FetchResultFromCursorDataset(string cursorName, NpgsqlConnection connection)
        {
            DataSet dsResult = new DataSet();
            try
            {
                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "fetch all in \"" + cursorName + "\";";
                    command.CommandType = CommandType.Text;

                    using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command))
                    {
                        adapter.Fill(dsResult);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
            return dsResult;
        }

        public static DataSet GetDataWithMultipleTables(string SqlQuery)
        {
            DataSet dataSet = new DataSet();
            DataSet GetcursorNameDs = new DataSet();

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    using (NpgsqlTransaction tran = conn.BeginTransaction())
                    {
                        NpgsqlCommand command = new NpgsqlCommand(SqlQuery, conn);
                        command.CommandTimeout = 500;

                        using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command))
                        {
                            adapter.Fill(GetcursorNameDs);
                        }

                        // Fetch results from cursor
                        int tabel = 0;
                        foreach (System.Data.DataRow CursorName in GetcursorNameDs.Tables[0].Rows)
                        {
                            DataSet fetchedResult = FetchResultFromCursorDataset(Convert.ToString(CursorName[0]), conn);

                            // Clone the DataTable before adding it to the dataSet
                            DataTable clonedTable = fetchedResult.Tables[0].Copy();

                            // Assign the unique name to the cloned DataTable
                            clonedTable.TableName = "Table_" + tabel;

                            dataSet.Tables.Add(clonedTable);
                            tabel++;
                        }

                        // Merge the cursorResult into dataSet

                        tran.Commit();
                    }
                }
                return dataSet;
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                Console.WriteLine($"Error throw this function:{SqlQuery}");
                // You might want to handle exceptions appropriately here.
            }
            return dataSet; // Return an empty dataset in case of an exception.
        }
    }
}