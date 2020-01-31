using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Caching;

namespace DisconnectedDataAccessUsingDataSetinCSharp
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// This function is responsible for - 
        ///     - Seting up the connection to the DB
        ///     - Using SqlDataAdapter to fetch the Students and Student Emails DataSet
        ///     - Cashe the DataSet for ofline use (disconnected from DB)
        ///     - Bind the Data Set to the GridView
        /// </summary>

        private void GetDataFromDB()
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using (SqlConnection connection1 = new SqlConnection(ConnectionString))
            {
                // First set the Query in the SqlDataAdapter.
                string strSelectQuery = "SELECT * FROM [Students]; SELECT * FROM [Emails]";
                SqlDataAdapter dataAdapter1 = new SqlDataAdapter(strSelectQuery, connection1);

                // Create DataSet to hold the data set returned by the SqlDataAdapter.
                DataSet dataSet1 = new DataSet();
                dataAdapter1.Fill(dataSet1);

                // It is always good to name the tables with approtriate names to make the code more readable.
                dataSet1.Tables[0].TableName = "Students";
                dataSet1.Tables[1].TableName = "Emails";

                // Identify and mark the Primarykey data column in the tables.
                // When you identify the data column as the PrimaryKey, the table automatically sets AllowDBNull property of the column to false.
                // Also sets the Unique property of the data column to true.
                dataSet1.Tables["Students"].PrimaryKey = new DataColumn[] { dataSet1.Tables["Students"].Columns["Id"] };
                dataSet1.Tables["Emails"].PrimaryKey = new DataColumn[] { dataSet1.Tables["Emails"].Columns["Id"] };

                // Identify the student id column in the student emails table as readonly. So that user cannot change it using the edit option in grid view.
                dataSet1.Tables["Emails"].Columns["StudentId"].AllowDBNull = false;
                dataSet1.Tables["Emails"].Columns["StudentId"].ReadOnly = true;

                // DataSet is cached for 24 hours in the memory. with label "DATASET"
                Cache.Insert("DATASET", dataSet1, null, DateTime.Now.AddHours(24), System.Web.Caching.Cache.NoSlidingExpiration);

                // Set the data source and bind the data set for the Grid view control.
                GrdvStudents.DataSource = dataSet1.Tables["Students"]; //First DataSet 
                GrdvStudents.DataBind();
                GrdvStudentEmails.DataSource = dataSet1.Tables["Emails"]; //Second DataSet 
                GrdvStudentEmails.DataBind();
            }
        }

        /// <summary>
        /// This function bind the Cached Students and Student Emails dataset.
        /// </summary>        
        private void GetDataFromCache()
        {
            if (Cache["DATASET"] != null)
            {
                DataSet dataSet1 = (DataSet)Cache["DATASET"];

                GrdvStudents.DataSource = dataSet1.Tables["Students"]; //First DataSet 
                GrdvStudents.DataBind();
                GrdvStudentEmails.DataSource = dataSet1.Tables["Emails"]; //Second DataSet 
                GrdvStudentEmails.DataBind();
            }
        }

        /// <summary>
        /// Get Students and Student Email data set from DB button click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnGetDataFromDB_Click(object sender, EventArgs e)
        {
            // Load student and Student Email data set from the Database.
            GetDataFromDB();
            LblMessage.Text = "Data Loaded from Database";
        }

        /// <summary>
        /// Students GridView row edit event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GrdvStudents_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GrdvStudents.EditIndex = e.NewEditIndex;
            GetDataFromCache();
            LblMessage.Text = "Make changes to the data in Cache.";
        }

        /// <summary>
        /// Students GridView cancel editing event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GrdvStudents_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GrdvStudents.EditIndex = -1;
            GetDataFromCache();
            LblMessage.Text = string.Empty;
        }

        /// <summary>
        /// Update event for the edited row in the Students GridView to the cached data set.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GrdvStudents_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            if (Cache["DATASET"] != null)
            {
                // Get the cached data set and update the row modified in the cached data set.
                DataSet dataSet1 = (DataSet)Cache["DATASET"];
                DataRow dataRow1 = dataSet1.Tables["Students"].Rows.Find(e.Keys["Id"]); // locate the row modified.
                dataRow1["FirstName"] = e.NewValues["FirstName"];
                dataRow1["LastName"] = e.NewValues["LastName"];

                Cache.Insert("DATASET", dataSet1, null, DateTime.Now.AddHours(24), System.Web.Caching.Cache.NoSlidingExpiration); // save to the cache again.
                GrdvStudents.EditIndex = -1;
                GetDataFromCache(); // refresh the veiw.
                LblMessage.Text = "Changes made to the data in Cache.";
            }
        }

        /// <summary>
        /// Delete event from the Students GridView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GrdvStudents_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (Cache["DATASET"] != null)
            {
                // Get the cached data set and delete the row in the cached data set.
                DataSet dataSet1 = (DataSet)Cache["DATASET"];
                DataRow dataRow1 = dataSet1.Tables["Students"].Rows.Find(e.Keys["Id"]); // locate the row for deleting
                dataRow1.Delete();

                Cache.Insert("DATASET", dataSet1, null, DateTime.Now.AddHours(24), System.Web.Caching.Cache.NoSlidingExpiration); // save the cache again.
                GrdvStudents.EditIndex = -1;
                GetDataFromCache(); // refresh the veiw.
                LblMessage.Text = "Row deleted from cache.";
            }
        }

        /// <summary>
        /// Student Emails GridView row edit event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GrdvStudentEmails_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GrdvStudentEmails.EditIndex = e.NewEditIndex;
            GetDataFromCache();
            LblMessage.Text = "Make changes to the data in Cache.";
        }

        /// <summary>
        /// Student Emails GridView cancel editing event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GrdvStudentEmails_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GrdvStudentEmails.EditIndex = -1;
            GetDataFromCache();
            LblMessage.Text = string.Empty;
        }

        /// <summary>
        /// Update event for the edited row in the Student Emails GridView to the cached data set.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GrdvStudentEmails_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            if (Cache["DATASET"] != null)
            {
                // Get the cached data set and update the row modified in the cached data set.
                DataSet dataSet1 = (DataSet)Cache["DATASET"];
                DataRow dataRow1 = dataSet1.Tables["Emails"].Rows.Find(e.Keys["Id"]); // locate the row modified.
                dataRow1["Email"] = e.NewValues["Email"];
                //dataRow1["StudentId"] = e.NewValues["StudentId"];

                Cache.Insert("DATASET", dataSet1, null, DateTime.Now.AddHours(24), System.Web.Caching.Cache.NoSlidingExpiration); // save to the cache again.
                GrdvStudentEmails.EditIndex = -1;
                GetDataFromCache(); // refresh the veiw.
                LblMessage.Text = "Changes made to the data in Cache.";
            }
        }

        /// <summary>
        /// Delete event from the Student Emails GridView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GrdvStudentEmails_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (Cache["DATASET"] != null)
            {
                // Get the cached data set and delete the row in the cached data set.
                DataSet dataSet1 = (DataSet)Cache["DATASET"];
                DataRow dataRow1 = dataSet1.Tables["Emails"].Rows.Find(e.Keys["Id"]); // locate the row for deleting
                dataRow1.Delete();

                Cache.Insert("DATASET", dataSet1, null, DateTime.Now.AddHours(24), System.Web.Caching.Cache.NoSlidingExpiration); // save the cache again.
                GrdvStudentEmails.EditIndex = -1;
                GetDataFromCache(); // refresh the veiw.
                LblMessage.Text = "Row deleted from cache.";
            }
        }

        /// <summary>
        /// This function updated the (updated) cached data set back to the DB tables - Students and Student Emails.
        ///     - Seting up the connection to the DB
        ///     - Using SqlDataAdapter to fetch the Student and Student Emails DataSet
        ///     - Construct the update and delete commands for the Students and Student Email database tables in the database
        ///     - Execute the update and delete commands on Students and Student Emails tables in the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnUpdateDB_Click(object sender, EventArgs e)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using (SqlConnection connection1 = new SqlConnection(ConnectionString))
            {
                // First fetch the Students and Student Email tables from the database.
                string strSelectStudentsQuery = "SELECT * FROM [Students]";
                string strSelectStudentEmailsQuery = "SELECT * FROM [Emails]";
                SqlDataAdapter dataAdapterStudents = new SqlDataAdapter(strSelectStudentsQuery, connection1);
                SqlDataAdapter dataAdapterStudentEmails = new SqlDataAdapter(strSelectStudentEmailsQuery, connection1);


                // Get the Cached data set for Students and Student Emails.
                DataSet dataSet1 = (DataSet)Cache["DATASET"];


                // Construct the Update command for Students table.
                string strUpdateStudents = "UPDATE [Students] set FirstName = @FirstName, LastName = @LastName WHERE Id = @Id";
                SqlCommand updateStudentscommand = new SqlCommand(strUpdateStudents, connection1);
                updateStudentscommand.Parameters.Add("@FirstName", SqlDbType.NVarChar, 100, "FirstName");
                updateStudentscommand.Parameters.Add("@LastName", SqlDbType.NVarChar, 100, "LastName");
                updateStudentscommand.Parameters.Add("@Id", SqlDbType.Int, 0, "Id");
                dataAdapterStudents.UpdateCommand = updateStudentscommand;

                // Construct the Delete command for the students table.
                string strDeleteStudents = "DELETE FROM [Students] WHERE Id = @Id";
                SqlCommand deleteStudentscommand = new SqlCommand(strDeleteStudents, connection1);
                deleteStudentscommand.Parameters.Add("@Id", SqlDbType.Int, 0, "Id");
                dataAdapterStudents.DeleteCommand = deleteStudentscommand;


                // Construct the Update command for Student Emails table.
                string strUpdateStudentEmails = "UPDATE [Emails] set Email = @Email, WHERE Id = @Id";
                SqlCommand updateStudentEmailscommand = new SqlCommand(strUpdateStudentEmails, connection1);
                updateStudentEmailscommand.Parameters.Add("@Email", SqlDbType.NVarChar, 100, "Email");
                //updateStudentEmailscommand.Parameters.Add("@StudentId", SqlDbType.Int, 0, "StudentId");
                updateStudentEmailscommand.Parameters.Add("@Id", SqlDbType.Int, 0, "Id");
                dataAdapterStudentEmails.UpdateCommand = updateStudentEmailscommand;

                // Construct the Delete command for Student Emails table.
                string strDeleteStudentEmails = "DELETE FROM [Emails] WHERE Id = @Id";
                SqlCommand deleteStudentEmailscommand = new SqlCommand(strDeleteStudentEmails, connection1);
                deleteStudentEmailscommand.Parameters.Add("@Id", SqlDbType.Int, 0, "Id");
                dataAdapterStudentEmails.DeleteCommand = deleteStudentEmailscommand;

                // Execute the Update and Delete commands on the Email table first. This is because Student ID has dependency on the Student Email entry.
                dataAdapterStudentEmails.Update(dataSet1.Tables["Emails"]);
                dataAdapterStudentEmails.Dispose(); // Release call components.
                // execute the Update and Delete commands on the Students table second.
                dataAdapterStudents.Update(dataSet1.Tables["Students"]);
                dataAdapterStudents.Dispose(); // Release all components.

                LblMessage.Text = "Any chnages made to the Students and StudentEmails updated.";
            }
        }

        /// <summary>
        /// This function adds new student to the Students table and student email id the Student Emails table.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnAddStudent_Click(object sender, EventArgs e)
        {
            if ((TbxEmail.Text != string.Empty) && (TbxFirstName.Text != string.Empty) && (TbxLastName.Text != string.Empty) && (TbxEmail.Text.Contains("@")))
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    // Create a Sqlcommand for calling the stored procedure that adds a student to the student table and returns the student id.
                    SqlCommand addStudent = new SqlCommand("spAddStudent", connection);
                    addStudent.CommandType = System.Data.CommandType.StoredProcedure; // identify as stored procedure.
                    addStudent.Parameters.AddWithValue("@FirstName", TbxFirstName.Text);
                    addStudent.Parameters.AddWithValue("@LastName", TbxLastName.Text);

                    SqlParameter outStudentId = new SqlParameter(); // set the out parameter for the stored procedure.
                    outStudentId.ParameterName = "@StudentId";
                    outStudentId.SqlDbType = System.Data.SqlDbType.Int;
                    outStudentId.Direction = System.Data.ParameterDirection.Output; // identify as output variable of the stored procedure.
                    addStudent.Parameters.Add(outStudentId);

                    // Execute the add student command.
                    connection.Open();
                    addStudent.ExecuteNonQuery();

                    // Add the email id of the student in the Student Email table along with the student Id from Student table.
                    SqlCommand addStudentEmail = new SqlCommand($"INSERT INTO Emails VALUES ('{TbxEmail.Text}', {outStudentId.Value.ToString()})", connection);
                    addStudentEmail.ExecuteNonQuery();

                    LblMessage.Text = "Add new student record with Student Id - " + outStudentId.Value.ToString();

                    // Empty the text feilds.
                    TbxFirstName.Text = string.Empty;
                    TbxLastName.Text = string.Empty;
                    TbxEmail.Text = string.Empty;
                }
            }
            else
            {
                if(!(TbxEmail.Text.Contains("@")))
                {
                    LblMessage.Text = "Enter a valid email Id";
                }
                else
                {
                    LblMessage.Text = "All feilds are required adding a student";
                }
            }
        }

        protected void BtnAddAdditionalEmail_Click(object sender, EventArgs e)
        {
            if ((TbxAdditionalEmail.Text != string.Empty) && (TbxStudentId.Text != string.Empty) && (TbxAdditionalEmail.Text.Contains("@")))
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    // Check if the student ID exist
                    DataSet dataSet1 = (DataSet)Cache["DATASET"];
                    if(dataSet1.Tables["Students"].Rows.Contains(TbxStudentId.Text)) // locate the row for deleting
                    {
                        // Execute the add student command.
                        connection.Open();

                        // Add the email id of the student in the Student Email table along with the student Id from Student table.
                        SqlCommand addStudentEmail = new SqlCommand($"INSERT INTO Emails VALUES ('{TbxAdditionalEmail.Text}', {TbxStudentId.Text})", connection);
                        addStudentEmail.ExecuteNonQuery();

                        LblMessage.Text = "Add new student email to the student record with Student Id - " + TbxStudentId.Text;
                    }
                    else
                    {
                        LblMessage.Text = "Student record not found for Student Id - " + TbxStudentId.Text + "; Additional Email can be only added for existing Student ID in the student record";
                    }

                    // Empty the text feilds.
                    TbxAdditionalEmail.Text = string.Empty;
                    TbxStudentId.Text = string.Empty;
                }
            }
            else
            {
                if (!(TbxAdditionalEmail.Text.Contains("@")))
                {
                    LblMessage.Text = "Enter a valid email Id";
                }
                else
                {
                    LblMessage.Text = "All feilds are required to insert an additional student email";
                }
            }
        }
    }
}