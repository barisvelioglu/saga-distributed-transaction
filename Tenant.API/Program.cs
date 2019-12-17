using Automatonymous;
using System;
using System.Threading.Tasks;

namespace Tenant.API
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var instance = new TenantCreateState();
            TenantCreateSaga machine = new TenantCreateSaga();

            await machine.RaiseEvent(instance, machine.CreateCSECommand);
            await machine.RaiseEvent(instance, machine.CSECreatedSucceedEvent);
            await machine.RaiseEvent(instance, machine.TenantCreatedSucceedEvent);

            await machine.RaiseEvent(instance, machine.PolicyCreatedFailedEvent);

            await machine.RaiseEvent(instance, machine.TenantCreatedRollbackSucceedEvent);
            await machine.RaiseEvent(instance, machine.CSECreatedRollbackSucceedEvent);

            Console.Read();
        }
    }
    public class TenantCreateState : SagaStateMachineInstance
    {
        public string TenantId { get; set; }
        public State CurrentState { get; set; }
        public Guid CorrelationId { get; set; }
    }

    public class TenantCreateSaga : MassTransitStateMachine<TenantCreateState>
    {
        public State AwaitingCSECreationApproval { get; private set; }
        public State AwaitingCSECreationRollbackApproval { get; private set; }
        public State AwaitingTenantCreationApproval { get; private set; }
        public State AwaitingTenantCreationRollbackApproval { get; private set; }
        public State AwaitingPolicyCreationApproval { get; private set; }
        public State AwaitingPolicyCreationRollbackApproval { get; private set; }

        public TenantCreateSaga()
        {
            InstanceState(x => x.CurrentState);

            Initially(
                When(CreateCSECommand)
                    .Then(context =>
                    {
                        Console.WriteLine("Creating CSE!");
                    })
                    .TransitionTo(AwaitingCSECreationApproval));

            During(AwaitingCSECreationApproval,
                   When(CSECreatedSucceedEvent)
                        .Then(context =>
                        {
                            Console.WriteLine("CSE Created!");
                            Console.WriteLine("Creating Tenant!");
                        })
                       .TransitionTo(AwaitingTenantCreationApproval),
                  When(CSECreatedFailedEvent)
                        .Then(context =>
                        {
                            Console.WriteLine("CSE Failed!");
                            Console.WriteLine("DONE");
                        })
                       .Finalize());


            During(AwaitingTenantCreationApproval,
                   When(TenantCreatedSucceedEvent)
                        .Then(context =>
                        {
                            Console.WriteLine("Tenant Created!");
                            Console.WriteLine("Creating Policy!");

                        })
                        .TransitionTo(AwaitingPolicyCreationApproval),
                  When(TenantCreatedFailedEvent)
                        .Then(context =>
                        {
                            Console.WriteLine("Tenant Failed!");
                            Console.WriteLine("Compensating CSE!");
                        })
                        .TransitionTo(AwaitingCSECreationRollbackApproval));

            During(AwaitingCSECreationRollbackApproval,
                 When(CSECreatedRollbackSucceedEvent)
                    .Then(context =>
                    {
                        Console.WriteLine("CSE rollback!");
                        Console.WriteLine("DONE");
                    })
                    .Finalize());

            During(AwaitingPolicyCreationApproval,
                    When(PolicyCreatedSucceedEvent)
                        .Then(context =>
                        {
                            Console.WriteLine("Policy Created!");
                            Console.WriteLine("DONE");
                        })
                        .TransitionTo(AwaitingPolicyCreationApproval)
                        .Finalize(),
                  When(PolicyCreatedFailedEvent)
                        .Then(context =>
                        {
                            Console.WriteLine("Policy Failed!");
                            Console.WriteLine("Compensating Tenant!");
                        })
                        .TransitionTo(AwaitingTenantCreationRollbackApproval));


            During(AwaitingTenantCreationRollbackApproval,
                 When(TenantCreatedRollbackSucceedEvent)
                    .Then(context =>
                    {
                        Console.WriteLine("Tenant rollback!");
                        Console.WriteLine("Compensating CSE!");
                    })
                    .TransitionTo(AwaitingCSECreationRollbackApproval));


        }

        public Event CreateCSECommand { get; private set; }
        public Event CSECreatedSucceedEvent { get; private set; }
        public Event CSECreatedFailedEvent { get; private set; }
        public Event CSECreatedRollbackSucceedEvent { get; private set; }
        public Event CSECreatedRollbackFailedEvent { get; private set; }

        public Event CreateTenantCommand { get; private set; }
        public Event TenantCreatedSucceedEvent { get; private set; }
        public Event TenantCreatedFailedEvent { get; private set; }
        public Event TenantCreatedRollbackSucceedEvent { get; private set; }
        public Event TenantCreatedRollbackFailedEvent { get; private set; }

        public Event CreatePolicyCommand { get; private set; }
        public Event PolicyCreatedSucceedEvent { get; private set; }
        public Event PolicyCreatedFailedEvent { get; private set; }
        public Event PolicyCreatedRollbackSucceedEvent { get; private set; }
        public Event PolicyCreatedRollbackFailedEvent { get; private set; }


    }

}
