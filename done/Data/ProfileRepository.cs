namespace MovingToAzure.Data;

public interface IProfileRepository
{
    int Save(ProfileEntity profile);
    List<ProfileEntity> GetAll();
    ProfileEntity? GetById(int id);
    int Delete(int id);
}

public class ProfileRepository : IProfileRepository
{
    private readonly ISqlDbContext db;

    public ProfileRepository(ISqlDbContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
    }

    // Returns number of rows saved
    public int Save(ProfileEntity profile)
    {
        if (profile.Id > 0)
        {
            db.Profiles.Update(profile); // set disconnected object as modified
        }
        else
        {
            db.Profiles.Add(profile);
        }
        return db.SaveChanges();
    }

    public List<ProfileEntity> GetAll()
    {
        return (
            from p in db.Profiles
            orderby p.Name
            select p
        ).ToList();
    }

    public ProfileEntity? GetById(int id)
    {
        if (id < 1)
        {
            return null; // no need to ask db for nothing
        }
        return (
            from p in db.Profiles
            where p.Id == id
            select p
        ).FirstOrDefault();
    }

    // Returns number of rows deleted
    // TODO: soft delete by setting active flag instead
    public int Delete(int id)
    {
        if (id < 1)
        {
            return 0; // no need to ask db for nothing
        }
        int profileId = (
            from p in db.Profiles
            where p.Id == id
            select p.Id
        ).FirstOrDefault();
        if (profileId < 1)
        {
            return 0;
        }
        db.Profiles.RemoveRange(new ProfileEntity { Id = profileId });
        return db.SaveChanges();
    }

}
