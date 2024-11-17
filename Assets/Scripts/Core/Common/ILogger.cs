namespace Core.Common
{
    public enum Severity
    {
        Info,
        Warning,
        Error
    }
    
    public interface ILogger
    {
        public void Log(object message, Severity severity = Severity.Info);
    }
}