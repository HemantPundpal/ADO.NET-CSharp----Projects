using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQLDataReaderinCSharp
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using (SqlConnection connection1 = new SqlConnection(ConnectionString))
            {
                connection1.Open();
                SqlCommand command1 = new SqlCommand("SELECT * FROM students", connection1);
                using (SqlDataReader rdr1 = command1.ExecuteReader()) //Note that new cannot be used to create a SqlDataReader object. ExecuteReader return object of SqlDataReader.
                {
                    DataTable table = new DataTable();
                    table.Columns.Add("ID");
                    table.Columns.Add("FirstName");
                    table.Columns.Add("LastName");
                    table.Columns.Add("FullName");

                    while (rdr1.Read())
                    {
                        DataRow dataRow = table.NewRow();

                        dataRow["ID"] = rdr1["Id"];
                        dataRow["FirstName"] = rdr1["FirstName"];
                        dataRow["LastName"] = rdr1["LastName"];
                        dataRow["FullName"] = rdr1["FirstName"] + " " + rdr1["LastName"];
                        table.Rows.Add(dataRow);

                    }
                    GridView1.DataSource = table;
                    GridView1.DataBind();
                }

                SqlCommand command2 = new SqlCommand("SELECT * FROM students; SELECT * FROM Emails", connection1); // there are two SQL queries that return two seperate data sets.
                using(SqlDataReader rdr2 = command2.ExecuteReader()) // both data sets are pointed by a single DataReader object.
                {
                    GrdvStudents.DataSource = rdr2; //first result set displayed.
                    GrdvStudents.DataBind(); 

                    rdr2.NextResult(); //this will make the result move to the next result object.

                    GrdvEmails.DataSource = rdr2; //second result set displayed.
                    GrdvEmails.DataBind();
                }
            }
        }
    }
}