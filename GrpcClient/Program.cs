﻿using Grpc.Core;
using Grpc.Net.Client;
using GrpcServer;
internal class Program
{
    private static async Task Main(string[] args)
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:5001");
        var client = new Greeter.GreeterClient(channel);

        using var call = client.SayHelloStream();

        var readTask = Task.Run(async () =>
        {
            await foreach (var response in call.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine(response.Message);
            }
        });

        while (true)
        {
            var result = Console.ReadLine();
            if(string.IsNullOrEmpty(result))
            {
                break;
            }
            await call.RequestStream.WriteAsync(new HelloRequest() { Name = result });
        }

        await call.RequestStream.CompleteAsync();
        await readTask;

        Console.ReadLine();
    }
}