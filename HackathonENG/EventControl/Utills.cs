using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EventControl
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

        public static void GetDataFromEvents(List<Element> addedElement, List<Element> modified, List<ElementId> deleted, List<string> transactions, string user, out DataTable add, out DataTable edit, out DataTable delete, out DataTable trans)
        {
            DateTime now = DateTime.Now;

            trans = new DataTable();
            add = new DataTable();
            edit = new DataTable();
            delete = new DataTable();

            trans.Columns.Add("Time");
            trans.Columns.Add("Transactions ID");
            trans.Columns.Add("Transaction Name");

            Guid transactionID = Guid.NewGuid();
            foreach (var item in transactions)
            {
                DataRow row = trans.NewRow();
                row["Time"] = now;
                row["Transactions ID"] = transactionID.ToString();
                row["Transaction Name"] = item;
                trans.Rows.Add(row);
            }

           
            add.Columns.Add("Time");//ok
            add.Columns.Add("ElementID");//ok
            add.Columns.Add("Revit Category");//ok
            add.Columns.Add("Object Type");//ok
            add.Columns.Add("Attribute Value");//ok
            add.Columns.Add("Transactions ID");//ok
            add.Columns.Add("Revit Family");//ok
            add.Columns.Add("Revit Type");//ok

            edit.Columns.Add("User");
            edit.Columns.Add("Time");//ok
            edit.Columns.Add("ElementID");//ok
            edit.Columns.Add("Revit Category");//ok
            edit.Columns.Add("Transactions ID");//ok
            edit.Columns.Add("Revit Family");//ok
            edit.Columns.Add("Revit Type");//ok

       
            delete.Columns.Add("Time");//ok
            delete.Columns.Add("Transactions ID");//ok
            delete.Columns.Add("ElementID");//ok


            if (addedElement.Count() > 0)
            {
                foreach (var item in addedElement)
                {
                    DataRow row = add.NewRow();
                    
                    row["Time"] = now;
                    row["Transactions ID"] = transactionID;
                    row["ElementID"] = item.Id.IntegerValue;
                    row["Revit Category"] = item.Category.Name;

                    if (item.Location is LocationCurve)
                    {
                        row["Object Type"] = "CURVE";
                        row["Attribute Value"] = (item.Location as LocationCurve).Curve.ApproximateLength;

                    }
                    else if (item.Location is LocationPoint)
                    {
                        row["Object Type"] = "POINT";
                        row["Attribute Value"] = 1;
                    }
                    else
                    {
                        row["Object Type"] = "POINT";
                        row["Attribute Value"] = 1;
                    }
                    if (item is FamilyInstance)
                    {
                        row["Revit Type"] = (item as FamilyInstance).Symbol.Name;
                        row["Revit Family"] = (item as FamilyInstance).Symbol.Family.Name;
                    }
                    else if (item is FamilySymbol)
                    {
                        row["Revit Type"] = (item as FamilySymbol).Name;
                        row["Revit Family"] = (item as FamilySymbol).Family.Name;
                    }
                    else if (item is ElementType)
                    {
                        row["Revit Type"] = (item as ElementType).Name;
                        row["Revit Family"] = (item as ElementType).FamilyName;
                    }
                    else if (item is Family)
                    {
                        row["Revit Type"] = "NULL";
                        row["Revit Family"] = item.Name;

                    }
                    else if ((item.GetTypeId() != null))
                    {
                        Document doc = item.Document;
                        var type = doc.GetElement(item.GetTypeId());
                        row["Revit Type"] = type.Name;
                        row["Revit Family"] = (type as ElementType).FamilyName;
                    }

                    add.Rows.Add(row);

                }
            }

            if (modified.Count() > 0)
            {
                foreach (var item in modified)
                {
                    DataRow row = edit.NewRow();
                    row["User"] = user;
                    row["Time"] = now;
                    row["Transactions ID"] = transactionID;
                    row["ElementID"] = item.Id.IntegerValue;
                    row["Revit Category"] = item.Category.Name;

                    if (item is FamilyInstance)
                    {
                        row["Revit Type"] = (item as FamilyInstance).Symbol.Name;
                        row["Revit Family"] = (item as FamilyInstance).Symbol.Family.Name;
                    }
                    else if (item is FamilySymbol)
                    {
                        row["Revit Type"] = (item as FamilySymbol).Name;
                        row["Revit Family"] = (item as FamilySymbol).Family.Name;
                    }
                    else if (item is ElementType)
                    {
                        row["Revit Type"] = (item as ElementType).Name;
                        row["Revit Family"] = (item as ElementType).FamilyName;
                    }
                    else if (item is Family)
                    {
                        row["Revit Type"] = "NULL";
                        row["Revit Family"] = item.Name;

                    }
                    else if ((item.GetTypeId() != null))
                    {
                        Document doc = item.Document;
                        var type = doc.GetElement(item.GetTypeId());
                        row["Revit Type"] = type.Name;
                        row["Revit Family"] = (type as ElementType).FamilyName;
                    }

                    edit.Rows.Add(row);

                }
            }

            if (deleted.Count() > 0)
            {
                foreach (var item in deleted)
                {
                    DataRow row = delete.NewRow();
                 
                    row["Time"] = now;
                    row["Transactions ID"] = transactionID;
                    row["ElementID"] = item.IntegerValue;

                    delete.Rows.Add(row);
                }
            }


        }

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


    }
}
