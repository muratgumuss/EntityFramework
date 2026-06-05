using AutoMapper;
using EFCore.CodeFirst.DAL;
using EFCore.CodeFirst.Dtos;
using Microsoft.Extensions.Logging.Abstractions;

namespace EFCore.CodeFirst.Mappers
{
    public class ObjectMapper
    {
        private static readonly Lazy<IMapper> layz = new Lazy<IMapper>(() => {
            var config = new MapperConfiguration(cfg => { cfg.AddProfile<CustomMapping>(); }, NullLoggerFactory.Instance);
            return config.CreateMapper();
        });

        public static IMapper Mapper => layz.Value;
    }

    internal class CustomMapping : Profile
    {

        public CustomMapping()
        {
            CreateMap<ProductDto, Product>().ReverseMap();
        }
    }


}
