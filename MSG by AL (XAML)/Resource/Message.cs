using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSG_by_AL__XAML_.Resource
{
    public class Message
    {
        //Пустой конструктор
        public Message()
        {
        }

        //Свойство, текст сообщения
        public string Message_Text { get; set; }

        //Свойсто дат отправки
        public string Message_Date { set; get; }


    }
}
