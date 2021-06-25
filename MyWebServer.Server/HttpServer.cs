using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using MyWebServer.Server;
using MyWebServer.Server.Http;
using MyWebServer.Server.Routing;
using MyWebServer.Server.Services;

namespace MyWebServer.Server
{
    public class HttpServer
    {
        private readonly IPAddress ipAddress;
        private readonly int port;
        private readonly TcpListener listener;

        private readonly IRoutingTable routingTable;
        private readonly IServiceCollection serviceCollection;

        private HttpServer(string ipAddress, int port, IRoutingTable routingTable)
        {
            this.ipAddress = IPAddress.Parse(ipAddress);
            this.port = port;

            this.listener = new TcpListener(this.ipAddress, this.port);

            this.serviceCollection = new ServiceCollection();
            this.routingTable = routingTable;
        }

        private HttpServer(int port, IRoutingTable routingTable) : this("127.0.0.1", port, routingTable)
        {
        }

        private HttpServer(IRoutingTable routingTable) : this(5000, routingTable)
        {
        }

        public static HttpServer WithRoutes(Action<IRoutingTable> routingTableConfiguration)
        {
            var routingTable = new RoutingTable();
            routingTableConfiguration(routingTable);
            var httpServer = new HttpServer(routingTable);

            return httpServer;

        }

        public HttpServer WithServices(Action<IServiceCollection> serviceCollectionConfiguration)
        {
            serviceCollectionConfiguration(this.serviceCollection);
            return this;
        }

        public HttpServer WithConfiguration<TService>(Action<TService> configuration)
            where TService : class
        {
            var service = this.serviceCollection.Get<TService>();

            if (service == null)
            {
                throw new InvalidOperationException($"Service '{typeof(TService).FullName}' is not registered.");
            }

            configuration(service);

            return this;
        }

        public async Task Start()
        {
            this.listener.Start();

            Console.WriteLine($"Server started on port {port}...");
            Console.WriteLine("Listening for requests ");
            Console.WriteLine();

            while (true)
            {

                var connection = await this.listener.AcceptTcpClientAsync();

                _ = Task.Run(async () =>
                {
                    var networkStream = connection.GetStream();
                    var requestText = await this.ReadRequest(networkStream);

                    Console.WriteLine(requestText);

                    try
                    {
                        var request = HttpRequest.Parse(requestText, this.serviceCollection);

                        var response = this.routingTable.ExecuteRequest(request);

                        this.PrepareSession(request, response);
                        this.LogPipeline(requestText, response.ToString());
                        await WriteResponse(networkStream, response);
                    }
                    catch (Exception ex)
                    {
                        var response = HttpResponse.CreateErrorResponse(ex.Message);
                        await WriteResponse(networkStream, response);
                        await ExceptionHandler(networkStream, ex);
                    }

                    connection.Close();
                });
            }
        }

        private async Task<string> ReadRequest(NetworkStream networkStream)
        {
            var bufferLength = 1024;
            var buffer = new byte[bufferLength];

            var requestBuilder = new StringBuilder();
            var totalBytes = 0;

            do
            {
                var bytesRead = await networkStream.ReadAsync(buffer, 0, bufferLength);
                totalBytes += bytesRead;

                if (totalBytes > 10 * 1024)
                {
                    throw new InvalidOperationException("Request is too large");
                }

                requestBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
            }
            while (networkStream.DataAvailable);

            return requestBuilder.ToString();
        }

        private void PrepareSession(HttpRequest request, HttpResponse response)
        {
            if(request.Session.IsNew)
            {
                response.Cookies.Add(HttpSession.SessionCookieName, request.Session.Id);
                request.Session.IsNew = false;
            }
        }

        private static async Task WriteResponse(NetworkStream networkStream, HttpResponse response)
        {
            var responseBytes = Encoding.UTF8.GetBytes(response.ToString());

            await networkStream.WriteAsync(responseBytes);

            if (response.HasContent)
            {
                await networkStream.WriteAsync(response.Content);
            }
        }

        private static async Task ExceptionHandler(NetworkStream networkStream, Exception ex)
        {
            var errorResponse = new HttpResponse(Http.HttpStatusCode.InternalServerError);

            await WriteResponse(networkStream, errorResponse);
        }

        private void LogPipeline(string request, string response)
        {
            var separator = new string('-', 50);
            var logger = new StringBuilder();

            logger.AppendLine();
            logger.AppendLine(separator);

            logger.AppendLine("REQUEST:");
            logger.AppendLine(request);

            logger.AppendLine();

            logger.AppendLine("RESPONSE:");
            logger.AppendLine(response);

            logger.AppendLine();

            Console.WriteLine(logger);
        }
    }
}
