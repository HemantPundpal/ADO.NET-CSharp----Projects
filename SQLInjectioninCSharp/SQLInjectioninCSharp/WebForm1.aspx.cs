using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQLInjectioninCSharp
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void BtnGetStudents_Click1(object sender, EventArgs e)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                //SqlCommand cmd = new SqlCommand("SELECT * FROM Students WHERE FirstName LIKE '" + TbxGetStudents.Text + "%'", connection); // Avoid string apending to make sql commands, possible sql injection attack.
                SqlCommand cmd = new SqlCommand("SELECT * FROM Students WHERE FirstName LIKE @ProductName", connection); // Use paramatarized sql commands to avoid possible sql injection attack.
                cmd.Parameters.AddWithValue("@Productname", TbxGetStudents.Text + "%");
                connection.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                GridView1.DataSource = rdr;
                GridView1.DataBind();
                rdr.Close();
            }
        }
    }
}