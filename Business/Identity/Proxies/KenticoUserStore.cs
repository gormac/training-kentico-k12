namespace Business.Identity.Proxies
{
    public class KenticoUserStore: Kentico.Membership.UserStore, IKenticoUserStore
    {
        public KenticoUserStore(string siteName) : base(siteName)
        {
        }
    }
}
