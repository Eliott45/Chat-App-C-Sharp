using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChatClient.MVVM.Core
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyname = null)
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyname));
        }
    }
}