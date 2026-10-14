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
                                SET DSC_CHAVE = @Key, DSC_VALOR = @Value
                                WHERE COD_PARAMETRO = @DictionaryId AND DSC_CHAVE = @OldKey";

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

        internal const string CreateModuleSql = @"INSERT INTO dbo.CONFIGURACAO_MODULO (NOM_MODULO) OUTPUT INSERTED.COD_MODULO VALUES (@ModuleName)";
        internal const string CreateDictionarySql = @"INSERT INTO dbo.CONFIGURACAO_PARAMETRO (COD_MODULO, NOM_PARAMETRO) OUTPUT INSERTED.COD_PARAMETRO VALUES (@ModuleId, @Name)";
        internal const string UpdateDictionarySql = @"UPDATE dbo.CONFIGURACAO_PARAMETRO SET NOM_PARAMETRO = @Name WHERE COD_PARAMETRO = @Id";
        internal const string DeleteDictionarySql = @"
BEGIN TRANSACTION;
BEGIN TRY
DELETE FROM dbo.CONFIGURACAO_CHAVE WHERE COD_PARAMETRO = @Id;
DELETE FROM dbo.CONFIGURACAO_PARAMETRO WHERE COD_PARAMETRO = @Id;
COMMIT TRANSACTION;
END TRY
BEGIN CATCH
IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
THROW;
END CATCH;";
        internal const string DeleteModuleAndChildrenSql = @"
BEGIN TRANSACTION;
BEGIN TRY
DELETE FROM dbo.CONFIGURACAO_CHAVE WHERE COD_PARAMETRO IN (SELECT COD_PARAMETRO FROM dbo.CONFIGURACAO_PARAMETRO WHERE COD_MODULO = @ModuleId);
DELETE FROM dbo.CONFIGURACAO_PARAMETRO WHERE COD_MODULO = @ModuleId;
DELETE FROM dbo.CONFIGURACAO_MODULO WHERE COD_MODULO = @ModuleId;
COMMIT TRANSACTION;
END TRY
BEGIN CATCH
IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
THROW;
END CATCH;";
    }
}
