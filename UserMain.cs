using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Windows;

namespace G8FinApp
{
    public class UserMain
    {

        User user = new User();
        Database.ProgramConsts prgrmConsts = new Database.ProgramConsts();

        public UserMain(string userName, string userPassword, string userRole)
        {
            user.UserName = userName;
            user.UserPassword = userPassword;
            user.UserRole = userRole;
        }


        public bool UserCheck()
        {
            using(OleDbConnection con = new OleDbConnection(prgrmConsts.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "SELECT * FROM UserMain";
                cmd.Connection = con;

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        if (reader[1].ToString().Trim() == user.UserName.Trim())
                        {
                            if (reader[2].ToString().Trim() == user.UserPassword.Trim() && reader[3].ToString().Trim() == user.UserRole)
                            {
                                return true;
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("Error:" + ex.Message);
                    return false;
                }
            }

            return false;
        }
    }
}
