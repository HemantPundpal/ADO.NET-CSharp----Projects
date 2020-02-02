using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StronglyTypedDataSetsinCSharp
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        private string GetStudentEmails(int StudentId)
        {
            string StudentEmails = "";
            if (Session["EMAILDATATABLE"] != null)
            {
                int EmailCount = 0;
                StudentDataSet.EmailsDataTable EmailDataTable = (StudentDataSet.EmailsDataTable)Session["EMAILDATATABLE"];
                foreach(var email in EmailDataTable)
                {
                    if (email.StudentId == StudentId)
                    {
                        EmailCount++;
                        if (EmailCount > 1)
                        {
                            StudentEmails += ", " + email.Email;
                        }
                        else
                        {
                            StudentEmails = email.Email;
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
                // In this example a strongly typed DataSet is used.
                // In a strongly typed DataSet the table columns becomes properties and type (class) associated with each of the table columns is known at design time
                // A seperate defination of the class for the DataSet table is not needed, also do not have to map the class properties to the DataSet table columns, Which is required for not strongly typed DataSet.
                // This makes development much easier and reduces errors in coding.
                StudentDataSetTableAdapters.StudentsTableAdapter StudentsTableAdapater = new StudentDataSetTableAdapters.StudentsTableAdapter();
                StudentDataSet.StudentsDataTable StudentDataTable = new StudentDataSet.StudentsDataTable();

                StudentDataSetTableAdapters.EmailsTableAdapter EmailsTableAdapter = new StudentDataSetTableAdapters.EmailsTableAdapter();
                StudentDataSet.EmailsDataTable EmailDataTable = new StudentDataSet.EmailsDataTable();

                Session["STUDENTDATATABLE"] = StudentDataTable;
                Session["EMAILDATATABLE"] = EmailDataTable;

                StudentsTableAdapater.Fill(StudentDataTable);
                EmailsTableAdapter.Fill(EmailDataTable);

                GrdvStudents.DataSource = from student in StudentDataTable
                                          // As it is a strongly typed DataSet, the type (Class) of table is known at design time and,
                                          // all table columns are properties.
                                          select new
                                          {
                                              student.Id,
                                              student.FirstName,
                                              student.LastName,
                                              studentEmail = GetStudentEmails(student.Id), // New property can be defined if additional information needs to be added as one more column programatically.
                                          };

                GrdvStudents.DataBind();
            }
        }

        protected void BtnSeachStudent_Click(object sender, EventArgs e)
        {
            if (Session["STUDENTDATATABLE"] != null)
            {
                StudentDataSet.StudentsDataTable StudentDataTable = (StudentDataSet.StudentsDataTable)Session["STUDENTDATATABLE"];
                if (string.IsNullOrEmpty(TbxSearchStudent.Text))
                {
                    GrdvStudents.DataSource = from student in StudentDataTable
                                              // As it is a strongly typed DataSet, the type (Class) of table is known at design time and,
                                              // all table columns are properties.
                                              select new
                                              {
                                                  student.Id,
                                                  student.FirstName,
                                                  student.LastName,
                                                  studentEmail = GetStudentEmails(student.Id), // New property can be defined if additional information needs to be added as one more column programatically.
                                              };

                    GrdvStudents.DataBind();
                }
                else
                {
                    GrdvStudents.DataSource = from student in StudentDataTable
                                              where student.FirstName.ToUpper().StartsWith(TbxSearchStudent.Text.ToUpper())
                                              // As it is a strongly typed DataSet, the type (Class) of table is known at design time and,
                                              // all table columns are properties.
                                              select new
                                              {
                                                  student.Id,
                                                  student.FirstName,
                                                  student.LastName,
                                                  studentEmail = GetStudentEmails(student.Id), // New property can be defined if additional information needs to be added as one more column programatically.
                                              };

                    GrdvStudents.DataBind();
                }
            }
        }
    }
}