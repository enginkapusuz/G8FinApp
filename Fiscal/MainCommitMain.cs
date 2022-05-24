using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Data.OleDb;

namespace G8FinApp.Fiscal
{
    public class MainCommitMain : ObservableCollection<MainCommit>
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();
        public MainCommitMain()
        {

        }

        public MainCommitMain(bool IsInitList)
        {
            InitList();
        }

        private void InitList()
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM FiscalMainCommit",
                };

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        MainCommit mainCommit = new MainCommit()
                        {
                            ID = reader["ID"].ToString(),
                            CommitNu = reader["CommitNu"].ToString(),
                            CommitDate = DateTime.Parse(reader["CommitDate"].ToString()),
                        };

                        Add(mainCommit);
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("MainCommitMain:InitList:" + ex.Message);
                    return;
                }
            }
        }

        public bool SaveData(MainCommit mainCommit)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO FiscalMainCommit (CommitNu, CommitDate, TableName) VALUES(?, ?, ?)",
                };

                cmd.Parameters.AddWithValue("@CommitNu", mainCommit.CommitNu);
                cmd.Parameters.AddWithValue("@CommitDate", mainCommit.CommitDate.ToString("d"));
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
                    _ = MessageBox.Show("MainCommitMain:SaveData:" + ex.Message);
                    return false;
                }
            }
            return true;
        }

        
        public bool UpdateData(CommitApprove commitApprove)
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "UPDATE FiscalMainCommit SET CommitNu= ? WHERE CommitNu = ?",
                };

                cmd.Parameters.AddWithValue("@CommitNu", "Cancelled");
                cmd.Parameters.AddWithValue("@CommitNu", commitApprove.CommitNu);

                try
                {
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        UpdateCommand = cmd,
                    };

                    if (!(adapter.UpdateCommand.ExecuteNonQuery() > 0))
                    {
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("MainCommitMain:DeleteData:" + ex.Message);
                    return false;
                }
            }
            return true;
        }
    }
}
