using Automatonymous;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.Saga;
using RabbitMQ.Client;
using SharedKernel.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tenant.API
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var instance = new TenantCreateState(Guid.NewGuid());
            var machine = new TenantCreateSaga();
            var repository = new InMemorySagaRepository<TenantCreateState>();

            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("localhost");
                cfg.ReceiveEndpoint("tenant_create_state", e =>
                {
                    e.StateMachineSaga(machine, repository);
                });

                cfg.Publish<ITenantCommand>(x => x.ExchangeType = ExchangeType.Direct);

            });

            await busControl.StartAsync();

            

            while (true)
            {
                var tenantCommand = new TenantCommand() { CorrelationId = Guid.NewGuid(), TenantId = "ABCDEF", TenantName = "TenantMyName" };

                await busControl.Publish(tenantCommand);
                Thread.Sleep(1000);
            }

            

            //var instance = new TenantCreateState();
            //TenantCreateSaga machine = new TenantCreateSaga(Guid.NewGuid());

            //await machine.RaiseEvent(instance, machine.CreateCSECommand);
            //await machine.RaiseEvent(instance, machine.CSECreatedSucceedEvent);
            //await machine.RaiseEvent(instance, machine.TenantCreatedSucceedEvent);

            //await machine.RaiseEvent(instance, machine.PolicyCreatedFailedEvent);

            //await machine.RaiseEvent(instance, machine.TenantCreatedRollbackSucceedEvent);
            //await machine.RaiseEvent(instance, machine.CSECreatedRollbackSucceedEvent);

            Console.WriteLine("Press any key...");
            Console.ReadKey(true);
        }
    }



}
