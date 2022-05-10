CREATE PROCEDURE GetSchools @Id INT
AS
BEGIN
	select s.SchoolName, s.[Image], s.FoundationYear, c.CountryName, ct.CityName from Schools as s, Locations as l, Countries as c, Cities as ct 
    where s.LocationId = l.LocationId
    and l.CountryId = c.CountryId
    and l.CityId = ct.CityId
    and s.SchoolId = @Id
END
GO