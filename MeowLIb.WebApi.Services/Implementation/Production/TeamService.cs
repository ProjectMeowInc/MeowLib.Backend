using MeowLib.Domain.DbModels.TeamEntity;
using MeowLib.Domain.DbModels.TeamMemberEntity;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.Team;
using MeowLib.Domain.Result;
using MeowLib.WebApi.DAL;
using MeowLib.WebApi.DAL.Repository.Interfaces;
using MeowLIb.WebApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MeowLIb.WebApi.Services.Implementation.Production;

public class TeamService : ITeamService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IUserRepository _userRepository;
    
    public TeamService(ApplicationDbContext dbContext, IUserRepository userRepository)
    {
        _dbContext = dbContext;
        _userRepository = userRepository;
    }

    public async Task<Result<TeamEntityModel>> CreateNewTeamAsync(int createdById, string name, string description)
    {
        var foundedOwner = await _userRepository.GetByIdAsync(createdById);
        if (foundedOwner is null)
        {
            return Result<TeamEntityModel>.Fail(new TeamOwnerNotFoundException(createdById));
        }
        
        // create team
        var createdEntry = await _dbContext.Teams.AddAsync(new TeamEntityModel
        {
            Name = name,
            Description = description,
            Members = new List<TeamMemberEntityModel>(),
            Owner = foundedOwner
        });

        await _dbContext.SaveChangesAsync();
        
        // add owner to team
        var createdTeam = createdEntry.Entity;
        createdTeam.Members.Add(new TeamMemberEntityModel
        {
            User = foundedOwner,
            Team = createdTeam,
            Role = UserTeamMemberRoleEnum.Admin
        });

        _dbContext.Update(createdTeam);
        await _dbContext.SaveChangesAsync();
        
        return Result<TeamEntityModel>.Ok(createdEntry.Entity);
    }

    public async Task<TeamEntityModel?> GetTeamByIdAsync(int teamId)
    {
        return await _dbContext.Teams
            .Include(t => t.Members)
            .ThenInclude(t => t.User)
            .Include(t => t.Owner)
            .FirstOrDefaultAsync(t => t.Id == teamId);
    }
}