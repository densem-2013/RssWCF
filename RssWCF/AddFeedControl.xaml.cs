using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using RssWCF.Web;
using System.ServiceModel.DomainServices.Client;
using RssWCF.RssServiceReference;

namespace RssWCF
{
    public partial class AddFeedControl : UserControl
    {
        /// <summary>
        /// Ссылка на экземпляр приложения
        /// </summary>
        protected App App
        {
            get { return (App)Application.Current; }
        }


        private MainPage mainPage;
        /// <summary>
        /// Ссылка на экземпляр контекста доступа к данным
        /// </summary>
        protected RssServiceClient RssServiceClient
        {
            get { return this.App.RssServiceClient; }
        }

        public AddFeedControl()
        {
            mainPage = this.App.RootVisual as MainPage;
            // Required to initialize variables
            InitializeComponent();
        }

        private void OnLoad(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            AnimLayout.Begin();
        }

        private void On_OK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            RssServiceClient.AddFeedAsync(TB_Name.Text, TB_URL.Text );
            mainPage.navFrame.Navigate(new Uri("", UriKind.Relative));
        }

        private void On_Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            mainPage.navFrame.Navigate(new Uri("", UriKind.Relative));
        }
    }
}