using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UploadToDBConsole
{
    class ListGroups
    {
        public List<actionGroup> ListOfGroups { get; }
        public string FullPath { get; }
        public string FullString { get; }

        public ListGroups(string path)
        {
            //Set empty values for properties
            this.ListOfGroups = new List<actionGroup>();
            this.FullPath = path;

            //Read the string from file path and check if it is empty
            this.FullString = Utills.getFullString(path);
            if (!String.IsNullOrEmpty(this.FullString))
            {
                List<int> allGroupMarkers = new List<int>(Regex.Matches(this.FullString, "<<Begin build CT>>").Cast<Match>().Select(m => m.Index).ToList());

                //Add 0 at beginning if it does not exist
                if (allGroupMarkers.Count() != 0)
                {
                    int currentIndex = 0;
                    while (currentIndex < allGroupMarkers.Count())
                    {
                        if (currentIndex == allGroupMarkers.Count() - 1)
                        {
                            this.ListOfGroups.Add(new actionGroup(this.FullString.Substring(allGroupMarkers.ElementAt(currentIndex) + 18)));
                            break;
                        }
                        this.ListOfGroups.Add(new actionGroup(this.FullString.Substring(allGroupMarkers.ElementAt(currentIndex) + 18, allGroupMarkers.ElementAt(currentIndex + 1) - 18 - allGroupMarkers.ElementAt(currentIndex))));
                        currentIndex++;
                    }
                }

                int x = 1;
            }
        }
    }
}
