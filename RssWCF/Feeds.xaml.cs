using System;
using System.Linq;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using RssWCF.Web;
using System.Collections;
using System.Collections.Generic;
using RssWCF.RssServiceReference;

namespace RssWCF
{
    public partial class Feeds : UserControl
    {
        /// <summary>
        /// Ссылка на экземпляр приложения
        /// </summary>
        public App App
        {
            get { return (App)Application.Current; }
        }
        public RssViewModel RssViewModel
        {
            get { return this.App.RssViewModel; }
        }
        private MainPage mainPage;

        /// <summary>
        /// Ссылка на экземпляр контекста доступа к данным
        /// </summary>
        public RssServiceClient RssServiceClient
        {
            get { return this.App.RssServiceClient; }
        }

        public Feeds()
        {
            mainPage = this.App.RootVisual as MainPage;
            // Required to initialize variables
            InitializeComponent();
        }

        private void On_AddFeed_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            mainPage.navFrame.Navigate(new Uri("addFeed", UriKind.Relative));
        }

        private void OnFeedDeleteClick(object sender, RoutedEventArgs e)
        {
            int selectedIndex = DataGridRow.GetRowContainingElement(sender as FrameworkElement).GetIndex();
            Feed deleteFeed = RssViewModel.Feeds.ElementAt(selectedIndex);
            RssServiceClient.RemoveFeedAsync(deleteFeed.ID);
        }

        private void onFeedSelectingChanged(object sender, SelectionChangedEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    if ((sender as DataGrid).SelectedItem != null)
                    {
                        int selectedIndex = (sender as DataGrid).SelectedIndex;
                        Feed selectedFeed = RssViewModel.Feeds.ElementAt(selectedIndex);
                        mainPage.NewsDataGrid.SelectedIndex = 0;
                        RssViewModel.SelectedFeed = selectedFeed;
                        RssViewModel.SelectedNews = RssViewModel.SelectedFeed.News.FirstOrDefault();
                    }
                    else
                    {
                        RssViewModel.SelectedFeed = null;
                        RssViewModel.SelectedNews = null;
                    }
                });
        }

    }
}