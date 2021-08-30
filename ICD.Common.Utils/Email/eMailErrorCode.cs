using System;
#if SIMPLSHARP
using Crestron.SimplSharp;
#else
using System.Net.Mail;
#endif

namespace ICD.Common.Utils.Email
{
	public enum eMailErrorCode
	{
		//
		// Summary:
		//     The transaction could not occur. You receive this error when the specified SMTP
		//     host cannot be found.
		GeneralFailure = -1,
		//
		// Summary:
		//     A system status or system Help reply.
		SystemStatus = 211,
		//
		// Summary:
		//     A Help message was returned by the service.
		HelpMessage = 214,
		//
		// Summary:
		//     The SMTP service is ready.
		ServiceReady = 220,
		//
		// Summary:
		//     The SMTP service is closing the transmission channel.
		ServiceClosingTransmissionChannel = 221,
		//
		// Summary:
		//     The email was successfully sent to the SMTP service.
		Ok = 250,
		//
		// Summary:
		//     The user mailbox is not located on the receiving server; the server forwards
		//     the e-mail.
		UserNotLocalWillForward = 251,
		//
		// Summary:
		//     The specified user is not local, but the receiving SMTP service accepted the
		//     message and attempted to deliver it. This status code is defined in RFC 1123,
		//     which is available at http://www.ietf.org/.
		CannotVerifyUserWillAttemptDelivery = 252,
		//
		// Summary:
		//     The SMTP service is ready to receive the e-mail content.
		StartMailInput = 354,
		//
		// Summary:
		//     The SMTP service is not available; the server is closing the transmission channel.
		ServiceNotAvailable = 421,
		//
		// Summary:
		//     The destination mailbox is in use.
		MailboxBusy = 450,
		//
		// Summary:
		//     The SMTP service cannot complete the request. This error can occur if the client's
		//     IP address cannot be resolved (that is, a reverse lookup failed). You can also
		//     receive this error if the client domain has been identified as an open relay
		//     or source for unsolicited e-mail (spam). For details, see RFC 2505, which is
		//     available at http://www.ietf.org/.
		LocalErrorInProcessing = 451,
		//
		// Summary:
		//     The SMTP service does not have sufficient storage to complete the request.
		InsufficientStorage = 452,
		//
		// Summary:
		//     The client was not authenticated or is not allowed to send mail using the specified
		//     SMTP host.
		ClientNotPermitted = 454,
		//
		// Summary:
		//     The SMTP service does not recognize the specified command.
		CommandUnrecognized = 500,
		//
		// Summary:
		//     The syntax used to specify a command or parameter is incorrect.
		SyntaxError = 501,
		//
		// Summary:
		//     The SMTP service does not implement the specified command.
		CommandNotImplemented = 502,
		//
		// Summary:
		//     The commands were sent in the incorrect sequence.
		BadCommandSequence = 503,
		//
		// Summary:
		//     The SMTP service does not implement the specified command parameter.
		CommandParameterNotImplemented = 504,
		//
		// Summary:
		//     The SMTP server is configured to accept only TLS connections, and the SMTP client
		//     is attempting to connect by using a non-TLS connection. The solution is for the
		//     user to set EnableSsl=true on the SMTP Client.
		MustIssueStartTlsFirst = 530,
		//
		// Summary:
		//     The destination mailbox was not found or could not be accessed.
		MailboxUnavailable = 550,
		//
		// Summary:
		//     The user mailbox is not located on the receiving server. You should resend using
		//     the supplied address information.
		UserNotLocalTryAlternatePath = 551,
		//
		// Summary:
		//     The message is too large to be stored in the destination mailbox.
		ExceededStorageAllocation = 552,
		//
		// Summary:
		//     The syntax used to specify the destination mailbox is incorrect.
		MailboxNameNotAllowed = 553,
		//
		// Summary:
		//     The transaction failed.
		TransactionFailed = 554
	}

	public static class MailErrorCodeUtils
	{
#if !SIMPLSHARP
		public static eMailErrorCode FromNetStandardMailCode(SmtpStatusCode code)
		{
			switch (code)
			{
				case SmtpStatusCode.BadCommandSequence:
					return eMailErrorCode.BadCommandSequence;
				case SmtpStatusCode.CannotVerifyUserWillAttemptDelivery:
					return eMailErrorCode.CannotVerifyUserWillAttemptDelivery;
				case SmtpStatusCode.ClientNotPermitted:
					return eMailErrorCode.ClientNotPermitted;
				case SmtpStatusCode.CommandNotImplemented:
					return eMailErrorCode.CommandNotImplemented;
				case SmtpStatusCode.CommandParameterNotImplemented:
					return eMailErrorCode.CommandParameterNotImplemented;
				case SmtpStatusCode.CommandUnrecognized:
					return eMailErrorCode.CommandUnrecognized;
				case SmtpStatusCode.ExceededStorageAllocation:
					return eMailErrorCode.ExceededStorageAllocation;
				case SmtpStatusCode.GeneralFailure:
					return eMailErrorCode.GeneralFailure;
				case SmtpStatusCode.HelpMessage:
					return eMailErrorCode.HelpMessage;
				case SmtpStatusCode.InsufficientStorage:
					return eMailErrorCode.InsufficientStorage;
				case SmtpStatusCode.LocalErrorInProcessing:
					return eMailErrorCode.LocalErrorInProcessing;
				case SmtpStatusCode.MailboxBusy:
					return eMailErrorCode.MailboxBusy;
				case SmtpStatusCode.MailboxNameNotAllowed:
					return eMailErrorCode.MailboxNameNotAllowed;
				case SmtpStatusCode.MailboxUnavailable:
					return eMailErrorCode.MailboxUnavailable;
				case SmtpStatusCode.MustIssueStartTlsFirst:
					return eMailErrorCode.MustIssueStartTlsFirst;
				case SmtpStatusCode.Ok:
					return eMailErrorCode.Ok;
				case SmtpStatusCode.ServiceClosingTransmissionChannel:
					return eMailErrorCode.ServiceClosingTransmissionChannel;
				case SmtpStatusCode.ServiceNotAvailable:
					return eMailErrorCode.ServiceNotAvailable;
				case SmtpStatusCode.ServiceReady:
					return eMailErrorCode.ServiceReady;
				case SmtpStatusCode.StartMailInput:
					return eMailErrorCode.StartMailInput;
				case SmtpStatusCode.SyntaxError:
					return eMailErrorCode.SyntaxError;
				case SmtpStatusCode.SystemStatus:
					return eMailErrorCode.SystemStatus;
				case SmtpStatusCode.TransactionFailed:
					return eMailErrorCode.TransactionFailed;
				case SmtpStatusCode.UserNotLocalTryAlternatePath:
					return eMailErrorCode.UserNotLocalTryAlternatePath;
				case SmtpStatusCode.UserNotLocalWillForward:
					return eMailErrorCode.UserNotLocalWillForward;
				default:
					throw new ArgumentOutOfRangeException(nameof(code), code, null);
			}
		}
#else
		public static eMailErrorCode FromSimplMailCode(CrestronMailFunctions.SendMailErrorCodes code)
		{
			switch (code)
			{
				case CrestronMailFunctions.SendMailErrorCodes.SMTP_OK:
					return eMailErrorCode.Ok;
				case CrestronMailFunctions.SendMailErrorCodes.SMTP_ERROR_FATAL:
					return eMailErrorCode.GeneralFailure;
				case CrestronMailFunctions.SendMailErrorCodes.SMTP_ERROR_ILLEGAL_CMD:
					return eMailErrorCode.CommandUnrecognized;
				case CrestronMailFunctions.SendMailErrorCodes.SMTP_ERROR_CONNECT:
					return eMailErrorCode.ServiceNotAvailable;
				case CrestronMailFunctions.SendMailErrorCodes.SMTP_ERROR_SEND:
					return eMailErrorCode.GeneralFailure;
				case CrestronMailFunctions.SendMailErrorCodes.SMTP_ERROR_RECV:
					return eMailErrorCode.GeneralFailure;
				case CrestronMailFunctions.SendMailErrorCodes.SMTP_ERROR_NU_CONNECT:
					return eMailErrorCode.GeneralFailure;
				case CrestronMailFunctions.SendMailErrorCodes.SMTP_ERROR_NU_BUFFERS:
					return eMailErrorCode.GeneralFailure;
				case CrestronMailFunctions.SendMailErrorCodes.SMTP_ERROR_AUTHENTICATION:
					return eMailErrorCode.GeneralFailure;
				case CrestronMailFunctions.SendMailErrorCodes.SMTP_ERROR_AUTH_LOGIN_UNSUPPORTED:
					return eMailErrorCode.MustIssueStartTlsFirst;
				case CrestronMailFunctions.SendMailErrorCodes.SMTP_INV_PARAM:
					return eMailErrorCode.SyntaxError;
				case CrestronMailFunctions.SendMailErrorCodes.SMTP_ETHER_NOT_ENABLED:
					return eMailErrorCode.ServiceNotAvailable;
				case CrestronMailFunctions.SendMailErrorCodes.SMTP_NO_SERVER_ADDRESS:
					return eMailErrorCode.ServiceNotAvailable;
				case CrestronMailFunctions.SendMailErrorCodes.SMTP_SEND_FAILED:
					return eMailErrorCode.GeneralFailure;
				case CrestronMailFunctions.SendMailErrorCodes.SMTP_GENERAL_SENDMAIL_ERROR:
					return eMailErrorCode.GeneralFailure;
				case CrestronMailFunctions.SendMailErrorCodes.SMTP_INVALID_FIRMWARE:
					return eMailErrorCode.GeneralFailure;
				default:
					throw new ArgumentOutOfRangeException("code");
			}
		}
#endif
	}
}
	