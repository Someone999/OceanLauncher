namespace OceanLauncher.Config.Converters
{

    public interface IConfigConverter<T>
    {
        new T Convert(object obj);
    }
}