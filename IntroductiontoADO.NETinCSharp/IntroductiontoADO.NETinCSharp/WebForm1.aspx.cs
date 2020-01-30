using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Odbc;

namespace IntroductiontoADO.NETinCSharp
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SqlConnection con1 = new SqlConnection("data source=.; database=MyNewDatabase2; integrated security=SSPI");
            SqlCommand cmd1 = new SqlCommand("INSERT INTO Students (FirstName, LastName) VALUES ('Jim', 'James')", con1);
            SqlCommand cmd2 = new SqlCommand("INSERT INTO Emails (Email, StudentId) VALUES ('Jim.James@xyz.com', 4)", con1);
            SqlCommand cmd3 = new SqlCommand("SELECT * FROM Students", con1);
            SqlCommand cmd4 = new SqlCommand("SELECT * FROM Emails", con1);
            con1.Open();
            cmd1.ExecuteNonQuery();
            cmd2.ExecuteNonQuery();
            SqlDataReader rdr1 = cmd3.ExecuteReader();
            gvStudents.DataSource = rdr1;
            gvStudents.DataBind();
            rdr1.Close();
            rdr1 = cmd4.ExecuteReader();
            gvStudentsEmails.DataSource = rdr1;
            gvStudentsEmails.DataBind();
            con1.Close();
        }
    }
}