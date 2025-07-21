using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GTA2ModManager48
{
    class CustomSorter : IComparer
    {
        private bool descending;

        public CustomSorter(bool descending = false)
        {
            this.descending = descending;
        }

        public int Compare(object x, object y)
        {
            ListViewItem X = ((ListViewItem)x);
            ListViewItem Y = ((ListViewItem)y);

            if (X.SubItems[1].Text == "No Mods/Unload Mods")
            {
                return -1;
            }else if  (((ListViewItem)y).SubItems[1].Text == "No Mods/Unload Mods")
            {
                return 1;
            }

            if (X.ImageKey == "heart" &&  Y.ImageKey != "heart")
            {
                return -1;
            }
            else if(Y.ImageKey == "heart" && X.ImageKey != "heart")
            {
                return 1;
            }

           


            string a = X.SubItems[1].Text;
            string b = Y.SubItems[1].Text;
            int result = String.Compare(a, b);
            return descending ? -result : result;
        }
    }
}
