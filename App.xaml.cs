using System;
using System.Reflection;
using System.Windows;
using NLog;

namespace WpfOpenTK
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private readonly Logger mLogger = LogManager.GetLogger( "App" );

		private void Application_Startup( object sender, StartupEventArgs e )
		{
			//MailMessage mailMessage = new MailMessage();
			//mailMessage.To.Add( "someone@somewhere.com" );
			//mailMessage.Subject = "Test";
			//mailMessage.Body = "<html><body>This is a test</body></html>";
			//mailMessage.IsBodyHtml = true;

			//// Create the credentials to login to the gmail account associated with my custom domain
			//string sendEmailsFrom = "marek.suplata@gmail.com";
			//string sendEmailsFromPassword = "bravoechojednajednaavgmHP!";
			//NetworkCredential cred = new NetworkCredential( sendEmailsFrom, sendEmailsFromPassword );

			//SmtpClient mailClient = new SmtpClient( "smtp.gmail.com", 587 );
			//mailClient.EnableSsl = true;
			//mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
			//mailClient.UseDefaultCredentials = false;
			//mailClient.Timeout = 20000;
			//mailClient.Credentials = cred;

			//var message = new MailMessage();
			//// here is an important part:
			//message.From = new MailAddress( "marek.suplata@gmail.com", "Mailer" );
			//// it's superfluous part here since from address is defined in .config file
			//// in my example. But since you don't use .config file, you will need it.

			////mailClient.Send( mailMessage );
			//mailClient.EnableSsl = true;
			//mailClient.Send( message );

			// UI Exceptions
			this.DispatcherUnhandledException += Application_DispatcherUnhandledException;

			// Thread exceptions
			//AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
		}

		// http://snipplr.com/view/24560/logging-in-net-with-nlog-default-config-file-catch-all-exceptions-and-route-to-logger-/

		private void Application_DispatcherUnhandledException( object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e )
		{
			e.Handled = true;
			var exception = e.Exception;
			HandleUnhandledException( exception );
		}

		private void CurrentDomainOnUnhandledException( object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs )
		{
			HandleUnhandledException( unhandledExceptionEventArgs.ExceptionObject as Exception );
			if ( unhandledExceptionEventArgs.IsTerminating )
			{
				mLogger.Info( "Application is terminating due to an unhandled exception in a secondary thread." );
			}
		}

		private void HandleUnhandledException( Exception exception )
		{
			try
			{
				AssemblyName assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
				string message = string.Format( "Unhandled exception in {0} v{1}", assemblyName.Name, assemblyName.Version );
				mLogger.Error( message );
			}
			catch ( Exception exc )
			{
				mLogger.ErrorException( "Exception in unhandled exception handler", exc );
			}
			finally
			{
				mLogger.ErrorException( exception.InnerException.StackTrace, exception );
			}
		}
	}
}