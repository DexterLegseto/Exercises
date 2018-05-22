using System;
using System.IO;

namespace ManualSerializerExercise
{
    public class Program
    {
        private static void Main(string[] args)
        {//Заполнение нового экземпляра DoubleLinkedList тестовыми данными, его сериализация и десериализация
            DoubleLinkedList doubleLinkedList = new DoubleLinkedList {"Chef", "Cortana", "Johnson", "Miranda", "Raya"};
            doubleLinkedList.AddFirst("Max");

            File.Delete("ListDataXml.xml");
            doubleLinkedList.Serialize(new FileStream("ListDataXml.xml", FileMode.Create));

            //Удаляем данные для теста
            doubleLinkedList = new DoubleLinkedList();

            doubleLinkedList.Deserialize(new FileStream("ListDataXml.xml", FileMode.Open));

            DoubleLinkedList.IterateAndReadFromList(doubleLinkedList);
            Console.ReadLine();
        }
    }
}
