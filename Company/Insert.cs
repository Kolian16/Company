using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company
{
    class Insert
    {
        public static void InsertOnLoad()
        {

            List<CreateDB.TableGroup> insertOnLoad = new List<CreateDB.TableGroup>();
               insertOnLoad.Add(new CreateDB.TableGroup() {  Name_group = "Employee", Rate = 25000,Percent=0.03});
               insertOnLoad.Add(new CreateDB.TableGroup() {  Name_group = "Manager", Rate = 30000,Percent=0.05,Percent_worker=0.005 });
               insertOnLoad.Add(new CreateDB.TableGroup() {  Name_group = "Salesman", Rate = 35000,Percent=0.01,Percent_worker=0.003 });
               insertOnLoad.Add(new CreateDB.TableGroup() {  Name_group = "Administrator"});
            List<CreateDB.TableDepartment> insert = new List<CreateDB.TableDepartment>();
            insert.Add(new CreateDB.TableDepartment() { Name = "ЛАПИН Н.Ю.", Group_ID = 1, Date = DateTime.Now, Password = "E33068276F160E7E4B86ADC79F3D1145689FF9B1CAE159B8BFB9D55807E74233" });//PASSWORD=200825
            insert.Add(new CreateDB.TableDepartment() { Name = "ВОРОТИЛОВ Л.Ю.", Group_ID = 3, Date = DateTime.Now, Password = "E33068276F160E7E4B86ADC79F3D1145689FF9B1CAE159B8BFB9D55807E74233" });
            insert.Add(new CreateDB.TableDepartment() { Name = "ТОКАРЕВ Г.Ю.", Group_ID = 2, Date = DateTime.Now, Password = "E33068276F160E7E4B86ADC79F3D1145689FF9B1CAE159B8BFB9D55807E74233" });

            using (var db = new SQLite.SQLiteConnection(CreateDB.GetDataBasePath, true))
            {

                db.InsertAll(insertOnLoad);
                db.InsertAll(insert);
                db.Dispose();
            }
        }
        public static List<string> GetSplitGroup()
        {
            List<string> splitContent = new List<string>(); 
            using (var db = new SQLite.SQLiteConnection(CreateDB.GetDataBasePath, true))
            {

               var contentSplit= db.Table<CreateDB.TableGroup>();
                foreach (var item in contentSplit)
                {
                    splitContent.Add(item.Name_group);
                }
                db.Dispose();
            }
            return splitContent;
        }
        public static List<string> GetSplitRate()
        {
            List<string> splitContent = new List<string>();
            using (var db = new SQLite.SQLiteConnection(CreateDB.GetDataBasePath, true))
            {

                var contentSplit = db.Table<CreateDB.TableGroup>();
                foreach (var item in contentSplit)
                {
                    splitContent.Add(item.Rate.ToString());
                }
                db.Dispose();
            }
            return splitContent;
        }
        public static List<string> GetSplitPercent()
        {

            List<string> splitContent = new List<string>();
            using (var db = new SQLite.SQLiteConnection(CreateDB.GetDataBasePath, true))
            {

                var contentSplit = db.Table<CreateDB.TableGroup>();
                foreach (var item in contentSplit)
                {
                    splitContent.Add(item.Percent.ToString());
                }
                db.Dispose();
            }
            return splitContent;
        }
        public static List<string> GetSplitPercentWorker()
        {
            List<string> splitContent = new List<string>();
            using (var db = new SQLite.SQLiteConnection(CreateDB.GetDataBasePath, true))
            {

                var contentSplit = db.Table<CreateDB.TableGroup>();
                foreach (var item in contentSplit)
                {
                    splitContent.Add(item.Percent_worker.ToString());
                }
                db.Dispose();
            }
            return splitContent;
        }
        public static List<string> GetSplitChief()
        {
            List<string> splitContent = new List<string>();
            using (var db = new SQLite.SQLiteConnection(CreateDB.GetDataBasePath, true))
            {

                var contentSplit = from s in db.Table<CreateDB.TableDepartment>()
                                   where s.Group_ID > 1 && s.Group_ID < 4
                                   select s;
                foreach (var item in contentSplit)
                {
                    splitContent.Add(item.Name);
                }
               
                db.Dispose();
            }
            return splitContent;
        }
    }
}
