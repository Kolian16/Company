using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls.Dialogs;
using System.IO;
using System.Data.SQLite;
using MahApps.Metro;


namespace Company
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Data.EventHandler = new Data.MyEvent(Rights);
            Data.Salary = new Data.MyEventSalary(Salary);
        }
        void Rights(int number)
        {
            groupBox.Visibility = number > 1 ? Visibility.Visible : Visibility.Hidden;
            expander2.Visibility = number > 3 ? Visibility.Visible : Visibility.Hidden;
            Application.Current.MainWindow.Height = number >= 1&& number < 4 ? 400:615;
           
          
        }
        void Salary(string text)
        {
            label1.Content = text;
           // "суммарная зарплата всех сотрудников  =  " + InteractionBD.TotalSalary() + " р.";
        }
        void InputAuthorization(string path,string title,string message)
        {
            
            LoginDialogSettings settingsAuthorization = new LoginDialogSettings();
            settingsAuthorization.AnimateShow = true;
            settingsAuthorization.UsernameWatermark = "Имя пользователя";
            settingsAuthorization.PasswordWatermark = "Пароль";
            settingsAuthorization.AffirmativeButtonText = "Зарегистрировать";
            settingsAuthorization.NegativeButtonVisibility = Visibility.Visible;
            settingsAuthorization.ColorScheme = MetroDialogColorScheme.Accented;
            settingsAuthorization.EnablePasswordPreview = true;
            var result = this.ShowModalLoginExternal(title,message, settingsAuthorization);
            try {
                if (result.Password.Equals(string.Empty) || result.Username==null||result.Username.Equals(string.Empty))
                {
                    
                    InputAuthorization(CreateDB.GetDataBasePath,title,message);
                }
            }
            catch
            {
                File.Delete(CreateDB.GetDataBasePath);
                Environment.Exit(0);
            }
            CreateDB.CreateDataBase(CreateDB.GetDataBasePath, result.Username.ToUpper(), result.Password);
           
            
        }
        void InputAuthentication(string path)
        {
            LoginDialogSettings settingsAuthentication = new LoginDialogSettings();
            settingsAuthentication.AnimateShow = true;
            settingsAuthentication.UsernameWatermark = "Имя пользователя";
            settingsAuthentication.PasswordWatermark = "Пароль";
            settingsAuthentication.AffirmativeButtonText = "Войти";
            settingsAuthentication.NegativeButtonVisibility = Visibility.Visible;
            settingsAuthentication.ColorScheme = MetroDialogColorScheme.Accented;
            settingsAuthentication.EnablePasswordPreview = true;
            LoginDialogData result;
            if (Action.Acces)
            {
                result = this.ShowModalLoginExternal("Вход в систему", "Аутентификация пользователя...", settingsAuthentication);
            }
            else
            {
                result = this.ShowModalLoginExternal("Вход в систему", "Ошибка учетных данных", settingsAuthentication);
            }
            
            try
            {
                if (result.Password.Equals(string.Empty) || result.Username == null || result.Username.Equals(string.Empty))
                {
                    Action.Acces = false;
                    InputAuthentication(CreateDB.GetDataBasePath);
                }
                else {
                    string pass = Action.Encrypt(result.Password);
                   
                    CreateDB.LogIn(CreateDB.GetDataBasePath, result.Username.ToUpper(), pass);
                    InteractionBD.Rights(result.Username.ToUpper());
                    dataGrid2.ItemsSource = InteractionBD.ShowWorker(result.Username.ToUpper());
                    LabelAccount.Content = Action.AccountName;
                    
                }
            }
            catch
            {

                Environment.Exit(0);
            }
            
            if (!CreateDB.Authentication)
            {
                Action.Acces = false;
                InputAuthentication(CreateDB.GetDataBasePath);
            }
           
           
        }


        private  void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //путь  к базе данных
            string path = @"D:\\CompanyDB.db";
 

            if (!File.Exists(path))
            {
                CreateDB.GetDataBasePath=path;
                try {
                    SQLiteConnection.CreateFile(path);
                }
                catch { MessageBox.Show("Внимание!", "Диск защищен от записи! Выберите другую директорию для Базы данных");
                    Environment.Exit(0);
                };
               InputAuthorization(CreateDB.GetDataBasePath, "Создайте имя пользователя и пароль", "для учетной записи администратора...");
                Insert.InsertOnLoad();
          
            }
            else
            {
                CreateDB.GetDataBasePath=path;
            }
            try
            {

                if (CreateDB.CheckAdministrator())
                {
                    InputAuthentication(CreateDB.GetDataBasePath);
                          
                }

            }
            catch
            {
               
                MessageBox.Show("Ошибка Базы дынных");
                this.Close();
            }
         
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            groupBox.Visibility = Visibility.Hidden;
            expander2.Visibility = Visibility.Hidden;
            label1.Content = string.Empty;
            InputAuthentication(CreateDB.GetDataBasePath);
          

        }

      

     

       

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (InteractionBD.CheckName(textBoxAddFIO.Text))
                {
                    InteractionBD.AddInDataBase(textBoxAddFIO.Text, comboBoxGroup.SelectedValue.ToString(), DatePickerAddDate.Text, textBoxAddPassword.Text, combobox.SelectedItems.Cast<Worker>().ToList(), comboboxChief.SelectedItems.Cast<Worker>().ToList());
                    dataGrid2.ItemsSource = InteractionBD.ShowWorker();
                    textBoxAddFIO.Clear();
                    textBoxAddPassword.Clear();
                    label1.Content = "Суммарная зарплата всех сотрудников  =  "+ InteractionBD.TotalSalary();
                    textBoxAddFIO.BorderBrush = Brushes.Black;
                    textBoxAddPassword.BorderBrush = Brushes.Black;
                    DatePickerAddDate.BorderBrush = Brushes.Black;
                    comboBoxGroup.BorderBrush = Brushes.Black;
                }
                else
                {
                    this.ShowMessageAsync("Внимание!", "Такой пользователь существует!");
                }
            }
            catch
            {
                textBoxAddFIO.BorderBrush = Brushes.Red;
                textBoxAddPassword.BorderBrush = Brushes.Red;
                DatePickerAddDate.BorderBrush = Brushes.Red;
                comboBoxGroup.BorderBrush= Brushes.Red;
                this.ShowMessageAsync("Внимание!", "Вы делаете что-то неправильно!");
            }
        }

        private void dataGrid2_AutoGeneratedColumns(object sender, EventArgs e)
        {
            dataGrid2.Columns[0].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            dataGrid2.Columns[1].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            dataGrid2.Columns[2].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            dataGrid2.Columns[3].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            dataGrid2.Columns[0].Header = "Ф.И.О";
            dataGrid2.Columns[1].Header = "группа";
            dataGrid2.Columns[2].Header = "Дата";
            dataGrid2.Columns[3].Header = "заработная плата";
          
        }

        private void expander2_Expanded(object sender, RoutedEventArgs e)
        {
            groupBox.Height = 210;
        }

        private void expander2_Collapsed(object sender, RoutedEventArgs e)
        {
            groupBox.Height = 450;
        }

        private void comboBoxGroup_DropDownOpened(object sender, EventArgs e)
        {
            combobox.SelectedItems.Clear();
            comboboxChief.SelectedItems.Clear();
            comboBoxGroup.ItemsSource = Insert.GetSplitGroup();
            SplitRate.ItemsSource = Insert.GetSplitRate();
            SplitPercent.ItemsSource = Insert.GetSplitPercent();
            SplitPercentWorker.ItemsSource = Insert.GetSplitPercentWorker();
            combobox.ItemsSource = InteractionBD.GetWorkers();
            comboboxChief.ItemsSource = InteractionBD.GetWorkersChief();
        }

        private void comboBoxGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            SplitRate.SelectedValue = InteractionBD.GetRate(comboBoxGroup.SelectedValue.ToString());
            SplitPercent.SelectedValue = InteractionBD.GetPercent(comboBoxGroup.SelectedValue.ToString());
            SplitPercentWorker.SelectedValue = InteractionBD.GetPercentWorker(comboBoxGroup.SelectedValue.ToString());
            //SplitChief.IsEnabled = SplitGroup.SelectedValue.ToString() == "Administrator" ? false:true ;
            SplitRate.IsEnabled = comboBoxGroup.SelectedValue.ToString() == "Administrator" ? false : true;
            SplitPercent.IsEnabled = comboBoxGroup.SelectedValue.ToString() == "Administrator" ? false : true;
            SplitPercentWorker.IsEnabled = comboBoxGroup.SelectedValue.ToString() == "Administrator" ? false : true;
            combobox.IsEnabled= ((comboBoxGroup.SelectedValue.ToString() == "Administrator")||(comboBoxGroup.SelectedValue.ToString() == "Employee" )) ? false : true;
            comboboxChief.IsEnabled = comboBoxGroup.SelectedValue.ToString() == "Administrator" ? false : true;
           // combobox.IsEnabled = comboBoxGroup.SelectedValue.ToString() == "Employee" ? false : true;
           
        }

       

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            InteractionBD.DeleteWorker(textBoxDel.Text);
               dataGrid2.ItemsSource =InteractionBD.ShowWorker();
            labelStatus.Foreground = Brushes.Green;
            labelStatus.Content = "Success";
        }

        private void dataGrid2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                labelStatus.Content = "";
                var ci = new DataGridCellInfo(dataGrid2.Items[dataGrid2.SelectedIndex], dataGrid2.Columns[0]);

                var content = ci.Column.GetCellContent(ci.Item) as TextBlock;

                textBoxDel.Text = content.Text;
                textBoxEditFio.Text = content.Text;
                comboboxSubEdit.ItemsSource = InteractionBD.GetWorkersSubEdit(textBoxEditFio.Text);
                comboboxChiefEdit.ItemsSource = InteractionBD.GetWorkersChiefEdit(textBoxEditFio.Text);
            }
            catch { };
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            this.ShowMessageAsync("Внимание!", "Пункт доделан не полностью,работает только просмотр начальников и подчинненых.");
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Height = 420;
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            groupBox.Height = 400;
        }
    }
}
