namespace TicketEase.Common.Utilities
{
    public static class TicketHelper
    {
        private const string ReferencePrefix = "TICKET-";

        public static string GenerateTicketReference()
        {
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string ticketReference = $"{ReferencePrefix}{timestamp}";

            return ticketReference;
        }
    }
}
