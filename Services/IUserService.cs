using DemoApi.Models;
using System.Threading.Tasks;

namespace DemoApi.Services
{
    public interface IUserService
	{
		Task<PagedResults<User>> GetUsersAsync(
			PagingOptions pagingOptions,
			SortOptions<User, UserEntity> sortOptions,
			SearchOptions<User, UserEntity> searchOptions);

		Task<(bool Succeeded, string ErrorMessage)> CreateUserAsync(RegisterForm registerForm);
	
	}

}
