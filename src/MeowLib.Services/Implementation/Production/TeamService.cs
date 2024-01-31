using MeowLib.DAL;
using MeowLib.Domain.Notification.Services;
using MeowLib.Domain.Shared;
using MeowLib.Domain.Shared.Result;
using MeowLib.Domain.Team.Dto;
using MeowLib.Domain.Team.Entity;
using MeowLib.Domain.Team.Exceptions;
using MeowLib.Domain.Team.Services;
using MeowLib.Domain.TeamMember.Entity;
using MeowLib.Domain.TeamMember.Enums;
using MeowLib.Domain.User.Entity;
using MeowLib.Domain.User.Exceptions;
using MeowLib.Domain.User.Services;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.Services.Implementation.Production;

public class TeamService(
    ApplicationDbContext dbContext,
    INotificationService notificationService,
    INotificationTokenService notificationTokenService,
    IUserService userService)
    : ITeamService
{
    public async Task<Result<TeamEntityModel>> CreateNewTeamAsync(int createdById, string name, string description)
    {
        var foundedOwner = await userService.GetUserByIdAsync(createdById);
        if (foundedOwner is null)
        {
            return Result<TeamEntityModel>.Fail(new TeamOwnerNotFoundException(createdById));
        }

        if (await dbContext.Teams.AnyAsync(t => t.Name == name))
        {
            return Result<TeamEntityModel>.Fail(new TeamNameAlreadyTakenException());
        }

        // create team
        var createdEntry = await dbContext.Teams.AddAsync(new TeamEntityModel
        {
            Name = name,
            Description = description,
            Members = new List<TeamMemberEntityModel>(),
            Owner = foundedOwner
        });

        await dbContext.SaveChangesAsync();

        // add owner to team
        var createdTeam = createdEntry.Entity;
        createdTeam.Members.Add(new TeamMemberEntityModel
        {
            User = foundedOwner,
            Team = createdTeam,
            Role = UserTeamMemberRoleEnum.Admin
        });

        dbContext.Update(createdTeam);
        await dbContext.SaveChangesAsync();

        return Result<TeamEntityModel>.Ok(createdEntry.Entity);
    }

    public async Task<TeamEntityModel?> GetTeamByIdAsync(int teamId)
    {
        return await dbContext.Teams
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
        dbContext.Update(foundedUser);
        await dbContext.SaveChangesAsync();
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
        // todo: add notification send?

        dbContext.Remove(foundedUser);
        await dbContext.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result> InviteUserToTeamAsync(int teamId, int userId)
    {
        var foundedTeam = await GetTeamByIdAsync(teamId);
        if (foundedTeam is null)
        {
            return Result.Fail(new TeamNotFoundException(teamId));
        }

        var foundedUser = await userService.GetUserByIdAsync(userId);
        if (foundedUser is null)
        {
            return Result.Fail(new UserNotFoundException(userId));
        }

        if (foundedTeam.Members.Any(m => m.User.Id == userId))
        {
            return Result.Fail(new UserAlreadyInTeamException(userId, teamId));
        }

        var sendNotificationResult = await notificationService.SendInviteToTeamNotificationAsync(foundedTeam.Id,
            foundedUser.Id);
        if (sendNotificationResult.IsFailure)
        {
            var errorMessage = sendNotificationResult.GetError().Message;
            return Result.Fail(
                new InnerException($"Ошибка отправки уведомления о вступлении в комманду: {errorMessage}"));
        }

        return Result.Ok();
    }

    public async Task<Result> AcceptInviteToTeamAsync(int userId, string token)
    {
        var parsedToken = await notificationTokenService.ParseInviteToTeamTokenAsync(token);
        if (parsedToken is null)
        {
            // todo: change this?
            return Result.Fail(new TeamInvitationIsNotForUserException());
        }

        if (userId != parsedToken.UserId)
        {
            return Result.Fail(new TeamInvitationIsNotForUserException());
        }

        if (DateTime.UtcNow > parsedToken.InviteExpiredAt)
        {
            return Result.Fail(new TeamInvitationExpiredException());
        }

        var foundedUser = await userService.GetUserByIdAsync(parsedToken.UserId);
        if (foundedUser is null)
        {
            return Result.Fail(new InnerException("Пользователь не найден"));
        }

        var foundedTeam = await GetTeamByIdAsync(parsedToken.TeamId);
        if (foundedTeam is null)
        {
            return Result.Fail(new InnerException("Команда не найдена"));
        }

        return await AddUserToTeamAsync(foundedUser, foundedTeam);
    }

    public async Task<Result> AddUserToTeamAsync(UserEntityModel user, TeamEntityModel team)
    {
        if (await dbContext.TeamMembers.AnyAsync(t => t.Team.Id == team.Id && t.User.Id == user.Id))
        {
            return Result.Fail(new UserAlreadyInTeamException(user.Id, team.Id));
        }

        await dbContext.TeamMembers.AddAsync(new TeamMemberEntityModel
        {
            User = user,
            Team = team,
            Role = UserTeamMemberRoleEnum.Standard
        });
        await dbContext.SaveChangesAsync();
        return Result.Ok();
    }

    public Task<bool> CheckUserInTeamAsync(int userId, int teamId)
    {
        return dbContext.TeamMembers.AnyAsync(tm => tm.Team.Id == teamId && tm.User.Id == userId);
    }

    public async Task<List<TeamDto>> GetAllUserTeams(int userId)
    {
        return await dbContext.TeamMembers.Where(t => t.User.Id == userId)
            .Select(t => new TeamDto
            {
                Id = t.Team.Id,
                Name = t.Team.Name
            })
            .ToListAsync();
    }

    public async Task<List<TeamDto>> GetTeamsAsync(int skipCount, int takeCount)
    {
        return await dbContext.Teams
            .OrderBy(t => t.Id)
            .Skip(skipCount)
            .Take(takeCount)
            .Select(t => new TeamDto
            {
                Id = t.Id,
                Name = t.Name
            })
            .ToListAsync();
    }
}