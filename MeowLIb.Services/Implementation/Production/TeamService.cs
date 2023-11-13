using MeowLib.DAL;
using MeowLib.DAL.Repository.Interfaces;
using MeowLib.Domain.DbModels.TeamEntity;
using MeowLib.Domain.DbModels.TeamMemberEntity;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.Team;
using MeowLib.Domain.Exceptions.User;
using MeowLib.Domain.Result;
using MeowLIb.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MeowLIb.Services.Implementation.Production;

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
    
    public async Task<Result> SetUserTeamRoleAsync(int teamId, int userId, UserTeamMemberRoleEnum role)
    {
        var foundedTeam = await GetTeamByIdAsync(teamId);
        if (foundedTeam is null)
        {
            return Result.Fail(new TeamNotFoundException(teamId));
        }

        if (foundedTeam.Owner.Id == userId)
        {
            return Result.Fail(new ChangeOwnerRoleException());
        }
        
        var foundedUser = foundedTeam.Members.FirstOrDefault(m => m.User.Id == userId);
        if (foundedUser is null)
        {
            return Result.Fail(new UserNotFoundException(userId));
        }

        foundedUser.Role = role;
        _dbContext.Update(foundedUser);
        await _dbContext.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<bool> CheckIsUserCanChangeTeamRoleAsync(int teamId, int userId)
    {
        var foundedTeam = await GetTeamByIdAsync(teamId);
        if (foundedTeam is null)
        {
            return false;
        }

        if (foundedTeam.Owner.Id == userId)
        {
            return true;
        }
        
        var foundedUser = foundedTeam.Members.FirstOrDefault(m => m.User.Id == userId);
        if (foundedUser is null)
        {
            return false;
        }

        return foundedUser.Role == UserTeamMemberRoleEnum.Admin;
    }

    public async Task<Result> RemoveFromTeamAsync(int teamId, int userId)
    {
        var foundedTeam = await GetTeamByIdAsync(teamId);
        if (foundedTeam is null)
        {
            return Result.Fail(new TeamNotFoundException(teamId));
        }

        if (foundedTeam.Owner.Id == userId)
        {
            // todo: maybe change to another exception?
            return Result.Fail(new ChangeOwnerRoleException());
        }

        var foundedUser = foundedTeam.Members.FirstOrDefault(m => m.User.Id == userId);
        if (foundedUser is null)
        {
            return Result.Fail(new UserNotFoundException(userId));
        }

        // todo: add send notification
        _dbContext.Remove(foundedUser);
        await _dbContext.SaveChangesAsync();

        return Result.Ok();
    }
}