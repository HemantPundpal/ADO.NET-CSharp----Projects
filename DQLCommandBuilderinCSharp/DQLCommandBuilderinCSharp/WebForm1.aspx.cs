using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DQLCommandBuilderinCSharp
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void BtnLoadStudent_Click(object sender, EventArgs e)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using (SqlConnection connection1 = new SqlConnection(ConnectionString))
            {
                string sqlQuery = "SELECT * FROM Students WHERE Id = @StudentId";
                SqlDataAdapter dataAdapter1 = new SqlDataAdapter(sqlQuery, connection1);
                dataAdapter1.SelectCommand.Parameters.AddWithValue("@StudentId", TbxStudentId.Text);

                DataSet dataSet1 = new DataSet();
                dataAdapter1.Fill(dataSet1, "Students");

                ViewState["QUERY"] = sqlQuery;
                ViewState["DATASET"] = dataSet1;

                if(dataSet1.Tables["Students"].Rows.Count > 0)
                {
                    DataRow dr = dataSet1.Tables["Students"].Rows[0];
                    TbxStudentFirstName.Text = dr["FirstName"].ToString();
                    TbxStudentLastName.Text = dr["LastName"].ToString();
                }
                else
                {
                    LblMessage.Text = "No Student with ID - " + TbxStudentId.Text;
                    TbxStudentFirstName.Text = string.Empty;
                    TbxStudentLastName.Text = string.Empty;
                }

                //GrdvStudentsbyId.DataSource = dataSet3;
                //GrdvStudentsbyId.DataBind();
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using (SqlConnection connection1 = new SqlConnection(ConnectionString))
            {
                SqlDataAdapter dataAdapter1 = new SqlDataAdapter((string)ViewState["QUERY"], connection1);
                dataAdapter1.SelectCommand.Parameters.AddWithValue("@StudentId", TbxStudentId.Text);
                SqlCommandBuilder cmdBuilder1 = new SqlCommandBuilder(dataAdapter1); // This binding of dataAdapter with commandBuilder is must for auto dataset update queries to work
                DataSet dataSet1 = (DataSet)ViewState["DATASET"];

                if (dataSet1.Tables["Students"].Rows.Count > 0)
                {
                    DataRow dr = dataSet1.Tables["Students"].Rows[0];
                    dr["FirstName"] = TbxStudentFirstName.Text;
                    dr["LastName"] = TbxStudentLastName.Text;
                }

                int rowsUpdated = dataAdapter1.Update(dataSet1, "Students");

                if (rowsUpdated > 0)
                {
                    LblMessage.Text = $"Student table - {rowsUpdated} row(s) updated.";
                }
                else
                {
                    LblMessage.Text = $"Student table - no row(s) updated.";
                }

                dataAdapter1.SelectCommand.Parameters.AddWithValue("@StudentId", TbxStudentId.Text);
            }
        }
    }
}