using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.ComponentModel;
using RssWCF.RssServiceReference;
using System.Collections.ObjectModel;
using RssWCF.DBRequest;
using System.Windows.Controls;

namespace RssWCF.Web
{
    public class RssViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private Feed _SelectedFeed;
        private News _SelectedNews;
        private ObservableCollection<Feed> _Feeds;
        private ObservableCollection<News> _News;
        private bool isbusy;
        public App App
        {
            get { return (App)Application.Current; }
        }

        public RssServiceClient client
        {
            get { return this.App.RssServiceClient; }
        }

        public ObservableCollection<Feed> Feeds
        {
            get
            {
                return _Feeds;
            }
            set
            {
                _Feeds = value;
                NotifyPropertyChanged("Feeds");
            }
        }

        public ObservableCollection<News> News
        {
            get
            {
                return _News;
            }
            set
            {
                _News = value;
                NotifyPropertyChanged("News");
            }
        }
        public Feed SelectedFeed
        {
            get
            {
                return _SelectedFeed;
            }
            set
            {
                _SelectedFeed = value;
                NotifyPropertyChanged("SelectedFeed");
            }
        }
        public News SelectedNews
        {
            get
            {
                return _SelectedNews;
            }
            set
            {
                _SelectedNews = value;
                NotifyPropertyChanged("SelectedNews");
            }
        }
        public bool IsBusyModel
        {
            get { return isbusy; }
            set
            {
                isbusy = value;
                NotifyPropertyChanged("IsBusyModel");
            }
        }
        public RssViewModel()
        {
            client.AddFeedCompleted += new EventHandler<AsyncCompletedEventArgs>(client_AddRemoveFeedCompleted);
            client.RemoveFeedCompleted += new EventHandler<AsyncCompletedEventArgs>(client_AddRemoveFeedCompleted);
            client.FetchAllFeedNewsCompleted += new EventHandler<AsyncCompletedEventArgs>(client_FetchAllFeedNewsCompleted);
            client.RemoveAllNewsCompleted += new EventHandler<AsyncCompletedEventArgs>(client_AddRemoveFeedCompleted);
            client.GetFeedsCompleted += new EventHandler<GetFeedsCompletedEventArgs>(client_GetFeedsCompleted);
        }

        void client_FetchAllFeedNewsCompleted(object sender, AsyncCompletedEventArgs e)
        {
            IsBusyModel = true;
            (this.App.RootVisual as MainPage).newsContent.Visibility = Visibility.Visible;

            client.GetFeedsAsync();
        }

        void client_GetFeedsCompleted(object sender, GetFeedsCompletedEventArgs e)
        {
            Request.Invoke(() =>
            {
                if (e.Result.Count != 0)
                {
                    this.Feeds = e.Result;
                    this.News = this.Feeds.FirstOrDefault().News;
                    this.SelectedFeed = this.Feeds.FirstOrDefault();
                    this.SelectedNews = this.News.FirstOrDefault();
                }
                IsBusyModel = false;
            });
        }

        void client_AddRemoveFeedCompleted(object sender, AsyncCompletedEventArgs e)
        {
            IsBusyModel = true;
            Request.Invoke(() =>
                {
                    client.GetFeedsAsync();
                });
        }
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }

    public class FeedToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = String.Format(" Feed Name = {0}  Building Date =  {1}", value != null ? ((Feed)value).Name.TrimEnd() : "No Feeds", value != null ? ((Feed)value).LastTimeUpdate.ToString() : "No Data");
            return result;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
