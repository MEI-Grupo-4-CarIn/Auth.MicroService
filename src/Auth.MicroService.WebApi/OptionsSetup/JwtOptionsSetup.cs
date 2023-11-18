using Auth.MicroService.Application.JwtUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Auth.MicroService.WebApi.OptionsSetup
{
    public class JwtOptionsSetup : IConfigureOptions<JwtOptions>
    {
        private const string SectionName = "JwtSettings";

        private readonly IConfiguration _configuration;

        public JwtOptionsSetup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(JwtOptions jwtOptions)
        {
            _configuration.GetSection(SectionName).Bind(jwtOptions);
        }
    }
}
