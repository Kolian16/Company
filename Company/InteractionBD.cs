using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company
{
    class InteractionBD
    {
        public static string GetRate(string nameGroup)
        {
            int rate;
            using (var db = new SQLite.SQLiteConnection(CreateDB.GetDataBasePath, true))
            {

                rate = db.ExecuteScalar<int>("select  rate from TableGroup where Name_group ='" + nameGroup + "'");
                db.Dispose();
            }
            return rate.ToString();
        }
        public static string GetPercent(string nameGroup)
        {
            double rate;
            using (var db = new SQLite.SQLiteConnection(CreateDB.GetDataBasePath, true))
            {

                rate = db.ExecuteScalar<double>("select  percent from TableGroup where Name_group ='" + nameGroup + "'");
                db.Dispose();
            }
            return rate.ToString();
        }
        public static string GetPercentWorker(string nameGroup)
        {
            double rate;
            using (var db = new SQLite.SQLiteConnection(CreateDB.GetDataBasePath, true))
            {

                rate = db.ExecuteScalar<double>("select percent_worker from TableGroup where Name_group ='" + nameGroup + "'");
                db.Dispose();
            }
            return rate.ToString();
        }
        public static void AddWage(string groupName, int id)
        {
            double wage;
            using (var db = new SQLite.SQLiteConnection(CreateDB.GetDataBasePath, true))
            {
                List<CreateDB.TableGroup> dataGroup = new List<CreateDB.TableGroup>();
                double AllSalarySub=0;
                var tableGroup = from s in db.Table<CreateDB.TableGroup>()
                                 where s.Name_group == groupName
                                 select s;
                var tableDepartment = db.Get<CreateDB.TableDepartment>(id);
                tableGroup.ToList().ForEach(item => dataGroup.Add(item));
                var querySub = db.Query<CreateDB.Show>("select b.name, d.name_group, b.Date, b.wage" +
                                                    " from TableDepartment as a, TableSubordination as c, TableDepartment as b, TableGroup as d" +
                                                     " where  c.chief_id = a.id" +
                                                     " and c.department_id = b.id" +
                                                     " and a.group_id = d.id" +
                                                     " and c.chief_id =" + id);
               
                querySub.ToList().ForEach((x) => AllSalarySub += x.Wage);
                double percent;
                switch (dataGroup[0].Id)
                {
                    case 1:
                        percent= GetYear(tableDepartment.Date, DateTime.Now) * dataGroup[0].Percent > 0.30 ? 0.30 : GetYear(tableDepartment.Date, DateTime.Now) * dataGroup[0].Percent;
                        break;
                    case 2:
                        percent = GetYear(tableDepartment.Date, DateTime.Now) * dataGroup[0].Percent > 0.40 ? 0.40 : GetYear(tableDepartment.Date, DateTime.Now) * dataGroup[0].Percent;
                        break;
                    case 3:
                        percent = GetYear(tableDepartment.Date, DateTime.Now) * dataGroup[0].Percent > 0.35 ? 0.35 : GetYear(tableDepartment.Date, DateTime.Now) * dataGroup[0].Percent;
                        break;
                    default:
                        percent = 0;
                        break;
                }
                
                wage = dataGroup[0].Rate + percent * dataGroup[0].Rate + AllSalarySub * dataGroup[0].Percent_worker;
                db.Execute("update TableDepartment set wage =" + Convert.ToInt32(wage) + " where id = " + id);
                db.Dispose();

            }
        }
        public static int GetYear(DateTime oldDate, DateTime Now)
        {
            var result = Now.Year - oldDate.Year;
            return result;
        }
        public static void AddInDataBase(string name, string groupName,string date, string password, List<Worker> workersSub,List<Worker> workerChief, double wage = 0)
        {
            
            using (var db = new SQLite.SQLiteConnection(CreateDB.GetDataBasePath, true))
            {
                List<CreateDB.TableDepartment> insert = new List<CreateDB.TableDepartment>();
                List<CreateDB.TableGroup> insertGroup = new List<CreateDB.TableGroup>();
                var tableGroup = from s in db.Table<CreateDB.TableGroup>()  
                                 where s.Name_group == groupName
                                 select s;
                
                tableGroup.ToList().ForEach(item => insertGroup.Add(item));
                insert.Add(new CreateDB.TableDepartment() {  Group_ID = insertGroup[0].Id, Name = name, Date = Convert.ToDateTime(date), Wage = wage, Password = Action.Encrypt(password) });
                db.InsertAll(insert);
                var id = db.ExecuteScalar<int>("select  id from TableDepartment where Name ='" + name + "'");
                AddSubordination(id, workersSub);
                AddChief(id, workerChief);
                AddWage(groupName, id);
                db.Dispose();
            }
        }
        public static void EditWorker()
        {
            using (var db = new SQLite.SQLiteConnection(CreateDB.GetDataBasePath, true))
            {
                List<CreateDB.TableDepartment> insert = new List<CreateDB.TableDepartment>();
                List<CreateDB.TableGroup> insertGroup = new List<CreateDB.TableGroup>();
                //var tableGroup = from s in db.Table<CreateDB.TableGroup>()
                //                 where s.Name_group == groupName
                //                 select s;

                //tableGroup.ToList().ForEach(item => insertGroup.Add(item));
                //insert.Add(new CreateDB.TableDepartment() { Group_ID = insertGroup[0].Id, Name = name, Date = Convert.ToDateTime(date), Wage = wage, Password = Action.Encrypt(password) });
                //db.InsertAll(insert);
                //var id = db.ExecuteScalar<int>("select  id from TableDepartment where Name ='" + name + "'");
                //AddSubordination(id, workersSub);
                //AddChief(id, workerChief);
                //AddWage(groupName, id);
                //db.Dispose();
            }
        }
        public static void DeleteWorker(string name)
        {
            using (var db = new SQLite.SQLiteConnection(CreateDB.GetDataBasePath, true))
            {
               
             
                var id = db.ExecuteScalar<int>("select  id from TableDepartment where Name ='" + name + "'");
                db.Delete<CreateDB.TableDepartment>(id);
                db.Execute("delete from TableSubordination where chief_id ="+ id);
                db.Execute("delete from TableSubordination where department_id =" + id);
                db.Dispose();
            }
        }
        public static List<CreateDB.Show> ShowWorker(string userName ="")
        {
            List<CreateDB.Show> content = new List<CreateDB.Show>();
            using (var db = new SQLite.SQLiteConnection(CreateDB.GetDataBasePath, true))
            {
                var id = userName == "" ? 4: db.ExecuteScalar<int>("select  group_id from TableDepartment where Name ='" + userName + "'");
                var idChief= db.ExecuteScalar<int>("select  id from TableDepartment where Name ='" + userName + "'");
                var querySub = db.Query<CreateDB.Show>("select b.name, d.name_group, b.Date, b.wage" +
                                                     " from TableDepartment as a, TableSubordination as c, TableDepartment as b, TableGroup as d" +
                                                      " where  c.chief_id = a.id" +
                                                      " and c.department_id = b.id" +
                                                      " and b.group_id = d.id" +
                                                      " and c.chief_id =" + idChief);
                var contentDB = db.Query<CreateDB.Show>("select a.name, b.name_group, a.Date, a.wage from TableDepartment as a, TableGroup as b where a.group_id == b.id");
                if (id != 4)
                {
                    querySub.ForEach((x) => content.Add(new CreateDB.Show() { Name = x.Name, Name_group = x.Name_group, Date = x.Date, Wage = x.Wage }));
                }
                else
                {
                    contentDB.ForEach((x) => content.Add(new CreateDB.Show() { Name = x.Name, Name_group = x.Name_group, Date = x.Date, Wage = x.Wage }));
                }
                db.Dispose();

            }
            return content;
        }
        public static void AddSubordination(int id, List<Worker> workers)
        {
            List<CreateDB.TableSubordination> subordination = new List<CreateDB.TableSubordination>();
            if (!workers.Equals(null))
            {
                using (var db = new SQLite.SQLiteConnection(CreateDB.GetDataBasePath, true))
                {

                    workers.ForEach((x) => subordination.Add(new CreateDB.TableSubordination() { Chief_ID = id, Department_ID = x.Id }));
                    db.InsertAll(subordination);
                    db.Dispose();
                }
            }
        }
        public static void AddChief(int id, List<Worker> workers)
        {
            List<CreateDB.TableSubordination> chief = new List<CreateDB.TableSubordination>();
            if (!workers.Equals(null))
            {
                using (var db = new SQLite.SQLiteConnection(CreateDB.GetDataBasePath, true))
                {

                    workers.ForEach((x) => chief.Add(new CreateDB.TableSubordination() { Chief_ID = x.Id, Department_ID = id }));
                    db.InsertAll(chief);
                    db.Dispose();
                }
            }
        }
        public static bool CheckName(string name)
        {
            int count;
            {
                using (var db = new SQLite.SQLiteConnection(CreateDB.GetDataBasePath, true))
                {
                    count = (from s in db.Table<CreateDB.TableDepartment>()
                             where s.Name == name
                             select s).Count();
                    db.Dispose();
                }
                return count > 0 ? false : true;
            }
        }
    
        public static List<Worker> GetWorkers()
        { 
           
                List<Worker> workersList = new List<Worker>();
                using (var db = new SQLite.SQLiteConnection(CreateDB.GetDataBasePath, true))
                {

                    var getworkers = from s in db.Table<CreateDB.TableDepartment>()
                                     where  s.Group_ID < 4
                                     select s;
                getworkers.ToList().ForEach((x) => workersList.Add(new Worker() { Id = x.Id, Name = x.Name }));
                db.Dispose();
            }
           
            return workersList;
        }
        public static List<Worker> GetWorkersChief()
        {

            List<Worker> workersList = new List<Worker>();
            using (var db = new SQLite.SQLiteConnection(CreateDB.GetDataBasePath, true))
            {

                var getworkers = from s in db.Table<CreateDB.TableDepartment>()
                                 where s.Group_ID > 1 && s.Group_ID < 4
                                 select s;
                getworkers.ToList().ForEach((x) => workersList.Add(new Worker() { Id = x.Id, Name = x.Name }));
                db.Dispose();
            }

            return workersList;
        }
        public static List<Worker> GetWorkersSubEdit(string userName)
        {

            List<Worker> workersList = new List<Worker>();
            using (var db = new SQLite.SQLiteConnection(CreateDB.GetDataBasePath, true))
            {
                var idChief = db.ExecuteScalar<int>("select  id from TableDepartment where Name ='" + userName + "'");
                var querySub = db.Query<CreateDB.Show>("select b.name, d.name_group, b.Date, b.wage" +
                                                     " from TableDepartment as a, TableSubordination as c, TableDepartment as b, TableGroup as d" +
                                                      " where  c.chief_id = a.id" +
                                                      " and c.department_id = b.id" +
                                                      " and b.group_id = d.id" +
                                                      " and c.chief_id =" + idChief);
                querySub.ToList().ForEach((x) => workersList.Add(new Worker() { Id = 1, Name = x.Name }));
                db.Dispose();
            }

            return workersList;
        }
        public static List<Worker> GetWorkersChiefEdit(string userName)
        {

            List<Worker> workersList = new List<Worker>();
            using (var db = new SQLite.SQLiteConnection(CreateDB.GetDataBasePath, true))
            {
                var idSub = db.ExecuteScalar<int>("select  id from TableDepartment where Name ='" + userName + "'");
                var querySub = db.Query<CreateDB.Show>("select b.name, d.name_group, b.Date, b.wage" +
                                                     " from TableDepartment as a, TableSubordination as c, TableDepartment as b, TableGroup as d" +
                                                      " where  c.department_id = a.id" +
                                                      " and c.chief_id = b.id" +
                                                      " and b.group_id = d.id" +
                                                      " and c.department_id =" + idSub);
                querySub.ToList().ForEach((x) => workersList.Add(new Worker() { Id = 1, Name = x.Name }));
                db.Dispose();
            }

            return workersList;
        }
        public static double TotalSalary()
        {
            double totalSalary=0;
            using (var db = new SQLite.SQLiteConnection(CreateDB.GetDataBasePath, true))
            {

                var getworkers = db.Table<CreateDB.TableDepartment>();
                                                                
                getworkers.ToList().ForEach((x) => totalSalary+=x.Wage);
                db.Dispose();
            }
            return totalSalary;
        }
        public static void Rights(string userName)
        {
            using (var db = new SQLite.SQLiteConnection(CreateDB.GetDataBasePath, true))
            {
                var id = db.ExecuteScalar<int>("select  group_id from TableDepartment where Name ='" + userName + "'");
                var wage = db.ExecuteScalar<int>("select  wage from TableDepartment where Name ='" + userName + "'");
                if (id == 4)
                {
                    Data.Salary("Суммарная зарплата всех сотрудников  =  " +TotalSalary() + " р.");
                }
                else
                {
                    Data.Salary("Ваша заработная плата за месяц составляет  =  " +wage+ " р.");
                }
                Data.EventHandler(id);
                db.Dispose();
            }
        }


    }
}
