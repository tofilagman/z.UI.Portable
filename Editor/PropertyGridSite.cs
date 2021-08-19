using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.UI.Editor
{
    public class PropertyGridSite : ISite
    {
        private readonly IServiceProvider _sp;
        private bool _inGetService;

        public PropertyGridSite(IServiceProvider sp, IComponent comp)
        {
            _sp = sp;
            Component = comp;
        }

        public IComponent Component { get; }

        public IContainer Container => null;

        public bool DesignMode => false;

        public string Name
        {
            get => null;
            set { }
        }

        public object GetService(Type t)
        {
            if (!_inGetService && _sp != null)
            {
                try
                {
                    _inGetService = true;
                    return _sp.GetService(t);
                }
                finally
                {
                    _inGetService = false;
                }
            }

            return null;
        }
    }
}
