 
using System;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Data;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Animation;
using DevComponents.DotNetBar.Rendering;

namespace DevComponents.Editors.DateTimeAdv
{
    /// <summary>
    /// Represents the container that presents single calendar month.
    /// </summary>
    [ToolboxItem(false), DesignTimeVisible(false), Designer("DevComponents.DotNetBar.Design.ItemContainerDesigner, DevComponents.DotNetBar.Design, Version=14.1.0.25, Culture=neutral,  PublicKeyToken=90f470f34c89ccaf")]
    public class SingleMonthCalendar : CalendarBase
    {
        private const string NavigationContainerName = "navigationContainer";
        #region Private Variables
        internal static readonly Size _DefaultDaySize = new Size(24, 15);
        internal static readonly Size _DefaultNavigationButtonSize = new Size(13, 18);
        private const string NavMonthLabel = "monthLabel";
        private const string NavYearLabel = "yearLabel";
        private const string NavDecreaseMonth = "decreaseMonth";
        private const string NavIncreaseMonth = "increaseMonth";
        private const string NavDecreaseYear = "decreaseYear";
        private const string NavIncreaseYear = "increaseYear";
        //private const string MonthsPopupMenu = "monthsPopupMenu";
        //private ButtonItem _MonthsPopupMenu = null;
        private YearSelectorControl _YearSelector = null;
        private MonthSelectorControl _MonthSelector = null;
        private YearSelectorContainer _YearSelectionItem = null;
        private MonthSelectorContainer _MonthSelectionItem = null;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when month displayed by the item has changed.
        /// </summary>
        public event EventHandler MonthChanged;

        /// <summary>
        /// Occurs before the month that is displayed is changed.
        /// </summary>
        public event EventHandler MonthChanging;


        /// <summary>
        /// Occurs when child label representing days is rendered and it allows you to override default rendering.
        /// </summary>
        public event DayPaintEventHandler PaintLabel;

        /// <summary>
        /// Occurs when mouse button is pressed over the day/week label inside of the calendar.
        /// </summary>
        [System.ComponentModel.Description("Occurs when mouse button is pressed over the day/week label inside of the calendar.")]
        public event System.Windows.Forms.MouseEventHandler LabelMouseDown;

        /// <summary>
        /// Occurs when mouse button is released over day/week label inside of the calendar.
        /// </summary>
        [System.ComponentModel.Description("Occurs when mouse button is released over day/week label inside of the calendar.")]
        public event System.Windows.Forms.MouseEventHandler LabelMouseUp;

        /// <summary>
        /// Occurs when mouse enters the day/week label inside of the calendar.
        /// </summary>
        [System.ComponentModel.Description("Occurs when mouse enters the day/week label inside of the calendar.")]
        public event EventHandler LabelMouseEnter;

        /// <summary>
        /// Occurs when mouse leaves the day/week label inside of the calendar.
        /// </summary>
        [System.ComponentModel.Description("Occurs when mouse leaves the day/week label inside of the calendar.")]
        public event EventHandler LabelMouseLeave;

        /// <summary>
        /// Occurs when mouse moves over the day/week label inside of the calendar.
        /// </summary>
        [System.ComponentModel.Description("Occurs when mouse moves over the day/week label inside of the calendar.")]
        public event System.Windows.Forms.MouseEventHandler LabelMouseMove;

        /// <summary>
        /// Occurs when mouse remains still inside an day/week label of the calendar for an amount of time.
        /// </summary>
        [System.ComponentModel.Description("Occurs when mouse remains still inside an day/week label of the calendar for an amount of time.")]
        public event EventHandler LabelMouseHover;

        /// <summary>
        /// Occurs when SelectedDate property has changed.
        /// </summary>
        [System.ComponentModel.Description("Occurs when SelectedDate property has changed.")]
        public event EventHandler DateChanged;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the CalendarMonth class.
        /// </summary>
        public SingleMonthCalendar()
        {
            _FirstDayOfWeek = DateTimeInput.GetActiveCulture().DateTimeFormat.FirstDayOfWeek;
            m_IsContainer = true;
            this.AutoCollapseOnClick = true;
            this.MouseUpNotification = true;
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            _Colors.Parent = this;
            for (int i = 0; i < 49; i++)
            {
                DayLabel day = new DayLabel();
                day.Visible = false;
                this.SubItems.Add(day);
            }

            // Add navigation container
            Size fixedButtonSize = _DefaultNavigationButtonSize;
            ItemContainer cont = new ItemContainer();
            cont.Name = NavigationContainerName;
            cont.AutoCollapseOnClick = false;
            cont.MinimumSize = new Size(0, fixedButtonSize.Height + 3);
            cont.LayoutOrientation = eOrientation.Horizontal;
            cont.HorizontalItemAlignment = eHorizontalItemsAlignment.Center;
            cont.VerticalItemAlignment = eVerticalItemsAlignment.Middle;
            cont.ItemSpacing = 2;
            ButtonItem nav = new ButtonItem(NavDecreaseMonth);
            nav.GlobalItem = false;
            nav._FadeEnabled = false;
            nav.Text = "<expand direction=\"left\"/>";
            nav.Click += new EventHandler(MonthNavigationDecreaseClick);
            nav.AutoCollapseOnClick = false;
            nav.FixedSize = fixedButtonSize;
            nav._FixedSizeCenterText = true;
            nav.ClickAutoRepeat = true;
            cont.SubItems.Add(nav);
            LabelItem label = new LabelItem(NavMonthLabel);
            label.AutoCollapseOnClick = false;
            label.GlobalItem = false;
            label.TextAlignment = StringAlignment.Center;
            label.TextLineAlignment = StringAlignment.Center;
            label.PaddingBottom = 2;
            label.Click += new EventHandler(ShowMonthSelection);
            label.Cursor = Cursors.Hand;
            cont.SubItems.Add(label);
            nav = new ButtonItem(NavIncreaseMonth);
            nav.GlobalItem = false;
            nav._FadeEnabled = false;
            nav.Text = "<expand direction=\"right\"/>";
            nav.Click += new EventHandler(MonthNavigationIncreaseClick);
            nav.AutoCollapseOnClick = false;
            nav.FixedSize = fixedButtonSize;
            nav._FixedSizeCenterText = true;
            nav.ClickAutoRepeat = true;
            cont.SubItems.Add(nav);

            // Year Navigation
            nav = new ButtonItem(NavDecreaseYear);
            nav.GlobalItem = false;
            nav._FadeEnabled = false;
            nav.Text = "<expand direction=\"left\"/>";
            nav.Click += new EventHandler(YearNavigationDecreaseClick);
            nav.AutoCollapseOnClick = false;
            nav.FixedSize = fixedButtonSize;
            nav._FixedSizeCenterText = true;
            nav.ClickAutoRepeat = true;
            cont.SubItems.Add(nav);
            label = new LabelItem(NavYearLabel);
            label.AutoCollapseOnClick = false;
            label.GlobalItem = false;
            label.TextAlignment = StringAlignment.Center;
            label.TextLineAlignment = StringAlignment.Center;
            label.PaddingBottom = 2;
            label.Click += ShowYearSelection;
            label.Cursor = Cursors.Hand;
            cont.SubItems.Add(label);
            nav = new ButtonItem(NavIncreaseYear);
            nav.GlobalItem = false;
            nav._FadeEnabled = false;
            nav.Text = "<expand direction=\"right\"/>";
            nav.Click += new EventHandler(YearNavigationIncreaseClick);
            nav.AutoCollapseOnClick = false;
            nav.FixedSize = fixedButtonSize;
            nav._FixedSizeCenterText = true;
            nav.ClickAutoRepeat = true;
            cont.SubItems.Add(nav);

            //_MonthsPopupMenu = new ButtonItem(MonthsPopupMenu);
            //_MonthsPopupMenu.Visible = false;
            //cont.SubItems.Add(_MonthsPopupMenu);

            this.SubItems.Add(cont);
        }
        #endregion

        #region Internal Implementation
        private bool _MonthSelectionMode = false;
        /// <summary>
        /// Indicates whether calendar is used in month selection mode which shows only month and year.
        /// </summary>
        [DefaultValue(false), Category("Behavior"), Description("Indicates whether calendar is used in month selection mode which shows only month and year.")]
        public bool MonthSelectionMode
        {
            get { return _MonthSelectionMode; }
            set
            {
                if (value != _MonthSelectionMode)
                {
                    bool oldValue = _MonthSelectionMode;
                    _MonthSelectionMode = value;
                    OnMonthSelectionModeChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when MonthSelectionMode property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnMonthSelectionModeChanged(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                _MonthSelectionItem = new MonthSelectorContainer();
                _MonthSelectionItem.FixedSize = new Size(162, 140);
                if (_SelectedDate != DateTime.MinValue)
                    _MonthSelectionItem.SelectedMonth = this.SelectedDate.Month;
                _MonthSelectionItem.SelectedMonthChanged += new EventHandler(MonthSelectionItemSelectedMonthChanged);
                _MonthSelectionItem.UseMonthNames = _MonthSelectorShortMonthNames;
                this.SubItems.Add(_MonthSelectionItem);
                _YearSelectionItem = new YearSelectorContainer();
                _YearSelectionItem.FixedSize = new Size(162, 140);
                if (_SelectedDate != DateTime.MinValue)
                    _YearSelectionItem.SelectedYear = this.SelectedDate.Year;
                _YearSelectionItem.SelectedYearChanged += new EventHandler(YearSelectionItemSelectedYearChanged);
                this.SubItems.Add(_YearSelectionItem);
            }
            else
            {
                this.SubItems.Remove(_MonthSelectionItem);
                this.SubItems.Remove(_YearSelectionItem);
                _MonthSelectionItem.Dispose();
                _MonthSelectionItem = null;
                _YearSelectionItem.Dispose();
                _YearSelectionItem = null;
                this.DisplayMonth = _DisplayMonth;
            }
            NeedRecalcSize = true;
            OnAppearanceChanged();
            //OnPropertyChanged(new PropertyChangedEventArgs("MonthSelectionMode"));
        }
        internal void DropDownClosed()
        {
            if (_YearSelectionItem != null)
                _YearSelectionItem.RemoveCenturySelector();
        }
        void YearSelectionItemSelectedYearChanged(object sender, EventArgs e)
        {
            if (_SelectedDate != DateTime.MinValue)
            {
                DateTime dt=new DateTime(_YearSelectionItem.SelectedYear, _SelectedDate.Month, _SelectedDate.Day, _SelectedDate.Hour, _SelectedDate.Minute, _SelectedDate.Second, _SelectedDate.Kind);
                DisplayMonth = dt;
                this.SelectedDate = dt;
            }
            else if (!_MonthSelectionItem.IsEmpty)
            {
                DateTime dt = new DateTime(_YearSelectionItem.SelectedYear, _MonthSelectionItem.SelectedMonth, 1);
                DisplayMonth = dt;
                this.SelectedDate = dt;
            }
            //else 
            //    this.SelectedDate = new DateTime(_YearSelectionItem.SelectedYear, DateTime.Now.Month, 1);
        }

        void MonthSelectionItemSelectedMonthChanged(object sender, EventArgs e)
        {
            if (_SelectedDate != DateTime.MinValue)
            {
                int selectedDay = Math.Min(_SelectedDate.Day,
                    DateTimeInput.GetActiveCulture()
                        .Calendar.GetDaysInMonth(_SelectedDate.Year, _MonthSelectionItem.SelectedMonth));
                DateTime dt = new DateTime(_SelectedDate.Year, _MonthSelectionItem.SelectedMonth, selectedDay, _SelectedDate.Hour, _SelectedDate.Minute, _SelectedDate.Second, _SelectedDate.Kind);
                DisplayMonth = dt;
                this.SelectedDate = dt;
            }
            else if (_YearSelectionItem.SelectedYear > 0)
            {
                DateTime dt = new DateTime(_YearSelectionItem.SelectedYear, _MonthSelectionItem.SelectedMonth, 1);
                DisplayMonth = dt;
                this.SelectedDate = dt;
            }
        }
        private bool _YearSelectionEnabled = true;
        /// <summary>
        /// Gets or sets whether Year/Century selection is enabled.
        /// </summary>
        [DefaultValue(true), Category("Behavior"), Description("Indicates whether Year/Century selection is enabled.")]
        public bool YearSelectionEnabled
        {
            get { return _YearSelectionEnabled; }
            set
            {
                if (_YearSelectionEnabled != value)
                {
                    _YearSelectionEnabled = value;
                    ItemContainer nav = GetNavigationContainer();
                    if (nav != null)
                    {
                        LabelItem label = nav.SubItems[NavYearLabel] as LabelItem;
                        if (label != null)
                        {
                            label.Cursor = _YearSelectionEnabled ? Cursors.Hand : null;
                        }
                    }
                }
            }
        }

        private bool _MonthSelectorShortMonthNames = true;
        /// <summary>
        /// Indicates whether month selector which is displayed when month label is clicked is using abbreviated month names instead of month number.
        /// </summary>
        [DefaultValue(true), Category("Behavior"), Description("Indicates whether month selector which is displayed when month label is clicked is using abbreviated month names instead of month number.")]
        public bool MonthSelectorShortMonthNames
        {
            get { return _MonthSelectorShortMonthNames; }
            set
            {
                _MonthSelectorShortMonthNames = value;
                if (_MonthSelectionItem != null)
                    _MonthSelectionItem.UseMonthNames = _MonthSelectorShortMonthNames;
            }
        }

        private eCurrentSingleMonthCalendarOperation _CurrentOperation = eCurrentSingleMonthCalendarOperation.None;
        private void ShowYearSelection(object sender, EventArgs e)
        {
            if (_YearSelector != null || !_YearSelectionEnabled || _CurrentOperation != eCurrentSingleMonthCalendarOperation.None || this.Parent is MonthCalendarItem && (((MonthCalendarItem)this.Parent).CalendarDimensions.Width > 1 || ((MonthCalendarItem)this.Parent).CalendarDimensions.Height > 1))
                return;
            _CurrentOperation = eCurrentSingleMonthCalendarOperation.AnimatingShowYearSelector;
            _YearSelector = new YearSelectorControl();
            _YearSelector.Size = new Size(8, 8);
            _YearSelector.Location = new Point(this.LeftInternal + (this.WidthInternal - 8) / 2, this.TopInternal + (this.HeightInternal - 8) / 2);
            _YearSelector.Visible = true;
            _YearSelector.Style = eDotNetBarStyle.StyleManagerControlled;

            if (BarFunctions.IsOffice2007Style(this.Style))
            {
                Office2007ColorTable table = ((Office2007Renderer)GlobalManager.Renderer).ColorTable;
                ElementStyle es = (ElementStyle)table.StyleClasses[ElementStyleClassKeys.ItemPanelKey];
                _YearSelector.BackgroundStyle.BackColor = es.BackColor;
                _YearSelector.BackgroundStyle.BackColor2 = es.BackColor2;
            }
            else
                _YearSelector.BackgroundStyle.BackColor = SystemColors.Window;
            ItemContainer nav = GetNavigationContainer();
            LabelItem label = nav.SubItems[NavMonthLabel] as LabelItem;

            if (!StyleManager.IsMetro(this.EffectiveStyle))
                _YearSelector.YearSelector.TextColor = label.ForeColor;

            Control cc = (Control)this.ContainerControl;
            cc.Controls.Add(_YearSelector);
            _YearSelector.YearSelector.CenturyYearStart = (this.DisplayMonth.Year / 10) * 10;
            _YearSelector.YearSelector.SelectedYear = this.DisplayMonth.Year;
            Rectangle targetBounds = this.Bounds;

            _YearSelector.YearSelector.SelectedYearChanged += YearSelectorSelectedYearChanged;

            DevComponents.DotNetBar.Animation.AnimationRectangle anim = new DevComponents.DotNetBar.Animation.AnimationRectangle(
                new AnimationRequest(_YearSelector, "Bounds", targetBounds),
                DevComponents.DotNetBar.Animation.AnimationEasing.EaseInOutQuad, 200);
            anim.AutoDispose = true;
            anim.AnimationCompleted += ShowAnimationCompleted;
            anim.Start();

        }
        private void ShowAnimationCompleted(object sender, EventArgs e)
        {
            _CurrentOperation = eCurrentSingleMonthCalendarOperation.None;
        }
        private void YearSelectorSelectedYearChanged(object sender, EventArgs e)
        {
            if (_CurrentOperation != eCurrentSingleMonthCalendarOperation.None)
                return;
            RemoveYearSelector(true);

            DateTime newDisplayMonth = new DateTime(_YearSelector.YearSelector.SelectedYear, this.DisplayMonth.Month, this.DisplayMonth.Day);
            if (newDisplayMonth < MinDate)
                newDisplayMonth = MinDate;
            else if (newDisplayMonth > MaxDate)
                newDisplayMonth = MaxDate;
            SetDisplayMonth(newDisplayMonth.Month, newDisplayMonth.Year, eEventSource.Mouse);
        }
        private void RemoveYearSelector(bool animate)
        {
            if (_CurrentOperation == eCurrentSingleMonthCalendarOperation.AnimatingHideYearSelector) return;
            if (animate)
            {
                _CurrentOperation = eCurrentSingleMonthCalendarOperation.AnimatingHideYearSelector;
                Rectangle targetBounds = new Rectangle(this.LeftInternal + (this.WidthInternal - 8) / 2, this.TopInternal + (this.HeightInternal - 8) / 2, 8, 8);
                DevComponents.DotNetBar.Animation.AnimationRectangle anim = new DevComponents.DotNetBar.Animation.AnimationRectangle(
                    new AnimationRequest(_YearSelector, "Bounds", targetBounds),
                    DevComponents.DotNetBar.Animation.AnimationEasing.EaseInOutQuad, 200);
                anim.AnimationCompleted += AnimationCompleted;
                anim.Start();
            }
            else
            {
                if (_YearSelector.Parent != null)
                    _YearSelector.Parent.Controls.Remove(_YearSelector);
                _YearSelector.YearSelector.SelectedYearChanged -= YearSelectorSelectedYearChanged;
                _YearSelector.Dispose();
                _YearSelector = null;
            }
        }
        void AnimationCompleted(object sender, EventArgs e)
        {
            DevComponents.DotNetBar.Animation.AnimationRectangle anim = (DevComponents.DotNetBar.Animation.AnimationRectangle)sender;
            anim.Dispose();
            if (_YearSelector.Parent != null)
                _YearSelector.Parent.Controls.Remove(_YearSelector);
            _YearSelector.YearSelector.SelectedYearChanged -= YearSelectorSelectedYearChanged;
            _YearSelector.Dispose();
            _YearSelector = null;
            _CurrentOperation = eCurrentSingleMonthCalendarOperation.None;
        }
        private void ShowMonthSelection(object sender, EventArgs e)
        {
            if (_MonthSelector != null || _CurrentOperation != eCurrentSingleMonthCalendarOperation.None || this.Parent is MonthCalendarItem && (((MonthCalendarItem)this.Parent).CalendarDimensions.Width > 1 || ((MonthCalendarItem)this.Parent).CalendarDimensions.Height > 1))
                return;
            if (!HeaderNavigationEnabled) return;

            _CurrentOperation = eCurrentSingleMonthCalendarOperation.AnimatingShowMonthSelector;
            _MonthSelector = new MonthSelectorControl();
            _MonthSelector.Size = new Size(8, 8);
            _MonthSelector.Location = new Point(this.LeftInternal + (this.WidthInternal - 8) / 2, this.TopInternal + (this.HeightInternal - 8) / 2);
            _MonthSelector.MonthSelector.UseMonthNames = _MonthSelectorShortMonthNames;
            _MonthSelector.Visible = true;
            _MonthSelector.Style = eDotNetBarStyle.StyleManagerControlled;

            if (BarFunctions.IsOffice2007Style(this.Style))
            {
                Office2007ColorTable table = ((Office2007Renderer)GlobalManager.Renderer).ColorTable;
                ElementStyle es = (ElementStyle)table.StyleClasses[ElementStyleClassKeys.ItemPanelKey];
                _MonthSelector.BackgroundStyle.BackColor = es.BackColor;
                _MonthSelector.BackgroundStyle.BackColor2 = es.BackColor2;
            }
            else
                _MonthSelector.BackgroundStyle.BackColor = SystemColors.Window;
            ItemContainer nav = GetNavigationContainer();
            LabelItem label = nav.SubItems[NavMonthLabel] as LabelItem;

            if (!StyleManager.IsMetro(this.EffectiveStyle))
                _MonthSelector.MonthSelector.TextColor = label.ForeColor;

            Control cc = (Control)this.ContainerControl;
            cc.Controls.Add(_MonthSelector);
            _MonthSelector.MonthSelector.SelectedMonth = this.DisplayMonth.Month;
            Rectangle targetBounds = this.Bounds;

            _MonthSelector.MonthSelector.SelectedMonthChanged += MonthSelectorSelectedMonthChanged;

            DevComponents.DotNetBar.Animation.AnimationRectangle anim = new DevComponents.DotNetBar.Animation.AnimationRectangle(
                new AnimationRequest(_MonthSelector, "Bounds", targetBounds),
                DevComponents.DotNetBar.Animation.AnimationEasing.EaseInOutQuad, 200);
            anim.AutoDispose = true;
            anim.AnimationCompleted += ShowAnimationCompleted;
            anim.Start();

        }
        private void MonthSelectorSelectedMonthChanged(object sender, EventArgs e)
        {
            if (_CurrentOperation != eCurrentSingleMonthCalendarOperation.None)
                return;
            RemoveMonthSelector(true);

            DateTime newDisplayMonth = new DateTime(this.DisplayMonth.Year, _MonthSelector.MonthSelector.SelectedMonth, this.DisplayMonth.Day);
            if (newDisplayMonth < MinDate)
                newDisplayMonth = MinDate;
            else if (newDisplayMonth > MaxDate)
                newDisplayMonth = MaxDate;
            SetDisplayMonth(newDisplayMonth.Month, newDisplayMonth.Year, eEventSource.Mouse);
        }
        private void RemoveMonthSelector(bool animate)
        {
            if (_CurrentOperation == eCurrentSingleMonthCalendarOperation.AnimatingHideMonthSelector) return;
            if (animate)
            {
                _CurrentOperation = eCurrentSingleMonthCalendarOperation.AnimatingHideMonthSelector;
                Rectangle targetBounds = new Rectangle(this.LeftInternal + (this.WidthInternal - 8) / 2, this.TopInternal + (this.HeightInternal - 8) / 2, 8, 8);
                DevComponents.DotNetBar.Animation.AnimationRectangle anim = new DevComponents.DotNetBar.Animation.AnimationRectangle(
                    new AnimationRequest(_MonthSelector, "Bounds", targetBounds),
                    DevComponents.DotNetBar.Animation.AnimationEasing.EaseInOutQuad, 200);
                anim.AnimationCompleted += MonthSelectorAnimationCompleted;
                anim.Start();
            }
            else
            {
                if (_MonthSelector.Parent != null)
                    _MonthSelector.Parent.Controls.Remove(_MonthSelector);
                _MonthSelector.MonthSelector.SelectedMonthChanged -= MonthSelectorSelectedMonthChanged;
                _MonthSelector.Dispose();
                _MonthSelector = null;
            }
        }
        void MonthSelectorAnimationCompleted(object sender, EventArgs e)
        {
            DevComponents.DotNetBar.Animation.AnimationRectangle anim = (DevComponents.DotNetBar.Animation.AnimationRectangle)sender;
            anim.Dispose();
            if (_MonthSelector.Parent != null)
                _MonthSelector.Parent.Controls.Remove(_MonthSelector);
            _MonthSelector.MonthSelector.SelectedMonthChanged -= MonthSelectorSelectedMonthChanged;
            _MonthSelector.Dispose();
            _MonthSelector = null;
            _CurrentOperation = eCurrentSingleMonthCalendarOperation.None;
        }

        //private void ShowMonthSelectionPopupMenu(object sender, EventArgs e)
        //{
        //    if (!HeaderNavigationEnabled) return;

        //    ButtonItem parent = _MonthsPopupMenu;
        //    foreach (BaseItem item in parent.SubItems)
        //    {
        //        item.Click -= new EventHandler(PopupMonthSelectionClick);
        //    }
        //    parent.SubItems.Clear();

        //    string[] months = DateTimeInput.GetActiveCulture().DateTimeFormat.MonthNames;
        //    for (int i = 0; i < months.Length; i++)
        //    {
        //        if (months[i] == null || months[i].Length == 0) continue;

        //        ButtonItem m = new ButtonItem((i + 1).ToString(), months[i]);
        //        m.AutoCollapseOnClick = false;
        //        parent.SubItems.Add(m);
        //        m.Click += new EventHandler(PopupMonthSelectionClick);
        //    }

        //    parent.Popup(Control.MousePosition);
        //}

        private void PopupMonthSelectionClick(object sender, EventArgs e)
        {
            ButtonItem b = sender as ButtonItem;
            if (b == null) return;
            int month = int.Parse(b.Name);
            ((PopupItem)b.Parent).Expanded = false;
            SetDisplayMonth(month, this.DisplayMonth.Year, eEventSource.Mouse);
        }

        public override void RecalcSize()
        {
            Point innerLocation = m_Rect.Location;
            bool disposeStyle = false;
            ElementStyle style = ElementStyleDisplay.GetElementStyle(this.BackgroundStyle, out disposeStyle);
            innerLocation.X += ElementStyleLayout.LeftWhiteSpace(style); // m_BackgroundStyle.PaddingLeft + m_BackgroundStyle.MarginLeft;
            innerLocation.Y += ElementStyleLayout.TopWhiteSpace(style);

            if (_MonthSelectionMode)
            {
                foreach (BaseItem item in this.SubItems)
                {
                    item.Displayed = false;
                }

                _MonthSelectionItem.LeftInternal = innerLocation.X;
                _MonthSelectionItem.TopInternal = innerLocation.Y + 22;
                _MonthSelectionItem.RecalcSize();
                _MonthSelectionItem.Displayed = true;
                _YearSelectionItem.LeftInternal = innerLocation.X + _MonthSelectionItem.WidthInternal;
                _YearSelectionItem.TopInternal = innerLocation.Y;
                _YearSelectionItem.RecalcSize();
                _YearSelectionItem.Displayed = true;
                m_Rect.Width = _MonthSelectionItem.WidthInternal + _YearSelectionItem.WidthInternal;
                m_Rect.Height = Math.Max(_MonthSelectionItem.HeightInternal, _YearSelectionItem.HeightInternal);
                m_Rect.Width += ElementStyleLayout.HorizontalStyleWhiteSpace(style);
                m_Rect.Height += ElementStyleLayout.VerticalStyleWhiteSpace(style);
            }
            else
            {
                int c = 49;
                int itemsInLine = 0;
                int lineHeight = 0;
                int daysSpacing = 2;

                Point weekStartPoint = innerLocation;
                Point daysStartPoint = innerLocation;

                Size daySize = Dpi.Size(_DaySize);
                if (_ShowWeekNumbers)
                {
                    daysStartPoint.X += daySize.Width;
                }

                ItemContainer nav = GetNavigationContainer();
                nav.LeftInternal = innerLocation.X;
                nav.TopInternal = innerLocation.Y;
                nav.WidthInternal = daySize.Width * (_ShowWeekNumbers ? 8 : 7);
                nav.RecalcSize();
                nav.WidthInternal = daySize.Width * (_ShowWeekNumbers ? 8 : 7);
                weekStartPoint.Y += nav.HeightInternal + 1;
                daysStartPoint.Y += nav.HeightInternal + 1;

                Point p = daysStartPoint;
                int line = 0;
                for (int i = 0; i < c; i++)
                {
                    BaseItem item = this.SubItems[i];
                    if (item is YearSelectorContainer) continue;
                    DayLabel day = item as DayLabel;
                    if (day != null)
                    {
                        day.RecalcSize();
                        day.Bounds = new Rectangle(p, daySize);
                    }
                    else
                    {
                        item.RecalcSize();
                    }

                    if (item.Visible)
                    {
                        if (item.HeightInternal > lineHeight)
                            lineHeight = item.HeightInternal;
                        p.X += item.WidthInternal;
                    }
                    itemsInLine++;
                    if (itemsInLine == 7)
                    {
                        if (_ShowWeekNumbers)
                        {
                            if (line > 0)
                            {
                                DayLabel weekLabel = this.SubItems[c + line - 1] as DayLabel;
                                weekLabel.RecalcSize();
                                weekLabel.Bounds = new Rectangle(weekStartPoint, daySize);
                                weekStartPoint.Y += lineHeight;
                            }
                            else
                                weekStartPoint.Y += lineHeight + daysSpacing;
                        }

                        itemsInLine = 0;
                        p.Y += lineHeight;
                        if (line == 0) p.Y += daysSpacing;
                        p.X = daysStartPoint.X;
                        lineHeight = 0;
                        line++;
                    }
                }
                m_Rect.Width = daySize.Width * 7;
                m_Rect.Height = daySize.Height * 7 + daysSpacing;
                if (_ShowWeekNumbers)
                    m_Rect.Width += daySize.Width;

                m_Rect.Width += ElementStyleLayout.HorizontalStyleWhiteSpace(style);
                m_Rect.Height += ElementStyleLayout.VerticalStyleWhiteSpace(style) + nav.HeightInternal + 1;
            }

            if (disposeStyle) style.Dispose();

            base.RecalcSize();
        }

        /// <summary>
        /// Must be overridden by class that is inheriting to provide the painting for the item.
        /// </summary>
        public override void Paint(ItemPaintArgs p)
        {
            ItemContainer nav = GetNavigationContainer();
            LabelItem label = nav.SubItems[NavMonthLabel] as LabelItem;
            label.ForeColor = p.Colors.ItemText;
            label = nav.SubItems[NavYearLabel] as LabelItem;
            label.ForeColor = p.Colors.ItemText;

            base.Paint(p);
        }

        /// <summary>
        /// Returns copy of the item.
        /// </summary>
        public override BaseItem Copy()
        {
            SingleMonthCalendar objCopy = new SingleMonthCalendar();
            this.CopyToItem(objCopy);
            return objCopy;
        }
        /// <summary>
        /// Copies the CalendarMonth specific properties to new instance of the item.
        /// </summary>
        /// <param name="c">New ButtonItem instance.</param>
        protected override void CopyToItem(BaseItem c)
        {
            SingleMonthCalendar copy = c as SingleMonthCalendar;
            base.CopyToItem(copy);
        }

        private void SetDisplayMonth(DateTime date, eEventSource source)
        {
            SetDisplayMonth(date.Month, date.Year, source);
        }

        private void SetDisplayMonth(int month, int year, eEventSource source)
        {
            DateTime d = new DateTime(year, month, 1);
            bool monthChanged = d != _DisplayMonth;

            if (_MonthSelectionMode)
            {
                _DisplayMonth = d;
                if (monthChanged)
                    OnMonthChanged(new DevComponents.DotNetBar.Events.EventSourceArgs(source));
                return;
            }

            if (monthChanged)
                OnMonthChanging(new DevComponents.DotNetBar.Events.EventSourceArgs(source));

            _DisplayMonth = d;
            // Initialize days
            string[] dayNames = DateTimeInput.GetActiveCulture().DateTimeFormat.AbbreviatedDayNames;
            string[] shortestDayNames = DateTimeInput.GetActiveCulture().DateTimeFormat.ShortestDayNames;
            if (_DayNames != null)
                dayNames = _DayNames;
            int firstDay = (int)_FirstDayOfWeek, currentDay = firstDay;
            for (int i = 0; i < 7; i++)
            {
                DayLabel day = this.SubItems[i] as DayLabel;
                day.Text = _TwoLetterDayName ? GetTwoLetterDayName(shortestDayNames[currentDay]) : dayNames[currentDay];
                day.Visible = true;
                day.Displayed = true;
                day.TrackMouse = false;
                day.Selectable = false;
                day.IsDayLabel = true;
                ApplyDayOfWeekMarker(day);
                currentDay++;
                if (currentDay > 6)
                    currentDay = 0;
            }

            // Recurring monthly markers
            bool[] monthlyMarkers = GetMonthlyMarkers();
            bool[] weeklyMarkers = GetWeeklyMarkers();
            bool[] annualMarkers = GetAnnualMarkers(month);
            bool[] markedDays = GetMarkedDays(month, year);

            DateTime today = _ShowTodayMarker ? (_TodayDate == DateTime.MinValue ? DateTime.Today : _TodayDate) : DateTime.MinValue;
            bool markToday = today != DateTime.MinValue;

            // Initialize days, If Visible=true, but Displayed=False item not displayed but takes space in layout. 
            // If Visible=false item not visible and does not take space in layout.
            int firstDayOfMonth = (int)d.DayOfWeek - firstDay;
            if (firstDayOfMonth < 0) firstDayOfMonth = 7 + firstDayOfMonth;
            int startIndex = 7;
            if (firstDayOfMonth > 0)
            {
                DateTime dt = d;
                dt = dt.AddDays(-firstDayOfMonth);

                annualMarkers = GetAnnualMarkers(dt.Month);
                markedDays = GetMarkedDays(dt.Month, dt.Year);

                for (int i = startIndex; i < startIndex + firstDayOfMonth; i++)
                {
                    DayLabel day = this.SubItems[i] as DayLabel;
                    day.Date = dt;
                    day.Visible = true;
                    ResetDayLabel(day);

                    if (weeklyMarkers[(int)dt.DayOfWeek])
                        ApplyWeeklyMarker(day);
                    if (monthlyMarkers[dt.Day - 1])
                        ApplyMonthlyMarker(day);
                    if (annualMarkers[dt.Day - 1])
                        ApplyAnnualMarker(day);
                    if (markedDays[dt.Day - 1])
                        ApplyDayMarker(day);

                    if (markToday && today == dt) day.IsToday = true;

                    if (!_WeekendDaysSelectable && DayLabel.IsWeekend(dt))
                    {
                        day.Selectable = false;
                        day.TrackMouse = false;
                    }
                    day.Enabled = CanSelect(dt);
                    day.Displayed = _TrailingDaysVisible && _TrailingDaysBeforeVisible;
                    day.IsTrailing = true;
                    dt = dt.AddDays(1);
                }
                startIndex += firstDayOfMonth;
            }

            annualMarkers = GetAnnualMarkers(month);
            markedDays = GetMarkedDays(month, year);
            bool newMonthFirstSwitch = true;

            int runningWeek = 0, dayCount = startIndex % 7;
            for (int i = startIndex; i < 49; i++)
            {
                DayLabel day = this.SubItems[i] as DayLabel;
                day.Date = d;
                ResetDayLabel(day);

                if (d.Month != month && newMonthFirstSwitch)
                {
                    annualMarkers = GetAnnualMarkers(d.Month);
                    markedDays = GetMarkedDays(d.Month, d.Year);
                    newMonthFirstSwitch = false;
                }

                if (weeklyMarkers[(int)d.DayOfWeek])
                    ApplyWeeklyMarker(day);
                if (monthlyMarkers[d.Day - 1])
                    ApplyMonthlyMarker(day);
                if (annualMarkers[d.Day - 1])
                    ApplyAnnualMarker(day);
                if (markedDays[d.Day - 1])
                    ApplyDayMarker(day);

                if (markToday && today == d) day.IsToday = true;

                if (!_WeekendDaysSelectable && DayLabel.IsWeekend(d))
                {
                    day.Selectable = false;
                    day.TrackMouse = false;
                }
                day.Enabled = CanSelect(d);

                day.Visible = true;
                if (d.Month != month)
                {
                    day.IsTrailing = true;
                    day.Displayed = _TrailingDaysVisible && _TrailingDaysAfterVisible;
                }
                else
                {
                    day.Displayed = true;
                    day.IsTrailing = false;
                    if (day.Date == _SelectedDate) day.IsSelected = true;
                }

                if (dayCount == 6)
                {
                    if (_ShowWeekNumbers)
                    {
                        DayLabel week = this.SubItems[49 + runningWeek] as DayLabel;
                        week.Text = DateTimeInput.GetActiveCulture().Calendar.GetWeekOfYear(d, _WeekOfYearRule, _FirstDayOfWeek).ToString();
                        week.Visible = true;
                        week.TrackMouse = false;
                        week.Selectable = false;
                        if (d.Month != month && d.AddDays(-dayCount).Month != month)
                            week.Displayed = _TrailingDaysVisible;
                        else
                            week.Displayed = true;
                        ApplyWeekOfYearMarker(week);
                    }
                    runningWeek++;
                    dayCount = -1;
                }

                d = d.AddDays(1);
                dayCount++;
            }

            // Update Navigation Container
            ItemContainer nav = GetNavigationContainer();
            nav.Visible = true;
            nav.Displayed = true;
            LabelItem label = nav.SubItems[NavMonthLabel] as LabelItem;
            //Size daySize = Dpi.Size(_DaySize);
            label.Width = _DaySize.Width * 3;
            label.Text = DateTimeInput.GetActiveCulture().DateTimeFormat.MonthNames[month - 1];
            label = nav.SubItems[NavYearLabel] as LabelItem;
            label.Text = year.ToString();
            label.Width = (int)(_DaySize.Width * 1.4);

            // Enable/disable navigation buttons
            UpdateNavigationButtonsEnabled();

            this.OnAppearanceChanged();
            this.Refresh();

            if (monthChanged)
            {
                OnMonthChanged(new DevComponents.DotNetBar.Events.EventSourceArgs(source));
                if (_YearSelector != null)
                    RemoveYearSelector(false);
                if (_MonthSelector != null)
                    RemoveMonthSelector(false);
            }
        }

        private void UpdateNavigationButtonsEnabled()
        {
            // Enable/disable navigation buttons
            ItemContainer nav = GetNavigationContainer();
            DateTime d = _DisplayMonth.AddMonths(_NavigationMonthsAheadVisibility);
            if (d > this.MaxDate)
            {
                nav.SubItems[NavIncreaseMonth].Enabled = false;
                nav.SubItems[NavIncreaseYear].Enabled = false;
            }
            else
            {
                nav.SubItems[NavIncreaseMonth].Enabled = true;
                d = _DisplayMonth.AddYears(1);
                nav.SubItems[NavIncreaseYear].Enabled = (MaxDate > d);
            }
            d = _DisplayMonth.AddMonths(-1);
            if (d < GetBeginningOfTheMonth(this.MinDate))
            {
                nav.SubItems[NavDecreaseMonth].Enabled = false;
                nav.SubItems[NavDecreaseYear].Enabled = false;
            }
            else
            {
                nav.SubItems[NavDecreaseMonth].Enabled = true;
                d = _DisplayMonth.AddYears(-1);
                nav.SubItems[NavDecreaseYear].Enabled = (d > MinDate);
            }
            d = _DisplayMonth;
            if (this.MinDate != DateTimeGroup.MinDateTime && d.Year > this.MinDate.Year && d.Month>=this.MinDate.Month)
                nav.SubItems[NavDecreaseYear].Enabled = true;
        }

        private DateTime GetBeginningOfTheMonth(DateTime date)
        {
            if (date.Day > 1)
                date = date.AddDays(-date.Day);
            return date;
        }

        private bool CanSelect(DateTime d)
        {
            if (d > MaxDate || d < MinDate)
                return false;
            return true;
        }

        private void ApplyWeekOfYearMarker(DayLabel day)
        {
            MonthCalendarColors c = GetColors();
            if (c == null || !c.WeekOfYear.IsCustomized) return;

            ApplyMarker(day, c.WeekOfYear);
        }

        private void ApplyDayOfWeekMarker(DayLabel day)
        {
            MonthCalendarColors c = GetColors();
            if (c == null || !c.DayLabel.IsCustomized) return;

            ApplyMarker(day, c.DayLabel);
        }

        private void ApplyDayMarker(DayLabel day)
        {
            MonthCalendarColors c = GetColors();
            if (c == null || !c.DayMarker.IsCustomized) return;

            ApplyMarker(day, c.DayMarker);
        }

        private bool[] GetMarkedDays(int month, int year)
        {
            bool[] markers = new bool[31];
            if (this.Parent is MonthCalendarItem)
            {
                MonthCalendarItem mc = this.Parent as MonthCalendarItem;
                DateTime[] dates = mc.MarkedDates;
                foreach (DateTime d in dates)
                {
                    if (d.Month == month && d.Year == year)
                    {
                        markers[d.Day - 1] = true;
                    }
                }
            }

            return markers;
        }

        private void ApplyAnnualMarker(DayLabel day)
        {
            MonthCalendarColors c = GetColors();
            if (c == null || !c.AnnualMarker.IsCustomized) return;

            ApplyMarker(day, c.AnnualMarker);
        }

        internal MonthCalendarColors GetColors()
        {
            MonthCalendarItem mc = this.Parent as MonthCalendarItem;
            MonthCalendarColors c = null;
            if (mc != null)
                c = mc.Colors;
            else
                c = _Colors;

            return c;
        }

        private void ApplyWeeklyMarker(DayLabel day)
        {
            MonthCalendarColors c = GetColors();
            if (c == null || !c.WeeklyMarker.IsCustomized) return;

            ApplyMarker(day, c.WeeklyMarker);
        }

        private void ApplyMonthlyMarker(DayLabel day)
        {
            MonthCalendarColors c = GetColors();
            if (c == null || !c.MonthlyMarker.IsCustomized) return;

            ApplyMarker(day, c.MonthlyMarker);
        }

        private void ApplyMarker(DayLabel day, DateAppearanceDescription c)
        {
            day.BackgroundStyle.BackColor = c.BackColor;
            day.BackgroundStyle.BackColor2 = c.BackColor2;
            day.BackgroundStyle.BackColorGradientAngle = c.BackColorGradientAngle;
            if (!c.BorderColor.IsEmpty)
            {
                day.BackgroundStyle.Border = eStyleBorderType.Solid;
                day.BackgroundStyle.BorderColor = c.BorderColor;
                day.BackgroundStyle.BorderWidth = 1;
            }
            day.IsBold = c.IsBold;
            if (!c.TextColor.IsEmpty)
                day.TextColor = c.TextColor;
            day.Selectable = c.Selectable;
            if (!c.Selectable)
                day.TrackMouse = false;
        }

        private bool[] GetMonthlyMarkers()
        {
            bool[] markers = new bool[31];
            if (this.Parent is MonthCalendarItem)
            {
                MonthCalendarItem mc = this.Parent as MonthCalendarItem;
                DateTime[] dates = mc.MonthlyMarkedDates;
                foreach (DateTime d in dates)
                {
                    markers[d.Day - 1] = true;
                }
            }

            return markers;
        }

        private bool[] GetAnnualMarkers(int month)
        {
            bool[] markers = new bool[31];
            if (this.Parent is MonthCalendarItem)
            {
                MonthCalendarItem mc = this.Parent as MonthCalendarItem;
                DateTime[] dates = mc.AnnuallyMarkedDates;
                foreach (DateTime d in dates)
                {
                    if (d.Month == month)
                    {
                        markers[d.Day - 1] = true;
                    }
                }
            }

            return markers;
        }

        private bool[] GetWeeklyMarkers()
        {
            bool[] markers = new bool[7];
            if (this.Parent is MonthCalendarItem)
            {
                MonthCalendarItem mc = this.Parent as MonthCalendarItem;
                DayOfWeek[] days = mc.WeeklyMarkedDays;
                foreach (DayOfWeek d in days)
                {
                    markers[(int)d] = true;
                }
            }

            return markers;
        }

        private ItemContainer GetNavigationContainer()
        {
            return this.SubItems[NavigationContainerName] as ItemContainer;
        }

        private void ResetDayLabel(DayLabel day)
        {
            day.IsSelected = false;
            day.Selectable = true;
            day.TrackMouse = true;
            day.IsToday = false;
            day.IsBold = false;
            day.Image = null;
            day.ImageAlign = eLabelPartAlignment.MiddleRight;
            day.TextAlign = _DefaultDayLabelTextAlign;
            day.TextColor = Color.Empty;
            day.Tooltip = "";
            day.ExpandOnMouseDown = false;
            day.SubItems.Clear();
            day.BackgroundStyle. Reset();
        }

        /// <summary>
        /// Raises the MonthChanged event.
        /// </summary>
        /// <param name="e">Provides additional event data.</param>
        protected virtual void OnMonthChanged(EventArgs e)
        {
            if (MonthChanged != null)
                MonthChanged(this, e);
        }

        /// <summary>
        /// Raises the MonthChanging event.
        /// </summary>
        /// <param name="e">Provides additional event data.</param>
        protected virtual void OnMonthChanging(EventArgs e)
        {
            if (MonthChanging != null)
                MonthChanging(this, e);
        }

        private DateTime _DisplayMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        [Description("")]
        public DateTime DisplayMonth
        {
            get { return _DisplayMonth; }
            set
            {
                if (value < MinDate)
                    value = MinDate;
                else if (value > MaxDate)
                    value = MaxDate;
                //if (value > MaxDate || value < MinDate)
                //{
                //    throw new ArgumentException("DisplayMonth must be less than MaxDate and greater than MinDate");
                //}

                SetDisplayMonth(value.Month, value.Year, eEventSource.Code);
            }
        }

        private string GetTwoLetterDayName(string s)
        {
            if (s.Length > 2)
                return s.Substring(0, 2);
            return s;
        }

        private DayOfWeek _FirstDayOfWeek = DayOfWeek.Sunday;
        /// <summary>
        /// Gets or sets the first day of week displayed on the calendar. Default value is Sunday.
        /// </summary>
        [DefaultValue(DayOfWeek.Sunday), Description("Indicates first day of week displayed on the calendar.")]
        public DayOfWeek FirstDayOfWeek
        {
            get { return _FirstDayOfWeek; }
            set
            {
                if (_FirstDayOfWeek != value)
                {
                    _FirstDayOfWeek = value;
                    SetDisplayMonth(_DisplayMonth.Month, _DisplayMonth.Year, eEventSource.Code);
                }
            }
        }

        internal bool _TrailingDaysVisible = true;
        /// <summary>
        /// Gets or sets whether trailing days outside of the current displayed month are visible on calendar.
        /// </summary>
        [DefaultValue(true)]
        public bool TrailingDaysVisible
        {
            get { return _TrailingDaysVisible; }
            set
            {
                if (_TrailingDaysVisible != value)
                {
                    _TrailingDaysVisible = value;
                    OnTrailingDaysVisibleChanged();
                }
            }
        }

        internal bool _TrailingDaysBeforeVisible = true;
        internal bool TrailingDaysBeforeVisible
        {
            get { return _TrailingDaysBeforeVisible; }
            set
            {
                if (_TrailingDaysBeforeVisible != value)
                {
                    _TrailingDaysBeforeVisible = value;
                    OnTrailingDaysVisibleChanged();
                }
            }
        }

        internal bool _TrailingDaysAfterVisible = true;
        internal bool TrailingDaysAfterVisible
        {
            get { return _TrailingDaysAfterVisible; }
            set
            {
                if (_TrailingDaysAfterVisible != value)
                {
                    _TrailingDaysAfterVisible = value;
                    OnTrailingDaysVisibleChanged();
                }
            }
        }
        private void OnTrailingDaysVisibleChanged()
        {
            if (_MonthSelectionMode) return;
            foreach (BaseItem item in this.SubItems)
            {
                DayLabel day = item as DayLabel;
                if (day != null && day.IsTrailing)
                {
                    bool visible = _TrailingDaysVisible;
                    if (day.Date < this.DisplayMonth)
                        visible &= _TrailingDaysBeforeVisible;
                    else
                        visible &= _TrailingDaysAfterVisible;
                    day.Displayed = visible;
                }
            }
            this.Refresh();
        }

        private bool _ShowWeekNumbers = false;
        /// <summary>
        /// Gets or sets whether week of year is visible. Default value is false.
        /// </summary>
        [DefaultValue(false), Description("Indicates whether week of year is visible.")]
        public bool ShowWeekNumbers
        {
            get { return _ShowWeekNumbers; }
            set
            {
                if (_ShowWeekNumbers != value)
                {
                    _ShowWeekNumbers = value;
                    OnShowWeekNumbersChanged();
                    NeedRecalcSize = true;
                    OnAppearanceChanged();
                }
            }
        }

        private void OnShowWeekNumbersChanged()
        {
            if (_ShowWeekNumbers)
            {
                // Add week of year labels, always after the days 
                for (int i = 0; i < 6; i++)
                {
                    DayLabel week = new DayLabel();
                    week.IsWeekOfYear = true;
                    this.SubItems.Insert(49 + i, week);
                }
            }
            else
            {
                // Remove week of year labels
                for (int i = 0; i < 7; i++)
                {
                    DayLabel week = this.SubItems[49] as DayLabel;
                    if (week != null && week.IsWeekOfYear)
                        this.SubItems.Remove(49);
                }
            }
        }

        private CalendarWeekRule _WeekOfYearRule = CalendarWeekRule.FirstDay;
        /// <summary>
        /// Gets or sets the rule used to determine first week of the year for week of year display on calendar. Default value is first-day.
        /// </summary>
        [DefaultValue(CalendarWeekRule.FirstDay), Description("Indicates rule used to determine first week of the year for week of year display on calendar.")]
        public CalendarWeekRule WeekOfYearRule
        {
            get { return _WeekOfYearRule; }
            set
            {
                _WeekOfYearRule = value;
                if (_ShowWeekNumbers)
                    this.OnAppearanceChanged();
            }
        }

        /// <summary>
        /// Returns the collection of sub items.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override SubItemsCollection SubItems
        {
            get
            {
                return base.SubItems;
            }
        }

        private Size _DaySize = _DefaultDaySize;
        /// <summary>
        /// Gets or sets the size of each day item on the calendar. Default value is 24, 15.
        /// </summary>
        [Description("Indicate size of each day item on the calendar.")]
        public Size DaySize
        {
            get { return _DaySize; }
            set
            {
                _DaySize = value;
                OnDaySizeChanged();
            }
        }

        private void OnDaySizeChanged()
        {
            ItemContainer ic = GetNavigationContainer();
            Size daySize = _DaySize;
            if (ic.MinimumSize.Height < daySize.Height)
                ic.MinimumSize = new Size(ic.MinimumSize.Width, daySize.Height);
            OnAppearanceChanged();
        }
        /// <summary>
        /// Gets whether property should be serialized. Provided for designer support.
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeDaySize()
        {
            return _DaySize.Width != _DefaultDaySize.Width || _DaySize.Height != _DefaultDaySize.Height;
        }
        /// <summary>
        /// Reset property to default value. Provided for designer support.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetDaySize()
        {
            this.DaySize = _DefaultDaySize;
        }

        private Size _NavigationButtonSize = _DefaultNavigationButtonSize;
        /// <summary>
        /// Gets or sets the size of date navigation buttons on the calendar. Default value is 13, 18. If you increase size of the navigation
        /// buttons change DaySize as well so everything fits.
        /// </summary>
        [Description("Indicate size of date navigation buttons on the calendar. If you increase size of the navigation buttons change DaySize as well so everything fits.")]
        public Size NavigationButtonSize
        {
            get { return _NavigationButtonSize; }
            set
            {
                _NavigationButtonSize = value;
                OnNavigationButtonSizeChanged();
            }
        }

        protected override void ScaleItem(SizeF factor)
        {
            base.ScaleItem(factor);
            OnNavigationButtonSizeChanged();
        }

        private void OnNavigationButtonSizeChanged()
        {
            Size fixedButtonSize = _NavigationButtonSize;
            ItemContainer cont = GetNavigationContainer();
            cont.MinimumSize = new Size(0, fixedButtonSize.Height + 3);

            ButtonItem nav = (ButtonItem)cont.SubItems[NavDecreaseMonth];
            nav.FixedSize = fixedButtonSize;

            nav = (ButtonItem)cont.SubItems[NavIncreaseMonth];
            nav.FixedSize = fixedButtonSize;

            nav = (ButtonItem)cont.SubItems[NavDecreaseYear];
            nav.FixedSize = fixedButtonSize;

            nav = (ButtonItem)cont.SubItems[NavIncreaseYear];
            nav.FixedSize = fixedButtonSize;

            OnAppearanceChanged();
        }
        /// <summary>
        /// Gets whether property should be serialized. Provided for designer support.
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeNavigationButtonSize()
        {
            return _NavigationButtonSize.Width != _DefaultNavigationButtonSize.Width || _NavigationButtonSize.Height != _DefaultNavigationButtonSize.Height;
        }
        /// <summary>
        /// Reset property to default value. Provided for designer support.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetNavigationButtonSize()
        {
            this.NavigationButtonSize = _DefaultNavigationButtonSize;
        }

        /// <summary>
        /// Raises the PaintLabel event.
        /// </summary>
        /// <param name="e">Provides event data.</param>
        internal void OnPaintLabel(DayLabel label, DayPaintEventArgs e)
        {
            if (PaintLabel != null)
                PaintLabel(label, e);
        }

        private MonthCalendarColors _Colors = new MonthCalendarColors();
        /// <summary>
        /// Gets the calendar colors used by the control.
        /// </summary>
        [NotifyParentPropertyAttribute(true), Category("Appearance"), Description("Gets colors used by control."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public MonthCalendarColors Colors
        {
            get { return _Colors; }
        }

        internal void DayLabelClick(DayLabel day)
        {
            if (!_MouseSelectionEnabled) return;

            if (day.Selectable && day.Date != DateTime.MinValue)
            {
                if (!_WeekendDaysSelectable && DayLabel.IsWeekend(day.Date))
                    return;

                if (MultiSelect)
                {
                    if (_SelectedDate == DateTime.MinValue)
                        _SelectedDate = day.Date;
                    day.IsSelected = !day.IsSelected;
                }
                else
                {
                    SelectedDate = day.Date;
                }
            }
        }

        private bool _MultiSelect = false;
        /// <summary>
        /// Gets or sets whether multiple days can be selected by clicking each day. Default value is false.
        /// </summary>
        [DefaultValue(false), Description("Indicates whether multiple days can be selected by clicking each day.")]
        public bool MultiSelect
        {
            get { return _MultiSelect; }
            set
            {
                if (_MultiSelect != value)
                {
                    _MultiSelect = value;
                }
            }
        }

        private bool _MouseSelectionEnabled = true;
        /// <summary>
        /// Gets or sets whether selection of dates using mouse is enabled. Default value is true.
        /// </summary>
        [DefaultValue(true), Description("Indicates whether selection of dates using mouse is enabled.")]
        public bool MouseSelectionEnabled
        {
            get { return _MouseSelectionEnabled; }
            set
            {
                if (_MouseSelectionEnabled != value)
                {
                    _MouseSelectionEnabled = value;
                }
            }
        }

        private DateTime _SelectedDate = DateTime.MinValue;
        /// <summary>
        /// Gets or sets the calendar selected date.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime SelectedDate
        {
            get { return _SelectedDate; }
            set
            {
                _SelectedDate = value;
                foreach (BaseItem item in this.SubItems)
                {
                    DayLabel day = item as DayLabel;
                    if (day == null || day.Date == DateTime.MinValue) continue;
                    if (day.Date == _SelectedDate.Date)
                    {
                        day.IsSelected = true;
                    }
                    else
                    {
                        day.IsSelected = false;
                    }
                }
                if (_MonthSelectionMode)
                {
                    if (value != DateTime.MinValue && value != DateTime.MaxValue)
                    {
                        _MonthSelectionItem.SelectedMonth = _SelectedDate.Month;
                        _YearSelectionItem.SelectedYear = _SelectedDate.Year;
                    }
                    else
                    {
                        _MonthSelectionItem.IsEmpty = true;
                        _YearSelectionItem.SelectedYear = 0;
                    }
                }
                OnSelectedDateChanged(new EventArgs());
            }
        }

        /// <summary>
        /// Raises the DateChanged event.
        /// </summary>
        /// <param name="e">Provides event data.</param>
        protected virtual void OnSelectedDateChanged(EventArgs e)
        {
            if (DateChanged != null)
                DateChanged(this, e);
        }

        private bool _WeekendDaysSelectable = true;
        /// <summary>
        /// Gets or sets whether weekend days can be selected. Default value is true.
        /// </summary>
        [DefaultValue(true), Description("Indicates whether weekend days can be selected.")]
        public bool WeekendDaysSelectable
        {
            get { return _WeekendDaysSelectable; }
            set
            {
                if (_WeekendDaysSelectable != value)
                {
                    _WeekendDaysSelectable = value;
                    SetDisplayMonth(_DisplayMonth.Month, _DisplayMonth.Year, eEventSource.Code);
                }
            }
        }

        /// <summary>
        /// Gets the DayLabel item assigned to the given date. Returns null if there is no label displayed for the date.
        /// </summary>
        /// <param name="date">Date to return label for.</param>
        /// <returns>DayLabel instance or null if date is not displayed on this calendar.</returns>
        public DayLabel GetDayLabel(DateTime date)
        {
            foreach (BaseItem item in this.SubItems)
            {
                DayLabel day = item as DayLabel;
                if (day != null && day.Date.Date == date.Date && day.Visible && day.Displayed)
                    return day;
            }

            return null;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item is expanded or not. For Popup items this would indicate whether the item is popped up or not.
        /// </summary>
        [System.ComponentModel.Browsable(false), System.ComponentModel.DefaultValue(false), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public override bool Expanded
        {
            get
            {
                return m_Expanded;
            }
            set
            {
                base.Expanded = value;
                if (!value)
                    BaseItem.CollapseSubItems(this);
            }
        }

        /// <summary>
        /// Occurs when sub item expanded state has changed.
        /// </summary>
        /// <param name="item">Sub item affected.</param>
        protected internal override void OnSubItemExpandChange(BaseItem item)
        {
            base.OnSubItemExpandChange(item);
            if (item.Expanded)
                this.Expanded = true;
        }

        private eLabelPartAlignment _DefaultDayLabelTextAlign = eLabelPartAlignment.MiddleCenter;
        /// <summary>
        /// Gets or sets default text alignment for the DayLabel items representing calendar days.
        /// </summary>
        [DefaultValue(eLabelPartAlignment.MiddleCenter), Description("Indicates default text alignment for the DayLabel items representing calendar days.")]
        public eLabelPartAlignment DefaultDayLabelTextAlign
        {
            get { return _DefaultDayLabelTextAlign; }
            set
            {
                if (_DefaultDayLabelTextAlign != value)
                {
                    _DefaultDayLabelTextAlign = value;
                    SetDisplayMonth(_DisplayMonth.Month, _DisplayMonth.Year, eEventSource.Code);
                    this.Refresh();
                }
            }
        }

        internal void OnLabelMouseDown(DayLabel sender, MouseEventArgs e)
        {
            if (LabelMouseDown != null)
                LabelMouseDown(sender, e);
        }

        internal void OnLabelMouseUp(DayLabel sender, MouseEventArgs e)
        {
            if (LabelMouseUp != null)
                LabelMouseUp(sender, e);
        }

        internal void OnLabelMouseEnter(DayLabel sender, EventArgs e)
        {
            if (LabelMouseEnter != null)
                LabelMouseEnter(sender, e);
        }

        internal void OnLabelMouseLeave(DayLabel sender, EventArgs e)
        {
            if (LabelMouseLeave != null)
                LabelMouseLeave(sender, e);
        }

        internal void OnLabelMouseMove(DayLabel sender, MouseEventArgs e)
        {
            if (LabelMouseMove != null)
                LabelMouseMove(sender, e);
        }

        internal void OnLabelMouseHover(DayLabel sender, EventArgs e)
        {
            if (LabelMouseHover != null)
                LabelMouseHover(sender, e);
        }

        private bool _TwoLetterDayName = true;
        /// <summary>
        /// Gets or sets whether control uses the two letter day names. Default value is true.
        /// </summary>
        [DefaultValue(true), Description("Indicates whether control uses the two letter day names.")]
        public bool TwoLetterDayName
        {
            get { return _TwoLetterDayName; }
            set
            {
                if (_TwoLetterDayName != value)
                {
                    _TwoLetterDayName = value;
                    SetDisplayMonth(_DisplayMonth.Month, _DisplayMonth.Year, eEventSource.Code);
                    this.Refresh();
                }
            }
        }

        private string[] _DayNames = null;
        /// <summary>
        /// Gets or sets the array of custom names for days displayed on calendar header. The array must have exactly 7 elements representing day names from 0 to 6.
        /// </summary>
        [DefaultValue(null), Description("Indicates array of custom names for days displayed on calendar header."), Localizable(true)]
        public string[] DayNames
        {
            get { return _DayNames; }
            set
            {
                if (value != null && value.Length != 7)
                    throw new ArgumentException("DayNames must have exactly 7 items in collection.");
                _DayNames = value;
            }
        }

        private void MonthNavigationDecreaseClick(object sender, EventArgs e)
        {
            SetDisplayMonth(_DisplayMonth.AddMonths(-1), eEventSource.Mouse);
            if (AutoSelectDate())
                this.SelectedDate = this.DisplayMonth;
        }

        private bool AutoSelectDate()
        {
            BaseItem parent = this.Parent;
            while (parent != null)
            {
                parent = parent.Parent;
                if (parent != null && parent.Name == "sysPopupProvider")
                {
                    DateTimeInput dt = parent.ContainerControl as DateTimeInput;
                    if (dt != null)
                        return dt.AutoSelectDate;
                }
            }

            return false;
        }

        private void MonthNavigationIncreaseClick(object sender, EventArgs e)
        {
            SetDisplayMonth(_DisplayMonth.AddMonths(1), eEventSource.Mouse);
            if (AutoSelectDate())
                this.SelectedDate = this.DisplayMonth;
        }

        private void YearNavigationDecreaseClick(object sender, EventArgs e)
        {
            SetDisplayMonth(_DisplayMonth.AddMonths(-12), eEventSource.Mouse);
            if (AutoSelectDate())
                this.SelectedDate = this.DisplayMonth;
        }

        private void YearNavigationIncreaseClick(object sender, EventArgs e)
        {
            SetDisplayMonth(_DisplayMonth.AddMonths(12), eEventSource.Mouse);
            if (AutoSelectDate())
                this.SelectedDate = this.DisplayMonth;
        }

        private bool _HeaderNavigationEnabled = true;
        /// <summary>
        /// Gets or sets whether header navigation buttons for month and year are visible. Default value is true.
        /// </summary>
        [DefaultValue(true), Description("Indicates whether header navigation buttons for month and year are visible.")]
        public bool HeaderNavigationEnabled
        {
            get { return _HeaderNavigationEnabled; }
            set
            {
                if (_HeaderNavigationEnabled != value)
                {
                    _HeaderNavigationEnabled = value;
                    OnHeaderNavigationEnabledChanged();
                }
            }
        }

        private void OnHeaderNavigationEnabledChanged()
        {
            ItemContainer nav = GetNavigationContainer();
            bool b = _HeaderNavigationEnabled;
            nav.SubItems[NavDecreaseMonth].Visible = b;
            nav.SubItems[NavIncreaseMonth].Visible = b;
            nav.SubItems[NavDecreaseYear].Visible = b;
            nav.SubItems[NavIncreaseYear].Visible = b;
            nav.NeedRecalcSize = true;
            this.OnAppearanceChanged();
            this.Refresh();
        }

        /// <summary>
        /// Specifies the navigation container background style. Navigation container displays month, year and optional buttons. Default value is an empty style which means that container does not display any background.
        /// BeginGroup property set to true will override this style on some styles.
        /// </summary>
        [Browsable(true), Category("Style"), Description("Indicates navigation container background style."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ElementStyle NavigationBackgroundStyle
        {
            get { return GetNavigationContainer().BackgroundStyle; }
        }

        private bool _ShowTodayMarker = true;
        /// <summary>
        /// Gets or sets whether today marker that indicates TodayDate is visible on the calendar. Default value is true.
        /// </summary>
        [DefaultValue(true), Description("Indicates whether today marker that indicates TodayDate is visible on the calendar.")]
        public bool ShowTodayMarker
        {
            get { return _ShowTodayMarker; }
            set
            {
                if (_ShowTodayMarker != value)
                {
                    _ShowTodayMarker = value;
                    UpdateTodayMarker();
                }
            }
        }

        private DateTime _TodayDate = DateTime.MinValue;
        /// <summary>
        /// Gets or sets the value that is used by calendar as today's date.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime TodayDate
        {
            get { return _TodayDate; }
            set
            {
                value = value.Date;
                if (_TodayDate != value)
                {
                    _TodayDate = value;
                    if (_ShowTodayMarker)
                        UpdateTodayMarker();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the TodayDate property has been explicitly set. 
        /// </summary>
        [Browsable(false)]
        public bool TodayDateSet
        {
            get { return _TodayDate != DateTime.MinValue; }
        }

        private void UpdateTodayMarker()
        {
            DateTime today = _ShowTodayMarker ? (_TodayDate == DateTime.MinValue ? DateTime.Today : DateTime.MinValue) : DateTime.MinValue;
            bool noMarker = today == DateTime.MinValue;
            foreach (BaseItem item in this.SubItems)
            {
                DayLabel day = item as DayLabel;
                if (day != null)
                {
                    if (noMarker)
                        day.IsToday = false;
                    else
                        day.IsToday = (day.Date == today);
                }
            }
        }

        private System.DateTime _MinDate = DateTimeGroup.MinDateTime;
        /// <summary>
        /// Gets or sets the minimum date and time that can be selected in the control.
        /// </summary>
        [Description("Indicates minimum date and time that can be selected in the control.")]
        public System.DateTime MinDate
        {
            get { return _MinDate; }
            set { _MinDate = value;
                UpdateNavigationButtonsEnabled();
            }
        }
        /// <summary>
        /// Gets whether Value property should be serialized by Windows Forms designer.
        /// </summary>
        /// <returns>true if value serialized otherwise false.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeMinDate()
        {
            return !MinDate.Equals(DateTimeGroup.MinDateTime);
        }
        /// <summary>
        /// Reset the MinDate property to its default value.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetMinDate()
        {
            MinDate = DateTimeGroup.MinDateTime;
        }

        private System.DateTime _MaxDate = DateTimeGroup.MaxDateTime;
        /// <summary>
        /// Gets or sets the maximum date and time that can be selected in the control.
        /// </summary>
        [Description("Indicates maximum date and time that can be selected in the control.")]
        public System.DateTime MaxDate
        {
            get { return _MaxDate; }
            set { _MaxDate = value;
                UpdateNavigationButtonsEnabled();
            }
        }
        /// <summary>
        /// Gets whether Value property should be serialized by Windows Forms designer.
        /// </summary>
        /// <returns>true if value serialized otherwise false.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeMaxDate()
        {
            return !_MaxDate.Equals(DateTimeGroup.MaxDateTime);
        }
        /// <summary>
        /// Reset the MaxDate property to its default value.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetMaxDate()
        {
            MaxDate = DateTimeGroup.MaxDateTime;
        }

        private int _NavigationMonthsAheadVisibility = 1;
        internal int NavigationMonthsAheadVisibility
        {
            get { return _NavigationMonthsAheadVisibility; }
            set
            {
                if (_NavigationMonthsAheadVisibility != value)
                {
                    _NavigationMonthsAheadVisibility = value;
                    UpdateNavigationButtonsEnabled();
                }
            }
        }

        private bool _DayClickAutoClosePopup = true;
        /// <summary>
        /// Gets or sets whether clicking the day closes the parent popup if item is on popup.
        /// </summary>
        [Browsable(false), DefaultValue(true)]
        public bool DayClickAutoClosePopup
        {
            get { return _DayClickAutoClosePopup; }
            set
            {
                if (value != _DayClickAutoClosePopup)
                {
                    bool oldValue = _DayClickAutoClosePopup;
                    _DayClickAutoClosePopup = value;
                    OnDayClickAutoClosePopupChanged(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Called when DayClickAutoClosePopup property has changed.
        /// </summary>
        /// <param name="oldValue">Old property value</param>
        /// <param name="newValue">New property value</param>
        protected virtual void OnDayClickAutoClosePopupChanged(bool oldValue, bool newValue)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("DayClickAutoClosePopup"));
            foreach (BaseItem item in this.SubItems)
            {
                if (item is DayLabel)
                    item.AutoCollapseOnClick = newValue;
            }
        }
        #endregion
    }

    #region Year Selector
    internal class YearSelectorControl : BaseItemControl
    {
        /// <summary>
        /// Initializes a new instance of the YearSelectoControl class.
        /// </summary>
        public YearSelectorControl()
        {
            _YearSelector = new YearSelectorContainer();
            this.HostItem = _YearSelector;
            this.FocusCuesEnabled = false;
        }

        protected override void OnResize(EventArgs e)
        {
            _YearSelector.FixedSize = Dpi.Descale(this.Size);
            base.OnResize(e);
        }

        private YearSelectorContainer _YearSelector;
        public YearSelectorContainer YearSelector
        {
            get { return _YearSelector; }
        }
    }
    internal class CenturySelectorControl : BaseItemControl
    {
        /// <summary>
        /// Initializes a new instance of the CenturySelectorControl class.
        /// </summary>
        public CenturySelectorControl()
        {
            _CenturySelector = new CenturySelectorContainer();
            this.HostItem = _CenturySelector;
            this.FocusCuesEnabled = false;
        }

        protected override void OnResize(EventArgs e)
        {
            _CenturySelector.FixedSize = Dpi.Descale(this.Size);
            base.OnResize(e);
        }

        private CenturySelectorContainer _CenturySelector;
        public CenturySelectorContainer CenturySelector
        {
            get { return _CenturySelector; }
        }
    }
    internal class YearSelectorContainer : ItemContainer
    {
        private Size _FixedButtonSize = new Size(13, 18);
        private ItemContainer _YearsContainer = null;
        private LabelItem _CenturyLabel = null;
        /// <summary>
        /// Initializes a new instance of the YearSelectorContainer class.
        /// </summary>
        public YearSelectorContainer()
        {
            this.LayoutOrientation = eOrientation.Vertical;

            // Add navigation container
            ItemContainer cont = new ItemContainer();
            cont.Name = "yearSelectonContainerHeader";
            cont.AutoCollapseOnClick = false;
            cont.MinimumSize = new Size(0, _FixedButtonSize.Height + 3);
            cont.LayoutOrientation = eOrientation.Horizontal;
            cont.HorizontalItemAlignment = eHorizontalItemsAlignment.Center;
            cont.VerticalItemAlignment = eVerticalItemsAlignment.Middle;
            cont.ItemSpacing = 2;
            ButtonItem nav = new ButtonItem("NavDecreaseYears");
            nav.GlobalItem = false;
            nav._FadeEnabled = false;
            nav.Text = "<expand direction=\"left\"/>";
            nav.Click += YearsDecreaseClick;
            nav.AutoCollapseOnClick = false;
            nav.FixedSize = _FixedButtonSize;
            nav._FixedSizeCenterText = true;
            nav.ClickAutoRepeat = true;
            cont.SubItems.Add(nav);
            LabelItem label = new LabelItem("CenturyLabel");
            label.AutoCollapseOnClick = false;
            label.GlobalItem = false;
            label.TextAlignment = StringAlignment.Center;
            label.TextLineAlignment = StringAlignment.Center;
            label.PaddingBottom = 2;
            label.Text = "2010-2019";
            label.Click += ShowCenturySelector;
            label.Cursor = Cursors.Hand;
            _CenturyLabel = label;
            cont.SubItems.Add(label);
            nav = new ButtonItem("NavIncreaseYears");
            nav.GlobalItem = false;
            nav._FadeEnabled = false;
            nav.Text = "<expand direction=\"right\"/>";
            nav.Click += YearsIncreaseClick;
            nav.AutoCollapseOnClick = false;
            nav.FixedSize = _FixedButtonSize;
            nav._FixedSizeCenterText = true;
            nav.ClickAutoRepeat = true;
            cont.SubItems.Add(nav);
            this.SubItems.Add(cont);

            cont = new ItemContainer();
            cont.LayoutOrientation = eOrientation.Horizontal;
            cont.MultiLine = true;
            this.SubItems.Add(cont);
            for (int i = 0; i < 12; i++)
            {
                ButtonItem by = new ButtonItem("yearSelector" + i.ToString());
                by.Text = (2009 + i).ToString();
                by.FixedSize = new Size(54, 35);
                by._FixedSizeCenterText = true;
                by.OptionGroup = "year";
                by.AutoCheckOnClick = true;
                by.AutoCollapseOnClick = false;
                by.Click += YearClick;
                cont.SubItems.Add(by);
            }
            _YearsContainer = cont;
            CenturyYearStart = (DateTime.Today.Year / 10) * 10;
        }

        private void YearClick(object sender, EventArgs e)
        {
            ButtonItem button = (ButtonItem)sender;
            int year = int.Parse(button.Text);
            this.SelectedYear = year;
            OnSelectedYearChanged(EventArgs.Empty);
        }

        public event EventHandler SelectedYearChanged;
        /// <summary>
        /// Raises SelectedYearChanged event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnSelectedYearChanged(EventArgs e)
        {
            EventHandler handler = SelectedYearChanged;
            if (handler != null)
                handler(this, e);
        }

        private Color _TextColor = Color.Empty;
        public Color TextColor
        {
            get { return _TextColor; }
            set { _TextColor = value; OnTextColorChanged(); }
        }
        private void OnTextColorChanged()
        {
            SetTextColor(this.SubItems);
        }

        private void SetTextColor(SubItemsCollection subItems)
        {
            foreach (BaseItem item in subItems)
            {
                if (item is ButtonItem)
                {
                    ((ButtonItem)item).ForeColor = _TextColor;
                    ((ButtonItem)item).HotForeColor = _TextColor;
                }
                else if (item is LabelItem)
                    ((LabelItem)item).ForeColor = _TextColor;
                else if (item is ItemContainer)
                    SetTextColor(item.SubItems);
            }
        }

        private int _CenturyYearStart;
        public int CenturyYearStart
        {
            get { return _CenturyYearStart; }
            set { _CenturyYearStart = value; UpdateYears(); }
        }
        private int _SelectedYear;
        public int SelectedYear
        {
            get { return _SelectedYear; }
            set
            {
                _SelectedYear = value;
                foreach (BaseItem item in _YearsContainer.SubItems)
                {
                    if (item.Name.StartsWith("yearSelector"))
                    {
                        ButtonItem by = (ButtonItem)item;
                        if (by.Text == _SelectedYear.ToString())
                        {
                            by.Checked = true;
                            break;
                        }
                        else if (by.Checked)
                            by.Checked = false;
                    }
                }
            }
        }
        private void UpdateYears()
        {
            // Assumes this indicates century start
            int y = _CenturyYearStart - 1;
            foreach (BaseItem item in _YearsContainer.SubItems)
            {
                if (item.Name.StartsWith("yearSelector"))
                {
                    ButtonItem by = (ButtonItem)item;
                    by.Text = y.ToString();
                    if (y == _SelectedYear)
                        by.Checked = true;
                    else if (by.Checked)
                        by.Checked = false;
                    y++;
                }
            }
            _CenturyLabel.Text = string.Format("{0}-{1}", _CenturyYearStart, _CenturyYearStart + 10);
            NeedRecalcSize = true;
            this.Refresh();
        }
        protected override void OnFixedSizeChanged(Size oldValue, Size newValue)
        {
            base.OnFixedSizeChanged(oldValue, newValue);
            // Update buttons size
            Size size = newValue;
            size.Height -= _FixedButtonSize.Height + 4;
            size.Width -= 4;
            Size sb = new Size(size.Width / 4, size.Height / 3);
            //sb.Width = (int)(sb.Width / Dpi.Factor.Width);
            //sb.Height = (int)(sb.Height / Dpi.Factor.Height);
            foreach (BaseItem item in _YearsContainer.SubItems)
            {
                if (item.Name.StartsWith("yearSelector"))
                {
                    ButtonItem by = (ButtonItem)item;
                    by.FixedSize = sb;
                }
            }
        }
        private void YearsDecreaseClick(object sender, EventArgs e)
        {
            if (CenturyYearStart - 10 > 1760)
                CenturyYearStart -= 10;
        }
        private void YearsIncreaseClick(object sender, EventArgs e)
        {
            CenturyYearStart += 10;
        }
        private CenturySelectorControl _CenturySelector = null;
        private void ShowCenturySelector(object sender, EventArgs e)
        {
            if (_CenturySelector != null) return;
            _CenturySelector = new CenturySelectorControl();
            Control parent = (Control)this.ContainerControl;

            _CenturySelector.Size = new Size(8, 8);
            _CenturySelector.Location = new Point(this.LeftInternal + (this.WidthInternal - 8) / 2, this.TopInternal + (this.HeightInternal - 8) / 2);
            _CenturySelector.Style = eDotNetBarStyle.StyleManagerControlled;
            if (BarFunctions.IsOffice2007Style(this.Style))
            {
                Office2007ColorTable table = ((Office2007Renderer)GlobalManager.Renderer).ColorTable;
                ElementStyle es = (ElementStyle)table.StyleClasses[ElementStyleClassKeys.ItemPanelKey];
                _CenturySelector.BackgroundStyle.BackColor = es.BackColor;
                _CenturySelector.BackgroundStyle.BackColor2 = es.BackColor2;
            }
            else
                _CenturySelector.BackgroundStyle.BackColor = SystemColors.Window;
            _CenturySelector.CenturySelector.TextColor = this.TextColor;
            parent.Controls.Add(_CenturySelector);

            Rectangle targetBounds = this.Bounds;
            DevComponents.DotNetBar.Animation.AnimationRectangle anim = new DevComponents.DotNetBar.Animation.AnimationRectangle(
                new AnimationRequest(_CenturySelector, "Bounds", targetBounds),
                DevComponents.DotNetBar.Animation.AnimationEasing.EaseInOutQuad, 200);
            anim.AutoDispose = true;
            anim.Start();

            _CenturySelector.CenturySelector.SelectedCenturyChanged += SelectedCenturyChanged;
        }
        private void SelectedCenturyChanged(object sender, EventArgs e)
        {
            this.CenturyYearStart = _CenturySelector.CenturySelector.SelectedCentury;
            Rectangle targetBounds = this.Bounds;
            DevComponents.DotNetBar.Animation.AnimationRectangle anim = new DevComponents.DotNetBar.Animation.AnimationRectangle(
                new AnimationRequest(_CenturySelector, "Bounds", new Rectangle(this.LeftInternal + (this.WidthInternal - 8) / 2, this.TopInternal + (this.HeightInternal - 8) / 2, 8, 8)),
                DevComponents.DotNetBar.Animation.AnimationEasing.EaseInOutQuad, 200);
            anim.AutoDispose = true;
            anim.AnimationCompleted += new EventHandler(AnimationCompleted);
            anim.Start();
        }
        public void RemoveCenturySelector()
        {
            if (_CenturySelector == null) return;
            Control parent = (Control)this.ContainerControl;
            parent.Controls.Remove(_CenturySelector);
            _CenturySelector.CenturySelector.SelectedCenturyChanged -= SelectedCenturyChanged;
            _CenturySelector.Dispose();
            _CenturySelector = null;
        }
        void AnimationCompleted(object sender, EventArgs e)
        {
            RemoveCenturySelector();
        }

    }
    #endregion

    #region Century Selector
    internal class CenturySelectorContainer : ItemContainer
    {
        private Size _FixedButtonSize = new Size(13, 18);
        private ItemContainer _YearsContainer = null;
        private LabelItem _CenturyLabel = null;
        /// <summary>
        /// Initializes a new instance of the YearSelectorContainer class.
        /// </summary>
        public CenturySelectorContainer()
        {
            this.LayoutOrientation = eOrientation.Vertical;

            // Add navigation container
            ItemContainer cont = new ItemContainer();
            cont.AutoCollapseOnClick = false;
            cont.MinimumSize = new Size(0, _FixedButtonSize.Height + 3);
            cont.LayoutOrientation = eOrientation.Horizontal;
            cont.HorizontalItemAlignment = eHorizontalItemsAlignment.Center;
            cont.VerticalItemAlignment = eVerticalItemsAlignment.Middle;
            cont.ItemSpacing = 2;
            ButtonItem nav = new ButtonItem("NavDecreaseYears");
            nav.GlobalItem = false;
            nav._FadeEnabled = false;
            nav.Text = "<expand direction=\"left\"/>";
            nav.Click += YearsDecreaseClick;
            nav.AutoCollapseOnClick = false;
            nav.FixedSize = _FixedButtonSize;
            nav._FixedSizeCenterText = true;
            nav.ClickAutoRepeat = true;
            cont.SubItems.Add(nav);
            LabelItem label = new LabelItem("CenturyLabel");
            label.AutoCollapseOnClick = false;
            label.GlobalItem = false;
            label.TextAlignment = StringAlignment.Center;
            label.TextLineAlignment = StringAlignment.Center;
            label.PaddingBottom = 2;
            label.Text = "2000-2099";
            _CenturyLabel = label;
            cont.SubItems.Add(label);
            nav = new ButtonItem("NavIncreaseYears");
            nav.GlobalItem = false;
            nav._FadeEnabled = false;
            nav.Text = "<expand direction=\"right\"/>";
            nav.Click += YearsIncreaseClick;
            nav.AutoCollapseOnClick = false;
            nav.FixedSize = _FixedButtonSize;
            nav._FixedSizeCenterText = true;
            nav.ClickAutoRepeat = true;
            cont.SubItems.Add(nav);
            this.SubItems.Add(cont);

            cont = new ItemContainer();
            cont.LayoutOrientation = eOrientation.Horizontal;
            cont.MultiLine = true;
            this.SubItems.Add(cont);
            for (int i = 0; i < 12; i++)
            {
                ButtonItem by = new ButtonItem("centurySelector" + i.ToString());
                by.Text = (2009 + i).ToString();
                by.FixedSize = new Size(54, 35);
                by._FixedSizeCenterText = true;
                by.OptionGroup = "century";
                by.AutoCheckOnClick = true;
                by.Click += CenturyClick;
                cont.SubItems.Add(by);
            }
            _YearsContainer = cont;
            CenturyYearStart = (DateTime.Today.Year / 100) * 100;
        }

        private void CenturyClick(object sender, EventArgs e)
        {
            ButtonItem button = (ButtonItem)sender;
            int year = (int)button.Tag;
            this.SelectedCentury = year;
        }

        public event EventHandler SelectedCenturyChanged;
        /// <summary>
        /// Raises SelectedYearChanged event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnSelectedCenturyChanged(EventArgs e)
        {
            EventHandler handler = SelectedCenturyChanged;
            if (handler != null)
                handler(this, e);
        }

        private Color _TextColor = Color.Empty;
        public Color TextColor
        {
            get { return _TextColor; }
            set { _TextColor = value; OnTextColorChanged(); }
        }
        private void OnTextColorChanged()
        {
            SetTextColor(this.SubItems);
        }

        private void SetTextColor(SubItemsCollection subItems)
        {
            foreach (BaseItem item in subItems)
            {
                if (item is ButtonItem)
                {
                    ((ButtonItem)item).ForeColor = _TextColor;
                    ((ButtonItem)item).HotForeColor = _TextColor;
                }
                else if (item is LabelItem)
                    ((LabelItem)item).ForeColor = _TextColor;
                else if (item is ItemContainer)
                    SetTextColor(item.SubItems);
            }
        }

        private int _CenturyYearStart;
        public int CenturyYearStart
        {
            get { return _CenturyYearStart; }
            set { _CenturyYearStart = value; UpdateYears(); }
        }
        private int _SelectedYear;
        public int SelectedCentury
        {
            get { return _SelectedYear; }
            set
            {
                _SelectedYear = value;
                foreach (BaseItem item in _YearsContainer.SubItems)
                {
                    if (item.Name.StartsWith("centurySelector"))
                    {
                        ButtonItem by = (ButtonItem)item;
                        if ((int)by.Tag == _SelectedYear)
                        {
                            by.Checked = true;
                            break;
                        }
                    }
                }
                OnSelectedCenturyChanged(EventArgs.Empty);
            }
        }
        private void UpdateYears()
        {
            // Assumes this indicates century start
            int y = _CenturyYearStart - 10;
            foreach (BaseItem item in _YearsContainer.SubItems)
            {
                if (item.Name.StartsWith("centurySelector"))
                {
                    ButtonItem by = (ButtonItem)item;
                    by.Text = string.Format("<div>{0}-<br/>{1}</div>", y, y + 9);
                    by.Tag = y;
                    if (y == _SelectedYear)
                        by.Checked = true;
                    else if (by.Checked)
                        by.Checked = false;
                    y += 10;
                }
            }
            _CenturyLabel.Text = string.Format("{0}-{1}", _CenturyYearStart, _CenturyYearStart + 99);
            NeedRecalcSize = true;
            this.Refresh();
        }
        protected override void OnFixedSizeChanged(Size oldValue, Size newValue)
        {
            base.OnFixedSizeChanged(oldValue, newValue);
            // Update buttons size
            Size size = newValue;
            size.Height -= _FixedButtonSize.Height + 4;
            size.Width -= 4;
            Size sb = new Size(size.Width / 4, size.Height / 3);
            //sb.Width = (int)(sb.Width / Dpi.Factor.Width);
            //sb.Height = (int)(sb.Height / Dpi.Factor.Height);
            foreach (BaseItem item in _YearsContainer.SubItems)
            {
                if (item.Name.StartsWith("centurySelector"))
                {
                    ButtonItem by = (ButtonItem)item;
                    by.FixedSize = sb;
                }
            }
        }
        private void YearsDecreaseClick(object sender, EventArgs e)
        {
            if (CenturyYearStart - 100 > 1760)
                CenturyYearStart -= 100;
        }
        private void YearsIncreaseClick(object sender, EventArgs e)
        {
            CenturyYearStart += 100;
        }
    }
    #endregion

    #region MonthSelector
    internal class MonthSelectorControl : BaseItemControl
    {
        /// <summary>
        /// Initializes a new instance of the CenturySelectorControl class.
        /// </summary>
        public MonthSelectorControl()
        {
            _MonthSelector = new MonthSelectorContainer();
            this.HostItem = _MonthSelector;
            this.FocusCuesEnabled = false;
        }

        protected override void OnResize(EventArgs e)
        {
            _MonthSelector.FixedSize = Dpi.Descale(this.Size);
            base.OnResize(e);
        }

        private MonthSelectorContainer _MonthSelector;
        public MonthSelectorContainer MonthSelector
        {
            get { return _MonthSelector; }
        }
    }
    internal class MonthSelectorContainer : ItemContainer
    {
        private const string STR_MonthSelector = "monthSelector";
        private Size _FixedButtonSize = new Size(13, 18);
        private ItemContainer _MonthsContainer = null;
        /// <summary>
        /// Initializes a new instance of the YearSelectorContainer class.
        /// </summary>
        public MonthSelectorContainer()
        {
            this.LayoutOrientation = eOrientation.Vertical;

            // Add navigation container
            ItemContainer cont = new ItemContainer();
            cont.LayoutOrientation = eOrientation.Horizontal;
            cont.MultiLine = true;
            cont.AutoCollapseOnClick = false;
            this.SubItems.Add(cont);

            for (int i = 1; i <= 12; i++)
            {
                ButtonItem by = new ButtonItem(STR_MonthSelector + i.ToString());
                by.Text = i.ToString();
                by.Tag = i;
                by.FixedSize = new Size(54, 35);
                by._FixedSizeCenterText = true;
                by.OptionGroup = "month";
                by.AutoCheckOnClick = true;
                by.Click += MonthClick;
                by.AutoCollapseOnClick = false;
                cont.SubItems.Add(by);
            }
            _MonthsContainer = cont;
        }

        public bool IsEmpty
        {
            get
            {
                foreach (BaseItem item in _MonthsContainer.SubItems)
                {
                    if(item is ButtonItem && item.Name.StartsWith(STR_MonthSelector) && ((ButtonItem)item).Checked)
                        return false;
                }
                return true;
            }
            set
            {
                if (value)
                {
                    _SelectedMonth = 1;
                    foreach (BaseItem item in _MonthsContainer.SubItems)
                    {
                        if (item is ButtonItem && item.Name.StartsWith(STR_MonthSelector) && ((ButtonItem)item).Checked)
                            ((ButtonItem)item).Checked = false;
                    }
                }
            }
        }

        private void MonthClick(object sender, EventArgs e)
        {
            ButtonItem button = (ButtonItem)sender;
            int month = (int)button.Tag;
            this.SelectedMonth = month;
            OnSelectedMonthChanged(EventArgs.Empty);
        }

        public event EventHandler SelectedMonthChanged;
        /// <summary>
        /// Raises SelectedYearChanged event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        protected virtual void OnSelectedMonthChanged(EventArgs e)
        {
            EventHandler handler = SelectedMonthChanged;
            if (handler != null)
                handler(this, e);
        }

        private Color _TextColor = Color.Empty;
        public Color TextColor
        {
            get { return _TextColor; }
            set { _TextColor = value; OnTextColorChanged(); }
        }
        private void OnTextColorChanged()
        {
            SetTextColor(this.SubItems);
        }

        private void SetTextColor(SubItemsCollection subItems)
        {
            foreach (BaseItem item in subItems)
            {
                if (item is ButtonItem)
                {
                    ((ButtonItem)item).ForeColor = _TextColor;
                    ((ButtonItem)item).HotForeColor = _TextColor;
                }
                else if (item is LabelItem)
                    ((LabelItem)item).ForeColor = _TextColor;
                else if (item is ItemContainer)
                    SetTextColor(item.SubItems);
            }
        }

        private int _SelectedMonth = 1;
        public int SelectedMonth
        {
            get { return _SelectedMonth; }
            set
            {
                if (value <= 0) value = 1;
                if (value > 12) value = 12;
                _SelectedMonth = value;

                ButtonItem month = (ButtonItem)_MonthsContainer.SubItems[STR_MonthSelector + _SelectedMonth.ToString()];
                if (month != null)
                {
                    month.Checked = true;
                }
            }
        }

        private bool _UseMonthNames = false;
        public bool UseMonthNames
        {
            get { return _UseMonthNames; }
            set
            {
                _UseMonthNames = value;
                UpdateMonthText();
            }
        }

        private void UpdateMonthText()
        {
            string[] monthNames = new string[0];
            if (_UseMonthNames)
            {
                CultureInfo ci = DateTimeInput.GetActiveCulture();
                monthNames = ci.DateTimeFormat.AbbreviatedMonthNames;
            }
            foreach (BaseItem item in _MonthsContainer.SubItems)
            {
                if (item.Name.StartsWith(STR_MonthSelector))
                {
                    ButtonItem by = (ButtonItem)item;
                    int month = (int)by.Tag;
                    if (_UseMonthNames)
                    {
                        by.Text = monthNames[month - 1];
                    }
                    else
                    {
                        by.Text = month.ToString();
                    }
                    //if (month == _SelectedMonth)
                    //    by.Checked = true;
                    //else if (by.Checked)
                    //    by.Checked = false;
                }
            }
            NeedRecalcSize = true;
            this.Refresh();
        }
        protected override void OnFixedSizeChanged(Size oldValue, Size newValue)
        {
            base.OnFixedSizeChanged(oldValue, newValue);
            // Update buttons size
            Size size = newValue;
            size.Height -= _FixedButtonSize.Height + 4;
            size.Width -= 4;
            Size sb = new Size(size.Width / 4, size.Height / 3);
            //sb.Width = (int)(sb.Width/Dpi.Factor.Width);
            //sb.Height = (int)(sb.Height/Dpi.Factor.Height);
            foreach (BaseItem item in _MonthsContainer.SubItems)
            {
                if (item.Name.StartsWith(STR_MonthSelector))
                {
                    ButtonItem by = (ButtonItem)item;
                    by.FixedSize = sb;
                }
            }
        }
    }
    #endregion

    internal enum eCurrentSingleMonthCalendarOperation
    {
        None,
        AnimatingShowYearSelector,
        AnimatingHideYearSelector,
        AnimatingShowMonthSelector,
        AnimatingHideMonthSelector
    }
}
 
