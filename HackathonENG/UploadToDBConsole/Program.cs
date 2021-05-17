using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UploadToDBConsole
{
    class Program
    {
        static void Main(string[] args)
        {


            //var user = int.Parse(File.ReadAllText(Utills.userPath));
            //var deleted = Utills.ConvertTXTtoDataTable(Utills.deletedPath);
            //var added = Utills.ConvertTXTtoDataTable(Utills.addPath);
            //var modify = Utills.ConvertTXTtoDataTable(Utills.editPath);
            //var transaction = Utills.ConvertTXTtoDataTable(Utills.transPath);

            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string path = folderPath + @"\Autodesk\Revit\Autodesk Revit 2019\Journals";
            string path2018 = folderPath + @"\Autodesk\Revit\Autodesk Revit 2018\Journals";
            List<string> paths = new List<string>();

            if (Directory.Exists(path))
            {
                var js = new DirectoryInfo(path);

                var files = js.GetFiles();

                List<FileInfo> info = new List<FileInfo>();
                foreach (var item in files)
                {
                    if (item.Name.EndsWith(".txt") && item.Name.StartsWith("journal"))
                    {
                        info.Add(item);
                    }
                }
                foreach (var item in info)
                {
                    StringReader reader = new StringReader(item.FullName);
                    var journal = Utills.GetJournalDatatable(reader.ReadLine());
                    Utills.SendUniqueFileToTableJournal(journal);
                }
            }
            if (Directory.Exists(path2018)) 
            {
                var js = new DirectoryInfo(path2018);

                var files = js.GetFiles();

                List<FileInfo> info = new List<FileInfo>();
                foreach (var item in files)
                {
                    if (item.Name.EndsWith(".txt") && item.Name.StartsWith("journal"))
                    {
                        info.Add(item);
                    }
                }
                foreach (var item in info)
                {
                    var journal = Utills.GetJournalDatatable(item.FullName);
                    Utills.SendUniqueFileToTableJournal(journal);
                }
            }
          
           




        }
    }
}
