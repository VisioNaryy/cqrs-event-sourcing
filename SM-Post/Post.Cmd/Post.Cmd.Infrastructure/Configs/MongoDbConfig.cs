namespace Post.Cmd.Infrastructure.Configs;

public class MongoDbConfig
{
    public string ConnectionString { get; set; }
    public string Database { get; set; }
    public string Collection { get; set; }
}