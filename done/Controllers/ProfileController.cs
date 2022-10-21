using Microsoft.AspNetCore.Mvc;
using MovingToAzure.Data;
using MovingToAzure.Models;
using MovingToAzure.Services;

namespace MovingToAzure.Controllers;

public class ProfileController : Controller
{
    private readonly IProfileRepository profileRepository;
    private readonly IImageRepository imageRepository;
    private readonly ICacheRepository cacheRepository;
    private readonly IProfileHelper profileHelper;
    private const string CACHE_KEY = "ProfileList";

    public ProfileController(IProfileRepository profileRepository, IImageRepository imageRepository, ICacheRepository cacheRepository, IProfileHelper profileHelper)
    {
        this.profileRepository = profileRepository ?? throw new ArgumentNullException(nameof(profileRepository));
        this.imageRepository = imageRepository ?? throw new ArgumentNullException(nameof(imageRepository));
        this.cacheRepository = cacheRepository ?? throw new ArgumentNullException(nameof(cacheRepository));
        this.profileHelper = profileHelper ?? throw new ArgumentNullException(nameof(profileHelper));
    }

    public ActionResult Index(CrudStatus? msg)
    {
        List<ProfileEntity>? list = cacheRepository.GetOrLoad(CACHE_KEY, profileRepository.GetAll);
        var models = (
            from p in list
            select profileHelper.EntityToView(p)
        ).ToList();
        return View(new ProfileListViewModel
        {
            Profiles = models,
            StatusMessage = msg
        });
    }

    [HttpGet]
    public ActionResult Create()
    {
        var model = new ProfileViewModel();
        return View("Edit", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(ProfilePostModel body)
    {
        if (!ModelState.IsValid)
        {
            // fix your errors first
            var model = profileHelper.PostToView(body);
            return View(model);
        }
        var entity = new ProfileEntity();
        profileHelper.PostToEntity(body, entity);
        profileRepository.Save(entity);
        if (body.ProfilePic != null)
        {
            entity.ProfilePic = await imageRepository.SaveProfilePic(entity.Id, body.ProfilePic);
            profileRepository.Save(entity);
        }
        cacheRepository.Clear(CACHE_KEY);
        return RedirectToAction(nameof(Index), new { msg = "created" });
    }

    [HttpGet]
    public ActionResult Edit(int id)
    {
        var entity = profileRepository.GetById(id);
        if (entity == null)
        {
            return View("NotFound");
        }
        var model = profileHelper.EntityToView(entity);
        return View("Edit", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(int id, ProfilePostModel body)
    {
        if (!ModelState.IsValid)
        {
            // fix your errors first
            var model = profileHelper.PostToView(body);
            return View(model);
        }
        var entity = profileRepository.GetById(id);
        if (entity == null)
        {
            return View("NotFound");
        }
        profileHelper.PostToEntity(body, entity);
        if (body.ProfilePic != null)
        {
            entity.ProfilePic = await imageRepository.SaveProfilePic(entity.Id, body.ProfilePic);
        }
        profileRepository.Save(entity);
        cacheRepository.Clear(CACHE_KEY);
        return RedirectToAction(nameof(Index), new { msg = CrudStatus.Saved });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id)
    {
        // TODO: delete image
        profileRepository.Delete(id);
        cacheRepository.Clear(CACHE_KEY);
        return RedirectToAction(nameof(Index), new { msg = CrudStatus.Deleted });
    }
}
