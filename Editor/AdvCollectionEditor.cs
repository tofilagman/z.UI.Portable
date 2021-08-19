using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.VisualStyles;

namespace z.UI.Editor
{
    /// <summary>
    ///  Provides a generic editor for most any collection.
    /// </summary>
    public class AdvCollectionEditor : UITypeEditor
    {
        private Type _collectionItemType;
        private Type[] _newItemTypes;
        private ITypeDescriptorContext _currentContext;

        private bool _ignoreChangedEvents;
        private bool _ignoreChangingEvents;

        /// <summary>
        ///  Initializes a new instance of the <see cref='System.ComponentModel.Design.AdvCollectionEditor'/> class using the specified collection type.
        /// </summary>
        public AdvCollectionEditor(Type type)
        {
            CollectionType = type;
        }

        /// <summary>
        ///  Gets or sets the data type of each item in the collection.
        /// </summary>
        public Type CollectionItemType
        {
            get => _collectionItemType ?? (_collectionItemType = CreateCollectionItemType());
        }

        /// <summary>
        ///  Gets or sets the type of the collection.
        /// </summary>
        public Type CollectionType { get; }

        /// <summary>
        ///  Gets or sets a type descriptor that indicates the current context.
        /// </summary>
        public ITypeDescriptorContext Context => _currentContext;

        /// <summary>
        ///  Gets or sets the available item types that can be created for this collection.
        /// </summary>
        public Type[] NewItemTypes => _newItemTypes ?? (_newItemTypes = CreateNewItemTypes());

        /// <summary>
        ///  Gets the help topic to display for the dialog help button or pressing F1. Override to display a different help topic.
        /// </summary>
        public virtual string HelpTopic => "net.ComponentModel.AdvCollectionEditor";

        /// <summary>
        ///  Gets or sets a value indicating whether original members of the collection can be removed.
        /// </summary>
        public virtual bool CanRemoveInstance(object value)
        {
            if (value is IComponent comp)
            {
                // Make sure the component is not being inherited -- we can't delete these!
                InheritanceAttribute ia = (InheritanceAttribute)TypeDescriptor.GetAttributes(comp)[typeof(InheritanceAttribute)];
                if (ia != null && ia.InheritanceLevel != InheritanceLevel.NotInherited)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///  Useful for derived classes to do processing when cancelling changes
        /// </summary>
        public virtual void CancelChanges()
        {
        }

        /// <summary>
        ///  Gets or sets a value indicating whether multiple collection members can be selected.
        /// </summary>
        public virtual bool CanSelectMultipleInstances() => true;

        /// <summary>
        ///  Creates a new form to show the current collection.
        /// </summary>
        protected virtual AdvCollectionForm CreateCollectionForm() => new AdvCollectionEditorCollectionForm(this);

        /// <summary>
        ///  Creates a new instance of the specified collection item type.
        /// </summary>
        public virtual object CreateInstance(Type itemType)
        {
            IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
            if (typeof(IComponent).IsAssignableFrom(itemType) && host != null)
            {
                IComponent instance = host.CreateComponent(itemType, null);

                // Set component defaults
                if (host.GetDesigner(instance) is IComponentInitializer init)
                {
                    init.InitializeNewComponent(null);
                }

                if (instance != null)
                {
                    return instance;
                }
            }

            return itemType?.UnderlyingSystemType == typeof(string)
                ? string.Empty
                : TypeDescriptor.CreateInstance(host, itemType, null, null);
        }

        /// <summary>
        ///  This Function gets the object from the givem object. The input is an arrayList returned as an Object.
        ///  The output is a arraylist which contains the individual objects that need to be created.
        /// </summary>
        public virtual IList GetObjectsFromInstance(object instance) => new ArrayList { instance };

        /// <summary>
        ///  Retrieves the display text for the given list item.
        /// </summary>
        public virtual string GetDisplayText(object value)
        {
            string text;

            if (value is null)
            {
                return string.Empty;
            }

            PropertyDescriptor prop = TypeDescriptor.GetProperties(value)["Name"];
            if (prop != null && prop.PropertyType == typeof(string))
            {
                text = (string)prop.GetValue(value);
                if (!string.IsNullOrEmpty(text))
                {
                    return text;
                }
            }

            prop = TypeDescriptor.GetDefaultProperty(CollectionType);
            if (prop != null && prop.PropertyType == typeof(string))
            {
                text = (string)prop.GetValue(value);
                if (!string.IsNullOrEmpty(text))
                {
                    return text;
                }
            }

            text = TypeDescriptor.GetConverter(value).ConvertToString(value);
            if (string.IsNullOrEmpty(text))
            {
                text = value.GetType().Name;
            }

            return text;
        }

        /// <summary>
        ///  Gets an instance of the data type this collection contains.
        /// </summary>
        protected virtual Type CreateCollectionItemType()
        {
            PropertyInfo[] props = TypeDescriptor.GetReflectionType(CollectionType).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            for (int i = 0; i < props.Length; i++)
            {
                if (props[i].Name.Equals("Item") || props[i].Name.Equals("Items"))
                {
                    return props[i].PropertyType;
                }
            }

            return typeof(object);
        }

        /// <summary>
        ///  Gets the data types this collection editor can create.
        /// </summary>
        protected virtual Type[] CreateNewItemTypes() => new Type[] { CollectionItemType };

        /// <summary>
        ///  Destroys the specified instance of the object.
        /// </summary>
        public virtual void DestroyInstance(object instance)
        {
            if (instance is IComponent compInstance)
            {
                if (GetService(typeof(IDesignerHost)) is IDesignerHost host)
                {
                    host.DestroyComponent(compInstance);
                }
                else
                {
                    compInstance.Dispose();
                }
            }
            else if (instance is IDisposable dispInstance)
            {
                dispInstance.Dispose();
            }
        }

        /// <summary>
        ///  Edits the specified object value using the editor style  provided by <see cref='System.ComponentModel.Design.AdvCollectionEditor.GetEditStyle'/>.
        /// </summary>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider?.GetService(typeof(IWindowsFormsEditorService)) is IWindowsFormsEditorService edSvc)
            {
                _currentContext = context;

                // child modal dialog -launching in System Aware mode
                AdvCollectionForm localCollectionForm = CreateCollectionForm(); //DpiHelper.CreateInstanceInSystemAwareContext(() => CreateCollectionForm());
                ITypeDescriptorContext lastContext = _currentContext;
                localCollectionForm.EditValue = value;
                _ignoreChangingEvents = false;
                _ignoreChangedEvents = false;
                DesignerTransaction trans = null;

                bool commitChange = true;
                IComponentChangeService cs = null;
                IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;

                try
                {
                    try
                    {
                        trans = host?.CreateTransaction(string.Format("Add or remove {0} objects", CollectionItemType.Name));
                    }
                    catch (CheckoutException cxe) when (cxe == CheckoutException.Canceled)
                    {
                        return value;
                    }

                    cs = host?.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                    if (cs != null)
                    {
                        cs.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
                        cs.ComponentChanging += new ComponentChangingEventHandler(OnComponentChanging);
                    }

                    if (localCollectionForm.ShowEditorDialog(edSvc) == DialogResult.OK)
                    {
                        value = localCollectionForm.EditValue;
                    }
                    else
                    {
                        commitChange = false;
                    }
                }
                finally
                {
                    localCollectionForm.EditValue = null;
                    _currentContext = lastContext;
                    if (trans != null)
                    {
                        if (commitChange)
                        {
                            trans.Commit();
                        }
                        else
                        {
                            trans.Cancel();
                        }
                    }

                    if (cs != null)
                    {
                        cs.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
                        cs.ComponentChanging -= new ComponentChangingEventHandler(OnComponentChanging);
                    }

                    localCollectionForm.Dispose();
                }
            }

            return value;
        }

        /// <summary>
        ///  Gets the editing style of the Edit method.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public bool IsAnyObjectInheritedReadOnly(object[] items)
        {
            // If the object implements IComponent, and is not sited, check with the inheritance service (if it exists) to see if this is a component
            // that is being inherited from another class.
            // If it is, then we do not want to place it in the collection editor. If the inheritance service
            // chose not to site the component, that indicates it should be hidden from  the user.
            IInheritanceService inheritanceService = null;
            bool isInheritanceServiceInitialized = false;

            foreach (object o in items)
            {
                if (o is IComponent comp && comp.Site is null)
                {
                    if (!isInheritanceServiceInitialized)
                    {
                        isInheritanceServiceInitialized = true;
                        if (Context != null)
                        {
                            inheritanceService = Context.GetService(typeof(IInheritanceService)) as IInheritanceService;
                        }
                    }

                    if (inheritanceService != null && inheritanceService.GetInheritanceAttribute(comp).Equals(InheritanceAttribute.InheritedReadOnly))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///  Converts the specified collection into an array of objects.
        /// </summary>
        public virtual object[] GetItems(object editValue)
        {
            // We look to see if the value implements ICollection, and if it does, we set through that.
            if (editValue is ICollection col)
            {
                var list = new ArrayList();
                foreach (object o in col)
                {
                    list.Add(o);
                }

                object[] values = new object[list.Count];
                list.CopyTo(values, 0);
                return values;
            }

            return Array.Empty<object>();
        }

        /// <summary>
        ///  Gets the requested service, if it is available.
        /// </summary>
        public object GetService(Type serviceType) => Context?.GetService(serviceType);

        /// <summary>
        ///  Reflect any change events to the instance object
        /// </summary>
        private void OnComponentChanged(object sender, ComponentChangedEventArgs e)
        {
            if (!_ignoreChangedEvents && sender != Context.Instance)
            {
                _ignoreChangedEvents = true;
                Context.OnComponentChanged();
            }
        }

        /// <summary>
        ///  Reflect any changed events to the instance object
        /// </summary>
        private void OnComponentChanging(object sender, ComponentChangingEventArgs e)
        {
            if (!_ignoreChangingEvents && sender != Context.Instance)
            {
                _ignoreChangingEvents = true;
                Context.OnComponentChanging();
            }
        }

        /// <summary>
        ///  Removes the item from the column header from the listview column header collection
        /// </summary>
        internal virtual void OnItemRemoving(object item)
        {
        }

        /// <summary>
        ///  Sets the specified collection to have the specified array of items.
        /// </summary>
        public virtual object SetItems(object editValue, object[] value)
        {
            // We look to see if the value implements IList, and if it does, we set through that.
            if (editValue is IList list)
            {
                list.Clear();
                if (value != null)
                {
                    for (int i = 0; i < value.Length; i++)
                    {
                        list.Add(value[i]);
                    }
                }
            }

            return editValue;
        }

        /// <summary>
        ///  Called when the help button is clicked.
        /// </summary>
        public virtual void ShowHelp()
        {
            if (GetService(typeof(IHelpService)) is IHelpService helpService)
            {
                helpService.ShowHelpFromKeyword(HelpTopic);
            }
        } 
    }
}
