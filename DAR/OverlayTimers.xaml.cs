using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Globalization;

namespace DAR
{
    /// <summary>
    /// Interaction logic for OverlayTimers.xaml
    /// </summary>
    public class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TimeSpan t = TimeSpan.FromSeconds(System.Convert.ToInt32(value.ToString()));
            return t.ToString();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public partial class OverlayTimers : Window
    {
        public ObservableCollection<TriggerTimer> timerBars = new ObservableCollection<TriggerTimer>();
        public double fSize;
        public OverlayTimers()
        {
            InitializeComponent();
            var listener = OcPropertyChangedListener.Create(timerBars);
            listener.PropertyChanged += Listener_PropertyChanged;
            AddTimer("Timer2", 5, true);
            AddTimer("Timer1", 6, true);
            AddTimer("Timer3", 3, true);
            listviewTimers.ItemsSource = timerBars;
        }
        public void AddTimer(String description, int duration, Boolean type)
        {
            TriggerTimer newTimer = new TriggerTimer();
            newTimer.SetProgress(1, duration);
            newTimer.SetTimer(description, duration, type);
            newTimer.StartTimer();
            timerBars.Add(newTimer);
        }
        private void Listener_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            TriggerTimer s = (TriggerTimer)sender;
            
            if ((s.Direction && (s.Progress.Value == s.TimerDuration)) || (!(s.Direction) && (s.Progress.Value == 0)))
            {
                s.StopTimer();
                timerBars.Remove(s);
            }              
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
              
            }
        }

    }
    #region ObservableCollectionListener
    public class OcPropertyChangedListener<T> : INotifyPropertyChanged where T : INotifyPropertyChanged
    {
        private readonly ObservableCollection<T> _collection;
        private readonly string _propertyName;
        private readonly Dictionary<T, int> _items = new Dictionary<T, int>(new ObjectIdentityComparer());
        public OcPropertyChangedListener(ObservableCollection<T> collection, string propertyName = "")
        {
            _collection = collection;
            _propertyName = propertyName ?? "";
            AddRange(collection);
            CollectionChangedEventManager.AddHandler(collection, CollectionChanged);
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddRange(e.NewItems.Cast<T>());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveRange(e.OldItems.Cast<T>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    AddRange(e.NewItems.Cast<T>());
                    RemoveRange(e.OldItems.Cast<T>());
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Reset();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private void AddRange(IEnumerable<T> newItems)
        {
            foreach (T item in newItems)
            {
                if (_items.ContainsKey(item))
                {
                    
                    _items[item]++;
                }
                else
                {
                    _items.Add(item, 1);
                    PropertyChangedEventManager.AddHandler(item, ChildPropertyChanged, _propertyName);
                }
            }
        }

        private void RemoveRange(IEnumerable<T> oldItems)
        {
            foreach (T item in oldItems)
            {
                _items[item]--;
                if (_items[item] == 0)
                {
                    
                    _items.Remove(item);
                    PropertyChangedEventManager.RemoveHandler(item, ChildPropertyChanged, _propertyName);
                }
            }
        }

        private void Reset()
        {
            foreach (T item in _items.Keys.ToList())
            {
                PropertyChangedEventManager.RemoveHandler(item, ChildPropertyChanged, _propertyName);
                _items.Remove(item);
            }
            AddRange(_collection);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void ChildPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(sender, e);
        }

        private class ObjectIdentityComparer : IEqualityComparer<T>
        {
            public bool Equals(T x, T y)
            {
                return object.ReferenceEquals(x, y);
            }
            public int GetHashCode(T obj)
            {
                return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
            }
        }
    }

    public static class OcPropertyChangedListener
    {
        public static OcPropertyChangedListener<T> Create<T>(ObservableCollection<T> collection, string propertyName = "") where T : INotifyPropertyChanged
        {
            return new OcPropertyChangedListener<T>(collection, propertyName);
        }
    }
    #endregion
}
