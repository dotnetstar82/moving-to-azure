using AutoMapper;
using MovingToAzure.Data;
using MovingToAzure.Models;

namespace MovingToAzure.Services;

public interface IProfileHelper
{
    ProfileViewModel PostToView(ProfilePostModel model);
    ProfileViewModel EntityToView(ProfileEntity entity);
    void PostToEntity(ProfilePostModel model, ProfileEntity entity);
}
public class ProfileHelper : IProfileHelper
{
    private readonly IMapper mapper;

    public ProfileHelper(IMapper mapper)
    {
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public ProfileViewModel EntityToView(ProfileEntity entity) =>
       this.mapper.Map<ProfileViewModel>(entity);

    public ProfileViewModel PostToView(ProfilePostModel model) =>
        this.mapper.Map<ProfileViewModel>(model);

    public void PostToEntity(ProfilePostModel model, ProfileEntity entity) =>
        this.mapper.Map<ProfilePostModel, ProfileEntity>(model, entity);

}
