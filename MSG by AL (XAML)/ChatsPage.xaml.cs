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
using System.Windows.Input;
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


        //Создание объекта подключения к БД
        MySqlConnection connection = DBUtils.GetDBConnection();

        public ChatsPage(int ID, string login)
        {
            IDuser = ID;
            NickName = login;
            InitializeComponent();
        }


        //Обновление списка чатов
        private void Update_Chat_List_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Открываем соединение
                connection.Open();

                //Команда для БД
                string sql_cmd = "SELECT server_chats.chats.ID, server_chats.chats.Chat_Name FROM server_chats.chats WHERE (ID_User_1=@ID OR ID_User_2=@ID);";

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
                //Открываем соединение
                connection.Open();

                //Строка запроса на поиск пользователей в БД
                string sql_cmd = "SELECT server_chats.users.ID, server_chats.users.Name FROM server_chats.users WHERE server_chats.users.Name=@NAME";

                //Создаём команду для запроса в БД
                MySqlCommand cmd = connection.CreateCommand();

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
                Border baseform = new Border();
                Border baseform2 = new Border();
                baseform.BorderThickness = new Thickness(2);
                baseform.BorderBrush = (Brush)Application.Current.Resources["IsMouseOverColor"];
                baseform.CornerRadius = new CornerRadius(10);
                baseform.Background = (Brush)Application.Current.Resources["BorderBrush"];
                baseform.HorizontalAlignment = HorizontalAlignment.Center;
                baseform.VerticalAlignment = VerticalAlignment.Center;

                TextBlock block1 = new TextBlock();
                block1.TextWrapping = TextWrapping.Wrap;
                block1.Text = "Привет\nКак дела?000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
                block1.HorizontalAlignment = HorizontalAlignment.Center;
                block1.VerticalAlignment = VerticalAlignment.Center;
                baseform2.Child = block1;
                Message_List.Items.Add(baseform2);

                TextBlock block2 = new TextBlock();
                block2.Text = "Привет, хорошо!\n Твои как?";
                baseform.Child = block2;
                Message_List.Items.Add(baseform);

                Message msg = new Message();
                msg.Message_Text = "1234";
                User_List.Items.Clear();
                User_List.Items.Add(msg);
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
