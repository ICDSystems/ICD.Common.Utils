#if SIMPLSHARP
using Crestron.SimplSharp;
#else
using System.Net.Mail;
#endif
using ICD.Common.Properties;

namespace ICD.Common.Utils.Email
{
	public sealed class EmailClient
	{
		private readonly EmailStringCollection m_To;
		private readonly EmailStringCollection m_Cc;
		private readonly EmailStringCollection m_Bcc;

		#region Properties

		public string Host { get; set; }
		public ushort Port { get; set; }
		public bool Secure { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string From { get; set; }
		public EmailStringCollection To { get { return m_To; } }
		public EmailStringCollection Cc { get { return m_Cc; } }
		public EmailStringCollection Bcc { get { return m_Bcc; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public EmailClient()
		{
			m_To = new EmailStringCollection();
			m_Cc = new EmailStringCollection();
			m_Bcc = new EmailStringCollection();
		}

		#region Methods

		/// <summary>
		/// Sends the email.
		/// </summary>
		/// <param name="subject"></param>
		/// <param name="message"></param>
		/// <returns>Error codes.</returns>
		[PublicAPI]
		public eMailErrorCode Send(string subject, string message)
		{
#if SIMPLSHARP
			var response = CrestronMailFunctions.SendMail(Host, Port, Secure, Username, Password, From, m_To.ToString(), "",
														  m_Cc.ToString(), m_Bcc.ToString(), subject, message,
			                                              CrestronMailFunctions.eMailPriority.Normal, 0, "");

			return MailErrorCodeUtils.FromSimplMailCode(response);
#else
			try
			{
				MailMessage mailMessage = new MailMessage
				{
					From = new MailAddress(From),
					To = {m_To.ToString()},
					CC = {m_Cc.ToString()},
					Bcc = {m_Bcc.ToString()},
					Subject = subject,
					Body = message
				};

				SmtpClient client = new SmtpClient
				{
					Port = Port,
					DeliveryMethod = SmtpDeliveryMethod.Network,
					UseDefaultCredentials = false,
					Host = Host
				};

				client.Send(mailMessage);
			}
			catch (SmtpException ex)
			{
				return MailErrorCodeUtils.FromNetStandardMailCode(ex.StatusCode);
			}

			return eMailErrorCode.Ok;
#endif
		}

		#endregion
	}
}
