using System.Collections.Generic;

namespace PaymentGateway.Domain.Entities
{
    public class Account : BaseEntity
    {
        public ICollection<Card> Cards { get; set; }
    }
}