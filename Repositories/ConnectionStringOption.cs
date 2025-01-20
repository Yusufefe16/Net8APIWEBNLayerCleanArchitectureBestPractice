namespace App.Repositories;

public class ConnectionStringOption
{
    
    public const string Key = "ConnectionString";
    public string SqlServer { get; set; } = default!;
}