namespace DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities
{
    public class ConfigurationEntity
    {
        public ConfigurationEntity()
        {
            
        }
        public ConfigurationEntity(int moduleId,string DictionaryId,IList<DictionaryEntity> Dictionaries)
        {
            ModuleId = moduleId;
            DictionaryId = DictionaryId;
        }

        public int ModuleId { get; set; }
        public string DictionaryId { get; set; }
        public string DictionaryKey { get; set; }
        public string DictionaryValue { get; set; }
    }
}
