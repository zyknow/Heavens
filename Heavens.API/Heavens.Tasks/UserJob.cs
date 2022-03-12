using Furion.DatabaseAccessor;
using Furion.TaskScheduler;

namespace Heavens.Tasks;

public class UserJob : ISpareTimeWorker
{
    public UserJob(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public IRepository<User> _userRepository { get; }

    /// <summary>
    /// 更新年龄
    /// </summary>
    /// <param name="timer"></param>
    /// <param name="count"></param>
    [SpareTime("Update User Age", DoOnce = false, StartNow = false, CronFormat = "* * 4 * * ? *")]
    public void DoSomething(SpareTimer timer, long count)
    {
        //// 更新年龄
        //List<User> users = await _userRepository.Where(u => u.Birth != null && (int)((DateTime.Now - (DateTime)u.Birth).TotalDays / 365) != u.Age).AsTracking().ToListAsync();
        //users.ForEach(u => u.Age = (int)((DateTime.Now - u.Birth)?.TotalDays / 365));
        //await _userRepository.SaveNowAsync();
    }
}
