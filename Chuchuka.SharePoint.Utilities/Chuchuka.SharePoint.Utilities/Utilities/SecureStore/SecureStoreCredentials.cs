using System.Security;

namespace Chuchuka.SharePoint.Utilities.Utilities.SecureStore
{
	public class SecureStoreCredentials
	{
		public string UserName { get; set; }
		public SecureString Password { get; set; }

		public string WindowsUserName { get; set; }
		public SecureString WindowsPassword { get; set; }

		public string Certificate { get; set; }
		public SecureString CertificatePassword { get; set; }
	}
}
