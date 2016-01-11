using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;

namespace Druware.Extensions.Windows.Forms
{

    class ListViewExtensionPrintObject
    {
        private Int32 currentRow;
        private int pageCount;
        private Font printFont;
        private ListView listView;
        private Dictionary<int, int> columns;

        public ListViewExtensionPrintObject(ListView lv)
        {
            printFont = new Font("Courier New", 8);
            listView = lv;
            currentRow = 0;
            pageCount = 1;
            columns = new Dictionary<int, int>();
        }

        public void Print()
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);
            Margins margins = new Margins(5, 5, 25, 25);
            pd.DefaultPageSettings.Margins = margins;

            for (int i = 0; i < listView.Columns.Count; i++)
            {
                columns.Add(i, 0);
            }

            // find the max length of each column
            foreach (ListViewItem lvi in listView.Items)
            {
                for (int x = 0; x < lvi.SubItems.Count; x++)
                {
                    if (lvi.SubItems[x].Text.Length > columns[x])
                    {
                        columns[x] = lvi.SubItems[x].Text.Trim().Length;
                    }
                }
            }

            // now scale the font to see if it will fit. smallest is 6pt.
            int lineWidth = 0;
            for (int i = 0; i < columns.Count; i++)
            {
                lineWidth = lineWidth + columns[i] + 1;
            } // total line character count

            if (lineWidth > 118)
            {
                pd.DefaultPageSettings.Landscape = true;
            }

            pd.Print();
        }

        private void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            float linesPerPage = 0;
            float yPos = 0;
            int count = 0;
            float leftMargin = ev.MarginBounds.Left;
            float topMargin = ev.MarginBounds.Top;
            float rightMargin = ev.MarginBounds.Right;

            // Convert the List View Name to something Pretty
            string title = listView.Name.Replace("_", " ");
            title = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title);

            Size textSize = TextRenderer.MeasureText(title, printFont);

            int titleWidth = textSize.Width;
            ev.Graphics.DrawString(
                title,
                printFont,
                Brushes.Black,
                (ev.MarginBounds.Width / 2) - (textSize.Width / 2),
                topMargin - (Convert.ToInt16(printFont.GetHeight(ev.Graphics) * 1.5)),
                new StringFormat());


            // print a header.
            string s = "";
            for (int i = 0; i < listView.Columns.Count; i++)
            {
                if (listView.Columns[i].Text.Length <= columns[i])
                {
                    s += listView.Columns[i].Text.PadRight(columns[i] + 1);
                }
                else
                {
                    s += listView.Columns[i].Text.Substring(0, columns[i]).PadRight(columns[i] + 1);
                }
            }
            yPos = topMargin;
            ev.Graphics.DrawString(s, printFont, Brushes.Black,
                leftMargin, yPos, new StringFormat());
            yPos = yPos + Convert.ToInt16(printFont.GetHeight(ev.Graphics) * 1.2);
            // draw an underline
            ev.Graphics.DrawLine(Pens.Black,
                new Point(Convert.ToInt16(leftMargin), Convert.ToInt16(yPos)),
                new Point(Convert.ToInt16(rightMargin), Convert.ToInt16(yPos)));
            yPos = yPos + Convert.ToInt16(printFont.GetHeight(ev.Graphics) * .2);

            // Calculate the number of lines per page.
            linesPerPage = (ev.MarginBounds.Height /
               printFont.GetHeight(ev.Graphics)) - 6;

            // Iterate over the file, printing each line.
            for (count = 0; count + currentRow < listView.Items.Count; count++)
            {
                if (count >= linesPerPage) { break; }
                ListViewItem lvi = listView.Items[count + currentRow];

                // This needs to be smarter about figuring out column widths, but for the moment,
                // it just spits it out to the printer.

                string st = "";
                for (int x = 0; x < lvi.SubItems.Count; x++)
                {
                    st += lvi.SubItems[x].Text.PadRight(1 + columns[x]);
                }
                ev.Graphics.DrawString(st, printFont, Brushes.Black,
                   leftMargin, yPos, new StringFormat());
                yPos = yPos + printFont.GetHeight(ev.Graphics);
            }
            currentRow = currentRow + count;

            string page = "Page " + pageCount.ToString();
            Size pageSize = TextRenderer.MeasureText(page, printFont);

            yPos = yPos + printFont.GetHeight(ev.Graphics);
            ev.Graphics.DrawString(
                page,
                printFont,
                Brushes.Black,
                (ev.MarginBounds.Width / 2) - (pageSize.Width / 2),
                yPos,
                new StringFormat());

            // If more lines exist, print another page.
            if (currentRow < listView.Items.Count)
                ev.HasMorePages = true;
            else
                ev.HasMorePages = false;
            pageCount++;
        }

    }

    public static class ListViewExtension
    {

        public static void ExportToCsv(this ListView lv, string toFileName)
        {
            // Write the string array to a new file named "WriteLines.txt".
            using (StreamWriter outputFile = new StreamWriter(toFileName))
            {
                foreach (ListViewItem i in lv.Items)
                {
                    for (int x = 0; x < i.SubItems.Count; x++)
                    {
                        string s = i.SubItems[x].Text;
                        outputFile.Write("\"" + s + "\"");
                        if (x < i.SubItems.Count - 1)
                        {
                            outputFile.Write(",");
                        }
                    }
                    outputFile.WriteLine("");
                }
            }
        }

        public static void PrintToDefaultPrinter(this ListView lv)
        {
            ListViewExtensionPrintObject lvp = new ListViewExtensionPrintObject(lv);
            lvp.Print();
        }

    }

}
