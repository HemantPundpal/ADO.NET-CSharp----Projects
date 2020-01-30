using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace SQLDataAdapterinCSharp
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using(SqlConnection connection1 = new SqlConnection(ConnectionString))
            {
                SqlDataAdapter dataAdapter1 = new SqlDataAdapter("SELECT * FROM Students", connection1);
                DataSet dataSet1 = new DataSet();

                dataAdapter1.Fill(dataSet1); // Fill method, opens the connection, executes the command, fills the DataSet and then immediatly closes the connection. All in one.
                GridView1.DataSource = dataSet1; // this SqlDataSet is available with no active open connection to data source. Unlike the SqlDataReader, which needs active open connection to data source.
                GridView1.DataBind();
            }

            using (SqlConnection connection2 = new SqlConnection(ConnectionString))
            {
                SqlDataAdapter dataAdapter2 = new SqlDataAdapter("spGetStudentsAndEmails", connection2); // This stored procedure returns two datasets - Students and Emails.
                dataAdapter2.SelectCommand.CommandType = CommandType.StoredProcedure;

                DataSet dataSet2 = new DataSet();
                dataAdapter2.Fill(dataSet2); // Fill method, opens the connection, executes the command, fills the DataSet and then immediatly closes the connection. All in one.

                dataSet2.Tables[0].TableName = "Students";
                dataSet2.Tables[1].TableName = "Emails";

                GrdvStudents.DataSource = dataSet2.Tables["Students"]; //First DataSet.
                GrdvStudents.DataBind();
                GrdvEmails.DataSource = dataSet2.Tables["Emails"]; //Second DataSet 
                GrdvEmails.DataBind();
            }
        }

        protected void BtnGetStudent_Click(object sender, EventArgs e)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using (SqlConnection connection3 = new SqlConnection(ConnectionString))
            {
                SqlDataAdapter dataAdapter3 = new SqlDataAdapter("spGetStudentById", connection3); // This stored procedure returns two datasets - Students and Emails.
                dataAdapter3.SelectCommand.CommandType = CommandType.StoredProcedure;
                dataAdapter3.SelectCommand.Parameters.AddWithValue("@StudentId", TbxStudentId.Text);

                DataSet dataSet3 = new DataSet();
                dataAdapter3.Fill(dataSet3); // Fill method, opens the connection, executes the command, fills the DataSet and then immediatly closes the connection. All in one.

                GrdvStudentsbyId.DataSource = dataSet3;
                GrdvStudentsbyId.DataBind();
            }
        }
    }
}