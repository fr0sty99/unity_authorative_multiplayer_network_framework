using MessagePack;

[MessagePackObject]
public class MyReceivePackage
{
    [Key(0)]
    public float x;
    [Key(1)]
    public float y;
    [Key(2)]
    public float z;
    [Key(3)]
    public float timeStamp;
}
