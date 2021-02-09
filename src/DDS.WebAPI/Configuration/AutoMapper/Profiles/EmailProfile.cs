using AutoMapper;
using DDS.Domain.Core.Models.ValueObjects;
using DDS.WebAPI.Models.ViewModels;

namespace DDS.WebAPI.Configuration.AutoMapper.Profiles
{
    public class EmailProfile : Profile
    {
        public EmailProfile()
        {
            CreateMap<Email, string>()
                   .ConvertUsing(c => c.Endereco);

            CreateMap<string, Email>()
                    .ConstructUsing(email => new Email(email));

            CreateMap<Email, EmailViewModel>();

            CreateMap<string, EmailViewModel>()
                .ConstructUsing(emailString => new EmailViewModel
                {
                    Endereco = emailString
                });

            CreateMap<EmailViewModel, Email>()
                .ConstructUsing(vm => new Email(vm.Endereco));
        }
    }
}
