using OceanLauncher.Launcher;

namespace OceanLauncher.Game.Tools.PathSearcher
{
    public interface IGamePathSearcher
    {
        LauncherInfo[] Search();
    }
}