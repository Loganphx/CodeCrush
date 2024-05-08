﻿using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, MemberDto>();
        CreateMap<Photo, PhotoDto>();
        CreateMap<Message, MessageDto>()
            .ForMember(d => d.SenderPhotoUrl, o => 
                o.MapFrom(s => s.Sender.Photos.FirstOrDefault(x => x.IsMain).Url))
        .ForMember(d => d.RecipientPhotoUrl, o => 
                o.MapFrom(s => s.Recipient.Photos.FirstOrDefault(x => x.IsMain).Url));
    }
}