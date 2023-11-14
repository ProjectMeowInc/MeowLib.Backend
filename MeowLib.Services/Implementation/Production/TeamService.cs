using MeowLib.DAL;
using MeowLib.DAL.Repository.Interfaces;
using MeowLib.Domain.DbModels.TeamEntity;
using MeowLib.Domain.DbModels.TeamMemberEntity;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.Team;
using MeowLib.Domain.Exceptions.User;
using MeowLib.Domain.Result;
using MeowLib.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.Services.Implementation.Production;

public class TeamService : ITeamService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationService;
    
    public TeamService(ApplicationDbContext dbContext, IUserRepository userRepository, 
        INotificationService notificationService)
    {
        _dbContext = dbContext;
        _userRepository = userRepository;
        _notificationService = notificationService;
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

    public async Task<bool> CheckUserIsTeamAdminAsync(int teamId, int userId)
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
    
    /// <summary>
    /// Метод отправляет приглашение на вступление в комманду.
    /// </summary>
    /// <param name="teamId">Id комманды.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Результат приглашения</returns>
    /// <exception cref="TeamNotFoundException">Возникает в случае, если комманда не была найдена.</exception>
    /// <exception cref="UserNotFoundException">Возникает в случае, пользователь не был найден.</exception>
    /// <exception cref="UserAlreadyInTeamException">Возникает в случае, если пользователь уже состоит в данной комманде</exception>
    public async Task<Result> InviteUserToTeamAsync(int teamId, int userId)
    {
        var foundedTeam = await GetTeamByIdAsync(teamId);
        if (foundedTeam is null)
        {
            return Result.Fail(new TeamNotFoundException(teamId));
        }

        var foundedUser = await _userRepository.GetByIdAsync(userId);
        if (foundedUser is null)
        {
            return Result.Fail(new UserNotFoundException(userId));
        }
        
        if (foundedTeam.Members.Any(m => m.User.Id == userId))
        {
            return Result.Fail(new UserAlreadyInTeamException(userId, teamId));
        }

        var sendNotificationResult = await _notificationService.SendInviteToTeamNotificationAsync(foundedTeam.Id, 
            foundedUser.Id);
        if (sendNotificationResult.IsFailure)
        {
            var errorMessage = sendNotificationResult.GetError().Message;
            return Result.Fail(
                new InnerException($"Ошибка отправки уведомления о вступлении в комманду: {errorMessage}"));
        }

        return Result.Ok();
    }
}