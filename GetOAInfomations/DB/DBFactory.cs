using System;
using System.Configuration;
using NNWebFlow.DBContexts;

namespace GetOAInfomations.DB
{
    public class DBFactory<T> where T : IUnitfoDatabase{

        public static T GetInstance(bool bUserTransaction = false)
        {
            var instance = Activator.CreateInstance<T>();
            return instance;
        }
    }
}
