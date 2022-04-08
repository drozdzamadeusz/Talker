using AutoMapper;
using talker.Application.Common.Mappings;

namespace talker.Application.Users.Queries
{
    public class ApplicationUserDto
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PictureUrl { get; set; }


    }
}
