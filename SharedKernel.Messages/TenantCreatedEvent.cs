using MassTransit;
using System;

namespace SharedKernel.Messages
{

    [Serializable]
    public class TenantCreatedEvent : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public string TenantName { get; set; }
        public string TenantId { get; set; }
    }
}
