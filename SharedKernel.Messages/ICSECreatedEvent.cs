using System;

namespace SharedKernel.Messages
{
    public interface ICSECreatedEvent
    {
        Guid CorrelationId { get; set; }
        string TenantId { get; set; }
        string TenantName { get; set; }
    }
}