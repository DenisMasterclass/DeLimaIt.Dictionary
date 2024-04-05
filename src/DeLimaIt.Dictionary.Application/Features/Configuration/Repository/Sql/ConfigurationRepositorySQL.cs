using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Sql
{
    internal static class ConfigurationRepositorySQL
    {
        internal const string GetSqlDictionaries = @"SELECT COD_PARAMETRO AS Id,
                                                    NOM_PARAMETRO AS Name
                                                    FROM CONFIGURACAO_PARAMETRO P WITH(NOLOCK)
                                                    WHERE P.COD_MODULO = @ModuleId";

        internal const string GetSqlDictionariesValues = @"SELECT COD_PARAMETRO 'DictionaryId',
                                                        DSC_CHAVE 'Key',
                                                        DSC_VALOR 'Value'
                                                        FROM CONFIGURACAO_CHAVE P 
                                                        WHERE P.COD_PARAMETRO = @Id
                                                        ";

        internal const string InsertSqlDictionaryValue = @"INSERT INTO CONFIGURACAO_CHAVE
                                                    (
                                                        COD_PARAMETRO,
                                                        DSC_CHAVE,
                                                        DSC_VALOR
                                                    )
                                                    VALUES
                                                    (
                                                        @DictionaryId,
                                                        @Key,
                                                        @Value
                                                    )
                                                    ";

        internal const string UpdateSqlDictionaryValue = @"UPDATE CONFIGURACAO_CHAVE 
                                SET DSC_VALOR = @Value 
                                WHERE COD_PARAMETRO = @DictionaryId AND DSC_CHAVE = @Key";

        internal const string DeleteSqlDictionaryValue = @"DELETE FROM CONFIGURACAO_CHAVE 
                                                       WHERE COD_PARAMETRO = @DictionaryId 
                                                       AND DSC_CHAVE = @Key";

        internal const string SelectSqlModules = @"SELECT COD_MODULO ModuleId
                                                  ,NOM_MODULO ModuleName
                                                   FROM CONFIGURACAO_MODULO";

        internal const string UpdateSqlModules = @" UPDATE CONFIGURACAO_MODULO
                                                    SET NOM_MODULO = @ModuleName 
                                                    WHERE COD_MODULO = @ModuleId";

        internal const string InsertSqlModules = @" INSERT INTO CONFIGURACAO_MODULO
                                                    (
                                                    NOM_MODULO
                                                    )
                                                    VALUES
                                                    (
                                                    @ModuleName
                                                    )";
        internal const string DeleteSqlModules = @"DELETE FROM CONFIGURACAO_MODULO WHERE COD_MODULO = @ModuleId";
    }
}
