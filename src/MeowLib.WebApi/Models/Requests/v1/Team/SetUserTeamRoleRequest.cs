﻿using MeowLib.Domain.TeamMember.Enums;

namespace MeowLib.WebApi.Models.Requests.v1.Team;

public class SetUserTeamRoleRequest
{
    public required UserTeamMemberRoleEnum NewRole { get; set; }
}