using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
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
                dataSet1.Tables["Students"].Columns["Id"].AutoIncrement = true;
                dataSet1.Tables["Emails"].Columns["Id"].AutoIncrement = true;


                // Get the largest primery key
                SqlCommand getLargestStudentsPrimeryKey = new SqlCommand("SELECT MAX(Id) FROM [Students]", connection1);
                SqlCommand getLargestStudentEmailsPrimeryKey = new SqlCommand("SELECT MAX(Id) FROM [Emails]", connection1);
                connection1.Open();
                long studentsTableSeedValue = 0;
                long studentEmailsTableSeedValue = 0;
                long.TryParse(getLargestStudentsPrimeryKey.ExecuteScalar().ToString(), out studentsTableSeedValue);
                long.TryParse(getLargestStudentEmailsPrimeryKey.ExecuteScalar().ToString(), out studentEmailsTableSeedValue);
                dataSet1.Tables["Students"].Columns["Id"].AutoIncrementSeed = ++studentsTableSeedValue;
                dataSet1.Tables["Emails"].Columns["Id"].AutoIncrementSeed = ++studentEmailsTableSeedValue;

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
            if (Cache["DATASET"] != null)
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

                    /* Update - Deleted */
                    // Construct the Delete command for the students table.
                    string strDeleteStudents = "DELETE FROM [Students] WHERE Id = @Id";
                    SqlCommand deleteStudentsCommand = new SqlCommand(strDeleteStudents, connection1);
                    deleteStudentsCommand.Parameters.Add("@Id", SqlDbType.Int, 0, "Id");
                    dataAdapterStudents.DeleteCommand = deleteStudentsCommand;

                    // Construct the Delete command for Student Emails table.
                    string strDeleteStudentEmails = "DELETE FROM [Emails] WHERE Id = @Id";
                    SqlCommand deleteStudentEmailsCommand = new SqlCommand(strDeleteStudentEmails, connection1);
                    deleteStudentEmailsCommand.Parameters.Add("@Id", SqlDbType.Int, 0, "Id");
                    dataAdapterStudentEmails.DeleteCommand = deleteStudentEmailsCommand;

                    // Execute the Delete commands on the Email table first. This is because Student ID has dependency on the Student Email entry.
                    dataAdapterStudentEmails.Update(dataSet1.Tables["Emails"].Select("", "", DataViewRowState.Deleted)); // Select only deleted rows.
                    dataAdapterStudentEmails.Dispose(); // Release all components.
                    // Execute the Delete commands on the Students table second.
                    dataAdapterStudents.Update(dataSet1.Tables["Students"].Select("", "", DataViewRowState.Deleted)); //Select only deleted rows.
                    dataAdapterStudents.Dispose(); // Release all components.

                    /* Update - Modified */
                    // Construct the Update command for Students table.
                    string strUpdateStudents = "UPDATE [Students] set FirstName = @FirstName, LastName = @LastName WHERE Id = @Id";
                    SqlCommand updateStudentsCommand = new SqlCommand(strUpdateStudents, connection1);
                    updateStudentsCommand.Parameters.Add("@FirstName", SqlDbType.NVarChar, 100, "FirstName");
                    updateStudentsCommand.Parameters.Add("@LastName", SqlDbType.NVarChar, 100, "LastName");
                    updateStudentsCommand.Parameters.Add("@Id", SqlDbType.Int, 0, "Id");
                    dataAdapterStudents.UpdateCommand = updateStudentsCommand;

                    // Construct the Update command for Student Emails table.
                    string strUpdateStudentEmails = "UPDATE [Emails] set Email = @Email WHERE Id = @Id";
                    SqlCommand updateStudentEmailsCommand = new SqlCommand(strUpdateStudentEmails, connection1);
                    updateStudentEmailsCommand.Parameters.Add("@Email", SqlDbType.NVarChar, 100, "Email");
                    updateStudentEmailsCommand.Parameters.Add("@Id", SqlDbType.Int, 0, "Id");
                    dataAdapterStudentEmails.UpdateCommand = updateStudentEmailsCommand;

                    // Execute the Update and Delete commands on the Students table second.
                    dataAdapterStudents.Update(dataSet1.Tables["Students"].Select("", "", DataViewRowState.ModifiedCurrent)); // Select only updated rows.
                    dataAdapterStudents.Dispose(); // Release all components.
                    // Execute the Update commands on the Email table first. This is because Student ID has dependency on the Student Email entry.
                    dataAdapterStudentEmails.Update(dataSet1.Tables["Emails"].Select("", "", DataViewRowState.ModifiedCurrent)); // Select only updated rows.
                    dataAdapterStudentEmails.Dispose(); // Release all components.

                    /* Update - Added */
                    // Construct the Insert command for the students table.
                    string strInsertStudents = "INSERT INTO [Students] (FirstName, LastName) VALUES (@FirstName, @LastName)";
                    SqlCommand insertStudentsCommand = new SqlCommand(strInsertStudents, connection1);
                    insertStudentsCommand.Parameters.Add("@FirstName", SqlDbType.NVarChar, 100, "FirstName");
                    insertStudentsCommand.Parameters.Add("@LastName", SqlDbType.NVarChar, 100, "LastName");
                    dataAdapterStudents.InsertCommand = insertStudentsCommand;

                    // Construct the Insert command for the student emails table.
                    string strInsertStudentEmails = "INSERT INTO [Emails] (Email, StudentId) VALUES (@Email, @StudentId)";
                    SqlCommand insertStudentEmailsCommand = new SqlCommand(strInsertStudentEmails, connection1);
                    insertStudentEmailsCommand.Parameters.Add("@Email", SqlDbType.NVarChar, 100, "Email");
                    insertStudentEmailsCommand.Parameters.Add("@StudentId", SqlDbType.Int, 0, "StudentId");
                    dataAdapterStudentEmails.InsertCommand = insertStudentEmailsCommand;

                    // Execute the Insert commands on the Students table first. This is because Student ID has dependency on the Student Email entry.
                    dataAdapterStudents.Update(dataSet1.Tables["Students"].Select("", "", DataViewRowState.Added)); // Select only Added rows.
                    dataAdapterStudents.Dispose(); // Release all components.
                    // Execute the Insert commands on the Email table Second.
                    dataAdapterStudentEmails.Update(dataSet1.Tables["Emails"].Select("", "", DataViewRowState.Added)); // Select only added rows.
                    dataAdapterStudentEmails.Dispose(); // Release all components.


                    LblMessage.Text = "Any chnages made to the Students and StudentEmails updated.";
                }
            }
            else
            {
                LblMessage.Text = "No dataset retrived from the database yet.";
            }
        }

        /// <summary>
        /// This function adds new student to the Students table and student email id the Student Emails table.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnAddStudent_Click(object sender, EventArgs e)
        {
            if (Cache["DATASET"] != null)
            {
                DataSet dataSet1 = (DataSet)Cache["DATASET"];
                if ((TbxEmail.Text != string.Empty) && (TbxFirstName.Text != string.Empty) && (TbxLastName.Text != string.Empty) && (TbxEmail.Text.Contains("@")))
                {
                    DataRow newStudentDataRow = dataSet1.Tables["Students"].NewRow();
                    newStudentDataRow["FirstName"] = TbxFirstName.Text;
                    newStudentDataRow["LastName"] = TbxLastName.Text;
                    dataSet1.Tables["Students"].Rows.Add(newStudentDataRow);

                    DataRow newStudentEmailDataRow = dataSet1.Tables["Emails"].NewRow();
                    newStudentEmailDataRow["Email"] = TbxEmail.Text;
                    newStudentEmailDataRow["StudentId"] = newStudentDataRow["Id"];
                    dataSet1.Tables["Emails"].Rows.Add(newStudentEmailDataRow);

                    // Empty the text feilds.
                    TbxFirstName.Text = string.Empty;
                    TbxLastName.Text = string.Empty;
                    TbxEmail.Text = string.Empty;

                    GetDataFromCache(); // refresh the veiw.
                }
                else
                {
                    if (!(TbxEmail.Text.Contains("@")))
                    {
                        LblMessage.Text = "Enter a valid email Id";
                    }
                    else
                    {
                        LblMessage.Text = "All feilds are required adding a student";
                    }
                }
            }
            else
            {
                LblMessage.Text = "No dataset retrived from the database yet.";
            }
        }

        /// <summary>
        /// This function inserts a additional email(s) for the existing student in the student record.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnAddAdditionalEmail_Click(object sender, EventArgs e)
        {
            if (Cache["DATASET"] != null)
            {
                DataSet dataSet1 = (DataSet)Cache["DATASET"];
                if ((TbxAdditionalEmail.Text != string.Empty) && (TbxStudentId.Text != string.Empty) && (TbxAdditionalEmail.Text.Contains("@")))
                {
                    if (dataSet1.Tables["Students"].Rows.Contains(TbxStudentId.Text))
                    {
                        DataRow newStudentEmailDataRow = dataSet1.Tables["Emails"].NewRow();
                        newStudentEmailDataRow["Email"] = TbxAdditionalEmail.Text;
                        newStudentEmailDataRow["StudentId"] = int.Parse(TbxStudentId.Text);
                        dataSet1.Tables["Emails"].Rows.Add(newStudentEmailDataRow);
                    }
                    else
                    {
                        LblMessage.Text = "Student record not found for Student Id - " + TbxStudentId.Text + "; Additional Email can be only added for existing Student ID in the student record";
                    }
                    // Empty the text feilds.
                    TbxStudentId.Text = string.Empty;
                    TbxAdditionalEmail.Text = string.Empty;

                    GetDataFromCache(); // refresh the veiw.
                }
                else
                {
                    if (!(TbxEmail.Text.Contains("@")))
                    {
                        LblMessage.Text = "Enter a valid email Id";
                    }
                    else
                    {
                        LblMessage.Text = "All feilds are required adding a student Email";
                    }
                }
            }
            else
            {
                LblMessage.Text = "No dataset retrived from the database yet.";
            }
        }

        /// <summary>
        /// This function shows the DataRow status of each DataTable in the DataSet (Students Table and Student Email Table in this case).
        /// Following Status can be seen - 
        ///     - Unchanged
        ///     - Added
        ///     - modified
        ///     - Deleted
        /// The status 'Detached' is not possible in this example as we are not creating any DataRow and not attaching them to the table.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnGetTablesStatus_Click(object sender, EventArgs e)
        {
            if (Cache["DATASET"] != null)
            {
                // Get the Cached DataSet
                DataSet dataSet1 = (DataSet)Cache["DATASET"];

                TbxStudentsTableStatus.Text = string.Empty;
                TbxStudentEmailTableStatus.Text = string.Empty;

                // Loop through each of the DataRows in Students DataTable in the Dataset.
                foreach (DataRow dataRowStudent in dataSet1.Tables["Students"].Rows)
                {
                    if (dataRowStudent.RowState == DataRowState.Deleted) // Check if deleted.
                    {
                        TbxStudentsTableStatus.Text += dataRowStudent["Id", DataRowVersion.Original].ToString() + " - " + dataRowStudent.RowState.ToString() + Environment.NewLine; // get status of the deleted row.
                    }
                    else
                    {
                        TbxStudentsTableStatus.Text += dataRowStudent["Id"].ToString() + " - " + dataRowStudent.RowState.ToString() + Environment.NewLine; // get status of the row.
                    }
                }
                TbxStudentsTableStatus.Visible = true;
                BtnHideStudentsTableStatus.Visible = true;

                // Loop through each of the DataRows in Student Email DataTable in the Dataset.
                foreach (DataRow dataRowStudentEmail in dataSet1.Tables["Emails"].Rows)
                {
                    if(dataRowStudentEmail.RowState == DataRowState.Deleted) // Check if deleted
                    {
                        TbxStudentEmailTableStatus.Text += dataRowStudentEmail["Id", DataRowVersion.Original].ToString() + " - " + dataRowStudentEmail.RowState.ToString() + Environment.NewLine; // get status of the deleted row.
                    }
                    else
                    {
                        TbxStudentEmailTableStatus.Text += dataRowStudentEmail["Id"].ToString() + " - " + dataRowStudentEmail.RowState.ToString() + Environment.NewLine; // get status of the row.
                    }
                }
                TbxStudentEmailTableStatus.Visible = true;
                BtnHideStudentEmailStatus.Visible = true;

                BtnAcceptChanges.Visible = true;
                BtnRejectChanges.Visible = true;
            }
            else
            {
                LblMessage.Text = "No dataset retrived from the database yet.";
            }
        }

        /// <summary>
        /// This function hides the Students table status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnHideStudentsTableStatus_Click(object sender, EventArgs e)
        {
            TbxStudentsTableStatus.Text = string.Empty;
            TbxStudentsTableStatus.Visible = false;
            BtnHideStudentsTableStatus.Visible = false;
            BtnAcceptChanges.Visible = false;
            BtnRejectChanges.Visible = false;
        }

        /// <summary>
        /// This function hides the Student Emails table status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnHideStudentEmailStatus_Click(object sender, EventArgs e)
        {
            TbxStudentEmailTableStatus.Text = string.Empty;
            TbxStudentEmailTableStatus.Visible = false;
            BtnHideStudentEmailStatus.Visible = false;
            BtnAcceptChanges.Visible = false;
            BtnRejectChanges.Visible = false;
        }

        /// <summary>
        /// This function accepts the changes made to the Students and Students Emails dataset.
        /// Note that the DB is not updated with this function. Only RowState is updated for the Cached dataset.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnAcceptChanges_Click(object sender, EventArgs e)
        {
            if (Cache["DATASET"] != null)
            {
                DataSet dataSet1 = (DataSet)Cache["DATASET"];
                if (dataSet1.HasChanges())
                {
                    dataSet1.AcceptChanges();
                    Cache.Insert("DATASET", dataSet1, null, DateTime.Now.AddHours(24), System.Web.Caching.Cache.NoSlidingExpiration);
                    GetDataFromCache();
                    LblMessage.Text = "All changes made are Accepted.";
                }
                else
                {
                    LblMessage.Text = "There are no changes to be Accepted.";
                }
            }
            else
            {
                LblMessage.Text = "No dataset retrived from the database yet.";
            }
        }

        /// <summary>
        /// This function rejects the changes made to the Students and Student Emails dataset.
        /// Note that the DB is not updated with this function. Only RowState is updated for the Cached dataset.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnRejectChanges_Click(object sender, EventArgs e)
        {
            if (Cache["DATASET"] != null)
            {
                DataSet dataSet1 = (DataSet)Cache["DATASET"];
                if (dataSet1.HasChanges())
                {
                    dataSet1.RejectChanges();
                    Cache.Insert("DATASET", dataSet1, null, DateTime.Now.AddHours(24), System.Web.Caching.Cache.NoSlidingExpiration);
                    GetDataFromCache();
                    LblMessage.Text = "All changes made are undone.";
                }
                else
                {
                    LblMessage.Text = "There are no changes to be undone.";
                }
            }
            else
            {
                LblMessage.Text = "No dataset retrived from the database yet.";
            }
        }
    }
}