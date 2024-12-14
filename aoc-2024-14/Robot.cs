class Robot
{
    public int InitialX;
    public int InitialY;
    public int X;
    public int Y;
    public int DeltaX;
    public int DeltaY;

    public Robot(int x, int y, int deltaX, int deltaY)
    {
        InitialX = X = x;
        InitialY = Y = y;
        DeltaX = deltaX;
        DeltaY = deltaY;
    }

    public void Reset()
    {
        X = InitialX;
        Y = InitialY;
    }
}