using System;
using System.Collections.Generic;
using System.Linq;

namespace GlobalSuite.Core.Helpers
{
    public static class EnumHelper
    {
        

        public static IEnumerable<EnumObject> ToEnumArray<T>()
        {
            if (typeof(T).BaseType != typeof(Enum))
                return null;
            var names = Enum.GetNames(typeof(T));
            var values = Enum.GetValues(typeof(T)).Cast<int>().ToList();

            var valuesToList = names.Select((n, index) =>
                new EnumObject { Id = values[index], Name = n});

            return valuesToList;
        }
    }
}