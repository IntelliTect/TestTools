using System;
using System.Collections.Generic;
using System.Linq;

namespace IntelliTect.TestTools.TestFramework
{
    // Temp name
    public class TestObjectsBag
    {
        // Should this be a TryAdd?
        // Currently we do NOT support collections
        public void AddItemToBag(params object[] item)
        {
            foreach (var i in item)
            {
                var existingType = DataBag.SingleOrDefault(f => f.GetType() == item.GetType());
                if (existingType != null)
                {
                    DataBag.Remove(existingType);
                }
                DataBag.Add(i);
            }
        }

        public void AddItemToBag<T>()
        {
            AddItemToBag(Activator.CreateInstance(typeof(T)));
        }

        public bool TryGetItemFromBag(object objectToFind, out object data)
        {
            return TryGetItemFromBag(objectToFind.GetType(), out data);
        }

        public bool TryGetItemFromBag(Type typeToFind, out object data)
        {
            data = DataBag.SingleOrDefault(d => d.GetType() == typeToFind);
            if (data == null)
            {
                foreach (var d in DataBag)
                {
                    Type[] interfaces = d.GetType().GetInterfaces();
                    if (interfaces.Length > 0 && interfaces.Contains(typeToFind))
                    {
                        data = d;
                        break;
                    }
                }

                // Probably shouldn't do below as it breaks with a lot of inheritence
                //data = Data.SingleOrDefault(d => d.GetType().BaseType == typeToFind);
                //if (data == null)
                //{
                //    // This will produce unexpected results if we load up two different browser types. It will grab whatever is first.
                //    foreach (var d in Data)
                //    {
                //        Type[] interfaces = d.GetType().GetInterfaces();
                //        if (interfaces.Length > 0 && interfaces.Contains(typeToFind))
                //        {
                //            data = d;
                //            break;
                //        }
                //    }
                //}
            }

            return data != null ? true : false;
        }

        public bool TryRemoveItemFromBag(object objectToRemove)
        {
            return TryRemoveItemFromBag(objectToRemove.GetType());
        }

        public bool TryRemoveItemFromBag(Type typeToFind)
        {
            if(TryGetItemFromBag(typeToFind, out var data))
            {
                DataBag.Remove(data);
            }

            return !TryGetItemFromBag(typeToFind, out _);
            // Throw exception here instead?
        }

        // Worth converting this to a HashSet? Or maybe a Dictionary with the type as a key (or a specifiable key?)
        private List<object> DataBag { get; set; } = new List<object>();
    }
}
