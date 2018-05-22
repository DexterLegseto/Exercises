using System;
using System.Collections;
using System.IO;
using System.Xml;

namespace ManualSerializerExercise
{
    /// <inheritdoc />
    /// <summary>
    /// Двусвязный список
    /// </summary>
    public class DoubleLinkedList : IEnumerable
    {
        private Node _head;

        private Node _tail;

        private int _count;

        /// <summary>
        /// Сериализует DoubleLinkedList в XML, не используя XmlSerializer, BinaryFormatter, JsonSerializer
        /// </summary>
        public void Serialize(FileStream s)
        {
            using (s)
            {
                XmlDocument xDoc = new XmlDocument();
                WriteNodesToXml<XmlDocument>(xDoc);
                xDoc.Save(s);
            }
            s.Close();
        }

        /// <summary>
        /// Десериализует DoubleLinkedList из XML документа
        /// </summary>
        public void Deserialize(FileStream s)
        {
            using (s)
            {
                var xDoc = new XmlDocument();
                var node = new Node();

                xDoc.Load(s);
                var xRoot = xDoc.DocumentElement;

                if (xRoot != null)
                {
                    var headId = xRoot.Attributes.GetNamedItem("HeadID").Value;
                    _count = Convert.ToInt32(xRoot.Attributes.GetNamedItem("Count").Value);

                    node = CreateListNode(xDoc, headId, null);
                }

                _head = node;
            }
            s.Close();
        }

        /// <summary>
        /// Рекурсивный метод создания Node из XML данных
        /// </summary>
        private Node CreateListNode(XmlDocument xDoc, string nextListNodeId, Node prev)
        {
            var xRoot = xDoc.DocumentElement;

            foreach (XmlElement xnode in xRoot)
            {
                var xnodeId = xnode.Attributes.GetNamedItem("ID");
                if (xnodeId.Value == nextListNodeId)
                {
                    Node node = new Node
                    {
                        Prev = prev,
                        Data = GetDataFromNode(xnode)
                    };
                    _tail = node;
                    node.Next = CreateListNode(xDoc, GetNextsNodeId(xnode), node);

                    return node;
                }

            }
            return null;
        }

        /// <summary>
        /// Добавление  новой ноды в конец списка
        /// </summary>
        public void Add(string data)
        {
            var node = new Node(data);

            if (_head == null)
                _head = node;
            else
            {
                _tail.Next = node;
                node.Prev = _tail;
            }
            _tail = node;
            _count++;
        }

        /// <summary>
        ///Добавление новой ноды в начало списка
        /// </summary>
        public void AddFirst(string data)
        {
            var node = new Node(data);
            Node temp = _head;
            node.Next = temp;
            _head = node;
            if (_count == 0)
                _tail = _head;
            else
                temp.Prev = node;
            _count++;
        }

        /// <summary>
        ///Возвращает ID следующей(Next) ноды
        /// </summary>
        private string GetNextsNodeId(XmlElement xnode)
        {
            if (xnode == null) throw new ArgumentNullException(nameof(xnode));

            foreach (XmlNode childnode in xnode.ChildNodes)
            {
                if (childnode.Name == "Next")
                    return childnode.InnerText;
            }
            throw new Exception("Could not find Next xnode in XML");
        }

        /// <summary>
        ///Возвращает Data property этой ноды
        /// </summary>
        private static string GetDataFromNode(XmlNode xnode)
        {
            foreach (XmlNode childnode in xnode.ChildNodes)
            {
                if (childnode.Name == "Data")
                    return childnode.InnerText;
            }
            throw new Exception("Could not find Data xnode in XML");
        }

        /// <summary>
        ///Возвращает Double-Linked DoubleLinkedList как ListXmlDocument, записывая инфо о каждой Node  в XML
        /// </summary>
        private XmlDocument WriteNodesToXml<T>(XmlDocument xDoc)
        {   //Запись свойств DoubleLinkedList
            var current = _head;
            int nodeId = 0; //Уникальный ID ноды записываемый в XML для более удобной навигации
            XmlElement xRoot = xDoc.CreateElement("DoubleLinkedList");
            xRoot.SetAttribute("HeadID", nodeId.ToString());
            xRoot.SetAttribute("TailID", (_count - 1).ToString());
            xRoot.SetAttribute("Count", _count.ToString());

            //Запись свойств Node 
            while (current != null)
            {
                XmlElement node = xDoc.CreateElement("Node");
                node.SetAttribute("ID", nodeId.ToString());


                XmlElement next = xDoc.CreateElement("Next");
                next.InnerText = current.Next != null ? (nodeId + 1).ToString() : "null";

                XmlElement prev = xDoc.CreateElement("Prev");
                prev.InnerText = current.Prev != null ? (nodeId - 1).ToString() : "null";

                XmlElement rand = xDoc.CreateElement("Rand");
                rand.InnerText = current.Rand != null ? current.Rand.Data : "null";

                XmlElement data = xDoc.CreateElement("Data");
                data.InnerText = current.Data ?? "null";

                node.AppendChild(next);
                node.AppendChild(prev);
                node.AppendChild(rand);
                node.AppendChild(data);
                xRoot.AppendChild(node);
                xDoc.AppendChild(xRoot);

                current = current.Next;
                nodeId++;
            }
            return xDoc;
        }

        /// <summary>
        ///Итерирует через все ноды и пишет всю информацию о списке и его нодах
        /// </summary>
        public static void IterateAndReadFromList(DoubleLinkedList doubleLinkedList)
        {
            Console.WriteLine("                   DoubleLinkedList properties");
            Console.WriteLine("----------------------------------------------------------------");
            Console.WriteLine("{0,15} {1} {2} {3} {4} {5}",
                "Head:", ToString(doubleLinkedList._head),
                "| Tail:", ToString(doubleLinkedList._tail),
                "| Count:", doubleLinkedList._count.ToString());
            Console.WriteLine("----------------------------------------------------------------");
            Console.WriteLine("         Reading all the LineNodes from the DoubleLinkedList");
            Console.WriteLine("----------------------------------------------------------------");
            Console.WriteLine("{0,-15} | {1,-15} | {2,-15} | {3,-15}", "Prev.Data", "Data", "Next.Data", "Rand.Data");
            Console.WriteLine("----------------------------------------------------------------");
            foreach (Node lNode in doubleLinkedList)
            {
                Console.WriteLine(String.Format("{0,-15} | {1,-15} | {2,-15} | {3,-15}", ToString(lNode.Prev), lNode.Data, ToString(lNode.Next), ToString(lNode.Rand)));
            }
            Console.WriteLine("----------------------------------------------------------------");
        }

        /// <inheritdoc />
        ///  <summary>
        /// Итератор через все ноды от начала к концу
        ///  </summary>
        public IEnumerator GetEnumerator()
        {
            Node current = _head;
            while (current != null)
            {
                yield return current;
                current = current.Next;
            }
        }

        private static string ToString(Node lNode)
        {
            if (lNode == null)
                return "null";
            else
                return lNode.Data;
        }
    }
}