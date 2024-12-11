class BlockData
{
    public DiskFile? Data;
    
    public string ID2Str()
    {
        return Data != null ? Data.ID.ToString() : ".";
    }
}