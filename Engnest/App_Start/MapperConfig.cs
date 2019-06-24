using AutoMapper;
using Engnest.Entities.Entity;
using Engnest.Entities.ViewModels;
using System.Collections.Generic;
using System.Web;
using System.Web.Optimization;

namespace Engnest
{
    public class MapperConfig
    {
        public static void RegisterMappers()
        {
            Mapper.Initialize(cfg => {
                cfg.CreateMap<User, ProfileModel>();
				cfg.CreateMap<Group, GroupModel>();
                cfg.CreateMap<Post, PostModel>();
                cfg.CreateMap<PostModel, Post>();
				cfg.CreateMap<List<CommentViewModel>, List<Comment>>();
            });
        }
    }
}
