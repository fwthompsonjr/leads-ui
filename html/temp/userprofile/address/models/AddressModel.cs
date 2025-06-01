public class AddressModel
{
    [Required]
    [ValidUSAddress]
    public string AddressBlock { get; set; }
}
