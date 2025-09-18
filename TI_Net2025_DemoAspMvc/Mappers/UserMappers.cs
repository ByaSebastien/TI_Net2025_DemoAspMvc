using TI_Net2025_DemoAspMvc.Models.Dtos.User;
using TI_Net2025_DemoAspMvc.Models.Entities;

namespace TI_Net2025_DemoAspMvc.Mappers
{
    public static class UserMappers
    {
        public static RegisterFormDto ToRegisterFormDto(this User u)
        {
            return new RegisterFormDto()
            {
                Email = u.Email,
                Username = u.Username,
            };
        }

        public static User ToUser(this RegisterFormDto u)
        {
            return new User()
            {
                Email = u.Email,
                Username = u.Username,
                Password = u.Password,
            };
        }
    }
}
