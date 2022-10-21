using AutoMapper;
using MovingToAzure.Data;
using MovingToAzure.Models;

namespace MovingToAzure.Services;

public class AutoMapperConfiguration : Profile
{
    public AutoMapperConfiguration()
    {
        CreateMap<ProfilePostModel, ProfileEntity>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.ProfilePic, opt => opt.Ignore());
        CreateMap<ProfileEntity, ProfileViewModel>(MemberList.Destination);
        CreateMap<ProfilePostModel, ProfileViewModel>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.ProfilePic, opt => opt.Ignore());
    }
}
