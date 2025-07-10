using HomeBoxLanding.Api.Core.Types;
using HomeBoxLanding.Api.Data;
using HomeBoxLanding.Api.Features.Builds.Types;

namespace HomeBoxLanding.Api.Features.Builds;

public interface IDockerBuildsRepository
{
    List<DockerBuildRecord> GetAll();
    SaveDockerBuildResponse SaveBuild(SaveDockerBuildRequest request);
}

public class DockerBuildsRepository : IDockerBuildsRepository
{
    public List<DockerBuildRecord> GetAll()
    {
        using (var context = new DatabaseContext())
        {
            try
            {
                return context.DockerBuilds
                    .ToList();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return new List<DockerBuildRecord>();
            }
        }
    }
    
    public SaveDockerBuildResponse SaveBuild(SaveDockerBuildRequest request)
    {
        var response = new SaveDockerBuildResponse();

        using (var context = new DatabaseContext())
        {
            try
            {
                var buildRecord = new DockerBuildRecord
                {
                    Identifier = Guid.NewGuid(),
                    StartedAt = request.StartedAt,
                    FinishedAt = request.FinishedAt,
                    Log = request.Log
                };

                context.Add(buildRecord);
                context.SaveChanges();

                response.DockerBuildIdentifier = buildRecord.Identifier;
            }
            catch (Exception exception)
            {
                response.AddError(new Error
                {
                    Code = ErrorCode.DatabaseError,
                    UserMessage = "Something went wrong attempting to save a build log.",
                    TechnicalMessage = $"The following exception was thrown: {exception.Message}"
                });
            }
        }

        return response;
    }
}