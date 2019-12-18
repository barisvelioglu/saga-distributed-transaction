using Automatonymous;
using System;

namespace Tenant.API
{
    public class TenantCreateState : SagaStateMachineInstance
    {
        public TenantCreateState(Guid correlationId)
        {
            this.CorrelationId = correlationId;
        }

        public string TenantId { get; set; }
        public State CurrentState { get; set; }
        public Guid CorrelationId { get; set; }
    }

}
