namespace DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities
{
    public class ConfigurationEntity
    {
        public ConfigurationEntity()
        {
            
        }
        public ConfigurationEntity(int moduleId,string parameterId,IList<ParameterEntity> parameters)
        {
            ModuleId = moduleId;
            ParameterId = parameterId;
        }

        public int ModuleId { get; set; }
        public string ParameterId { get; set; }
        public string ParameterKey { get; set; }
        public string ParameterValue { get; set; }
    }
}
