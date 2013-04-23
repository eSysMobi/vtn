using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VlastelinClient.Util
{
    public static class ClientMsg
    {
        public static String ErrorService = "Возникла ошибка при соединении с сервером!";
        public static String ErrorUnknown = "Неопознанная ошибка! При повторном появлении ошибки обратитесь к администратору!";

        public static String ErrorNoKKM = "ККМ не подключен";
        public static String ErrorKKM = "Ошибка ККМ";
    }
}
