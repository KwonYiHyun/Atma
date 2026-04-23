namespace ServerCore.Service
{
    public interface ITimeService
    {
        Task<DateTime> getNowAsync();
    }
}
