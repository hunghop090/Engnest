using AutoMapper;
using Engnest.Entities.Entity;
using Engnest.Entities.ViewModels;
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
            });
        }
    }
}
