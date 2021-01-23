using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.Common;
using MySql.Data.MySqlClient;
using ConnectionDB;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MSG_by_AL__XAML_
{
    /// <summary>
    /// Логика взаимодействия для SignUpWindow.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        //Объект нашего соединения
        MySqlConnection connection = DBUtils.GetDBConnection();

        public SignUpWindow()
        {
            InitializeComponent();
        }

        //Регистрация нового пользователя
        private void SignUp_Click(object sender, RoutedEventArgs e)
        { 
            try
            {
                //Открываем соединение 
                connection.Open();

                //Создаём строку запроса
                string sql_cmd = "INSERT INTO server_chats.users (User_Name, User_Password, User_Nickname) VALUES (@NICKNAME,@PASSWORD,@LOGIN);";
                //Создаём объект запроса
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = sql_cmd;

                //Добавляем параметры запроса
                MySqlParameter login_parameter = new MySqlParameter("@LOGIN", MySqlDbType.VarChar);
                login_parameter.Value = login_text.Text;
                cmd.Parameters.Add(login_parameter);

                MySqlParameter password_parameter = new MySqlParameter("@PASSWORD", MySqlDbType.VarChar);
                password_parameter.Value = password_text.Password;
                cmd.Parameters.Add(password_parameter);

                MySqlParameter nickname_parameter = new MySqlParameter("@NICKNAME", MySqlDbType.VarChar);
                nickname_parameter.Value = name_text.Text;
                cmd.Parameters.Add(nickname_parameter);

                //Выполняем запрос
                cmd.ExecuteNonQuery();

                //Если запрос успешно выполнен, то выведется соответствующее сообщение
                //Иначе выпадет исключение
                MessageBox.Show("Пользователь успешно зарегистрирован!", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            catch (MySqlException ex)
            {
                //Выводим исключение, если таковое имеется
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                //Закрываем соединение
                connection.Close();
            }
        }

        private void PB_MouseMove(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
