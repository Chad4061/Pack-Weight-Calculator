using Microsoft.Win32;
using Pack_Weight_Calculator.Commands;
using Pack_Weight_Calculator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LumenWorks.Framework.IO.Csv;

namespace Pack_Weight_Calculator.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged

    {
        private ObservableCollection<PackItem> _itemsInInventory;
        private ObservableCollection<PackItem> _itemsInPack;
        private string _filepath;
        private string _filename;
        private PackItem _selectedPackItem;
        private PackItem _selectedInventoryItem;
        private string _packWeight;
        private string _searchItemName;
        private ObservableCollection<PackItem> _backupInventory;

        public string SearchItemName
        {
            get { return _searchItemName; }
            set 
            { 
                _searchItemName = value;
                OnPropertyChanged("SearchItemName");
            }
        }


        public string PackWeight
        {
            get { return _packWeight; }
            set 
            { 
                _packWeight = value;
                OnPropertyChanged("PackWeight");
            }
        }


        public PackItem SelectedPackItem
        {
            get { return _selectedPackItem; }
            set 
            { 
                _selectedPackItem = value;
                OnPropertyChanged("SelectedPackItem");
            }
        } 

        public PackItem SelectedInventoryItem
        {
            get { return _selectedInventoryItem; }
            set 
            { 
                _selectedInventoryItem = value;
                OnPropertyChanged("SelectedInventoryItem");
            }
        }


        public string Filename
        {
            get { return _filename; }
            set 
            { 
                _filename = value;
                OnPropertyChanged("Filename");
            }
        }
        public string Filepath
        {
            get { return _filepath; }
            set
            {
                _filepath = value;
                OnPropertyChanged("Filepath");
            }
        }
        public ObservableCollection<PackItem> ItemsInInventory
        {
            get { return _itemsInInventory; }
            set
            {
                _itemsInInventory = value;
                OnPropertyChanged("ItemsInInventory");
            }
        }
        public ObservableCollection<PackItem> ItemsInPack
        {
            get { return _itemsInPack; }
            set
            {
                _itemsInPack = value;
                OnPropertyChanged("ItemsInPack");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SimpleCommand BrowseCommand { get; set; }
        public SimpleCommand AddItemToPackCommand { get; set; }
        public SimpleCommand RemoveItemFromPackCommand { get; set; }
        public SimpleCommand SearchInventoryCommand { get; set; }
        public SimpleCommand ClearSearchCommand { get; set; }

        public MainViewModel()
        {
            _itemsInInventory = new ObservableCollection<PackItem>();
            _itemsInPack = new ObservableCollection<PackItem>();
            _backupInventory = new ObservableCollection<PackItem>();
            BrowseCommand = new SimpleCommand(this, "BrowseButton");
            AddItemToPackCommand = new SimpleCommand(this, "AddToPackButton");
            RemoveItemFromPackCommand = new SimpleCommand(this, "RemoveFromPackButton");
            SearchInventoryCommand = new SimpleCommand(this, "SearchInventory");
            ClearSearchCommand = new SimpleCommand(this, "ClearSearch");
        }

        public void OpenFileBrowserDialog()
        {
            Filepath = string.Empty;

            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "CSV files (*.csv)|*.csv";

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                Filepath = openFileDialog.FileName;
                Filename = Path.GetFileName(Filepath);
                ParseFileToLoadInventory();
            }
        }

        public void ParseFileToLoadInventory()
        {
            using (CsvReader reader = new CsvReader(new StreamReader(Filepath), true))
            {
                string[] headers = reader.GetFieldHeaders();
                while (reader.ReadNextRecord())
                {
                   ItemsInInventory.Add( new PackItem { ItemName = reader[0], ItemWeight = double.Parse(reader[1])});
                }
            }
        }

        public void AddItemToPack()
        {
            if (SelectedInventoryItem == null)
                return;

            ItemsInPack.Add(SelectedInventoryItem);
            ItemsInInventory.Remove(SelectedInventoryItem);
            CalculatePackWeight();
            
            Console.WriteLine("Added Item to Pack");
        }

        public void RemoveItemFromPack()
        {
            if (SelectedPackItem == null)
                return;

            ItemsInInventory.Add(SelectedPackItem);
            ItemsInPack.Remove(SelectedPackItem);
            CalculatePackWeight();
            
            Console.WriteLine("Removed Item from Pack");
        }

        public void CalculatePackWeight()
        {
            var tempWeightOunces = 0.0;
            foreach (var item in ItemsInPack)
            {
                tempWeightOunces += item.ItemWeight;
            }
            PackWeight = (tempWeightOunces/16).ToString("##.##");
            PackWeight += " lbs";
        }

        public void SearchInventory()
        {
            _backupInventory = ItemsInInventory;

            var loopList = new ObservableCollection<PackItem>();

            foreach (var item in ItemsInInventory)
                if (item.ItemName.ToLower().Contains(SearchItemName.ToLower()))
                    loopList.Add(item);

            ItemsInInventory = loopList;
        }

        public void ClearSearch()
        {
            SearchItemName = "";

            var loopList = new ObservableCollection<PackItem>();

            for (int i = 0; i < _backupInventory.Count; i++)
                if (ItemsInPack.Contains(_backupInventory[i]))
                    _backupInventory.Remove(_backupInventory[i]);

            ItemsInInventory = _backupInventory;
        }
    }
}
