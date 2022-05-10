SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCandidates @Id INT
AS
BEGIN
	select us.UserName, us.FirstName, us.LastName, us.Email, us.PhoneNumber
	from Candidates as can, AspNetUsers as us where us.Id = can.ZhetistikUserId and can.CandidateId = @Id
END
GO
EXEC GetCandidate @Id = 1