using AutoMapper;
using MyCompany.Domain.Entities;
using MyCompany.Domain.Models;
using MyCompany.Domain.Models.Actions;
using MyCompany.Api.Controllers.v1.Players.Dto;
using MyCompany.Api.Controllers.v1.Team.Dto;

namespace MyCompany.Api.Mapping;

internal sealed class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        MapApiPlayerProfiles(); MapDomainPlayerProfiles();
        MapApiTeamProfiles(); MapDomainTeamProfiles();
    }

    private void MapApiPlayerProfiles()
    {
        // POST: Domain -> Response
        CreateMap<Player, CreatePlayerResponse>().IncludeBase<Player, PlayerDetails>();
        
        // POST: Request -> NewTeam 
        CreateMap<CreatePlayerRequest, NewPlayer>();
        
        //PATCH: Request -> UpdatePlayer
        CreateMap<PatchPlayerRequest, UpdatePlayer>();
        
        // Get: Domain -> Response
        CreateMap<Player, GetPlayerResponse>().IncludeBase<Player, PlayerDetails>();
        
        // Shared
        CreateMap<Player, PlayerDetails>();
    }

    private void MapApiTeamProfiles()
    {
        // POST: Domain -> Response
        CreateMap<Team, CreateTeamResponse>().IncludeBase<Team, TeamDetails>();
        
        // POST: Request -> NewTeam 
        CreateMap<CreateTeamRequest, NewTeam>();
        
        //PATCH: Request -> UpdateTeam
        CreateMap<PatchTeamRequest, UpdateTeam>();

        // Get: Domain -> Response
        CreateMap<Team, GetTeamDetailsResponse>().IncludeBase<Team, TeamDetails>();

        // Get: Domain with players -> response
        CreateMap<Team, GetTeamDetailsWithPlayers>()
            .ForMember(dest => dest.Players, opt => opt.MapFrom(src => src.Players));

        // Shared
        CreateMap<Team, TeamDetails>();
    }

    private void MapDomainPlayerProfiles()
    {
        CreateMap<NewPlayer, PlayerEntity>();
        //CreateMap<UpdatePlayer, PlayerEntity>();
        
        CreateMap<UpdatePlayer, PlayerEntity>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<PlayerEntity, Player>();
    }

    private void MapDomainTeamProfiles()
    {
        CreateMap<NewTeam, TeamEntity>();
        //CreateMap<UpdateTeam, TeamEntity>();
        
        //We dont want to add nulls for properties that are not part of the update
        CreateMap<UpdateTeam, TeamEntity>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<TeamEntity, Team>();
    }
}