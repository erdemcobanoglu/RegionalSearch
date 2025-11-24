using RegionalSearch.Application.Common.Interfaces;

namespace Presentation.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId
        {
            get
            {
                // Mock / gerçek kullanıcı id’sini buradan çekersin
                // Şimdilik fake:
                return 1;
            }
        }
    }
}
