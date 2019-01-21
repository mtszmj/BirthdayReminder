using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace BirthdayReminder
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Return of <code>typeof(xxxView)</code> is handled by the ViewModel when overriding.
        /// </summary>
        protected abstract Type _Window { get; }
        protected Window _View;
        protected Window View
        {
            get
            {
                if (_View != null)
                    return _View;
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == _Window)
                    {
                        _View = window;
                        return _View;
                    }
                }
                _View = Activator.CreateInstance(_Window, this) as Window;
                return _View;
            }
        }

        public void Show()
        {
            View?.Show();
            View?.Activate();
        }

        public void Hide()
        {
            View?.Hide();
            return;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
