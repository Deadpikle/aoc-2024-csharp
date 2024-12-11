class Node
{
    public char Symbol;
    public int X;
    public int Y;
    public Node(char symbol, int x, int y)
    {
        Symbol = symbol;
        X = x;
        Y = y;
    }

    public Node GetAntinode(Node other)
    {
        var deltaX = this.X - other.X;
        var deltaY = this.Y - other.Y;
        // Console.WriteLine("    deltaX = {0}, deltaY = {1}", deltaX, deltaY);
        return new Node(Symbol, other.X - deltaX, other.Y - deltaY);
    }

    public string GetKey()
    {
        return string.Format("{0},{1}", X, Y);
    }
}