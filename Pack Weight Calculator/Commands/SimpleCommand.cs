using Pack_Weight_Calculator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pack_Weight_Calculator.Commands
{
    public class SimpleCommand : ICommand
    {
        public MainViewModel ViewModel { get; set; }
        public string ButtonName { get; set; }
        public SimpleCommand(MainViewModel mainViewModel, string buttonName)
        {
            this.ViewModel = mainViewModel;
            ButtonName = buttonName;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (ButtonName == "BrowseButton")
                ViewModel.OpenFileBrowserDialog();

            if (ButtonName == "AddToPackButton")
                ViewModel.AddItemToPack();

            if (ButtonName == "RemoveFromPackButton")
                ViewModel.RemoveItemFromPack();
        }
    }
}
