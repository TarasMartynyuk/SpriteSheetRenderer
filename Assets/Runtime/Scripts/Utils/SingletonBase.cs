using UnityEngine;

namespace TMUtils.Singletons
{
    public class SingletonBase<T> where T 
        : class, new()
    {
        public static T Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new T();
                }
                return m_instance;

            }
            private set => m_instance = value;
        }

        private static T m_instance;
    }

    public class SingletonInterfaceBase<TInterface, TConcrete> 
        where TInterface : class
        where TConcrete : TInterface, new()
    {
        public static TInterface Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new TConcrete();
                }
                return m_instance;

            }
            private set => m_instance = value;
        }

        private static TInterface m_instance;
    }
}