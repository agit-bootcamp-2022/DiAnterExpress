namespace DiAnterExpress.Dtos
{
    public class ShipmentFeeAllInput
    {
        public Location SenderAddress { get; set; }
        public Location ReceiverAddress { get; set; }
        public double Weight { get; set; }
    }
}
