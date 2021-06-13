using System.Collections.Generic;

namespace PaymentGateway.Models
{
    public record PageResult<T>
    {
        public int? Top { get; set; }

        public int Skip { get; set; }

        public int Total { get; set; }

        public ICollection<T> Records { get; set; }
    }
}