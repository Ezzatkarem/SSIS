public class PaymobCallBackDto
{
    public string Type { get; set; }
    public PaymobTransaction Obj { get; set; }
}

public class PaymobTransaction
{
    public long Id { get; set; }
    public bool Success { get; set; }
    public long Amount_Cents { get; set; }
    public string Currency { get; set; }
    public PaymobOrder Order { get; set; }
}

public class PaymobOrder
{
    public long Id { get; set; }
}
