using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace DevComponents.DotNetBar
{
    public class MultiFormAppContext : ApplicationContext
    {
        private List<WeakReference> _OpenedForms = new List<WeakReference>();

        public MultiFormAppContext(Form initialForm)
        {
            // Handle the ApplicationExit event to know when the application is exiting.
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);
            WeakReference reference=new WeakReference(initialForm);
            _OpenedForms.Add(reference);
            initialForm.FormClosed += FormClosed;
            initialForm.Show();
        }

        public void RegisterOpenForm(Form form)
        {
            bool opened = false;
            foreach (WeakReference item in _OpenedForms)
            {
                object target = item.Target;

                if (target != null && target.Equals(form))
                {
                    opened = true;
                    break;
                }
            }

            if (!opened)
            {
                WeakReference reference = new WeakReference(form);
                _OpenedForms.Add(reference);
                form.FormClosed += FormClosed;
            }
        }
        /// <summary>
        /// Returns read-only list of opened forms. 
        /// </summary>
        public List<Form> OpenedForms
        {
            get
            {
                List<Form> list=new List<Form>();
                foreach (WeakReference item in _OpenedForms)
                {
                    object target = item.Target;
                    if (target != null && target is Form)
                    {
                        list.Add((Form)target);
                    }
                }

                return list;
            }
        }

        private void FormClosed(object sender, FormClosedEventArgs e)
        {
            Form form = (Form) sender;
            form.FormClosed -= FormClosed;
            foreach (WeakReference item in _OpenedForms)
            {
                object target = item.Target;

                if (target != null && target.Equals(form))
                {
                    _OpenedForms.Remove(item);
                    break;
                }
            }

            if(_OpenedForms.Count==0)
                ExitThread();
        }

        protected  virtual void OnApplicationExit(object sender, EventArgs e)
        {
            
        }

        private static MultiFormAppContext _CurrentMultiFormAppContext=null;
        /// <summary>
        /// Gets or sets current MultiFormAppContext context. If you are using MultiFormAppContext to start your app then you should also
        /// set that instance of MultiFormAppContext to this property so internally TabFormControl can register any new deattached forms 
        /// and prevent application from closing if startup form is closed first.
        /// </summary>
        public static MultiFormAppContext Current
        {
            get { return _CurrentMultiFormAppContext; }
            set { _CurrentMultiFormAppContext = value; }
        }
    }
}
