using MessagePack;

    [MessagePackObject]
public class MyPackage {
    [Key(0)]
    public float horizontal;
    [Key(1)]
    public float vertical;
    [Key(2)]
    public float timeStamp;
}