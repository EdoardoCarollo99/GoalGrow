-- Verifica che KeycloakSubjectId sia stato aggiunto
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users' 
  AND COLUMN_NAME = 'KeycloakSubjectId';

-- Verifica stato attuale utenti
SELECT 
    Id,
    FirstName,
    LastName,
    EmailAddress,
    UserType,
    KeycloakSubjectId
FROM Users;
