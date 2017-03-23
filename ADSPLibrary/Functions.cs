using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ADSPLibrary
{
    public static class Functions
    {
        public static object FindComponent(Control SearchParent, string SearchName)
        {
            Control[] pics;
            try
            {
                pics = SearchParent.Controls.Find(SearchName, true);
            }
            catch
            {
                pics = new Control[0];
            }
            if (pics.Length != 0) { return pics[0]; } else return null;
        }
    }
}
