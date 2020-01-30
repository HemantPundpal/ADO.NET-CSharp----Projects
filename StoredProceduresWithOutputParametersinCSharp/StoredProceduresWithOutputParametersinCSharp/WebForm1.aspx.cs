using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StoredProceduresWithOutputParametersinCSharp
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void BtnSubmitStudent_Click(object sender, EventArgs e)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using(SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command1 = new SqlCommand("spAddStudent", connection);
                command1.CommandType = System.Data.CommandType.StoredProcedure;

                command1.Parameters.AddWithValue("@FirstName", TbxFirstName.Text);
                command1.Parameters.AddWithValue("@LastName", TbxLastName.Text);

                SqlParameter outputParameter = new SqlParameter();
                outputParameter.ParameterName = "@StudentId";
                outputParameter.SqlDbType = System.Data.SqlDbType.Int;
                outputParameter.Direction = System.Data.ParameterDirection.Output;
                command1.Parameters.Add(outputParameter);

                connection.Open();
                command1.ExecuteNonQuery();

                string StudentId = outputParameter.Value.ToString();

                SqlCommand command2 = new SqlCommand($"INSERT INTO Emails VALUES ('{TbxEmail.Text}', {StudentId})", connection);
                command2.ExecuteNonQuery();

                LMessage.Text = "Student Id " + StudentId;
            }

        }
    }
}