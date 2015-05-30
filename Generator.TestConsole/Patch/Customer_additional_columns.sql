ALTER TABLE dbo.Customer ADD FullName AS ([FirstName] + ' ' + [LastName]);

ALTER TABLE dbo.Customer ADD Comment NVARCHAR(MAX);

ALTER TABLE dbo.Customer ADD FullDetail AS ('FullName: ' + [FirstName] + ' ' + [LastName] + char(10) + 'Email: ' + ISNULL([Email], '') + char(10) + 'Comment: ' + ISNULL([Comment], ''));
