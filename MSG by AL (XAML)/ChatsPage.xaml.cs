﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.Common;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using ConnectionDB;
using MSG_by_AL__XAML_.Resource;

namespace MSG_by_AL__XAML_
{
    /// <summary>
    /// Логика взаимодействия для ChatsPage.xaml
    /// </summary>
    public partial class ChatsPage : Window
    {
        //Логин авторизованного пользователя
        public static string NickName = "null";
        //ID авторизованного пользователя
        public static int IDuser = -1;

        //ID и никнейм собеседника
        public static int IDFriend = -1;
        public static string Friend_Nick="null";

        //ID активного диалога и количество сообщений в нём
        public static int IDChat = -1;
        public static int MessageCount = -1;


        //Создание объекта подключения к БД
        MySqlConnection connection = DBUtils.GetDBConnection();

        //Очистка всех списков (сообщений, чатов, пользователей)
        public void Clear_List()
        {
            User_List.Items.Clear();
            Message_List.Items.Clear();
            Chat_list.Items.Clear();
        }

        public ChatsPage(int ID, string login)
        {
            IDuser = ID;
            NickName = login;
            InitializeComponent();
            Clear_List();
        }

        //Закрытие основного окна
        private void ChatPage_Closing(object sender, EventArgs e)
        {
                IDuser = -1;
                IDFriend = -1;
                Friend_Nick = "null";
                NickName = "null";
                MainWindow main = new MainWindow();
                main.Show();
        }


        //Обновление списка чатов
        private void Update_Chat_List_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Открываем соединение
                connection.Open();

                //Команда для БД
                string sql_cmd = "SELECT * FROM server_chats.chats WHERE (ID_User_1=@ID OR ID_User_2=@ID);";

                //Создаём команду запроса
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = sql_cmd;

                //Добавляем параметры
                MySqlParameter id_parameter = new MySqlParameter("@ID", MySqlDbType.Int32);
                id_parameter.Value = IDuser;
                cmd.Parameters.Add(id_parameter);

                //Вот это обязательно
                Chat_list.Items.Clear();

                //...
                using (DbDataReader reader=cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Chat_List list = new Chat_List();
                            list.ID = int.Parse(reader.GetString(0));
                            list.Name = reader.GetString(1);
                            if (int.Parse(reader.GetString(2)) == IDuser) list.ID_Friend = int.Parse(reader.GetString(3));
                            else list.ID_Friend = int.Parse(reader.GetString(2));
                            Chat_list.Items.Add(list);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                //Закрываем соединение
                connection.Close();
            }
        }

        //Поиск пользователей
        private void User_Search_Changed(object sender, RoutedEventArgs e)
        {
            try
            {
                //Предварительно очищаем список
                User_List.Items.Clear();

                //Открываем соединение
                connection.Open();

                //Строка запроса на поиск пользователей в БД
                string sql_cmd = "SELECT server_chats.users.ID, server_chats.users.User_Name, server_chats.users.User_Nickname FROM server_chats.users WHERE server_chats.users.User_Name=@NAME";

                //Создаём команду для запроса в БД
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = sql_cmd;

                //Добавляем параметры в команду
                MySqlParameter name_parameter = new MySqlParameter("@NAME", MySqlDbType.VarChar);
                name_parameter.Value = User_Search.Text;
                cmd.Parameters.Add(name_parameter);

                //...
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            //Создаём кастомизированный item для списка пользователей и добавляем ему свойства
                            Chat_List user = new Chat_List();
                            IDFriend = int.Parse(reader.GetString(0));
                            user.Name = reader.GetString(1);
                            User_List.Items.Add(user);
                            Friend_Nick = reader.GetString(2);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //Выводим сообщения об ошибке
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                //Закрываем соединение
                connection.Close();
                if (User_List.Items.Count==0) 
                {
                    IDFriend = -1;
                    Friend_Nick = "null"; 
                }
            }
        }

        //Отпрвака сообщения
        private void Send_Message_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Открываем соединение
                connection.Open();

                //Строка запроса для БД (недописана)
                string sql_cmd = "INSERT INTO server_chats.";

                Chat_List msg1 = new Chat_List();
                msg1.Name = "1234";
                Message msg = new Message();
                Message msg2 = new Message();
                msg.Message_Text = "Привет!\nКак дела?\nЧто нового?";
                msg2.Message_Text = "Привет, просто привет\nАхахвхаха";
                msg.borderBrush = (Brush)Application.Current.Resources["IsMouseOverColor"];
                msg.backGround = (Brush)Application.Current.Resources["BorderBrush"];
                msg2.backGround = (Brush)Application.Current.Resources["IsMouseOverColor"];
                msg2.borderBrush = (Brush)Application.Current.Resources["BorderBrush"];
                Message_List.Items.Add(msg);
                Message_List.Items.Add(msg2);


                User_List.Items.Clear();
                User_List.Items.Add(msg);
                Chat_list.Items.Add(msg1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                //Закрываем соединение
                connection.Close();
            }

        }

        //Создание или открытие диалога с пользователем (необработана проверка на наличие существующего диалога)
        private void User_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //Открываем соединение
                connection.Open();

                //Строка запроса
                string sql_cmd = "INSERT INTO server_chats.chats (Chat_name, ID_User_1, ID_User_2) VALUES (@CHATNAME,@IDUSER1, @IDUSER2)";

                //Команда запроса
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = sql_cmd;

                //Добавляем параметры в наш запрос
                MySqlParameter name_parameter = new MySqlParameter("@CHATNAME", MySqlDbType.VarChar);
                name_parameter.Value = Convert.ToString(NickName+" to "+Friend_Nick);
                cmd.Parameters.Add(name_parameter);

                MySqlParameter id1 = new MySqlParameter("@IDUSER1", MySqlDbType.Int32);
                id1.Value = IDuser;
                cmd.Parameters.Add(id1);

                MySqlParameter id2 = new MySqlParameter("@IDUSER2",MySqlDbType.Int32);
                id2.Value = IDFriend;
                cmd.Parameters.Add(id2);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                //Закрываем соединение
                connection.Close();
            }
        }

        //Открытие диалога с пользователем
        private void Chat_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Создаём объект Chat_List, чтобы узнать ID нашего собеседника
            Chat_List item = (Chat_List) Chat_list.SelectedItem;
            MessageBox.Show(item.ID_Friend.ToString());
            IDFriend = item.ID_Friend;

            try
            {
                
                //Открываем соединение
                connection.Open();

                //Строка запроса на определения количества сообщений в диалоге
                string sql_cmd = "SELECT COUNT(*) FROM server_chats.messages WHERE (ID_Sender = @MYID AND ID_Reciever = @IDFRIEND) OR (ID_Sender = @IDFRIEND AND ID_Reciever = @MYID);";

                //Команда запроса
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = sql_cmd;

                //Параметры запроса
                MySqlParameter myID = new MySqlParameter("@MYID", MySqlDbType.Int32);
                myID.Value = IDuser;
                cmd.Parameters.Add(myID);

                MySqlParameter friendID = new MySqlParameter("@IDFRIEND", MySqlDbType.Int32);
                friendID.Value = IDFriend;
                cmd.Parameters.Add(friendID);

                //Получаем количество сообщений в диалоге
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            MessageCount = int.Parse(reader.GetString(0));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                //Закрываем соединение
                connection.Close();

                //Вызываем функцию загрузки сообщений
                if (MessageCount <= 100) Loading_Messages("SELECT * FROM server_chats.messages WHERE (ID_Sender = @MYID AND ID_Reciever = @IDFRIEND) OR (ID_Sender = @IDFRIEND AND ID_Reciever = @MYID)");
                else Loading_Messages("SELECT * FROM server_chats.messages WHERE (ID_Sender = @MYID AND ID_Reciever = @IDFRIEND) OR (ID_Sender = @IDFRIEND AND ID_Reciever = @MYID) LIMIT @COUNT-100,@COUNT;");
            }
        }

        //Выгрузка сообщений
        public void Loading_Messages(string SQL_Command)
        {
            try
            {
                //Открываем соединение
                connection.Open();

                //Запрос на выгрузку сообщений (максимум 100)
                string sql_cmd = SQL_Command;

                //Команда запроса
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = sql_cmd;

                //Добавляем параметры
                MySqlParameter myID = new MySqlParameter("@MYID", MySqlDbType.Int32);
                myID.Value = IDuser;
                cmd.Parameters.Add(myID);

                MySqlParameter friendID = new MySqlParameter("@IDFRIEND", MySqlDbType.Int32);
                friendID.Value = IDFriend;
                cmd.Parameters.Add(friendID);

                MySqlParameter count_messages = new MySqlParameter("@COUNT", MySqlDbType.Int32);
                count_messages.Value = MessageCount;
                cmd.Parameters.Add(count_messages);

                //Здесь прописывается логика отображения сообщений в окне дилога
                //У "моих" сообщений и сообщений собеседника будет различное цветовое оформление 
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            //
                            if(int.Parse(reader.GetString(3)) == IDuser)
                            {
                                //Создадим объект привязки данных и определим свойства
                                Message MSG = new Message();
                                MSG.Message_Text = reader.GetString(1);
                                MSG.borderBrush = (Brush)Application.Current.Resources["IsMouseOverColor"];
                                MSG.backGround = (Brush)Application.Current.Resources["IsMouseOverColor"];
                                Message_List.Items.Add(MSG);
                            }
                            if (int.Parse(reader.GetString(3)) == IDFriend)
                            {
                                Message MSG = new Message();
                                MSG.Message_Text = reader.GetString(1);
                                MSG.borderBrush = (Brush)Application.Current.Resources["BorderBrush"];
                                MSG.backGround = (Brush)Application.Current.Resources["FriendMessage"];
                                Message_List.Items.Add(MSG);
                            }
                        }
                    }
                }

                //Запускаем запрос на отметку сообщений, как прочитанные
                sql_cmd = "UPDATE server_chats.messages SET Visible_Message = 1 WHERE (ID_Reciever=@MYID AND Visible_Message = 0) LIMIT 1000";
                //Создаём команду запроса
                cmd.CommandText = sql_cmd;
                //Осуществляем запрос
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                //Закрываем соединение
                connection.Close();
            }
        }
    }
}
