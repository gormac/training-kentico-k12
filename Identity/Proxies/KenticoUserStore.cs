namespace Identity.Proxies
{
    class KenticoUserStore: Kentico.Membership.UserStore, IKenticoUserStore
    {
        public KenticoUserStore(string siteName) : base(siteName)
        {
        }
    }
}
