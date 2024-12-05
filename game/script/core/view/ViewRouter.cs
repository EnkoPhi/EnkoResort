namespace core.view
{
    public class ViewRouter
    {
        public RouterBase entry;
        public bool MapRoute<T>(string path) where T : ViewBase
        {
            return false;
        }
    }
}