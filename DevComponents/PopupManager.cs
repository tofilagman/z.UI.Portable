using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DevComponents.DotNetBar
{
    /// <summary>
    /// Class that tracks lists of all controls that host currently open popups.
    /// </summary>
    public static class PopupManager
    {
        #region Constructor
        private static List<WeakReference> _RegisteredPopups = new List<WeakReference>();
        private static ReaderWriterLock ReadWritePopupsListLock;

        static PopupManager()
        {
            PopupManager.ReadWritePopupsListLock = new ReaderWriterLock();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Registers IOwnerMenuSupport popup host in response to host displaying its first popup.
        /// </summary>
        /// <param name="popupHost">IOwnerMenuSupport host to register</param>
        public static void RegisterPopup(IOwnerMenuSupport popupHost)
        {
            if (popupHost == null) return;
            if (GetHostReference(popupHost) == null)
            {
                WeakReference reference = new WeakReference(popupHost);
                reference.Target = popupHost;

                LockCookie cookie1 = new LockCookie();
                bool readerLockHeld = PopupManager.ReadWritePopupsListLock.IsReaderLockHeld;
                if (readerLockHeld)
                {
                    cookie1 = PopupManager.ReadWritePopupsListLock.UpgradeToWriterLock(-1);
                }
                else
                {
                    PopupManager.ReadWritePopupsListLock.AcquireWriterLock(-1);
                }
                try
                {
                    _RegisteredPopups.Add(reference);
                }
                finally
                {
                    if (readerLockHeld)
                    {
                        PopupManager.ReadWritePopupsListLock.DowngradeFromWriterLock(ref cookie1);
                    }
                    else
                    {
                        PopupManager.ReadWritePopupsListLock.ReleaseWriterLock();
                    }
                }

                popupHost = null;
            }
        }

        private static WeakReference GetHostReference(IOwnerMenuSupport host)
        {
            List<WeakReference> registeredControls = _RegisteredPopups;
            if (registeredControls == null) return null;

            PopupManager.ReadWritePopupsListLock.AcquireReaderLock(-1);
            try
            {
                foreach (WeakReference item in registeredControls)
                {
                    object target = item.Target;
                    if (target != null && target.Equals(host))
                        return item;
                }
            }
            finally
            {
                PopupManager.ReadWritePopupsListLock.ReleaseReaderLock();
            }
            return null;
        }
        /// <summary>
        /// Unregisters IOwnerMenuSupport popup host in response to host closing its last popup.
        /// </summary>
        /// <param name="popupHost">IOwnerMenuSupport host to unregister</param>
        public static void UnregisterPopup(IOwnerMenuSupport popupHost)
        {
            if (popupHost == null) return;
            LockCookie cookie1 = new LockCookie();
            bool readerLockHeld = PopupManager.ReadWritePopupsListLock.IsReaderLockHeld;
            if (readerLockHeld)
            {
                cookie1 = PopupManager.ReadWritePopupsListLock.UpgradeToWriterLock(-1);
            }
            else
            {
                PopupManager.ReadWritePopupsListLock.AcquireWriterLock(-1);
            }
            try
            {

                foreach (WeakReference item in _RegisteredPopups)
                {
                    object target = item.Target;
                    if (target != null && target.Equals(popupHost))
                    {
                        _RegisteredPopups.Remove(item);
                        break;
                    }
                }
            }
            finally
            {
                if (readerLockHeld)
                {
                    PopupManager.ReadWritePopupsListLock.DowngradeFromWriterLock(ref cookie1);
                }
                else
                {
                    PopupManager.ReadWritePopupsListLock.ReleaseWriterLock();
                }
            }
        }
        /// <summary>
        /// Closes all currently open popups.
        /// </summary>
        public static void CloseAllPopups()
        {
            CloseAllPopups(null);
        }
        /// <summary>
        /// Closes all currently open popups excluding specified popup host.
        /// </summary>
        /// <param name="excludeHost">IOwnerMenuSupport host to exclude from closing or null</param>
        public static void CloseAllPopups(IOwnerMenuSupport excludeHost)
        {
            WeakReference[] references;
            PopupManager.ReadWritePopupsListLock.AcquireReaderLock(-1);
            try
            {
                references = new WeakReference[_RegisteredPopups.Count];
                _RegisteredPopups.CopyTo(references);
            }
            finally
            {
                PopupManager.ReadWritePopupsListLock.ReleaseReaderLock();
            }

            foreach (WeakReference reference in references)
            {
                object target = reference.Target;
                if (reference.IsAlive)
                {
                    if (target != null)
                    {
                        IOwnerMenuSupport popupHost = target as IOwnerMenuSupport;
                        if (popupHost != null && popupHost != excludeHost)
                        {
                            popupHost.ClosePopups();
                        }
                    }
                }
            }
        }
        #endregion
    }
}
