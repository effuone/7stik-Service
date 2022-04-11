SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCandidates
AS
BEGIN
	select 
	portfolios.PortfolioId,
	candidates.CandidateId,
	users.FirstName,
	users.LastName,
	candidates.Birthday,
	countries.CountryName,
	cities.CityName,
	schools.SchoolName,
	candidates.GraduateYear
	from 
	Portfolios as portfolios,
	Candidates as candidates,
	AspNetUsers as users,
	Schools as schools,
	Locations as locations,
	Countries as countries,
	Cities as cities
	where 
	candidates.CandidateId = portfolios.CandidateId
	and candidates.ZhetistikUserId = users.Id
	and locations.LocationId = candidates.LocationId
	and countries.CountryId = locations.CountryId 
	and cities.CityId = locations.CityId
	and schools.SchoolId = candidates.SchoolId
END
GO
