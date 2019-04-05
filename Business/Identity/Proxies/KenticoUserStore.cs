namespace Business.Identity.Proxies
{
    /// <summary>
    /// Wrapper of the <see cref="Kentico.Membership.UserStore"/> object.
    /// </summary>
    public class KenticoUserStore: Kentico.Membership.UserStore, IKenticoUserStore
    {
        public KenticoUserStore(string siteName) : base(siteName)
        {
        }
    }
}
