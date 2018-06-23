using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Company
{
    public class MainModel : ViewModelBase
    {
        private ObservableCollection<Worker> _items;
        private ObservableCollection<Worker> _items2;
        private List<Worker> _selectedItems;
        private List<Worker> _selectedItems2;


        public ObservableCollection<Worker> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                NotifyPropertyChanged("Items");
            }
        }
        public ObservableCollection<Worker> Items2
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                NotifyPropertyChanged("Items2");
            }
        }

        public List<Worker> Workers
        {
            get
            {
                return _selectedItems;
            }
            set
            {
                _selectedItems = value;
                NotifyPropertyChanged("SelectedItems");
            }
        }
        public List<Worker> Workers2
        {
            get
            {
                return _selectedItems2;
            }
            set
            {
                _selectedItems2 = value;
                NotifyPropertyChanged("SelectedItems");
            }
        }




        public MainModel()
        {
            var items = new ObservableCollection<Worker>();
            var items2 = new ObservableCollection<Worker>();

            var workers = new List<Worker>();
            var workerChief =new List<Worker>();
            

            Workers = workers;
            Workers2 = workerChief;
        }

        private void Submit()
        {

        }


    }


    public class Worker : ViewModelBase
    {

        private int _Id;
        public int Id
        {
            get { return _Id; }

            set
            {
                _Id = value;
                NotifyPropertyChanged("Id");
            }
        }
        private string _Name;
        public string Name
        {
            get { return _Name; }

            set
            {
                _Name = value;
                NotifyPropertyChanged("Name");
            }
        }

    }
}

