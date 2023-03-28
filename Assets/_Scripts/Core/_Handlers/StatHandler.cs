using Photon.Realtime;

namespace Playstel
{
    public static class StatHandler 
    {
        public static int GetLocalCurrency(Player player)
        {
            return PhotonHandler.GetPlayerHash<int>(PhotonHandler.Hash.LocalCurrency, player);
        }
        
        public static int GetMainCurrency(Player player)
        {
            return PhotonHandler.GetPlayerHash<int>(PhotonHandler.Hash.MainCurrency, player);
        }
        
        public static int GetExperience(Player player)
        {
            return PhotonHandler.GetPlayerHash<int>(PhotonHandler.Hash.Experience, player);
        }
        
        public static int GetLevel(Player player)
        {
            return PhotonHandler.GetPlayerHash<int>(PhotonHandler.Hash.Level, player);
        }

        public static int GetFrags(Player player)
        {
            return PhotonHandler.GetPlayerHash<int>(PhotonHandler.Hash.Frags, player);
        }

        public static int GetDeaths(Player player)
        {
            return PhotonHandler.GetPlayerHash<int>(PhotonHandler.Hash.Deaths, player);
        }

        public static int GetTurnover(Player player)
        {
            return PhotonHandler.GetPlayerHash<int>(PhotonHandler.Hash.Turnover, player);
        }

        public static int GetRoundScore(Player player)
        {
            var frags = PhotonHandler.GetPlayerHash<int>(PhotonHandler.Hash.Frags, player);
            var turnover = PhotonHandler.GetPlayerHash<int>(PhotonHandler.Hash.Turnover, player);
            
            return frags * 800 + turnover;
        }
    }
}