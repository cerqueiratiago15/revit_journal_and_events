using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UploadToDBConsole
{
    class actionGroup
    {
        public List<action> ListOfActions { get; }

        internal string Description { get; }
        internal string Actions { get; }
        public string EditMode { get; }
        public string Command { get; }
        public string CT { get; }

        public actionGroup(string groupString)
        {
            //Initiate empty list of actions
            this.ListOfActions = new List<action>();

            int indexEndBuild = groupString.IndexOf("<<End build CT>>") == -1 ? -1 : groupString.IndexOf("<<End build CT>>") + 16;
            this.Description = groupString.Substring(0, indexEndBuild == -1 ? 0 : indexEndBuild);
            this.Actions = groupString.Substring(indexEndBuild == -1 ? 0 : indexEndBuild);

            //get description properties
            this.EditMode = string.Empty;
            this.Command = string.Empty;
            this.CT = string.Empty;

            int indexEditModeStart = this.Description.IndexOf("Edit mode:") == -1 ? -1 : this.Description.IndexOf("Edit mode:") + 10;
            if (indexEditModeStart != -1)
            {
                int indexEditModeEnd = this.Description.IndexOf("\r\n", indexEditModeStart) == -1 ? -1 : this.Description.IndexOf("\r\n", indexEditModeStart);
                if (indexEditModeEnd != -1)
                {
                    this.EditMode = this.Description.Substring(indexEditModeStart, indexEditModeEnd - indexEditModeStart).Trim();
                }

            }

            int indexCommandStart = this.Description.IndexOf("Command:") == -1 ? -1 : this.Description.IndexOf("Command:") + 8;
            if (indexCommandStart != -1)
            {
                int indexCommandEnd = this.Description.IndexOf("\r\n", indexCommandStart) == -1 ? -1 : this.Description.IndexOf("\r\n", indexCommandStart);
                if (indexCommandEnd != -1)
                {
                    this.Command = this.Description.Substring(indexCommandStart, indexCommandEnd - indexCommandStart).Trim();
                }

            }

            int indexCTStart = this.Description.IndexOf("'CT [") == -1 ? -1 : this.Description.IndexOf("'CT [") + 5;
            if (indexCTStart != -1)
            {
                int indexCTEnd = this.Description.IndexOf("]", indexCTStart) == -1 ? -1 : this.Description.IndexOf("]", indexCTStart);
                if (indexCTEnd != -1)
                {
                    this.Command = this.Description.Substring(indexCTStart, indexCTEnd - indexCTStart).Trim();
                }

            }

            //GetHashCode List of actions 
            List<string> ActionStartId = new List<string>(new string[] { @"'H ", @"'E ", @"'C " });

            List<int> allIndexes = new List<int>();
            foreach (string start in ActionStartId)
            {
                allIndexes = allIndexes.Union(Regex.Matches(this.Actions, start).Cast<Match>().Select(m => m.Index)).ToList();
            }

            if (allIndexes.Any())
            {
                allIndexes = allIndexes.OrderBy(o => o).ToList();

                int currentIndex = 0;
                while (currentIndex < allIndexes.Count())
                {
                    if (currentIndex == allIndexes.Count() - 1)
                    {
                        ListOfActions.Add(new action(this.Actions.Substring(allIndexes.ElementAt(currentIndex))));
                        break;
                    }
                    ListOfActions.Add(new action(this.Actions.Substring(allIndexes.ElementAt(currentIndex), allIndexes.ElementAt(currentIndex + 1) - allIndexes.ElementAt(currentIndex))));
                    currentIndex++;
                }
            }
            else
            {
                ListOfActions.Add(new action(this.Actions));
            }

        }
    }
}
