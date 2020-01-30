using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace ConnectionStringForADO.NETinCSharp
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //string ConnectionString = "data source = .; database = MyNewDatabase2; integrated security = SSPI";
            string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString; // gets connection string from Web.config file.
            using (SqlConnection con1 = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd1 = new SqlCommand("SELECT * FROM Students", con1);
                SqlCommand cmd2 = new SqlCommand("SELECT COUNT (Id) from [Students]", con1);
                SqlCommand cmd3 = new SqlCommand("INSERT INTO Students (FirstName, LastName) VALUES ('Will', 'Simp')", con1);
                SqlCommand cmd4 = new SqlCommand("INSERT INTO Emails (Email, StudentId) VALUES ('Will.Simp@xyz.com', 5)", con1);
                con1.Open();

                cmd3.ExecuteNonQuery(); // ExecureNonQuery is used to updated, instert, delete the db tables.
                cmd4.ExecuteNonQuery();

                SqlDataReader rdr1 = cmd1.ExecuteReader(); //Execute Reader is used when multiple rows of infomration is retrived from data base.
                GridView1.DataSource = rdr1;
                GridView1.DataBind();
                rdr1.Close();

                int StudentCount = (int)cmd2.ExecuteScalar(); //ExecuteScaler is used for single return value. Returns a object type, but should be converted as per received value.
                Response.Write($"Total Students = {StudentCount}");
            }
        }
    }
}