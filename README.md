# Attendance Management System

The Attendance Management System is a C# application built with WinForms using .NET 6 in Visual Studio. It serves as a tool for managing attendance records, tracking student attendance, and generating reports. The application uses MySQL as the database to store and retrieve data. The database creation and population code can be found in the `populate.py` file in the `populate-db` folder which requires external libraries to be installed.


## Features

- User Authentication: Allows users to log in and log out of the system.
- Attendance Management: Provides functionality to mark and update student attendance records.
- Student Management: Enables the addition and modification of student information.
- Teacher Management: Supports the addition and modification of teacher information.
- Course Management: Allows the addition and modification of course information.
- Reporting: Generates reports for student attendance, providing insights into attendance trends and statistics.


## Prerequisites

Before running the Attendance Management System, ensure that the following prerequisites are met:

- Visual Studio with .NET 6 installed.
- MySQL Server installed and running.
- MySQL Connector/NET installed.


## Getting Started

1. Clone the repository to your local machine or download the source code.
2. Open the solution file [`AttendanceManagementSystem.sln`]('https://github.com/nihaalnz/Attendance-Management-System/blob/master/Attendance%20Management%20System/Attendance%20Management%20System.sln') in Visual Studio.
3. Set up the MySQL database:
   - Create a new database using your preferred MySQL management tool.
   - Update the connection string in the application to match your MySQL server and database.
4. Build the solution to restore NuGet packages and compile the project.
5. Run the application from Visual Studio, and the Attendance Management System will launch.


## Usage

1. Log in using your credentials or create a new user account.
2. Navigate through the application using the menu and buttons to perform various tasks:
   - Add and manage students, teachers, and courses.
   - Mark and update attendance for students.
   - Generate attendance reports.
3. Log out when finished using the system.


## Contributing

Contributions to the Attendance Management System are welcome. If you find any bugs or have suggestions for enhancements, please open an issue or submit a pull request.

## License

The Attendance Management System is released under the [MIT License](LICENSE).

## Acknowledgments

- [MySQL](https://www.mysql.com/) - The open-source relational database management system.
- [MySQL Connector/NET](https://dev.mysql.com/downloads/connector/net/) - The ADO.NET driver for MySQL.
- [Visual Studio](https://visualstudio.microsoft.com/) - The integrated development environment for building .NET applications.
