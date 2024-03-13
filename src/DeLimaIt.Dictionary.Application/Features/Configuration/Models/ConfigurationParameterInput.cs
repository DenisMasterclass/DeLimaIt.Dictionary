namespace DeLimaIt.Dictionary.Application.Features.Configuration.Models
{
    public enum OperationType: int
    {
        Insert,
        Update,
        Delete
    }
    public sealed class ConfigurationParameterInput
    {
        public ConfigurationParameterInput(OperationType operation, int parameterId, string key, string value)
        {
            Operation = operation;
            ParameterId = parameterId;
            Key = key;
            Value = value;
        }

        public OperationType Operation { get; set; }
        public int ParameterId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
