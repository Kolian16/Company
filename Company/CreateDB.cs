using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;

namespace Company
{
    class CreateDB
    {
        static string dbPath;
        static bool authentication;
        public static string GetDataBasePath
        {
            get { return dbPath; }
            set { dbPath = value; }
        }

        public static bool Authentication
        {
            get { return authentication; }
            set { authentication = value; }
        }
        public class TableGroup
        {
            [SQLite.PrimaryKey, SQLite.Unique, SQLite.NotNull,SQLite.AutoIncrement]
            public int Id { get; set; }

            [SQLite.MaxLength(20), SQLite.NotNull]
            public string Name_group { get; set; }
            
            public int Rate { get; set; }
            public double Percent { get; set; }
            public double Percent_worker { get; set; }

        }
        public class TableDepartment
        {
            [SQLite.PrimaryKey, SQLite.Unique, SQLite.NotNull, SQLite.AutoIncrement]
            public int Id { get; set; }
            [ SQLite.NotNull]
            public int Group_ID { get; set; }

            [SQLite.MaxLength(50), SQLite.NotNull, SQLite.Unique]
            public string Name { get; set; }

            [SQLite.MaxLength(20)]
            public DateTime Date { get; set; }

            [SQLite.MaxLength(100), SQLite.NotNull]
            public double Wage { get; set; } 
            public string Password { get; set; }
        }
      

        public class TableSubordination
        {
            [SQLite.PrimaryKey, SQLite.Unique, SQLite.NotNull, SQLite.AutoIncrement]
            public int Id { get; set; }
           
            [SQLite.NotNull]
            public int Chief_ID { get; set; }
            [SQLite.NotNull]
            public int Department_ID { get; set; }


        }
        public class Show
        {
    

            public string Name { get; set; }

            public string Name_group { get; set; }
            public DateTime Date { get; set; }

          
            public double Wage { get; set; }
            
        }


        public static  void CreateDataBase(string dataBaseName, string userName, string pass)
        {
            
            using  (var db = new SQLite.SQLiteConnection(dataBaseName, true))
            {
                db.CreateTable<TableDepartment>();
                db.CreateTable<TableGroup>();
               // db.CreateTable<TableGroupAdministarator>();
                db.CreateTable<TableSubordination>();
             //  db.Execute("create view DepartmentView as select  a.name, b.name_group, a.Date, a.wage from TableDepartment as a, TableGroup as b where a.group_id == b.id");

                var insertAccount = new TableDepartment()
                {
                    Name = userName,
                    Password = Action.Encrypt(pass),
                    Group_ID = 4,
                    Date = DateTime.Now
                    


                };
                db.Insert(insertAccount);
                db.Dispose();
            }
        }
        
        public static bool CheckAdministrator()
        {
            int completed = 0;

            using (var db = new SQLite.SQLiteConnection(GetDataBasePath, true))
            {
                completed = (from s in db.Table<TableDepartment>()
                             select s).Count();


                db.Dispose();
            }
            return completed > 0 ? true : false;
        }
        public static void LogIn(string dataBaseName, string userName, string pass)
        {
            using (var db = new SQLite.SQLiteConnection(dataBaseName, true))
            {

                int Account = (from s in db.Table<TableDepartment>()
                               where s.Name.Equals(userName)
                               where s.Password.Equals(pass)
                               select s).Count();
                db.Dispose();
                Authentication = Account.Equals(0) ? false : true;
                Action.AccountName = authentication.Equals(true) ?"Account : "+ userName:"";
                
              
            }
            
        }
    }
}
