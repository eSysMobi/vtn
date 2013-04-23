using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using VlastelinClient.ServiceReference1;
using Vlastelin.Data.Model;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace VlastelinClient.Util
{
    /// <summary>
    /// класс для работы с входом пользователя в систему
    /// </summary>
    public static class LoginManager
    {
        /// <summary>
        /// ключ для шифрования строки
        /// </summary>
        private const String key = "k1";

        /// <summary>
		/// кэшированные логин и пароль
		/// используются для переподчллючения к серверу
		/// </summary>
        public static String Login { get; set; }
		public static String Password { get; set; }
		
		/// <summary>
        /// попытка входа в систему
        /// </summary>
        /// <returns>результат - удалось ли оператору войти в систему или нет</returns>
        public static bool AttemptLogin(String login, String password)
        {
			// создаем данные для входа в систему
            ClientCredentials clientCredentials = new ClientCredentials();
			clientCredentials.UserName.UserName = login;
			clientCredentials.UserName.Password = password;
            clientCredentials.ServiceCertificate.Authentication.CertificateValidationMode =
                System.ServiceModel.Security.X509CertificateValidationMode.None;

			Login = login;
            Password = EncryptOrDecrypt(password, key);

            // удаляем старые данные и добавляем новые
            UtilManager.Instance.Client.ChannelFactory.Endpoint.Behaviors.Remove(typeof(ClientCredentials));
            UtilManager.Instance.Client.ChannelFactory.Endpoint.Behaviors.Add(clientCredentials);

            bool result = false;
            FuncExec.Execute(() =>
            {
                UtilManager.Instance.CurrentOperator = UtilManager.Instance.Client.OperatorGetByLogin(login);
				Properties.Settings.Default.LastSuccessfulLogin = Login;
				Properties.Settings.Default.Save();

                result = true;
            });
            return result;
        }

		/// <summary>
		/// шифроварие строки
		/// </summary>
		/// <param name="str">исходная строка</param>
		/// <returns>зашифрованная строка</returns>
        private static string EncryptOrDecrypt(string text, string key)
        {
            var result = new StringBuilder();

            for (int c = 0; c < text.Length; c++)
                result.Append((char)((uint)text[c] ^ (uint)key[c % key.Length]));

            return result.ToString();
        }

        /// <summary>
        /// получает расшифрованный пароль
        /// </summary>
        /// <returns></returns>
        public static String GetPassword()
        {
            return EncryptOrDecrypt(Password, key);
        }
    }
}
