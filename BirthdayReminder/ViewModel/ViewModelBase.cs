using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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

        public void Show()
        {
            foreach (Window window in Application.Current.Windows)
            {
                window.GetType();
                
                if (window.GetType() == _Window)
                {
                    window.Activate();
                    return;
                }
            }
            var view = Activator.CreateInstance(_Window, this) as Window;
            view.Show();
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
