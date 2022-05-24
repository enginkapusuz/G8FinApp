using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Windows;

namespace G8FinApp.Purchasing
{
    public class ContractItemsMain: ObservableCollection<Item>
    {
        Database.ProgramConsts prgrmConst = new Database.ProgramConsts();
        public bool OpenInitData(string biddingId)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM PurchasingContractItems WHERE BiddingId = ?",
                };

                cmd.Parameters.AddWithValue("@BiddingId", biddingId);

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Item item = new Item()
                        {
                            ID = reader["ID"].ToString(),
                            ItemNu = reader["ItemNu"].ToString(),
                            BiddingId = reader["BiddingId"].ToString(),
                            Description = reader["Description"].ToString(),
                            Brand = reader["Brand"].ToString() ?? string.Empty,
                            Quantity = float.Parse(reader["Quantity"].ToString()),
                            Unit = reader["Unit"].ToString(),
                            UnitPrice = decimal.Parse(reader["UnitPrice"].ToString()),
                            TotalAmount = decimal.Parse(reader["Quantity"].ToString()) * decimal.Parse(reader["UnitPrice"].ToString()),
                        };

                        Add(item);
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("ItemsMain:OpenInitData:" + ex.Message);
                    return false;
                }

                return true;
            }
        }


        public bool OpenSecondInitData(string biddingId)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM PurchasingItems WHERE BiddingId = ?",
                };

                cmd.Parameters.AddWithValue("@BiddingId", biddingId);

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Item item = new Item()
                        {
                            ID = reader["ID"].ToString(),
                            ItemNu = reader["ItemNu"].ToString(),
                            BiddingId = reader["BiddingId"].ToString(),
                            Description = reader["Description"].ToString(),
                            Quantity = float.Parse(reader["Quantity"].ToString()),
                            Unit = reader["Unit"].ToString(),
                            UnitPrice = decimal.Parse(reader["UnitPrice"].ToString()),
                            TotalAmount = decimal.Parse(reader["Quantity"].ToString()) * decimal.Parse(reader["UnitPrice"].ToString()),
                        };

                        Add(item);
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("ItemsMain:OpenInitData:" + ex.Message);
                    return false;
                }

                return true;
            }
        }

        public bool DeleteData(string biddingId)
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "DELETE FROM PurchasingContractItems WHERE BiddingId = ?",
                };
                cmd.Parameters.AddWithValue("@BiddingId", biddingId);

                try
                {
                    con.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter()
                    {
                        DeleteCommand = cmd,
                    };

                    _ = adapter.DeleteCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("ContractItemsMain:DeleteData:" + ex.Message);
                    return false;
                }
            }
            return true;
        }

        public bool SaveData()
        {
            using (OleDbConnection con = new OleDbConnection(prgrmConst.connectionString))
            {
                try
                {
                    con.Open();

                    foreach (Item itm in Items)
                    {
                        OleDbCommand cmd = new OleDbCommand()
                        {
                            Connection = con,
                            CommandType = System.Data.CommandType.Text,
                        };

                        OleDbDataAdapter adapter = new OleDbDataAdapter()
                        {
                            InsertCommand = cmd,
                        };

                        cmd.CommandText = "INSERT INTO PurchasingContractItems (ItemNu, BiddingId, Description, Quantity, Unit, UnitPrice) VALUES(?, ?, ?, ?, ?, ?)";
                        cmd.Parameters.AddWithValue("@ItemNu", itm.ItemNu);
                        cmd.Parameters.AddWithValue("@BiddingId", itm.BiddingId);
                        cmd.Parameters.AddWithValue("@Description", itm.Description);
                        cmd.Parameters.AddWithValue("@Quantity", itm.Quantity.ToString());
                        cmd.Parameters.AddWithValue("@Unit", itm.Unit);
                        cmd.Parameters.AddWithValue("@UnitPrice", itm.UnitPrice.ToString());

                        if (!(adapter.InsertCommand.ExecuteNonQuery() > 0))
                        {
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show("ItemsMain:SaveData:" + ex.Message);
                    return false;
                }

                return true;
            }
        }
    }
}
