namespace TestProject3;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
    }
}
public class Testing
{
    
//"Review the following C# async method and identify any issues that could cause deadlocks or performance bottlenecks. Suggest improvements:


// public string GetData()
//
// {
//     var data = await GetDataAsync().Result;
//     return data;
//
// }
//
// public async Task<string> GetDataAsync()
//
// {
//     await Task.Delay(1000);
//     return ""Hello World"";
//
// }
// Given the following Kafka consumer code snippet, identify any potential issues that could lead to message loss or processing delays and suggest improvements:
    public void Processing()
    {
        var consumer = new ConsumerBuilder<Ignore, string>(config).Build();

        consumer.Subscribe(""my-topic"");

        while (true) 
        {
            var consumeResult = consumer.Consume();
            ProcessMessage(consumeResult.Message.Value);
            consumer.Commit(consumeResult);

        }
    }
   
