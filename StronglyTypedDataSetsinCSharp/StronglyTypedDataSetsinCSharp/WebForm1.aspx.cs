using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;

namespace StronglyTypedDataSetsinCSharp
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        private string GetStudentEmails(int StudentId)
        {
            string StudentEmails = "";
            if (Session["DATASET"] != null)
            {
                int EmailCount = 0;
                DataSet dataSet1 = (DataSet)Session["DATASET"];
                foreach (DataRow dataRowStudentEmail in (dataSet1.Tables["StudentEmails"]).Rows)
                {
                    if (Convert.ToInt32(dataRowStudentEmail["StudentId"]) == StudentId)
                    {
                        EmailCount++;
                        if(EmailCount > 1)
                        {
                            StudentEmails += ", " + dataRowStudentEmail["Email"].ToString();
                        }
                        else
                        {
                            StudentEmails = dataRowStudentEmail["Email"].ToString();
                        }
                    }
                }
            }
            return StudentEmails;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
                using (SqlConnection connection1 = new SqlConnection(ConnectionString))
                {
                    string strSelectQuery = "SELECT * FROM [Students]; SELECT * FROM [Emails]";
                    SqlDataAdapter dataAdapter1 = new SqlDataAdapter(strSelectQuery, connection1);

                    DataSet dataSet1 = new DataSet();
                    dataAdapter1.Fill(dataSet1);
                    dataSet1.Tables[0].TableName = "Students";
                    dataSet1.Tables[1].TableName = "StudentEmails";

                    Session["DATASET"] = dataSet1;

                    GrdvStudents.DataSource = from dataRowStudent in dataSet1.Tables["Students"].AsEnumerable()
                                              // Here the program defines a Student class and its properties and,
                                              // a collection of student object is created for each of the database table,
                                              // where table colume are mapped to the Student Class properties.
                                              select new Student 
                                              {
                                                  Id = Convert.ToInt32(dataRowStudent["Id"]),
                                                  FirstName = dataRowStudent["FirstName"].ToString(),
                                                  LastName = dataRowStudent["LastName"].ToString(),
                                                  Emails = GetStudentEmails(Convert.ToInt32(dataRowStudent["Id"])),
                                              };

                    GrdvStudents.DataBind();
                }
            }
        }

        protected void BtnSeachStudent_Click(object sender, EventArgs e)
        {
            if(Session["DATASET"] != null)
            {
                DataSet dataSet1 = (DataSet)Session["DATASET"];
                if(string.IsNullOrEmpty(TbxSearchStudent.Text))
                {
                    GrdvStudents.DataSource = from dataRowStudent in dataSet1.Tables["Students"].AsEnumerable()
                                                  // Here the program defines a Student class and its properties and,
                                                  // a collection of student object is created for each of the database table,
                                                  // where table colume are mapped to the Student Class properties.
                                              select new Student
                                              {
                                                  Id = Convert.ToInt32(dataRowStudent["Id"]),
                                                  FirstName = dataRowStudent["FirstName"].ToString(),
                                                  LastName = dataRowStudent["LastName"].ToString(),
                                                  Emails = GetStudentEmails(Convert.ToInt32(dataRowStudent["Id"])),
                                              };

                    GrdvStudents.DataBind();
                }
                else
                {
                    GrdvStudents.DataSource = from dataRowStudent in dataSet1.Tables["Students"].AsEnumerable()
                                              where dataRowStudent["FirstName"].ToString().ToUpper().StartsWith(TbxSearchStudent.Text.ToUpper())
                                              // Here the program defines a Student class and its properties and,
                                              // a collection of student object is created for each of the database table,
                                              // where table colume are mapped to the Student Class properties.
                                              select new Student
                                              {
                                                  Id = Convert.ToInt32(dataRowStudent["Id"]),
                                                  FirstName = dataRowStudent["FirstName"].ToString(),
                                                  LastName = dataRowStudent["LastName"].ToString(),
                                                  Emails = GetStudentEmails(Convert.ToInt32(dataRowStudent["Id"])),
                                              };

                    GrdvStudents.DataBind();
                }
            }
        }
    }
}