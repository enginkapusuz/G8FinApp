using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Collections.ObjectModel;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace G8FinApp.Reports
{
    public class ReportRangeMain : ObservableCollection<ReportRange>
    {
        Database.ProgramConsts prgrmConsts = new Database.ProgramConsts();
        public ReportRangeMain()
        {

        }

        public Dictionary<string, Excel.Range> LoadRangeData(ReportExcel reportExcel, ReportFile reportFile)
        {
            Dictionary<string, Excel.Range> dictExcelRange = null;

            using (OleDbConnection con = new OleDbConnection(prgrmConsts.connectionString))
            {
                OleDbCommand cmd = new OleDbCommand()
                {
                    Connection = con,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "SELECT * FROM ReportMainRange WHERE ReportMainID = ? AND TableName = ?",
                };

                MessageBox.Show(reportFile.ID.ToString() + Environment.NewLine +
                    reportFile.TableName);

                cmd.Parameters.AddWithValue("@ReportMainID", reportFile.ID);
                cmd.Parameters.AddWithValue("@TableName", reportFile.TableName);

                try
                {
                    con.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        if (dictExcelRange is null)
                        {
                            dictExcelRange = new Dictionary<string, Excel.Range>();
                        }
                        dictExcelRange[reader["RangeName"].ToString()] = reportExcel.ReportSheet.Range[reader["RangeAddress"].ToString()];
                    }
                }
                catch(Exception ex)
                {
                    _ = MessageBox.Show("ReportRangeMain:LoadRangeData:" + ex.Message);
                    return null;
                }
            }

            return dictExcelRange;
        }
    }
}
