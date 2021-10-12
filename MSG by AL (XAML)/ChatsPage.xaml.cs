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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
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
        MySqlConnection connection_async = DBUtils.GetDBConnection();

        public ChatsPage(int ID, string login)
        {
            IDuser = ID;
            NickName = login;
            InitializeComponent();
            Clear_List();
            Update_Dialog_List();
            Update_Friend_List();
        }

        //Очистка всех списков (сообщений, чатов, пользователей)
        public void Clear_List()
        {
            User_List.Items.Clear();
            Message_List.Items.Clear();
            Chat_list.Items.Clear();
        }

        //Метод обновления списка чатов
        public void Update_Dialog_List()
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
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Chat_List list = new Chat_List();
                            list.ID = int.Parse(reader.GetString(1));
                            list.Name = reader.GetString(2);
                            if (int.Parse(reader.GetString(3)) == IDuser) list.ID_Friend = int.Parse(reader.GetString(4));
                            else list.ID_Friend = int.Parse(reader.GetString(3));
                            Chat_list.Items.Add(list);
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

        public void Update_Friend_List()
        {
            try
            {
                //Предварительно очищаем список
                Friend_List.Items.Clear();

                //Открываем соединение
                connection.Open();

                //Строка запроса для выборки друзей авторизованного пользователя
                string sql_cmd = "SELECT * FROM server_chats.friend WHERE ID_User = @MYID;";

                //Команда запроса
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = sql_cmd;

                //Добавляем параметры
                MySqlParameter myID = new MySqlParameter("@MYID", MySqlDbType.Int32);
                myID.Value = IDuser;
                cmd.Parameters.Add(myID);

                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Chat_List user = new Chat_List();
                            user.ID_Friend = int.Parse(reader.GetString(1));
                            user.Name = reader.GetString(2);
                            Friend_List.Items.Add(user);
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
                                MSG.Message_ID = int.Parse(reader.GetString(0));
                                MSG.Message_Text = reader.GetString(1);
                                MSG.Message_Date = reader.GetString(2);
                                MSG.borderBrush = (Brush)Application.Current.Resources["MyMessageColor"];
                                MSG.backGround = (Brush)Application.Current.Resources["MyMessageColor"];
                                Message_List.Items.Add(MSG);
                            }
                            if (int.Parse(reader.GetString(3)) == IDFriend)
                            {
                                Message MSG = new Message();
                                MSG.Message_ID = int.Parse(reader.GetString(0));
                                MSG.Message_Text = reader.GetString(1);
                                MSG.Message_Date = reader.GetString(2);
                                MSG.borderBrush = (Brush)Application.Current.Resources["BorderBrush"];
                                MSG.backGround = (Brush)Application.Current.Resources["FriendMessageColor"];
                                Message_List.Items.Add(MSG);
                            }
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
                Mark_Read();
            }
        }

        //Отметка сообщений прочитанными
        public void Mark_Read()
        {
            try
            {
                //Открываем соединение
                connection.Open();

                //Запускаем запрос на отметку сообщений, как прочитанные
                string sql_cmd = "UPDATE server_chats.messages SET Visible_Message = 1 WHERE (ID_Reciever=@MYID AND ID_Sender = @FRIENDID AND Visible_Message = 0) LIMIT 1000";

                //Создаём команду запроса
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = sql_cmd;

                //Добавляем параметры в запрос
                MySqlParameter myID = new MySqlParameter("@MYID", MySqlDbType.Int32);
                myID.Value = IDuser;
                cmd.Parameters.Add(myID);

                MySqlParameter friendID = new MySqlParameter("@FRIENDID", MySqlDbType.Int32);
                friendID.Value = IDFriend;
                cmd.Parameters.Add(friendID);

                //Осуществляем запрос
                cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                //Закрыавем соединение 
                connection.Close();
            }
        }

        //Запуск асинхронной операции обновления текущего диалога
        public async void Refresh_Chat_Async()
        {
            while (IDFriend != -1)
            {
                //Проверка на наличие непрочитанных сообщений
                bool unreadMessage = false;
                try
                {
                    //Открываем соединение
                    await connection_async.OpenAsync();
                    //Строка запроса
                    string sql_cmd = "SELECT * FROM server_chats.messages WHERE (ID_Reciever = @MYID AND ID_Sender = @FRIENDID AND Visible_Message = 0);";

                    //Команда запроса
                    MySqlCommand cmd = connection_async.CreateCommand();
                    cmd.CommandText = sql_cmd;

                    //Добавляем параметры запроса
                    MySqlParameter myID = new MySqlParameter("@MYID", MySqlDbType.Int32);
                    myID.Value = IDuser;
                    cmd.Parameters.Add(myID);

                    MySqlParameter friendID = new MySqlParameter("@FRIENDID", MySqlDbType.Int32);
                    friendID.Value = IDFriend;
                    cmd.Parameters.Add(friendID);

                    //Проверяем в БД непрочитанные нами сообщения
                    using (DbDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Message friend_message = new Message();
                                friend_message.Message_ID = int.Parse(reader.GetString(0));
                                friend_message.Message_Text = reader.GetString(1);
                                friend_message.Message_Date = reader.GetString(2);
                                friend_message.backGround = (Brush)Application.Current.Resources["FriendMessageColor"];
                                friend_message.borderBrush = (Brush)Application.Current.Resources["FriendMessageColor"];
                                //Выполняет указанный делегат в оснвном потоке (т.к. к Control'у я не могу обратиться из этого потока)
                                Dispatcher.Invoke(() => Message_List.Items.Add(friend_message));
                                //Если непрочитанные сообщения есть, то нужно отметить их прочитанными
                                unreadMessage = true;
                            }
                            Dispatcher.Invoke(()=>Message_List.ScrollIntoView(Message_List.Items[Message_List.Items.Count-1]));
                        }
                    }
                    //В зависимости от того, есть ли сообщения непрочитанные
                    //Выполняем функцию отметки сообщений
                    if (unreadMessage) Mark_Read();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    //Закрываем соединение
                    await connection_async.CloseAsync();
                }

                //Приостанавливаем поток данной функции (снижает нагрузку на БД, ОЗУ, ЦП + 1,5 сек. не страшная задержка)
                System.Threading.Thread.Sleep(1500);
            }
        }

        //Создание нового диалога с пользователем
        public bool CreateNewChat(int friendID, string friend_nick)
        {
            bool succesfull = false;
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
                name_parameter.Value = Convert.ToString(NickName + " to " + friend_nick);
                cmd.Parameters.Add(name_parameter);

                MySqlParameter id1 = new MySqlParameter("@IDUSER1", MySqlDbType.Int32);
                id1.Value = IDuser;
                cmd.Parameters.Add(id1);

                MySqlParameter id2 = new MySqlParameter("@IDUSER2", MySqlDbType.Int32);
                id2.Value = friendID;
                cmd.Parameters.Add(id2);

                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                {
                    connection.Close();
                    return false;
                }
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                //Закрываем соединение
                connection.Close();
                succesfull = true;
            }
            Update_Dialog_List();
            return succesfull;
        }

        //Метод открытия диалога с пользователем
        public async void OpenChat(int friend_ID)
        {
            //Закрываем предыдущий диалог
            Dispatcher.Invoke(()=>Message_List.Items.Clear());

            try
            {
                    IDFriend = friend_ID;
                    Dispatcher.Invoke(()=>Close_Dialog.Visibility = Visibility.Visible);
                    Dispatcher.Invoke(()=>MySlider.Visibility = Visibility.Visible);
                    Dispatcher.Invoke(()=>Name_Friend.Visibility = Visibility.Visible);
                    Dispatcher.Invoke(() => Name_Friend.Content = GetChatName(friend_ID));
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
                if (MessageCount <= 100) Dispatcher.Invoke(()=>Loading_Messages("SELECT * FROM server_chats.messages WHERE (ID_Sender = @MYID AND ID_Reciever = @IDFRIEND) OR (ID_Sender = @IDFRIEND AND ID_Reciever = @MYID)"));
                else Dispatcher.Invoke(()=>Loading_Messages("SELECT * FROM server_chats.messages WHERE (ID_Sender = @MYID AND ID_Reciever = @IDFRIEND) OR (ID_Sender = @IDFRIEND AND ID_Reciever = @MYID) LIMIT @COUNT-100,@COUNT;"));

                await Task.Run(() => Refresh_Chat_Async());
            }
        }

        //Функция возвращает имя чата по ID его пользователей
        public string GetChatName(int friend_ID)
        {
            //Имя чата
            string NAME = "null";

            //Объект для создания подключения к БД
            MySqlConnection conn = DBUtils.GetDBConnection();

            try
            {    
                //Открываем соединение
                conn.Open();

                //Строка запроса
                string sql_cmd = "SELECT server_chats.chats.Chat_Name FROM server_chats.chats WHERE (ID_User_1 = @MYID AND ID_User_2 = @IDFRIEND) OR (ID_User_1 = @IDFRIEND AND ID_User_2 = @MYID);";

                //Команда запроса
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql_cmd;

                //Добавляем параметры
                MySqlParameter myID = new MySqlParameter("@MYID", MySqlDbType.Int32);
                myID.Value = IDuser;
                cmd.Parameters.Add(myID);

                MySqlParameter friendID = new MySqlParameter("@IDFRIEND", MySqlDbType.Int32);
                friendID.Value = friend_ID;
                cmd.Parameters.Add(friendID);

                //Получаем имя чата
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            NAME = reader.GetString(0);
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
                conn.Close();
            }

            return NAME;
        }

        //Функция возвращает имя пользователя по его ID
        public string GetUserName(int friend_ID)
        {
            //Имя пользователя
            string NAME="null";

            //Объект для создания подключения к БД
            MySqlConnection conn = DBUtils.GetDBConnection();

            try
            {
                //Открываем соединение
                conn.Open();

                //Строка запроса
                string sql_cmd = "SELECT server_chats.users.User_Name FROM server_chats.users WHERE ID = @IDFRIEND";

                //Команда запроса
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql_cmd;

                //Добавляем параметры
                MySqlParameter friendID = new MySqlParameter("@IDFRIEND", MySqlDbType.Int32);
                friendID.Value = friend_ID;
                cmd.Parameters.Add(friendID);

                //Получаем имя чата
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            NAME = reader.GetString(0);
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
                conn.Close();
            }

            //Возвращаем имя пользователя
            return NAME;
        }




        /*Все функции, находящиеся снизу - события
         * Все функции сверху - собственные методы, для осуществления деёствия на форме
         */

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
            Update_Dialog_List();
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
                            user.ID_Friend = IDFriend;
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
                if (User_List.Items.Count == 0)
                {
                    IDFriend = -1;
                    Friend_Nick = "null";
                }
            }
        }

        //Отпрвака сообщения
        private void Send_Message_Click(object sender, RoutedEventArgs e)
        {
            //Чтобы не отправлялись пустые сообщения
            if (TextBox_Message.Text.Length != 0)
            {
                try
                {
                    //Открываем соединение
                    connection.Open();

                    //Строка запроса для БД (недописана)
                    string sql_cmd = "INSERT INTO server_chats.messages (Text_Message, Date_Message, ID_Sender, ID_Reciever, Visible_Message) VALUES (@TEXT, NOW(), @MYID, @FRIENDID, 0);";

                    //Команда запроса
                    MySqlCommand cmd = connection.CreateCommand();
                    cmd.CommandText = sql_cmd;

                    //Добавляем параметры
                    MySqlParameter text_message = new MySqlParameter("@TEXT", MySqlDbType.Text);
                    text_message.Value = TextBox_Message.Text;
                    cmd.Parameters.Add(text_message);

                    MySqlParameter myID = new MySqlParameter("@MYID", MySqlDbType.Int32);
                    myID.Value = IDuser;
                    cmd.Parameters.Add(myID);

                    MySqlParameter friendID = new MySqlParameter("@FRIENDID", MySqlDbType.Int32);
                    friendID.Value = IDFriend;
                    cmd.Parameters.Add(friendID);

                    //Выполняем запрос
                    cmd.ExecuteNonQuery();

                    //Добавляем сообщение в диалог
                    //Нет возможности добавить ID для своего сообщения, т.к. его формирует БД
                    //Отправленное сообщение возможно не получится удалить, пока не перезайти в диалог
                    Message my_message = new Message();
                    my_message.Message_ID = -1;
                    my_message.Message_Text = TextBox_Message.Text;
                    my_message.Message_Date = DateTime.Now.ToString();
                    my_message.backGround = (Brush)Application.Current.Resources["MyMessageColor"];
                    my_message.borderBrush = (Brush)Application.Current.Resources["MyMessageColor"];
                    Message_List.Items.Add(my_message);
                    Message_List.ScrollIntoView(Message_List.Items[Message_List.Items.Count - 1]);
                    TextBox_Message.Text = "";
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

        //Действия при закрытии диалога
        private void Close_Dialog_Click(object sender, RoutedEventArgs e)
        {
            //Очищаем список сообщений
            Message_List.Items.Clear();

            //Стираем все данные о собеседнике
            IDFriend = -1;
            Friend_Nick = "null";

            //Скрываем элементы управления диалогом
            Close_Dialog.Visibility = Visibility.Hidden;
            MySlider.Visibility = Visibility.Hidden;
            Name_Friend.Visibility = Visibility.Hidden;

        }

        //Добавление пользователя в друзья
        private void AddToFriend_Click(object sender, RoutedEventArgs e)
        {
                try
                {
                    //Объект item'а, но только первого (если будут с одинаковыми именами, тогда будут проблемы)
                    //Нужно изменить функцию поиска с имени на никнейм
                    Chat_List user = (Chat_List)User_List.Items[0];

                    //Открываем соединение 
                    connection.Open();

                    //Строка запроса на добавление пользователя в список друзей
                    string sql_cmd = "INSERT INTO server_chats.friend VALUES (@MYID, @IDFRIEND, @FRIENDNAME);";

                    //Команда запроса
                    MySqlCommand cmd = connection.CreateCommand();
                    cmd.CommandText = sql_cmd;

                    //Добавляем параметры
                    MySqlParameter myID = new MySqlParameter("@MYID", MySqlDbType.Int32);
                    myID.Value = IDuser;
                    cmd.Parameters.Add(myID);

                    MySqlParameter friendID = new MySqlParameter("@IDFRIEND", MySqlDbType.Int32);
                    friendID.Value = user.ID_Friend;
                    cmd.Parameters.Add(friendID);

                    MySqlParameter name = new MySqlParameter("@FRIENDNAME", MySqlDbType.VarChar);
                    name.Value = user.Name;
                    cmd.Parameters.Add(name);

                    //Запускаем команду
                    cmd.ExecuteNonQuery();

                    //После успешного выполнения команды, будет дополнен список друзей
                    Friend_List.Items.Add(user);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    //Закрываем соединение
                    connection.Close();
                    User_Search.Clear();
                }
        }

        //Открытие диалога
        private async void Chat_list_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IDFriend = -1;
            Chat_List item = (Chat_List)Dispatcher.Invoke(()=>Chat_list.SelectedItem);
            await Task.Run(() => OpenChat(item.ID_Friend));
        }

        //Удаление пользователя из друзей
        private void DeleteFromFriend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;

                //Открываем соединение
                connection.Open();

                //Получаем объект нашего пользователя (произойдёт только при выборе элемента сначала )
                //значит нужно кнопку сделать недоступной, пока не выбирут его
                Chat_List user = (Chat_List)Friend_List.SelectedItem;

                //Строка запроса на удаление пользователя из друзей
                string sql_cmd = "DELETE FROM server_chats.friend WHERE ID_User = @MYID AND ID_Friend = @IDFRIEND";

                //Создаём команду запроса
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = sql_cmd;

                //Добавляем параметры
                MySqlParameter myID = new MySqlParameter("@MYID", MySqlDbType.Int32);
                myID.Value = IDuser;
                cmd.Parameters.Add(myID);

                MySqlParameter friendID = new MySqlParameter("@IDFRIEND", MySqlDbType.Int32);
                friendID.Value = int.Parse(btn.Content.ToString());
                cmd.Parameters.Add(friendID);

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

                //Обновляем список друзей
                Update_Friend_List();
            }
        }

        private async void Writing_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            //Очищаем список сообщений
            Message_List.Items.Clear();

            //Стираем все данные о собеседнике
            IDFriend = int.Parse(Dispatcher.Invoke(() => btn.Content.ToString()));
            Friend_Nick = GetUserName(IDFriend);
            if(CreateNewChat(IDFriend, Friend_Nick)!=true) await Task.Run(() => OpenChat(IDFriend));
        }

        //Удаление сообщения из диалога
        private void DeleteMessage_Click(object sender, RoutedEventArgs e)
        {
            //Кнопка удаления сообщения, хранящее ID удаляемого сообщения
            Button message = sender as Button;
            try
            {
                //Открываем соединение
                connection.Open();

                //Строка запроса
                string sql_cmd = "DELETE FROM server_chats.messages WHERE ID = @IDMESSAGE;";

                //Команда запроса
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = sql_cmd;

                //Добавляем параметры
                MySqlParameter messageID = new MySqlParameter("@IDMESSAGE", MySqlDbType.Int32);
                messageID.Value = message.Content.ToString();
                cmd.Parameters.Add(messageID);

                //Выполняем запрос
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
                //Вызываем функцию загрузки сообщений
                Message_List.Items.Clear();
                if (MessageCount <= 1000) Loading_Messages("SELECT * FROM server_chats.messages WHERE (ID_Sender = @MYID AND ID_Reciever = @IDFRIEND) OR (ID_Sender = @IDFRIEND AND ID_Reciever = @MYID)");
                else Loading_Messages("SELECT * FROM server_chats.messages WHERE (ID_Sender = @MYID AND ID_Reciever = @IDFRIEND) OR (ID_Sender = @IDFRIEND AND ID_Reciever = @MYID) LIMIT @COUNT-1000,@COUNT;");
                Message_List.ScrollIntoView(Message_List.Items[Message_List.Items.Count - 1]);

            }
        }
    }
}
