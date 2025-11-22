using SomeTrade.Infrastructure.Interfaces;

namespace SomeTrade.Infrastructure.Logging.DummyLog
{
    public class Logger<T> : ILogger<T>
    {
        public void Debug(string message){}
        public void Debug(string message, T t) { }
        public void Debug(string message, params object[] propertyValues)
        {
        }

        public void Error(string message) { }
        public void Error(string message, T t) { }
        public void Error(string message, params object[] propertyValues)
        {
        }

        public void Fatal(string message) { }
        public void Fatal(string message, T t) { }
        public void Fatal(string message, params object[] propertyValues)
        {
        }

        public void Information(string message) { }
        public void Information(string message, T t) { }
        public void Information(string message, params object[] propertyValues)
        {
        }

        public void Verbose(string message) { }
        public void Verbose(string message, T t) { }
        public void Verbose(string message, params object[] propertyValues)
        {
            throw new System.NotImplementedException();
        }

        public void Warning(string message) { }
        public void Warning(string message, T t) { }
        public void Warning(string message, params object[] propertyValues)
        {
        }
    }
}
