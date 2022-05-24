using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Windows;

namespace G8FinApp.Fiscal
{
    public class MissCommitMain : ObservableCollection<MainCommit>
    {
        Database.ProgramConsts programConsts = new Database.ProgramConsts();

        public MissCommitMain()
        {

        }

        public void InitList()
        {
            using(OleDbConnection con = new OleDbConnection(programConsts.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM FiscalMissCommit",
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        MainCommit mainCommit = new MainCommit()
                        {
                            CommitDate = DateTime.Parse(reader["CommitDate"].ToString()),
                            CommitNu = reader["CommitNu"].ToString(),
                            ID = reader["ID"].ToString(),
                            TableName = reader["TableName"].ToString(),
                        };

                        Add(mainCommit);
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("MissCommitMain:" + ex.Message);
                    return;
                }
            }
        }

        public bool SaveData(MainCommit mainCommit)
        {
            using(OleDbConnection con = new OleDbConnection(programConsts.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO FiscalMissCommit (CommitNu, CommitDate, TableName) VALUES(?, ?, ?)",
                };

                cmd.Parameters.AddWithValue("@CommitNu", mainCommit.CommitNu);
                cmd.Parameters.AddWithValue("@CommitDate", mainCommit.CommitDate.ToString());
                cmd.Parameters.AddWithValue("@TableName", mainCommit.TableName);

                try
                {
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        InsertCommand = cmd,
                    };

                    if (!(adapter.InsertCommand.ExecuteNonQuery() > 0))
                    {
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("MissCommitMain:" + ex.Message);
                    return false;
                }
            }
            return true;
        }
    }
}
