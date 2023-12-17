using MeowLib.DAL;
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

public class TeamService(
    ApplicationDbContext dbContext,
    INotificationService notificationService,
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

        var sendNotificationResult =
            await notificationService.SendInviteToTeamNotificationAsync(foundedTeam.Id, foundedUser.Id);
        if (sendNotificationResult.IsFailure)
        {
            return Result.Fail(new InnerException(sendNotificationResult.GetError().Message));
        }

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

    public Task<bool> CheckUserInTeamAsync(int userId, int teamId)
    {
        return dbContext.TeamMembers.AnyAsync(tm => tm.Team.Id == teamId && tm.User.Id == userId);
    }
}