using MassTransit;
using System;

namespace SharedKernel.Messages
{
    [Serializable]
    public class CSECreatedEvent : CorrelatedBy<Guid>, ICSECreatedEvent
    {
        public Guid CorrelationId { get; set; }
        public string TenantName { get; set; }
        public string TenantId { get; set; }
    }
}
