using System;
using System.Windows;
using RssWCF.Web;
using RssWCF.RssServiceReference;
using RssWCF.DBRequest;
using System.Net.NetworkInformation;
using System.Windows.Controls;

namespace RssWCF
{
	public partial class App : Application
    {
        /// <summary>
        /// Глобальный экземпляр контекста доступа к данным
        /// </summary>
        public RssServiceClient RssServiceClient { get; set; }

        public RssViewModel RssViewModel { get; set; }
        /// <summary>
        /// Задизаблить все окно
        /// </summary>
        /// <param name="message"></param>
		public App()
        {

			this.Startup += this.Application_Startup;
			this.Exit += this.Application_Exit;
			this.UnhandledException += this.Application_UnhandledException;

			InitializeComponent();
		}

        private void Application_Startup(object sender, StartupEventArgs e)
        {

            this.RssServiceClient = new RssServiceClient();

            this.RssViewModel = new RssViewModel();
            this.RssViewModel.IsBusyModel = true;

            this.Resources.Add("RssViewModel", this.RssViewModel);

            // Делаем его доступным для байндинга
            this.Resources.Add("RssServiceClient", this.RssServiceClient);


            // Show some UI to the user while LoadUser is in progress

            this.RootVisual = new MainPage();


        }

        /// <summary>
        /// Initializes the <see cref="Application.RootVisual"/> property.
        /// </summary>

		private void Application_Exit(object sender, EventArgs e)
		{
            this.RssServiceClient.CloseAsync();
		}

		private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            this.RssViewModel.IsBusyModel = false;
            // If the app is running outside of the debugger then report the exception using a ChildWindow control.
            if (!System.Diagnostics.Debugger.IsAttached/*true*/)
            {
                // NOTE: This will allow the application to continue running after an exception has been thrown but not handled. 
                // For production applications this error handling should be replaced with something that will report the error to the website and stop the application.
                e.Handled = true;
                ErrorWindow.CreateNew(e.ExceptionObject);
            }
		}

	}
}