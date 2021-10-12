using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.Common;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using ConnectionDB;

namespace MSG_by_AL__XAML_
{
    public partial class MainWindow : Window
    {

        //Имя активного пользователя
        public static string NickName = "null";

        //ID активного пользователя
        public static int IDuser = -1;

        //Создание объекта подключения к БД
        MySqlConnection connection = DBUtils.GetDBConnection();
        public MainWindow()
        {
            InitializeComponent();
        }

        //Авторизация пользователя
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    //Открываем соединение 
            //    connection.Open();

            //    //Создаём строку запроса
            //    string sql_cmd = "SELECT server_chats.users.ID, server_chats.users.User_Name, server_chats.users.User_Password FROM server_chats.users WHERE (User_Nickname=@LOGIN AND User_Password=@PASSWORD);";
            //    //Создаём объект запроса
            //    MySqlCommand cmd = connection.CreateCommand();
            //    cmd.CommandText = sql_cmd;

            //    //Добавляем параметры запроса
            //    MySqlParameter login_parameter = new MySqlParameter("@LOGIN", MySqlDbType.VarChar);
            //    login_parameter.Value = login_txt.Text;
            //    cmd.Parameters.Add(login_parameter);

            //    MySqlParameter password_parameter = new MySqlParameter("@PASSWORD", MySqlDbType.VarChar);
            //    password_parameter.Value = password_txt.Password;
            //    cmd.Parameters.Add(password_parameter);

            //    //using используется для того, чтобы объект reader освободил память после использвания
            //    using (DbDataReader reader = cmd.ExecuteReader())
            //    {
            //        //Если в модуле данных для чтения есть хотя бы одна строка
            //        if (reader.HasRows)
            //        {
            //            //Пока происходит перебор всех строк
            //            while (reader.Read())
            //            {
            //                //В возвращаемой строке пароль будет иметь индекс 2
            //                string pass = reader.GetString(2);
            //                //Если пароли совпадают, то открывается следующее окно
            //                if (pass == password_txt.Password)
            //                {
            //                    NickName = login_txt.Text;
            //                    IDuser = int.Parse(reader.GetString(0));
            //                    MessageBox.Show("Авторизация прошла успешно!", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            //                    //Открываем основное окно и передаём в него сведения об авторизованном пользователе
            //                    ChatsPage chatpage = new ChatsPage(IDuser, NickName);
            //                    chatpage.Show();
            //                    this.Close();
            //                }
            //                else MessageBox.Show("Неверный логин или пароль!");
            //            }
            //        }
            //        else MessageBox.Show("Неверный логин или пароль!");
            //    }
            //}
            //catch (MySqlException ex)
            //{
            //    //Выводим исключение, если таковое имеется
            //    MessageBox.Show(ex.ToString());
            //}
            //finally
            //{
            //    //Закрываем соединение
            //    connection.Close();
            //}

            List<string> values = ServerConnect.RecieveDataFromDB("01#", login_txt.Text + "~" + password_txt.Password);
            if (values[0] != "ERROR")
            {
                if (password_txt.Password == values[2] && login_txt.Text == values[1])
                {
                    NickName = login_txt.Text;
                    IDuser = int.Parse(values[0]);
                    MessageBox.Show("Авторизация прошла успешно!", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    //Открываем основное окно и передаём в него сведения об авторизованном пользователе
                    ChatsPage chatpage = new ChatsPage(IDuser, NickName);
                    chatpage.Show();
                    this.Close();
                }
            }
            else MessageBox.Show("Неверный логин или пароль");
        }

        //Открываем окно регистрации пользователя
        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            SignUpWindow Form = new SignUpWindow();
            Form.Show();
            this.Close();
        }
    }
}
