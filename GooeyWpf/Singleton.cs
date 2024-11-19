namespace GooeyWpf
{
    internal class Singleton<T> where T : class
    {
        private static T? instance;

        public static T Instance
        {
            get
            {
                return instance ??= Activator.CreateInstance<T>();
            }
        }
    }
}