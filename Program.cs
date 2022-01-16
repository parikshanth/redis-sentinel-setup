// paste this into a Console project to start
using StackExchange.Redis;

Console.WriteLine("Starting Sentinel Demo");


var configString = "192.168.1.5:26379,192.168.1.6:26379,192.168.1.8:26379, serviceName=mymaster,allowAdmin=true,password=Complex-Password-Goes-Here";
ConfigurationOptions options = ConfigurationOptions.Parse(configString);


var redis = ConnectionMultiplexer.Connect(options);




while (true)
{
    try
    {
        IDatabase db = redis.GetDatabase();
        db.StringIncrement("test");

        Console.WriteLine(db.StringGet("test"));

        System.Threading.Thread.Sleep(5000);

    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
        System.Threading.Thread.Sleep(1000);
    }
}
