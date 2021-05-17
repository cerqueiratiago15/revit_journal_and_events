using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadToDBConsole
{
    class action
    {
        #region Properties

        public string Time { get; }
        public char Type { get; }
        public string Jrn { get; }
        public string Description { get; }
        public string FullLine { get; }

        public action(string line)
        {
            int currentIndex = 0;
            this.Time = string.Empty;
            this.Jrn = string.Empty;
            this.Description = string.Empty;
            this.FullLine = line;


            //Set Command Type
            switch (line.Substring(0, 2))
            {
                case @"'C":
                    this.Type = 'C';
                    currentIndex = 2;
                    break;
                case @"'H":
                    this.Type = 'H';
                    currentIndex = 2;
                    break;
                case @"'E":
                    this.Type = 'E';
                    currentIndex = 2;
                    break;
                default:
                    this.Type = 'N';
                    currentIndex = 0;
                    break;
            }

            int nextEnd = 0;

            if (!this.Type.Equals('N'))
            {
                nextEnd = line.IndexOf(';', currentIndex);
                this.Time = line.Substring(currentIndex, nextEnd - currentIndex).Trim();
                currentIndex = nextEnd + 1;

            }

            int jrnIndex = line.IndexOf("Jrn.", currentIndex);

            if (jrnIndex != -1)
            {
                nextEnd = line.IndexOf(" ", jrnIndex);
                this.Jrn = line.Substring(jrnIndex + 4, nextEnd - jrnIndex - 4).TrimEnd(new char[] { ']', ' ' });
                currentIndex = nextEnd + 1;
            }

            nextEnd = currentIndex;
            while (nextEnd < line.Length)
            {
                if (nextEnd == line.Length - 1)
                {
                    this.Description = line.Substring(currentIndex).Trim().Replace("_\r\n", string.Empty);
                }
                nextEnd = line.IndexOf("\r\n", nextEnd + 4);
                if (nextEnd==-1)
                {
                    this.Description = line.Substring(currentIndex).Trim().Replace("_\r\n", string.Empty);
                    break;
                }
                 if (!line.ElementAt(nextEnd - 1).Equals('_'))
                {
                    this.Description = line.Substring(currentIndex, nextEnd - currentIndex).Trim().Replace("_\r\n", string.Empty);
                    break;
                }
            }
        }
        #endregion
    }
}
