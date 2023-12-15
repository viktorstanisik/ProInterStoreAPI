-- Based on send filters modify the Where part

--Query For Filtering 
SELECT *
FROM dbo.StoreItems si
    INNER JOIN dbo.StoreItemAttribute sa
        ON si.Id = sa.StoreItemId
WHERE si.Name = 'TestName'
      AND si.ItemCode = 'Code2'
      AND sa.AttributeColor = 'Red'
--