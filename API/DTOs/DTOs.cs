namespace CleanApi.Api.Dtos;

public class CreateOrderDto
{
    public string CustomerName { get; set; }
    public string StatusName { get; set; }
    public decimal TotalAmount { get; set; }
}

public class UpdateOrderStatusDto
{
    public string NewStatusName { get; set; }
}

public class OrderSummaryResponseDto
{
    public int OrderId { get; set; }
    public string Status { get; set; } 
    public decimal Total { get; set; }
    public string FormattedDate { get; set; } 
}