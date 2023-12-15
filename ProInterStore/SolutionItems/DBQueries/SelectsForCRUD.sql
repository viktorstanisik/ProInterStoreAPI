--CRUD SELECTS -  Change the Item Code to track Create Update Delete
SELECT *
FROM dbo.StoreItems
WHERE ItemCode = 'Code7'


SELECT *
FROM dbo.StoreItemAttribute
WHERE StoreItemId IN
      (
          SELECT Id FROM dbo.StoreItems WHERE ItemCode = 'Code7'
      )

SELECT *
FROM dbo.AuditInfo
WHERE StoreItemId IN
      (
          SELECT Id FROM dbo.StoreItems WHERE ItemCode = 'Code7'
      )

