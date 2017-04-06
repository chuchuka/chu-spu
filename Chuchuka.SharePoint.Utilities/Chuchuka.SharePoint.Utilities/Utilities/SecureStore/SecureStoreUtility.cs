using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.BusinessData.Infrastructure.SecureStore;
using Microsoft.Office.SecureStoreService.Server;
using Microsoft.SharePoint;

namespace Chuchuka.SharePoint.Utilities.Utilities.SecureStore
{
	public static class SecureStoreUtility
	{
		/// <summary>
		/// Get credentials from Secure Store Service
		/// </summary>
		/// <param name="targetAppId">Target Application ID for the Secure Store</param>
		/// <param name="site"></param>
		/// <returns>Object of NetworkCredential class. This class provides credentials for password-based authentication schemes such as basic, digest, NTLM, and Kerberos authentication.</returns>
		public static SecureStoreCredentials GetCredentials(string targetAppId, SPSite site)
		{
			// Get the default Secure Store Service provider.
			ISecureStoreProvider provider = SecureStoreProviderFactory.Create();
			if (provider == null)
				throw new Exception("Unable to get an ISecureStoreProvider.");

			ISecureStoreServiceContext providerContext = provider as ISecureStoreServiceContext;
			if (providerContext != null) providerContext.Context = SPServiceContext.GetContext(site);

			var credentials = new SecureStoreCredentials();
			using (SecureStoreCredentialCollection credentialCollection = provider.GetCredentials(targetAppId))
			{
				foreach (ISecureStoreCredential credential in credentialCollection)
				{
					switch (credential.CredentialType)
					{
						case SecureStoreCredentialType.UserName:
							credentials.UserName = GetStringFromSecureString(credential.Credential);
							break;
						case SecureStoreCredentialType.Password:
							credentials.Password = credential.Credential;
							break;
						case SecureStoreCredentialType.WindowsUserName:
							credentials.WindowsUserName = GetStringFromSecureString(credential.Credential);
							break;
						case SecureStoreCredentialType.WindowsPassword:
							credentials.WindowsPassword = credential.Credential;
							break;
						case SecureStoreCredentialType.Certificate:
							credentials.Certificate = GetStringFromSecureString(credential.Credential);
							break;
						case SecureStoreCredentialType.CertificatePassword:
							credentials.CertificatePassword = credential.Credential;
							break;
					}
				}
			}
			return credentials;
		}

		/// <summary>
		/// Get credentials from Secure Store Service
		/// </summary>
		/// <param name="targetAppId">Target Application ID for the Secure Store</param>
		/// <param name="site"></param>
		/// <returns>Object of NetworkCredential class. This class provides credentials for password-based authentication schemes such as Basic, Digest, NTLM, and Kerberos authentication.</returns>
		public static NetworkCredential GetNetworkCredentials(string targetAppId, SPSite site)
		{
			SecureStoreCredentials credentials = GetCredentials(targetAppId, site);
			return new NetworkCredential(credentials.UserName, credentials.Password);
		}

		private static string GetStringFromSecureString(SecureString secStr)
		{
			if (secStr == null) return null;

			IntPtr pPlainText = IntPtr.Zero;
			try
			{
				pPlainText = Marshal.SecureStringToBSTR(secStr);
				return Marshal.PtrToStringBSTR(pPlainText);
			}
			finally
			{
				if (pPlainText != IntPtr.Zero)
				{
					Marshal.FreeBSTR(pPlainText);
				}
			}
		}
	}
}
