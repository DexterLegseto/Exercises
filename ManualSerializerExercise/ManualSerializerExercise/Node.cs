namespace ManualSerializerExercise
{
    /// <summary>
    /// Узел двусвязного списка (нод)
    /// </summary>
    public class Node
    {

        public Node Prev;

        public Node Next;

        public Node Rand;// произвольный элемент внутри списка

        public string Data;

        public Node(string data)
        {
            Data = data;
        }

        public Node()
        {
        }
    }
}