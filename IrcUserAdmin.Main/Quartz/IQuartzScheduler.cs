namespace IrcUserAdmin.Quartz
{
    public interface IQuartzScheduler
    {
        void ScheduleDailyJob();
        void StopScheduler();
        void FireTrigger(QuartzJobs job);
    }
}