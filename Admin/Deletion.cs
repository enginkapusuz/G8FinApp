using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Windows;


namespace G8FinApp.Admin
{
    public class Deletion
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();

        public Deletion()
        {

        }
        public bool ResetAutoNumber(OleDbConnection con, string tableName)
        {
            using (OleDbCommand cmd = new OleDbCommand())
            {
                cmd.Connection = con;
                cmd.CommandText = "ALTER TABLE " + tableName + " ALTER COLUMN ID COUNTER(1,1)";
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.ExecuteNonQuery();
            }

            return true;
        }
        private bool DeleteTable(OleDbConnection con, string tableName)
        {
            using(OleDbCommand cmd = new OleDbCommand())
            {
                cmd.Connection = con;
                cmd.CommandText = "DELETE * FROM " + tableName;

                OleDbDataAdapter adapter = new OleDbDataAdapter()
                {
                    DeleteCommand = cmd,
                };

                adapter.DeleteCommand.ExecuteNonQuery();
            }

            return true;
        }

        public bool DeleteTableGroup(string groupName)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM " + groupName,
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (MessageBox.Show("Do you want to delete " + reader["TableName"].ToString(), "Confirmation!", MessageBoxButton.YesNo) == MessageBoxResult.No)
                        {
                            continue;
                        }

                        DeleteTable(con, reader["TableName"].ToString());
                        ResetAutoNumber(con, reader["TableName"].ToString());
                        _ = MessageBox.Show(reader["TableName"].ToString());
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("DeleteTableGroup:" + ex.Message);
                    return false;
                }
            }
            return true;
        }
    }
}
