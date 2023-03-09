using System;
using System.Threading;
using System.Threading.Tasks;
using CommonNamespace;
using MassTransit;
using MassTransit.RabbitMqTransport;

namespace Producer
{
    public class Program
    {
        public static async Task Main()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(Configure);

            await busControl.StartAsync();
            try
            {       
                //PUBLISH
                await busControl.Publish(new MessageDto
                {
                    Content = "message!"
                });

                //SEND
                /**/
                var queueName = "masstransit";
                var sendEndpoint = await busControl.GetSendEndpoint(new Uri($"queue:{queueName}"));
                if (sendEndpoint == null)
                {
                    throw new Exception($"Не удалось найти очередь {queueName}");
                }
                await sendEndpoint.Send(new MessageDto
                {
                    Content = "message!"
                }, CancellationToken.None);                    
                /**/
                
                /**/
                //REQUEST
                var response = await busControl.Request<Request, CommonNamespace.Response>( new Request
                {
                    Message = new MessageDto
                    {
                        Content = "message!"
                    }
                }, CancellationToken.None);
                Console.WriteLine($"Response success: {response.Message.IsSuccess}");
                /**/
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
        
        /// <summary>
        /// Конфигурирование
        /// </summary>
        /// <param name="configurator"></param>
        private static void Configure(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.Host("moose.rmq.cloudamqp.com",
                "wxuscjul",
                h =>
                {
                    h.Username("wxuscjul");
                    h.Password("d86v39oHoWxr-ZWWZpDTO6uVhdBlUTVv");
                });
        }
    }
}