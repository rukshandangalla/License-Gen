#r "Newtonsoft.Json"
#load "..\Shared\OrderHelper.csx"

using System;
using System.Net;
using Newtonsoft.Json;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log, 
        IAsyncCollector<Order> outputQueueItem, IAsyncCollector<Order> outputTable)
{
    log.Info("Order Received!!");
    string jsonContent = await req.Content.ReadAsStringAsync();
    var order = JsonConvert.DeserializeObject<Order>(jsonContent);
    log.Info($"Order {order.OrderId} received from {order.Email} for product {order.ProductId}");
    order.PartitionKey = "Orders";
    order.RowKey = order.OrderId;

    await outputQueueItem.AddAsync(order);
    await outputTable.AddAsync(order);

    return req.CreateResponse(HttpStatusCode.OK, new { message = $"Thank you for your order!"});
}

