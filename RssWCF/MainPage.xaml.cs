using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using RssWCF.Web;
using RssWCF.RssServiceReference;
using System.ServiceModel;
using RssWCF.DBRequest;
using System.Net.NetworkInformation;

namespace RssWCF
{
    public partial class MainPage : UserControl
    {
        //public BusyIndicator busyIndicator;
        /// <summary>
        /// Ссылка на экземпляр приложения
        /// </summary>
        public App App
        {
            get { return (App)Application.Current; }
        }

        /// <summary>
        /// Ссылка на экземпляр контекста доступа к данным
        /// </summary>
        public RssServiceClient RssServiceClient
        {
            get { return this.App.RssServiceClient; }
        }

        public RssViewModel RssViewModel
        {
            get { return this.App.RssViewModel; }
        }
        public MainPage()
        {
            this.Unloaded += new RoutedEventHandler(MainPage_Unloaded);
            BindingValidationError += new EventHandler<ValidationErrorEventArgs>(MainPage_BindingValidationError);
            // Required to initialize variables
            InitializeComponent();
        }

        void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using a ChildWindow control.
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                // NOTE: This will allow the application to continue running after an exception has been thrown but not handled. 
                // For production applications this error handling should be replaced with something that will report the error to the website and stop the application.

                ErrorWindow.CreateNew(e.ToString());
            }
        }

        void MainPage_BindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using a ChildWindow control.
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                // NOTE: This will allow the application to continue running after an exception has been thrown but not handled. 
                // For production applications this error handling should be replaced with something that will report the error to the website and stop the application.
                e.Handled = true;
                ErrorWindow.CreateNew(e.ToString());
            }
        }

        private void OnDeleteAllNewsClick(object sender, RoutedEventArgs e)
        {
            Request.Invoke(() =>
                {
                    RssServiceClient.RemoveAllNewsAsync();
                    navFrame.Navigate(new Uri("", UriKind.Relative));
                });
        }

        private void OnNewsDeleteClick(object sender, RoutedEventArgs e)
        {
            int selectedIndex = DataGridRow.GetRowContainingElement(sender as FrameworkElement).GetIndex();
            News removeNews = RssViewModel.News.ElementAt(selectedIndex);
            Request.Invoke(() =>
                {
                    RssServiceClient.RemoveNewsAsync(removeNews.ID);
                });
        }

        private void onNewsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = (sender as DataGrid).SelectedItem != null ? (sender as DataGrid).SelectedIndex : 0;
            if ((sender as DataGrid).SelectedItem != null)
            {
                News selectedNews = RssViewModel.SelectedFeed.News.ElementAt(selectedIndex);
                RssViewModel.SelectedNews = selectedNews;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //this.App.SetIsBusy(ApplicationStrings.LoadingDataText);

            if (NetworkInterface.GetIsNetworkAvailable())
            {
                this.RssServiceClient.FetchAllFeedNewsAsync();
            }
            else
            {
                this.RssServiceClient.GetFeedsAsync();
            }
        }

    }
}