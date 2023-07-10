namespace MigrationRunner;

public record DatabaseInfo(string label, string connectionString)
{
    public override string ToString()
    {
        return label;
    }
}