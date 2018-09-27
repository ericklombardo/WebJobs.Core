namespace Remax.WebJobs
{
    public interface INotificationManager
    {
        void SitesUpdated(SiteUpdatedDetail[] detail);
    }
}
