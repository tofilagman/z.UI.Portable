using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace DevComponents.DotNetBar.Controls
{
    /// <summary>
    /// Class represents single token in TokenEditor control.
    /// </summary>
    [ToolboxItem(false)]
    public class EditToken : Component
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the EditToken class.
        /// </summary>
        /// <param name="value">Indicates token value.</param>
        public EditToken(string value)
        {
            _Value = value;
        }
        /// <summary>
        /// Initializes a new instance of the EditToken class.
        /// </summary>
        /// <param name="value">Indicates token value</param>
        /// <param name="text">Indicates token text</param>
        public EditToken(string value, string text)
        {
            _Value = value;
            _Text = text;
        }
        /// <summary>
        /// Initializes a new instance of the EditToken class.
        /// </summary>
        /// <param name="value">Indicates token value</param>
        /// <param name="text">Indicates token text</param>
        /// <param name="image">Indicates token image</param>
        public EditToken(string value, string text, Image image)
        {
            _Value = value;
            _Text = text;
            _Image = image;
        }
        #endregion

        #region Implementation
        private eTokenPart _MouseOverPart;
        private string _Value = "";
        /// <summary>
        /// Indicates the token value, for example an email token has email address as token Value and full name as token Text.
        /// </summary>
        [DefaultValue(""), Category("Data"), Description("Indicates the token value, for example an email token has email address as token Value and full name as token Text.")]
        public string Value
        {
            get { return _Value; }
            set
            {
                if (value != _Value)
                {
                    string oldValue = _Value;
                    _Value = value;
                    OnValueChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when Value property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnValueChanged(string oldValue, string newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("Value"));
        }

        private string _Text = "";
        /// <summary>
        /// Indicates the token text, for example an email token has email address as token Value and full name as token Text.
        /// </summary>
        [DefaultValue(""), Category("Data"), Description("Indicates the token text, for example an email token has email address as token Value and full name as token Text.")]
        public string Text
        {
            get { return _Text; }
            set
            {
                if (value != _Text)
                {
                    string oldValue = _Text;
                    _Text = value;
                    OnTextChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when Text property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnTextChanged(string oldValue, string newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("Text"));

        }

        private object _Tag = null;
        /// <summary>
        /// Gets or sets custom data associated with the object.
        /// </summary>
        [DefaultValue((string)null), Localizable(false), TypeConverter(typeof(StringConverter)), Category("Data"), Description("Custom data associated with the object")]
        public object Tag
        {
            get { return _Tag; }
            set { _Tag = value; }
        }

        private Rectangle _Bounds = Rectangle.Empty;
        /// <summary>
        /// Gets the display bounds of the token, if displayed, inside of TokenEditor control.
        /// </summary>
        [Browsable(false)]
        public Rectangle Bounds
        {
            get { return _Bounds; }
            internal set { _Bounds = value; }
        }

        private Image _Image = null;
        /// <summary>
        /// Indicates the image that is displayed next to the token
        /// </summary>
        [DefaultValue(null), Category("Appearance"), Description("Indicates the image that is displayed next to the token")]
        public Image Image
        {
            get { return _Image; }
            set
            {
                if (value != _Image)
                {
                    Image oldValue = _Image;
                    _Image = value;
                    OnImageChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when Image property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnImageChanged(Image oldValue, Image newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("Image"));
        }

        /// <summary>
        /// Gets the realized symbol string.
        /// </summary>
        [Browsable(false)]
        public string SymbolRealized
        {
            get { return _SymbolRealized; }
        }
        private string _Symbol = "", _SymbolRealized = "";
        /// <summary>
        /// Indicates the symbol displayed on face of the token instead of the image. Setting the symbol overrides the image setting.
        /// </summary>
        [DefaultValue(""), Category("Appearance"), Description("Indicates the symbol displayed on face of the token instead of the image. Setting the symbol overrides the image setting.")]
        [Editor("DevComponents.DotNetBar.Design.SymbolTypeEditor, DevComponents.DotNetBar.Design, Version=14.1.0.25, Culture=neutral,  PublicKeyToken=90f470f34c89ccaf", typeof(System.Drawing.Design.UITypeEditor))]
        public string Symbol
        {
            get { return _Symbol; }
            set
            {
                if (value != _Symbol)
                {
                    string oldValue = _Symbol;
                    _Symbol = value;
                    OnSymbolChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when Symbol property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnSymbolChanged(string oldValue, string newValue)
        {
            if (string.IsNullOrEmpty(newValue))
                _SymbolRealized = "";
            else
                _SymbolRealized = Symbols.GetSymbol(newValue);
            //OnPropertyChanged(new PropertyChangedEventArgs("Symbol"));
            this.Invalidate();
        }

        private eSymbolSet _SymbolSet = eSymbolSet.Awesome;
        /// <summary>
        /// Gets or sets the symbol set used to represent the Symbol.
        /// </summary>
        [Browsable(false), DefaultValue(eSymbolSet.Awesome)]
        public eSymbolSet SymbolSet
        {
            get { return _SymbolSet; }
            set
            {
                if (_SymbolSet != value)
                {
                    eSymbolSet oldValue = _SymbolSet;
                    _SymbolSet = value;
                    OnSymbolSetChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when SymbolSet property value changes.
        /// </summary>
        /// <param name="oldValue">Indciates old value</param>
        /// <param name="newValue">Indicates new value</param>
        protected virtual void OnSymbolSetChanged(eSymbolSet oldValue, eSymbolSet newValue)
        {
            this.Invalidate();
        }

        private void Invalidate()
        {
            //throw new NotImplementedException();
        }

        private Color _SymbolColor = Color.Empty;
        /// <summary>
        /// Gets or sets the color of the Symbol.
        /// </summary>
        [Category("Appearance"), Description("Indicates color of the Symbol.")]
        public Color SymbolColor
        {
            get { return _SymbolColor; }
            set { _SymbolColor = value; this.Invalidate(); }
        }
        /// <summary>
        /// Gets whether property should be serialized.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeSymbolColor()
        {
            return !_SymbolColor.IsEmpty;
        }
        /// <summary>
        /// Resets property to its default value.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetSymbolColor()
        {
            this.SymbolColor = Color.Empty;
        }

        private string _Tooltip = "";
        /// <summary>
        /// Indicates tooltip that is displayed when mouse is over the token and token is selected.
        /// </summary>
        [DefaultValue(""), Category("Behavior"), Description("Indicates tooltip that is displayed when mouse is over the token and token is selected")]
        public string Tooltip
        {
            get { return _Tooltip; }
            set
            {
                if (value !=_Tooltip)
                {
                    string oldValue = _Tooltip;
                    _Tooltip = value;
                    OnTooltipChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when Tooltip property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnTooltipChanged(string oldValue, string newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("Tooltip"));
            
        }

        /// <summary>
        /// Gets the part of the token mouse is over. Valid only when token is selected.
        /// </summary>
        [Browsable(false)]
        public eTokenPart MouseOverPart
        {
            get { return _MouseOverPart; }
            internal set
            {
                if (value !=_MouseOverPart)
                {
                    eTokenPart oldValue = _MouseOverPart;
                    _MouseOverPart = value;
                    OnMouseOverPartChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when MouseOverPart property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnMouseOverPartChanged(eTokenPart oldValue, eTokenPart newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("MouseOverPart"));
            
        }

        private Rectangle _RemoveButtonBounds = Rectangle.Empty;
        /// <summary>
        /// Gets the bounds of the remove button if displayed. Valid only when token is selected.
        /// </summary>
        [Browsable(false)]
        public Rectangle RemoveButtonBounds
        {
            get
            {
                return _RemoveButtonBounds;
            }
            internal set
            {
                _RemoveButtonBounds = value;
            }
        }
        private Rectangle _ImageBounds = Rectangle.Empty;
        /// <summary>
        /// Gets the bounds of the image if displayed. Valid only when token is selected.
        /// </summary>
        [Browsable(false)]
        public Rectangle ImageBounds
        {
            get
            {
                return _ImageBounds;
            }
            internal set
            {
                _ImageBounds = value;
            }
        }

        private bool _IsSelected = false;
        /// <summary>
        /// Indicates whether token is selected.
        /// </summary>
        [Browsable(false)]
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            internal set
            {
                _IsSelected = value;
            }
        }

        private bool _IsFocused = false;
        /// <summary>
        /// Indicates whether token is focused while selected.
        /// </summary>
        [Browsable(false)]
        public bool IsFocused
        {
            get
            {
                return _IsFocused;
            }
            internal set
            {
                _IsFocused = value;
            }
        }
        #endregion
    }

    /// <summary>
    /// Defines the token parts.
    /// </summary>
    public enum eTokenPart
    {
        /// <summary>
        /// Identifies no token part.
        /// </summary>
        None,
        /// <summary>
        /// Identifies the token body/text.
        /// </summary>
        Token,
        /// <summary>
        /// Identifies the remove token button.
        /// </summary>
        RemoveButton,
        /// <summary>
        /// Identifies the token image.
        /// </summary>
        Image
    }
}
