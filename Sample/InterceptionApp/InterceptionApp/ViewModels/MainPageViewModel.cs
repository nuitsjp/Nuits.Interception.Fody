using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using InterceptionApp.Models;
using Xamarin.Forms;

namespace InterceptionApp.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly Calculator _calculator = new Calculator();
        private int _value1 = 1;

        public int Value1
        {
            get => _value1;
            set => SetProperty(ref _value1, value);
        }

        private int _value2 = 2;

        public int Value2
        {
            get => _value2;
            set => SetProperty(ref _value2, value);
        }

        private int? _result;

        public int? Result
        {
            get => _result;
            set => SetProperty(ref _result, value);
        }

        public ICommand AddCommand => new Command(Add);

        private void Add()
        {
            Result = _calculator.Add(Value1, Value2);
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if(Equals(field, value)) return false;

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
        #endregion
    }
}
