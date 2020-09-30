# ADO.NET-CSharp----Projects
Introduction of basic concepts of ADO.NET
- I have removed all the dependencies from the project to reduce the number of files in the prokect.
- Make sure REBUILD Solution, Clean Solution, again REBUILD Solution is issued before running any of the basic concepts programs.
- Additionally for For WebForms, please select the appropriate "WebForm1.aspx.cs" before running the program.


Following Database tables are used through out the example projects in Introduction to ADO.NET

CREATE TABLE [Students]

(

	[Id] INT IDENTITY NOT NULL,
	
	[FirstName] NVARCHAR(100) NOT NULL,
	
	[LastName] NVARCHAR(100) NOT NULL,
	
	CONSTRAINT [PK_Students] PRIMARY KEY ([Id])
	
)



CREATE TABLE [Emails]

(

	[Id] INT IDENTITY NOT NULL,
	
	[Email] NVARCHAR(100) NOT NULL,
	
	[StudentId] INT NOT NULL,
	
	CONSTRAINT [PK_Emails] PRIMARY KEY ([Id]),
	
	CONSTRAINT [FK_Students_Emails] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]),
	
)

