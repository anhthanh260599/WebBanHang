using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Models.ServicePattern
{
    public interface ISocialMediaService
    {
        List<SocialMediaProfiles> GetAllProfile();
        void AddProfile(SocialMediaProfiles profile);
        SocialMediaProfiles GetProfileById(int id);
        void UpdateProfile(SocialMediaProfiles profile);
        void DeleteProfile(int id);

        //Hàm logic không truy cập CSDL
        bool IsProfileNameValid(string socialMediaName);
    }
}
