using System;

namespace SharedKernel.Messages
{
    [Serializable]
    public class TenantCommand : ITenantCommand
    {
        public string TenantName { get; set; }
        public string TenantId { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
