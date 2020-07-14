using System.Data;
using System.Text;

namespace Helper
{
    /// <summary>
    /// Helpes handling json format
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// Datatable to json
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get tree json
        /// Need data column "id", "parent"
        /// DataTable must be sorted with "tree-order"
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetTreeJson(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{ \"tree\" : [");            
            GetInnerTree(dt, ref sb, 0, 0);
            sb.Append("]}");
            return sb.ToString();
        }

        private static int GetInnerTree(DataTable dt
            , ref StringBuilder sb, int position, int parent)
        {
            int i;
            int j = 0;

            for (i = position; i < dt.Rows.Count; i++)
            {
                if ((int)dt.Rows[i]["Parent"] == parent)
                {
                    j++;
                    sb.Append("{");
                    foreach (DataColumn c in dt.Columns)
                    {
                        sb.Append("\"" + c.ColumnName + "\" : " + "\"" + dt.Rows[i][c] + "\",");
                    }
                    
                    sb.Append("\"child\" : [");
                    if (GetInnerTree(dt, ref sb, i + 1, (int)dt.Rows[i]["id"]) > 0)
                    {
                        sb.Append("]");
                    }
                    else
                    {
                        sb.Remove(sb.Length - 11, 11);
                    }                    
                    sb.Append("},");
                }
            }
            sb.Remove(sb.Length - 1, 1);
            return j;
        }
    }
}
