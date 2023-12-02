using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Models.ServicePattern
{
    public class SocialMediaService : ISocialMediaService
    {
        private readonly ApplicationDbContext db;

        public SocialMediaService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public List<SocialMediaProfiles> GetAllProfile()
        {
            return db.SocialMediaProfiles.ToList();
        }

        public void AddProfile(SocialMediaProfiles profile)
        {
            db.SocialMediaProfiles.Add(profile);
            db.SaveChanges();
        }

        public SocialMediaProfiles GetProfileById(int id)
        {
            return db.SocialMediaProfiles.Find(id);
        }

        public void UpdateProfile(SocialMediaProfiles profile)
        {
            db.Entry(profile).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        public void DeleteProfile(int id)
        {
            var profile = db.SocialMediaProfiles.Find(id);
            if (profile != null)
            {
                db.SocialMediaProfiles.Remove(profile);
                db.SaveChanges();
            }
        }
    }
}