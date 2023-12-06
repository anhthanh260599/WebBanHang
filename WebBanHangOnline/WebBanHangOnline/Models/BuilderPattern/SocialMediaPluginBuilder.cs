using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Models.BuilderPattern
{
    public class SocialMediaPluginBuilder
    {
        private SocialMediaPlugin _socialMediaPlugin = new SocialMediaPlugin();

        public SocialMediaPluginBuilder WithName(string name) 
        {
            _socialMediaPlugin.Name = name;
            return this;
        }

        public SocialMediaPluginBuilder WithPlugin(string plugin)
        {
            _socialMediaPlugin.Plugin = plugin;
            return this;
        }

        public SocialMediaPluginBuilder WithImage (string image)
        {
            _socialMediaPlugin.Image = image;
            return this;
        }

        public SocialMediaPlugin Build()
        {
            return _socialMediaPlugin;
        }
    }
}