using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar.ScrollBar;
using System.Drawing;
using System.Runtime.InteropServices;

namespace DevComponents.DotNetBar.Controls
{
    internal class ScrollbarSkinner : IDisposable
    {
        #region Constructor
        private const int ScrollPositionUpdateDelay = 400;
        private HScrollBarAdv _HScrollBar = null;
        private VScrollBarAdv _VScrollBar = null;
        private Control _Thumb = null;
        private Control _Parent = null;
        public ScrollbarSkinner(Control parent)
        {
            if(parent == null)
                throw new ArgumentNullException("Parent cannot be null");
            if (!(parent is IScrollBarOverrideSupport))
                throw new ArgumentException("parent must implement IScrollBarOverrideSupport interface.");

            _Parent = parent;

            _HScrollBar = new HScrollBarAdv();
            _HScrollBar.Visible = false;
            _HScrollBar.Appearance = _ScrollBarAppearance;
            _HScrollBar.Scroll += HScrollBarScroll;
            _VScrollBar = new VScrollBarAdv();
            _VScrollBar.Visible = false;
            _VScrollBar.Appearance = _ScrollBarAppearance;
            _VScrollBar.Scroll += VScrollBarScroll;
            _Thumb = new Control();
            _Thumb.Visible = false;
            _Thumb.BackColor = SystemColors.Window;

            if (_Parent.Parent != null)
            {
                AttachScrollbars();
            }
            WireParent();

            if(_Parent.IsHandleCreated)
                UpdateScrollBars();
        }

        #endregion

        #region Implementation

        private void WireParent()
        {
            _Parent.ParentChanged += ParentParentChanged;
            _Parent.HandleCreated += ParentHandleCreated;
            _Parent.Resize += ParentResize;
            IScrollBarOverrideSupport sbo = (IScrollBarOverrideSupport)_Parent;
            sbo.NonClientSizeChanged += NonClientSizeChanged;
            sbo.ScrollBarValueChanged += ControlScrollBarValueChanged;
        }

        void ParentResize(object sender, EventArgs e)
        {
            UpdateScrollBars();
        }

        void ParentHandleCreated(object sender, EventArgs e)
        {
            UpdateScrollBars();
        }
        private void NonClientSizeChanged(object sender, EventArgs e)
        {
            UpdateScrollBars();
        }
        private void ControlScrollBarValueChanged(object sender, ScrollValueChangedEventArgs e)
        {
            if ((e.ScrollChange & eScrollBarScrollChange.Vertical) == eScrollBarScrollChange.Vertical)
            {
                UpdateVerticalScrollBarValues();
            }
            if ((e.ScrollChange & eScrollBarScrollChange.MouseWheel) == eScrollBarScrollChange.MouseWheel)
            {
                if (_VScrollBar.Visible)
                {
                    UpdateVerticalScrollBarValues();
                    InvokeDelayed(new MethodInvoker(delegate { UpdateVerticalScrollBarValues(); }), ScrollPositionUpdateDelay);
                }
                else if (_HScrollBar.Visible)
                {
                    UpdateHorizontalScrollBarValues();
                    InvokeDelayed(new MethodInvoker(delegate { UpdateHorizontalScrollBarValues(); }), ScrollPositionUpdateDelay);
                }
            }
            if ((e.ScrollChange & eScrollBarScrollChange.Horizontal) == eScrollBarScrollChange.Horizontal)
            {
                UpdateHorizontalScrollBarValues();
            }
            UpdateScrollBars();
        }
        private void UnwireParent()
        {
            _Parent.ParentChanged -= ParentParentChanged;
            _Parent.HandleCreated -= ParentHandleCreated;
            _Parent.Resize -= ParentResize;
            IScrollBarOverrideSupport sbo = (IScrollBarOverrideSupport)_Parent;
            sbo.NonClientSizeChanged -= NonClientSizeChanged;
            sbo.ScrollBarValueChanged -= ControlScrollBarValueChanged;
        }

        void ParentParentChanged(object sender, EventArgs e)
        {
            AttachScrollbars();
            UpdateScrollBars();
        }

        private bool _UpdatingScrollbars = false;
        protected virtual void UpdateScrollBarsDelayed()
        {
            InvokeDelayed(new MethodInvoker(delegate { UpdateScrollBars(); }), ScrollPositionUpdateDelay);
        }
        protected virtual void UpdateScrollBars()
        {
            if (_Parent == null || _Parent.Parent == null || !_Parent.IsHandleCreated || _UpdatingScrollbars) return;
            _UpdatingScrollbars = true;
            try
            {
                Control scrollOverrideControl = _Parent;
                if (scrollOverrideControl == null || ((IScrollBarOverrideSupport)scrollOverrideControl).DesignMode)
                {
                    if (_HScrollBar.Visible) _HScrollBar.Visible = false;
                    if (_VScrollBar.Visible) _VScrollBar.Visible = false;
                    return;
                }
                WinApi.SCROLLBARINFO psbi = new WinApi.SCROLLBARINFO();
                psbi.cbSize = Marshal.SizeOf(psbi);
                WinApi.GetScrollBarInfo(scrollOverrideControl.Handle, (uint)WinApi.eObjectId.OBJID_VSCROLL, ref psbi);
                if (psbi.rgstate[0] != (int)WinApi.eStateFlags.STATE_SYSTEM_INVISIBLE)
                {
                    Rectangle vsBounds = psbi.rcScrollBar.ToRectangle();
                    vsBounds.Location = scrollOverrideControl.Parent.PointToClient(vsBounds.Location);
                    Rectangle scrollCountrolBounds = scrollOverrideControl.Bounds;
                    if (!scrollCountrolBounds.Contains(vsBounds))
                    {
                        // We need to guess bounds for best performance and appearance
                        if (vsBounds.Right > scrollCountrolBounds.Right)
                        {
                            vsBounds.X = scrollCountrolBounds.Right - vsBounds.Width;
                        }
                        else if (vsBounds.X < scrollCountrolBounds.X)
                        {
                            vsBounds.X = 0;
                        }
                    }
                    if (_VScrollBar.Bounds != vsBounds)
                    {
                        _VScrollBar.Bounds = vsBounds;
                        UpdateVerticalScrollBarValues();
                    }
                    if (!_VScrollBar.Visible)
                    {
                        _VScrollBar.Visible = true;
                        _VScrollBar.BringToFront();
                        _VScrollBar.Refresh();
                        InvokeDelayed(new MethodInvoker(delegate { UpdateVerticalScrollBarValues(); }), ScrollPositionUpdateDelay);
                    }
                    else
                    {
                        _VScrollBar.BringToFront();
                    }

                    if (psbi.rgstate[0] == (int)WinApi.eStateFlags.STATE_SYSTEM_UNAVAILABLE)
                        _VScrollBar.Enabled = false;
                    else if (!_VScrollBar.Enabled)
                        _VScrollBar.Enabled = true;
                    //Console.WriteLine("VscrollBar Bounds detection {0}", vsBounds);
                }
                else if (_VScrollBar.Visible)
                    _VScrollBar.Visible = false;

                psbi = new WinApi.SCROLLBARINFO();
                psbi.cbSize = Marshal.SizeOf(psbi);
                WinApi.GetScrollBarInfo(scrollOverrideControl.Handle, (uint)WinApi.eObjectId.OBJID_HSCROLL, ref psbi);
                if (psbi.rgstate[0] != (int)WinApi.eStateFlags.STATE_SYSTEM_INVISIBLE)
                {
                    Rectangle hsBounds = psbi.rcScrollBar.ToRectangle();
                    hsBounds.Location = scrollOverrideControl.Parent.PointToClient(hsBounds.Location);
                    Rectangle scrollCountrolBounds = scrollOverrideControl.Bounds;
                    if (!scrollCountrolBounds.Contains(hsBounds))
                    {
                        // We need to guess bounds for best performance and appearance
                        if (hsBounds.Bottom > scrollCountrolBounds.Bottom)
                        {
                            hsBounds.Y = scrollCountrolBounds.Bottom - hsBounds.Height;
                        }
                    }
                    if (_VScrollBar.Visible && hsBounds.Width == scrollCountrolBounds.Width)
                        hsBounds.Width -= _VScrollBar.Width;
                    if (_HScrollBar.Bounds != hsBounds)
                    {
                        _HScrollBar.Bounds = hsBounds;
                        UpdateHorizontalScrollBarValues();
                    }
                    if (!_HScrollBar.Visible)
                    {
                        _HScrollBar.Visible = true;
                        _HScrollBar.BringToFront();
                        _HScrollBar.Refresh();
                        InvokeDelayed(new MethodInvoker(delegate { UpdateHorizontalScrollBarValues(); }), ScrollPositionUpdateDelay);
                    }
                    else
                    {
                        _HScrollBar.BringToFront();
                    }

                    if (psbi.rgstate[0] == (int)WinApi.eStateFlags.STATE_SYSTEM_UNAVAILABLE)
                        _HScrollBar.Enabled = false;
                    else if (!_HScrollBar.Enabled)
                        _HScrollBar.Enabled = true;
                }
                else if (_HScrollBar.Visible)
                    _HScrollBar.Visible = false;

                if (_HScrollBar.Visible && _VScrollBar.Visible)
                {
                    _Thumb.Bounds = new Rectangle(_VScrollBar.Left, _VScrollBar.Bounds.Bottom, _VScrollBar.Width, _HScrollBar.Height);
                    _Thumb.Visible = true;
                    _Thumb.BringToFront();
                }
                else
                {
                    _Thumb.Visible = false;
                }
            }
            finally
            {
                _UpdatingScrollbars = false;
            }
        }
        private void AttachScrollbars()
        {
            DetachScrollbars();

            Control host = _Parent.Parent;
            if (host != null)
            {
                if (host is TableLayoutPanel)
                {
                    while (host is TableLayoutPanel)
                    {
                        host = host.Parent;
                    }
                }
                if (host != null)
                {
                    host.Controls.Add(_VScrollBar);
                    host.Controls.Add(_HScrollBar);
                    host.Controls.Add(_Thumb);
                }
            }
        }

        private void DetachScrollbars()
        {
            if (_VScrollBar.Parent != null)
                _VScrollBar.Parent.Controls.Remove(_VScrollBar);
            if (_HScrollBar.Parent != null)
                _HScrollBar.Parent.Controls.Remove(_HScrollBar);
            if (_Thumb.Parent != null)
                _Thumb.Parent.Controls.Remove(_Thumb);

            _VScrollBar.Visible = false;
            _HScrollBar.Visible = false;
            _Thumb.Visible = false;
        }

        private eScrollBarAppearance _ScrollBarAppearance = eScrollBarAppearance.Default;
        /// <summary>
        /// Gets or sets the scroll-bar visual style.
        /// </summary>
        [DefaultValue(eScrollBarAppearance.Default), Category("Appearance"), Description("Gets or sets the scroll-bar visual style.")]
        public eScrollBarAppearance ScrollBarAppearance
        {
            get { return _ScrollBarAppearance; }
            set
            {
                _ScrollBarAppearance = value;
                OnScrollBarAppearanceChanged();
            }
        }
        private void OnScrollBarAppearanceChanged()
        {
            if (_VScrollBar != null) _VScrollBar.Appearance = _ScrollBarAppearance;
            if (_HScrollBar != null) _HScrollBar.Appearance = _ScrollBarAppearance;
        }

        private bool _InternalVScrollPositionUpdated = false;
        private void VScrollBarScroll(object sender, ScrollEventArgs e)
        {
            if (e.NewValue == e.OldValue && e.Type != ScrollEventType.EndScroll) return;
            _InternalVScrollPositionUpdated = true;
            //Console.WriteLine("{0} Setting Sys_VScrollBar value {1}, {2}", DateTime.Now, e.NewValue, e.Type);
            if (e.Type == ScrollEventType.ThumbTrack) // We need to send this becouse ScrollableControl internally will get the current scroll position using GetScrollInfo instead from window message LParam
            {
                WinApi.SCROLLINFO si = new WinApi.SCROLLINFO();
                si.nTrackPos = e.NewValue;
                si.nPos = e.NewValue;
                si.fMask = WinApi.ScrollInfoMask.SIF_POS;
                si.cbSize = Marshal.SizeOf(si);
                WinApi.SetScrollInfo(_Parent.Handle, WinApi.SBOrientation.SB_VERT, ref si, false);
            }
            WinApi.SendMessage(_Parent.Handle, (int)WinApi.WindowsMessages.WM_VSCROLL, WinApi.CreateLParam(MapVScrollType(e.Type), e.NewValue), IntPtr.Zero);
            _InternalVScrollPositionUpdated = false;
            
            if (e.Type == ScrollEventType.EndScroll)
                UpdateVerticalScrollBarValues();
        }
        private bool _InternalHScrollPositionUpdated = false;
        private void HScrollBarScroll(object sender, ScrollEventArgs e)
        {
            if (e.NewValue == e.OldValue && e.Type != ScrollEventType.EndScroll) return;
            _InternalHScrollPositionUpdated = true;
            //Console.WriteLine("{0} Setting Sys_HScrollBar value {1}, {2}", DateTime.Now, e.NewValue, e.Type);
            if (e.Type == ScrollEventType.ThumbTrack) // We need to send this becouse ScrollableControl internally will get the current scroll position using GetScrollInfo instead from window message LParam
            {
                WinApi.SCROLLINFO si = new WinApi.SCROLLINFO();
                si.nTrackPos = e.NewValue;
                si.nPos = e.NewValue;
                si.fMask = WinApi.ScrollInfoMask.SIF_POS;
                si.cbSize = Marshal.SizeOf(si);
                WinApi.SetScrollInfo(_Parent.Handle, WinApi.SBOrientation.SB_HORZ, ref si, false);
            }
            WinApi.SendMessage(_Parent.Handle, (int)WinApi.WindowsMessages.WM_HSCROLL, WinApi.CreateLParam(MapHScrollType(e.Type), e.NewValue), IntPtr.Zero);
            _InternalHScrollPositionUpdated = false;

            if (e.Type == ScrollEventType.EndScroll)
                UpdateHorizontalScrollBarValues();
        }
        private static int MapVScrollType(ScrollEventType type)
        {
            if (type == ScrollEventType.EndScroll)
                return (int)WinApi.ScrollBarCommands.SB_ENDSCROLL;
            else if (type == ScrollEventType.First)
                return (int)WinApi.ScrollBarCommands.SB_TOP;
            else if (type == ScrollEventType.LargeDecrement)
                return (int)WinApi.ScrollBarCommands.SB_PAGEUP;
            else if (type == ScrollEventType.LargeIncrement)
                return (int)WinApi.ScrollBarCommands.SB_PAGEDOWN;
            else if (type == ScrollEventType.Last)
                return (int)WinApi.ScrollBarCommands.SB_BOTTOM;
            else if (type == ScrollEventType.SmallDecrement)
                return (int)WinApi.ScrollBarCommands.SB_LINEUP;
            else if (type == ScrollEventType.SmallIncrement)
                return (int)WinApi.ScrollBarCommands.SB_LINEDOWN;
            else if (type == ScrollEventType.ThumbPosition)
                return (int)WinApi.ScrollBarCommands.SB_THUMBPOSITION;
            else if (type == ScrollEventType.ThumbTrack)
                return (int)WinApi.ScrollBarCommands.SB_THUMBTRACK;

            return 0;
        }
        private static int MapHScrollType(ScrollEventType type)
        {
            if (type == ScrollEventType.EndScroll)
                return (int)WinApi.ScrollBarCommands.SB_ENDSCROLL;
            else if (type == ScrollEventType.First)
                return (int)WinApi.ScrollBarCommands.SB_LEFT;
            else if (type == ScrollEventType.LargeDecrement)
                return (int)WinApi.ScrollBarCommands.SB_PAGELEFT;
            else if (type == ScrollEventType.LargeIncrement)
                return (int)WinApi.ScrollBarCommands.SB_PAGERIGHT;
            else if (type == ScrollEventType.Last)
                return (int)WinApi.ScrollBarCommands.SB_RIGHT;
            else if (type == ScrollEventType.SmallDecrement)
                return (int)WinApi.ScrollBarCommands.SB_LINELEFT;
            else if (type == ScrollEventType.SmallIncrement)
                return (int)WinApi.ScrollBarCommands.SB_LINERIGHT;
            else if (type == ScrollEventType.ThumbPosition)
                return (int)WinApi.ScrollBarCommands.SB_THUMBPOSITION;
            else if (type == ScrollEventType.ThumbTrack)
                return (int)WinApi.ScrollBarCommands.SB_THUMBTRACK;

            return 0;
        }


        private void UpdateVerticalScrollBarValues()
        {
            if (_InternalVScrollPositionUpdated) return;
            WinApi.SCROLLINFO scrollInfo = new WinApi.SCROLLINFO();
            scrollInfo.cbSize = Marshal.SizeOf(scrollInfo);
            scrollInfo.fMask = WinApi.ScrollInfoMask.SIF_POS | WinApi.ScrollInfoMask.SIF_RANGE | WinApi.ScrollInfoMask.SIF_PAGE;
            if (WinApi.GetScrollInfo(_Parent.Handle, WinApi.SBOrientation.SB_VERT, ref scrollInfo))
            {
                //Console.WriteLine("{0}   scrollInfo={1}", DateTime.Now, scrollInfo.ToString());
                if (_VScrollBar.Minimum != scrollInfo.nMin)
                    _VScrollBar.Minimum = scrollInfo.nMin;
                if (_VScrollBar.Maximum != scrollInfo.nMax)
                    _VScrollBar.Maximum = scrollInfo.nMax;
                if (_VScrollBar.Value != scrollInfo.nPos)
                    _VScrollBar.Value = scrollInfo.nPos;
                if (_VScrollBar.LargeChange != scrollInfo.nPage)
                    _VScrollBar.LargeChange = scrollInfo.nPage;
            }
        }
        private void UpdateHorizontalScrollBarValues()
        {
            if (_InternalHScrollPositionUpdated) return;
            WinApi.SCROLLINFO scrollInfo = new WinApi.SCROLLINFO();
            scrollInfo.cbSize = Marshal.SizeOf(scrollInfo);
            scrollInfo.fMask = WinApi.ScrollInfoMask.SIF_POS | WinApi.ScrollInfoMask.SIF_RANGE | WinApi.ScrollInfoMask.SIF_PAGE | WinApi.ScrollInfoMask.SIF_TRACKPOS;
            if (WinApi.GetScrollInfo(_Parent.Handle, WinApi.SBOrientation.SB_HORZ, ref scrollInfo))
            {
                //Console.WriteLine("{0}   TRACKPOS={1}", DateTime.Now, scrollInfo);
                if (_HScrollBar.Minimum != scrollInfo.nMin)
                    _HScrollBar.Minimum = scrollInfo.nMin;
                if (_HScrollBar.Maximum != scrollInfo.nMax)
                    _HScrollBar.Maximum = scrollInfo.nMax;
                if (_HScrollBar.Value != scrollInfo.nPos)
                    _HScrollBar.Value = scrollInfo.nPos;
                if (_HScrollBar.LargeChange != scrollInfo.nPage)
                    _HScrollBar.LargeChange = scrollInfo.nPage;
            }
        }

        public VScrollBarAdv VScrollBar
        {
            get
            {
                return _VScrollBar;
            }
        }

        public HScrollBarAdv HScrollBar
        {
            get
            {
                return _HScrollBar;
            }
        }
        
        #endregion

        #region Invoke Delayed
        protected void InvokeDelayed(MethodInvoker method)
        {
            InvokeDelayed(method, 10);
        }
        protected void InvokeDelayed(MethodInvoker method, int delayInterval)
        {
            if (delayInterval <= 0) { method.Invoke(); return; }

            Timer delayedInvokeTimer = new Timer();
            delayedInvokeTimer = new Timer();
            delayedInvokeTimer.Tag = method;
            delayedInvokeTimer.Interval = delayInterval;
            delayedInvokeTimer.Tick += new EventHandler(DelayedInvokeTimerTick);
            delayedInvokeTimer.Start();
        }
        void DelayedInvokeTimerTick(object sender, EventArgs e)
        {
            Timer timer = (Timer)sender;
            MethodInvoker method = (MethodInvoker)timer.Tag;
            timer.Stop();
            timer.Dispose();
            if (!this.IsDisposed)
                method.Invoke();
        }
        #endregion

        private bool _IsDisposed = false;

        public bool IsDisposed
        {
            get
            {
                return _IsDisposed;
            }
        }
        public void Dispose()
        {
            UnwireParent();
            if(_VScrollBar.Parent == null)
                _VScrollBar.Dispose();
            if (_HScrollBar.Parent == null)
                _HScrollBar.Dispose();
            if (_Thumb.Parent == null)
                _Thumb.Dispose();
            _IsDisposed = true;
        }
    }
}
