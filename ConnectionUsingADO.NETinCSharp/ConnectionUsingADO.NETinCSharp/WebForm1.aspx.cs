using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace ConnectionUsingADO.NETinCSharp
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string ConnectionString = "data source = .; database = MyNewDatabase2; integrated security = SSPI";
            SqlConnection con1 = new SqlConnection(ConnectionString);

            try
            {
                SqlCommand cmd1 = new SqlCommand("SELECT * FROM Students", con1);
                con1.Open();
                GridView1.DataSource = cmd1.ExecuteReader();
                GridView1.DataBind();
                con1.Close();
            }
            catch
            {
                // exception handling and logging
            }
            finally
            {
                if (con1 != null)
                {
                    con1.Close(); // Close the connection safely incase of exception.
                }
            }

            /* OR */
            using (SqlConnection con2 = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd2 = new SqlCommand("SELECT * FROM Students", con2);
                con2.Open();
                GridView2.DataSource = cmd2.ExecuteReader();
                GridView2.DataBind();
            } // Connection is closed safely at the closing of the using block.
        }
    }
}