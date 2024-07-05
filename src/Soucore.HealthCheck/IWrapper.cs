namespace Soucore.HealthCheck
{
    internal interface IWrapper
    {
        string Alias { get; }
        void SetAlias(string alias);
    }
}