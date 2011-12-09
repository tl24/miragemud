
namespace Mirage.Game.World.Query
{

    public interface IUriContainer
    {
        object GetChild(string uri);
        QueryHints GetChildHints(string uri);
    }    
}
