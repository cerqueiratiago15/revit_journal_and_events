using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;

namespace UploadToDBConsole
{
    public static class Utills
    {

        public static string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\ENGHackathon-DONT DELETE IT";
        public static string transPath = path + @"\trans.txt";
        public static string addPath = path + @"\add.txt";
        public static string editPath = path + @"\edit.txt";
        public static string deletedPath = path + @"\delete.txt";
        public static string userPath = path + @"\user.txt";
        public static string journal = path + @"\journal.txt";


        public static void ToTXT(this DataTable dtDataTable, string strFilePath)
        {
            DataTable tbl = null;
            if (File.Exists(strFilePath))
            {
                tbl = ConvertTXTtoDataTable(strFilePath);
                tbl.Merge(dtDataTable);

            }
            else
            {
                tbl = dtDataTable;
            }

            using (StreamWriter sw = new StreamWriter(strFilePath, false))
            {
                //headers  
                for (int i = 0; i < tbl.Columns.Count; i++)
                {
                    sw.Write(tbl.Columns[i]);
                    if (i < tbl.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
                foreach (DataRow dr in tbl.Rows)
                {
                    for (int i = 0; i < tbl.Columns.Count; i++)
                    {
                        if (!Convert.IsDBNull(dr[i]))
                        {
                            string value = dr[i].ToString();
                            if (value.Contains(','))
                            {
                                value = String.Format("\"{0}\"", value);
                                sw.Write(value);
                            }
                            else
                            {
                                sw.Write(dr[i].ToString());
                            }
                        }
                        if (i < tbl.Columns.Count - 1)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.Write(sw.NewLine);
                }
                sw.Close();
            }

        }

        public static DataTable ConvertTXTtoDataTable(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath))
            {
                string[] headers = sr.ReadLine().Split(',');

                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = Regex.Split(sr.ReadLine(), ",");
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }



        public static void SendUniqueFileToTableJournal( DataTable journal)
        {
            Database cn = new Database();

            //SqlCommand cmd = new SqlCommand("insert_Data", cn.Connection);

            ////set the command type  to stored procedure
            //cmd.CommandType = CommandType.StoredProcedure;

            ////add parameter
            //cmd.Parameters.AddWithValue("@username", user);
            //cmd.Parameters.AddWithValue("@addedFileTable", added);
            //cmd.Parameters.AddWithValue("@deletedFileTable", delete);
            //cmd.Parameters.AddWithValue("@editFileTable", modify);
            //cmd.Parameters.AddWithValue("@transactions", transaction);
            //cmd.Parameters.AddWithValue("@journalFileTable", journal);
            
            SqlCommand cmd = new SqlCommand("insert_Journals", cn.Connection);

            var username = Environment.UserName;

            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@journalFileTable", journal);


            try
            {
                cn.Connection.Open();
                cmd.ExecuteNonQuery();
                cn.Connection.Close();
            }
            catch (Exception ex)
            {
                if (cn.Connection.State == System.Data.ConnectionState.Open)
                {
                    cn.Connection.Close();
                }
            }

        }

        //GetFullString From txt File
        public static string getFullString(string path)
        {
            //Check if File Exists
            if (!File.Exists(path))
            {
                return string.Empty;
            }

            //Read File in Streaming (IN USE)
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var textReader = new StreamReader(fileStream))
            {
                return textReader.ReadToEnd();
            }

        }

        public static DataTable GetJournalDatatable(string path)
        {
            ListGroups groupOfGroups = new ListGroups((new StringReader(path)).ReadLine());

            DataTable journaltable = new DataTable();
            journaltable.Columns.Add("Time", typeof(string));
            journaltable.Columns.Add("EditMode", typeof(string));
            journaltable.Columns.Add("Command", typeof(string));
            journaltable.Columns.Add("Type", typeof(char));
            journaltable.Columns.Add("Jrn", typeof(string));
            journaltable.Columns.Add("Description", typeof(string));

            foreach (actionGroup gr in groupOfGroups.ListOfGroups)
            {
                foreach (action ac in gr.ListOfActions)
                {

                    DataRow row = journaltable.NewRow();
                    row["Time"] = ac.Time;
                    row["EditMode"] = gr.EditMode;
                    row["Command"] = gr.Command;
                    row["Type"] = ac.Type;
                    row["Jrn"] = ac.Jrn;
                    row["Description"] = ac.Description;
                    journaltable.Rows.Add(row);

                }
            }

            return journaltable;

        }

    }
}
