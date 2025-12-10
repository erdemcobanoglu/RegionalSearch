
-- sorgular organizasyon kisi sayisi
SELECT 
    o.Name AS Organization,
    COUNT(p.Id) AS PeopleCount
FROM Organizations o
LEFT JOIN People p ON p.OrganizationId = o.Id
GROUP BY o.Id, o.Name
ORDER BY PeopleCount DESC;



-- sorgular organizasyon sehir kisi sayisi
SELECT 
    o.Name AS Organization,
    p.BirthPlace,
    COUNT(*) AS PersonCount
FROM People p
JOIN Organizations o ON o.Id = p.OrganizationId
GROUP BY o.Id, o.Name, p.BirthPlace
ORDER BY COUNT(*) DESC;   -- ⬅ en çoktan en aza

