-- ============================================
-- VERIFICA AGGIUNTA KeycloakSubjectId
-- ============================================

-- 1. Verifica che la colonna esista
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users' 
  AND COLUMN_NAME = 'KeycloakSubjectId';

-- 2. Verifica struttura completa tabella Users
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    CHARACTER_MAXIMUM_LENGTH,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users'
ORDER BY ORDINAL_POSITION;

-- 3. Verifica migrazioni applicate (deve esserci AddKeycloakSubjectId)
SELECT 
    MigrationId,
    ProductVersion
FROM __EFMigrationsHistory
ORDER BY MigrationId DESC;

-- 4. Test inserimento con KeycloakSubjectId (NON ESEGUIRE se non necessario)
/*
UPDATE Users 
SET KeycloakSubjectId = 'test-keycloak-sub-id'
WHERE Id = (SELECT TOP 1 Id FROM Users WHERE UserType = 3); -- Admin user
*/

-- 5. Verifica utenti esistenti
SELECT 
    Id,
    FirstName,
    LastName,
    EmailAddress,
    UserType,
    KeycloakSubjectId
FROM Users;
