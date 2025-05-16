
public class MySaveList<TSaveObject>
{
    public class MyDoublyList<T>
    {
        public T Data;
        public MyDoublyList<T> Prev { get; set; }
        public MyDoublyList<T> Next { get; set; }

        public MyDoublyList(T data, MyDoublyList<T> prev, MyDoublyList<T> next)
        {
            this.Data = data;
            this.Prev = prev;
            this.Next = next;
        }
        public MyDoublyList(T data)
        {
            this.Data = data;
            Prev = null;
            Next = null;
        }
    
        public void Add(T data)
        {
            Next= new MyDoublyList<T>(data, this, null);
        }
    }

    private readonly int capacity;
    private int cnt { get; set; }
    private int pos;

    private MyDoublyList<TSaveObject> head, tail;
    public MyDoublyList<TSaveObject> curNode { get; set; }

    public MySaveList( TSaveObject data,int capacity)
    {
        head = tail = curNode = new MyDoublyList<TSaveObject>(data);
        this.capacity = capacity;
        pos = 1;
    }

    public void AddSaveObject(TSaveObject data)
    {
        curNode.Add(data);
        curNode = curNode.Next;
        tail = curNode;
        if (pos == capacity)
        {
            head = head.Next;
            head.Prev = null;
        }
        else
        {
            ++pos;
        }
    }

    public void Undo()
    {
        if (curNode.Prev != null)
        {
            curNode = curNode.Prev;
            pos--;
        }
    }

    public void Redo()
    {
        if (curNode.Next != null)
        {
            curNode = curNode.Next;
            pos++;
        }
    }
}


