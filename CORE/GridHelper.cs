using System.Data;
using System.Text;
using System.Xml;

namespace CORE
{
    /// <summary>
    /// Helpes handling html table, json etc..
    /// </summary>
    public static class GridHelper
    {
        /// <summary>
        /// Datatable to html table
        /// </summary>
        /// <param name="datatable">Datatable</param>
        /// <param name="style">Table style class name</param>
        /// <param name="header">Draw header to column name</param>
        /// <returns>Html string</returns>
        public static string GetHtmlTable(DataTable datatable, string style, bool header = true)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<table class='" + style + "'>");

            if (header)
            {
                sb.Append("<tr>");
                foreach (DataColumn col in datatable.Columns)
                {
                    sb.Append("<th>" + col.ColumnName + "</th>");
                }
                sb.Append("</tr>");
            }

            foreach (DataRow row in datatable.Rows)
            {
                sb.Append("<tr>");
                foreach (DataColumn col in datatable.Columns)
                {
                    sb.Append("<td>" + row[col] + "</td>");
                }
                sb.Append("</tr>");
            }

            sb.Append("</table>");
            return sb.ToString();
        }

        public static string GetJson(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{ \"table\" : [");

            foreach (DataRow r in dt.Rows)
            {
                sb.Append("{");
                foreach (DataColumn c in dt.Columns)
                {
                    sb.Append("\"" + c.ColumnName + "\" : " + "\"" + r[c] + "\",");
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("},");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("]}");
            return sb.ToString();
        }

        public static string GetTreeJson(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{ \"tree\" : [");
            GetInnerTree(dt, ref sb, 0, 0);
            sb.Append("]}");
            return sb.ToString();
        }

        public static int GetInnerTree(DataTable dt, ref StringBuilder sb, int position, int parent = 0)
        {
            int i;
            int j = 0;

            for (i = position; i < dt.Rows.Count; i++)
            {
                if (int.Parse(dt.Rows[i]["parent"].ToString()) != parent) continue;
                j++;
                sb.Append("{");
                foreach (DataColumn c in dt.Columns)
                {
                    sb.Append("\"" + c.ColumnName + "\" : " + "\"" + dt.Rows[i][c] + "\",");
                }

                if (dt.Rows[i]["Type"].ToString() == "Group")
                {
                    sb.Append("\"child\" : [");
                    if (GetInnerTree(dt, ref sb, i + 1, int.Parse(dt.Rows[i]["id"].ToString())) > 0)
                        sb.Append("]");
                    else sb.Remove(sb.Length - 11, 11);                    
                }
                else //User
                {
                    sb.Remove(sb.Length - 1, 1);
                }
                sb.Append("},");
            }
            sb.Remove(sb.Length - 1, 1);
            return j;
        }
    }
}
