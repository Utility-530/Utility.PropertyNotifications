using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;
using System.Collections;

namespace Leepfrog.WpfFramework
{
    public static class LeepfrogBindingOperations
    {
        public static void EnableCollectionSynchronization(IEnumerable collection, object mutex, Dispatcher dispatcher)
        {
            if ((dispatcher != null) && (!dispatcher.CheckAccess())) // if there is a dispatcher, but we're not currently on the right thread...
            {
                // we can't await...  we'll just fire and hope?!
                // invoke can cause a lock
                dispatcher.InvokeAsync(
                    () =>
                    {
                        BindingOperations.EnableCollectionSynchronization(collection, mutex);
                    }
                    );
            }
            else
            {
                BindingOperations.EnableCollectionSynchronization(collection, mutex);
            }
        }

        public static void EnableCollectionSynchronization(IEnumerable collection, AsyncLock mutex, Dispatcher dispatcher)
        {
            if ((dispatcher != null) && (!dispatcher.CheckAccess())) // if there is a dispatcher, but we're not currently on the right thread...
            {
                // we can't await...  we'll just fire and hope?!
                // invoke can cause a lock
                dispatcher.InvokeAsync(
                    () =>
                    {
                        BindingOperations.EnableCollectionSynchronization(collection, mutex, syncCallback);
                    }
                    );
            }
            else
            {
                BindingOperations.EnableCollectionSynchronization(collection, mutex, syncCallback);
            }
        }

        private static async void syncCallback(IEnumerable collection, object context, Action accessMethod, bool writeAccess)
        {
            //=================================================================
            using (await ((AsyncLock)context).LockAsync().ConfigureAwait(false)) // configureawait added ml 2019-01-13
            {
                accessMethod();
            }
            //=================================================================
        }

    }
}
