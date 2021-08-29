using System;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Runtime.CompilerServices;

namespace com.asteriasgi
{
	public class Asterisk
	{
		private TcpClient asterisk = new TcpClient ();
		public String host = "127.0.0.1";
		public int port = 5038;
		public string logFile;

		private chaosweb.utils.logger log;
		private Boolean logging;
		private StreamReader Reader;
		private StreamWriter Writer;

		public Asterisk (string host, int port)
		{
			this.host = host;
			this.port = port;
		}

		public Asterisk (string host, int port, String logFile)
		{
			this.host = host;
			this.port = port;
			if (!String.IsNullOrEmpty(logFile)) {
				logging = true;
				this.logFile = logFile;
				log = new chaosweb.utils.logger (logFile);
			}
		}

		public Boolean Connect ()
		{
			logThis(string.Format("Connecting to {0}:{1}", host, port));
			try {
				asterisk.Connect(host, port);
			} catch (Exception e) {
				logThis (string.Format ("Failed to connect due to error: {0}", e.ToString ()));
				throw new Exception ("Failed to Connect", e);
			}

			if (asterisk.Connected) {
				logThis(string.Format("Connected to {0}:{1}", host, port));
				Reader = new StreamReader (asterisk.GetStream());
				Writer = new StreamWriter (asterisk.GetStream());
				return true;
			} else {
				return false;
			}
		}
		private String readline ()
		{
			try {
				String line = Reader.ReadLine ();
				return line.ToString ();
			} catch (Exception e) {
				logThis(string.Format ("An error occured while reading: {0}", e.ToString()));
				throw new Exception ("Failed to read", e);
			}
		}

		private Boolean writeline (string message)
		{
			try {
				Writer.WriteLine(message);
				Writer.Flush();
				return true;
			} catch (Exception e) {
				logThis (string.Format("An error occured while writing: {0}",e.ToString()));
				throw new Exception ("Failed to write", e);
			}
		}

		public Boolean Login (string username, string secret)
		{
			String message = readline ();

			logThis(string.Format("Received banner: {0}", message));
				
			if (!message.Contains ("Asterisk")) {
				return false;
			}
			
			logThis(string.Format("Sending login.  Username: {0}, Secret: REDACTED", username));
			writeline ("Action: Login");
			writeline (string.Format ("Username: {0}", username));
			writeline (string.Format ("Secret: {0}", secret));
			writeline ("Events: off\n");
			
			message = readline ();
			logThis(string.Format("Got response: {0}", message));
			
			if (message.Contains("Succe")) {
					return true;
			} else {
					return false;
			}
		}

		public Boolean Originate (string device, string dial_number)
		{
			return false;
		}

		public Boolean Originate (int exten, string dial_number)
		{
			return false;
		}

		private void logThis (String message,	[CallerFilePath] string callerSourceFile = null,
												[CallerMemberName] string callerMember = null,
												[CallerLineNumber] int callerLineNumber = 0)
		{
			if(logging) {
				log.WriteEntry(message, callerSourceFile, callerMember, callerLineNumber);
			}
		}
	}
}

