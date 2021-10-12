using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Data.Common;
using System.Collections.Generic;
using System.Windows;

namespace MSG_by_AL__XAML_
{
    class ServerConnect
    {
        //Адрес и порт для подключения к серверу
        static int port = 8005;
        static string IP = "10.192.129.233";

        //Пустой конструктор класса
        public ServerConnect()
        {

        }

        //Метод, осуществляющий соединение с сервером и получения от него соответствующих данных
        public static List<string> RecieveDataFromDB(string numberCommand, string parameters = "")
        {
            //Список значений, полученных от сервера
            List<string> values = new List<string>();

            try
            {
                //Создаем удаленную конечную точку сервера
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(IP), port);

                //Определяем объект сокета, для подключения к серверу по удаленной конечной точке
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //Подключаемся к удаленному хосту
                socket.Connect(ipPoint);
                byte[] data = Encoding.Unicode.GetBytes(numberCommand + parameters);
                socket.Send(data);

                // получаем ответ
                data = new byte[8192]; // буфер для ответа
                StringBuilder builder = new StringBuilder();
                int bytes = 0; // количество полученных байт
                    
                do
                {
                    bytes = socket.Receive(data, data.Length, 0);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (socket.Available > 0);

                /*Получаем данные от сервера в виде строки: value1~%value2~%...valueN~% 
                 Обрабатываем данную строку, чтобы разделить значения и добавить их в список*/
                string msg = builder.ToString();

                foreach (string value in msg.Split('~'))
                {
                    if(value != "") values.Add(value);
                }

                // закрываем сокет
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                //Выводим сообщения об возникшем исключении
                MessageBox.Show(ex.Message);
            }
            //Возвращаем список значений для дальнейших действий
            return values;
        }
    }
}
