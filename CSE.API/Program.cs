using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.Util;
using RabbitMQ.Client;
using SharedKernel.Messages;
using System;
using Topshelf;

namespace CSE.API
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello CSE API!");

            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("localhost");

                cfg.ReceiveEndpoint("blabla", ec =>
                {
                    ec.AutoDelete = false;
                    ec.Durable = true;

                    ec.Consumer<CreateCSEConsumer>();
                });

            });

            busControl.Start();

            var handle = busControl.ConnectConsumer<CreateCSEConsumer>();

            Console.ReadLine();
        }
    }


    public class ConsumerService : ServiceControl
    {
        private IBusControl _busControl;
        private BusHandle _busHandle;

        public bool Start(HostControl hostControl)
        {
            (this._busControl, this._busHandle) = this.CreateBus();
            return true;
        }

        private (IBusControl, BusHandle) CreateBus()
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(this.ConfigureBus);
            var busHandle = TaskUtil.Await(() => bus.StartAsync());
            return (bus, busHandle);
        }

        private void ConfigureBus(IRabbitMqBusFactoryConfigurator factoryConfigurator)
        {
            var rabbitHost = new Uri("rabbitmq://localhost:5672/saga");
            var inputQueue = "consumer-request";
            var host = factoryConfigurator.Host(rabbitHost, this.ConfigureCredential);
            factoryConfigurator.ReceiveEndpoint(host, inputQueue, this.ConfigureEndPoint);
        }

        private void ConfigureCredential(IRabbitMqHostConfigurator hostConfigurator)
        {
            var user = "guest";
            var password = "guest";

            hostConfigurator.Username(user);
            hostConfigurator.Password(password);
        }

        private void ConfigureEndPoint(IRabbitMqReceiveEndpointConfigurator endPointConfigurator)
        {
            endPointConfigurator.Consumer(() => new CreateCSEConsumer());
        }

        public bool Stop(HostControl hostControl)
        {
            this._busHandle?.Stop();
            return true;
        }
    }

    public class CreateCSEConsumer : IConsumer<TenantCreatedEvent>
    {
        public async System.Threading.Tasks.Task Consume(ConsumeContext<TenantCreatedEvent> context)
        {
            await Console.Out.WriteLineAsync($"Updating tenant: {context.Message.CorrelationId}");

            await context.Publish<CSECreatedEvent>(new CSECreatedEvent()
            {
                CorrelationId = context.Message.CorrelationId,
                TenantId = "XXXXXXXXXXX",
                TenantName = "BLABLABLA"
            });
        }
    }

}
