namespace Remax.WebJobs
{
    public interface INotificationManager
    {
        void SitesUpdated(SiteUpdatedDetail[] sitesSuccesUpdated,
            SiteUpdatedDetail[] sitesFailed
        );
    }
}
