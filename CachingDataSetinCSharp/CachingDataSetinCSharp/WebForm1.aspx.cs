using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CachingDataSetinCSharp
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void BtnLoadData_Click(object sender, EventArgs e)
        {
            if (Cache["Data"] == null)
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
                using (SqlConnection connection2 = new SqlConnection(ConnectionString))
                {
                    SqlDataAdapter dataAdapter2 = new SqlDataAdapter("spGetStudentsAndEmails", connection2); // This stored procedure returns two datasets - Students and Emails.
                    dataAdapter2.SelectCommand.CommandType = CommandType.StoredProcedure;

                    DataSet dataSet2 = new DataSet();
                    dataAdapter2.Fill(dataSet2); // Fill method, opens the connection, executes the command, fills the DataSet and then immediatly closes the connection. All in one.

                    dataSet2.Tables[0].TableName = "Students";
                    dataSet2.Tables[1].TableName = "Emails";

                    Cache["Data"] = dataSet2; // Cache is a global object. You can label your DataSet in the Cashe using a name (eg "Data").
                                                // Every DataSet Stored in the Cache with a label is kept seperately and retrived with the help of label.

                    GrdvStudents.DataSource = dataSet2.Tables["Students"]; //First DataSet.
                    GrdvStudents.DataBind();
                    GrdvEmails.DataSource = dataSet2.Tables["Emails"]; //Second DataSet 
                    GrdvEmails.DataBind();

                    LblMessage.Text = "Data Loaded from the Database";
                }
            }
            else
            {
                DataSet dataSet1 = (DataSet) Cache["Data"]; // DateSet retrived with the help of the label, should be typecasted to the object type of the data.
                GrdvStudents.DataSource = dataSet1.Tables["Students"]; //First DataSet.
                GrdvStudents.DataBind();
                GrdvEmails.DataSource = dataSet1.Tables["Emails"]; //Second DataSet 
                GrdvEmails.DataBind();

                LblMessage.Text = "Data Loaded from the Cache";
            }
        }

        protected void btnClearCache_Click(object sender, EventArgs e)
        {
            if(Cache["Data"] != null)
            {
                Cache.Remove("Data"); // Cached DataSet can be removed, again using the specific label of the DataSet.
                LblMessage.Text = "Data is removed from the cache";
            }
            else
            {
                LblMessage.Text = "There is nothing in the Cache to be removed.";
            }
        }
    }
}