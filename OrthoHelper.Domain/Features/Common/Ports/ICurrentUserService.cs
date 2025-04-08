namespace OrthoHelper.Domain.Features.Common.Ports
{
    public interface ICurrentUserService
    {
        int UserId { get; }

        public string UserName { get; }
        bool IsAuthenticated { get; }

        //public string UseModelName { get; }
    }
}
