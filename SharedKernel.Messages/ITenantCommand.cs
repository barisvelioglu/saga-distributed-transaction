using System;

namespace SharedKernel.Messages
{
    public interface ITenantCommand
    {
        Guid CorrelationId { get; }
        string TenantName { get; set; }
        string TenantId { get; set; }
    }
}
