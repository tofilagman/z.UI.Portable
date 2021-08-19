using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DevComponents.DotNetBar.Controls
{
    /// <summary>
    /// Displays the desktop alerts with optional image or symbol. Text on alerts supports text-markup.
    /// </summary>
    public static class DesktopAlert
    {
        /// <summary>
        /// Shows desktop alert. 
        /// </summary>
        /// <param name="text">Text to show on the alert. Text supports text-markup.</param>
        public static void Show(string text)
        {
            Show(text, _AlertColor, _AlertPosition, null);
        }

        /// <summary>
        /// Shows desktop alert. 
        /// </summary>
        /// <param name="text">Text to show on the alert. Text supports text-markup.</param>
        /// <param name="referenceControl">Specifies reference control which is used to find target screen alert is displayed on.</param>
        public static void Show(string text, Control referenceControl)
        {
            Show(text, _AlertColor, _AlertPosition, null, referenceControl);
        }

        /// <summary>
        /// Shows desktop alert.
        /// </summary>
        /// <param name="text">Text to show on the alert. Text supports text-markup.</param>
        /// <param name="markupLinkClickHandler">Text-markup link click event handler.</param>
        public static void Show(string text, MarkupLinkClickEventHandler markupLinkClickHandler)
        {
            Show(text, _AlertColor, _AlertPosition, markupLinkClickHandler);
        }

        /// <summary>
        /// Shows desktop alert.
        /// </summary>
        /// <param name="text">Text to show on the alert. Text supports text-markup.</param>
        /// <param name="alertColor">Specifies alert color.</param>
        public static void Show(string text, eDesktopAlertColor alertColor)
        {
            Show(text, alertColor, _AlertPosition, null);
        }

        /// <summary>
        /// Shows desktop alert.
        /// </summary>
        /// <param name="text">Text to show on the alert. Text supports text-markup.</param>
        /// <param name="alertColor">Specifies alert color.</param>
        /// <param name="referenceControl">Specifies reference control which is used to find target screen alert is displayed on.</param>
        public static void Show(string text, eDesktopAlertColor alertColor, Control referenceControl)
        {
            Show(text, alertColor, _AlertPosition, null, referenceControl);
        }

        /// <summary>
        /// Shows desktop alert at specific screen position.
        /// </summary>
        /// <param name="text">Text to show on the alert. Text supports text-markup.</param>
        /// <param name="position">Alert position on the screen.</param>
        public static void Show(string text, eAlertPosition position)
        {
            Show(text, _AlertColor, position, null);
        }

        /// <summary>
        /// Shows desktop alert at specific screen position.
        /// </summary>
        /// <param name="text">Text to show on the alert. Text supports text-markup.</param>
        /// <param name="position">Alert position on the screen.</param>
        /// <param name="referenceControl">Specifies reference control which is used to find target screen alert is displayed on.</param>
        public static void Show(string text, eAlertPosition position, Control referenceControl)
        {
            Show(text, _AlertColor, position, null, referenceControl);
        }

        /// <summary>
        /// Shows desktop alert at specific screen position.
        /// </summary>
        /// <param name="text">Text to show on the alert. Text supports text-markup.</param>
        /// <param name="image">Image to display on alert.</param>
        public static void Show(string text, Image image)
        {
            Show(text, image, _AlertColor, _AlertPosition, _AutoCloseTimeOut, 0, null);
        }

        /// <summary>
        /// Shows desktop alert.
        /// </summary>
        /// <param name="text">Text to show on the alert. Text supports text-markup.</param>
        /// <param name="alertColor">Alert color.</param>
        /// <param name="position">Alert position on the screen.</param>
        public static void Show(string text, eDesktopAlertColor alertColor, eAlertPosition position)
        {
            Show(text, alertColor, position, null);
        }

        /// <summary>
        /// Shows desktop alert.
        /// </summary>
        /// <param name="text">Text to show on the alert. Text supports text-markup.</param>
        /// <param name="alertColor">Alert color</param>
        /// <param name="position">Alert position on the screen</param>
        /// <param name="markupLinkClickHandler">Text-markup link click event handler.</param>
        /// <param name="referenceControl">Specifies reference control which is used to find target screen alert is displayed on.</param>
        public static void Show(string text, eDesktopAlertColor alertColor, eAlertPosition position, MarkupLinkClickEventHandler markupLinkClickHandler, Control referenceControl)
        {
            DesktopAlertWindow alert = new DesktopAlertWindow();
            alert.Text = text;
            alert.MaximumSize = Dpi.Size(_MaximumAlertSize);
            alert.AlertPosition = position;
            alert.AutoCloseTimeOut = _AutoCloseTimeOut;
            alert.TextMarkupEnabled = _TextMarkupEnabled;
            if (markupLinkClickHandler != null)
                alert.MarkupLinkClick += markupLinkClickHandler;
            alert.AlertAnimationDuration = _AlertAnimationDuration;
            alert.PlaySound = _PlaySound;
            alert.ReferenceControl = referenceControl;
            SetColors(alert, alertColor);
            alert.Show();
        }

        /// <summary>
        /// Shows desktop alert.
        /// </summary>
        /// <param name="text">Text to show on the alert. Text supports text-markup.</param>
        /// <param name="alertColor">Alert color</param>
        /// <param name="position">Alert position on the screen</param>
        /// <param name="markupLinkClickHandler">Text-markup link click event handler.</param>
        public static void Show(string text, eDesktopAlertColor alertColor, eAlertPosition position, MarkupLinkClickEventHandler markupLinkClickHandler)
        {
            Show(text, alertColor, position, markupLinkClickHandler, null);
        }

        /// <summary>
        /// Shows desktop alert.
        /// </summary>
        /// <param name="text">Text to show on the alert. Text supports text-markup.</param>
        /// <param name="alertId">Alert ID used to recognize alert if clicked and specified Action is called</param>
        /// <param name="alertClickAction">Action method to call if alert is clicked.</param>
        public static void Show(string text, long alertId, Action<long> alertClickAction)
        {
            Show(text, null, _AlertColor, _AlertPosition, _AutoCloseTimeOut, alertId, alertClickAction);
        }

        /// <summary>
        /// Shows desktop alert.
        /// </summary>
        /// <param name="text">Text to show on the alert. Text supports text-markup.</param>
        /// <param name="alertColor">Alert color</param>
        /// <param name="alertId">Alert ID used to recognize alert if clicked and specified Action is called</param>
        /// <param name="alertClickAction">Action method to call if alert is clicked.</param>
        public static void Show(string text, eDesktopAlertColor alertColor, long alertId, Action<long> alertClickAction)
        {
            Show(text, null, alertColor, _AlertPosition, _AutoCloseTimeOut, alertId, alertClickAction);
        }

        /// <summary>
        /// Shows desktop alert.
        /// </summary>
        /// <param name="text">Text to show on the alert. Text supports text-markup.</param>
        /// <param name="symbol">Symbol to show on the alert, see http://www.devcomponents.com/kb2/?p=1347 </param>
        /// <param name="symbolSet">Symbol set to use</param>
        /// <param name="symbolColor">Symbol color or Color.Empty to use default text color</param>
        /// <param name="alertColor">Alert color</param>
        /// <param name="position">Alert position on the screen</param>
        /// <param name="alertDurationSeconds">Duration of alert in the seconds.</param>
        /// <param name="alertId">Alert ID used to recognize alert if clicked and specified Action is called</param>
        /// <param name="alertClickAction">Action method to call if alert is clicked.</param>
        public static void Show(string text, string symbol, eSymbolSet symbolSet, Color symbolColor, eDesktopAlertColor alertColor, eAlertPosition position,
            int alertDurationSeconds, long alertId, Action<long> alertClickAction)
        {
            Show(text, symbol, symbolSet, symbolColor, alertColor,position, alertDurationSeconds, alertId, alertClickAction, null);
        }

        /// <summary>
        /// Shows desktop alert.
        /// </summary>
        /// <param name="text">Text to show on the alert. Text supports text-markup.</param>
        /// <param name="symbol">Symbol to show on the alert, see http://www.devcomponents.com/kb2/?p=1347 </param>
        /// <param name="symbolSet">Symbol set to use</param>
        /// <param name="symbolColor">Symbol color or Color.Empty to use default text color</param>
        /// <param name="position">Alert position on the screen</param>
        /// <param name="alertColor">Alert color</param>
        /// <param name="alertDurationSeconds">Duration of alert in the seconds.</param>
        /// <param name="alertId">Alert ID used to recognize alert if clicked and specified Action is called</param>
        /// <param name="alertClickAction">Action method to call if alert is clicked.</param>
        /// <param name="markupLinkClickHandler">Text-markup link click event handler.</param>
        public static void Show(string text, string symbol, eSymbolSet symbolSet, Color symbolColor, eDesktopAlertColor alertColor, eAlertPosition position,
            int alertDurationSeconds, long alertId, Action<long> alertClickAction, MarkupLinkClickEventHandler markupLinkClickHandler)
        {
            Show(text, symbol, symbolSet, symbolColor, alertColor, position, alertDurationSeconds, alertId, alertClickAction, markupLinkClickHandler, null);
        }

        /// <summary>
        /// Shows desktop alert.
        /// </summary>
        /// <param name="text">Text to show on the alert. Text supports text-markup.</param>
        /// <param name="symbol">Symbol to show on the alert, see http://www.devcomponents.com/kb2/?p=1347 </param>
        /// <param name="symbolSet">Symbol set to use</param>
        /// <param name="symbolColor">Symbol color or Color.Empty to use default text color</param>
        /// <param name="position">Alert position on the screen</param>
        /// <param name="alertColor">Alert color</param>
        /// <param name="alertDurationSeconds">Duration of alert in the seconds.</param>
        /// <param name="alertId">Alert ID used to recognize alert if clicked and specified Action is called</param>
        /// <param name="alertClickAction">Action method to call if alert is clicked.</param>
        /// <param name="markupLinkClickHandler">Text-markup link click event handler.</param>
        /// <param name="referenceControl">Specifies reference control which is used to find target screen alert is displayed on.</param>
        public static void Show(string text, string symbol, eSymbolSet symbolSet, Color symbolColor, eDesktopAlertColor alertColor, eAlertPosition position,
            int alertDurationSeconds, long alertId, Action<long> alertClickAction, MarkupLinkClickEventHandler markupLinkClickHandler, Control referenceControl)
        {
            DesktopAlertWindow alert = new DesktopAlertWindow();
            alert.Text = text;
            alert.MaximumSize = Dpi.Size(_MaximumAlertSize);
            alert.AlertPosition = position;
            alert.Symbol = symbol;
            alert.SymbolSet = symbolSet;
            alert.SymbolColor = symbolColor;
            alert.AlertId = alertId;
            alert.ClickAction = alertClickAction;
            alert.AutoCloseTimeOut = alertDurationSeconds;
            alert.TextMarkupEnabled = _TextMarkupEnabled;
            if (markupLinkClickHandler != null)
                alert.MarkupLinkClick += markupLinkClickHandler;
            alert.AlertAnimationDuration = _AlertAnimationDuration;
            alert.PlaySound = _PlaySound;
            alert.ReferenceControl = referenceControl;
            SetColors(alert, alertColor);
            alert.Show();
        }

        /// <summary>
        /// Shows desktop alert.
        /// </summary>
        /// <param name="text">Text to show on the alert. Text supports text-markup.</param>
        /// <param name="image">Image to display on the alert</param>
        /// <param name="position">Alert screen position</param>
        /// <param name="alertColor">Alert color</param>
        /// <param name="alertDurationSeconds">Duration of alert in seconds</param>
        /// <param name="alertId">Alert ID used to recognize alert if clicked and specified Action is called</param>
        /// <param name="alertClickAction">Action method to call if alert is clicked.</param>
        public static void Show(string text, Image image, eDesktopAlertColor alertColor, eAlertPosition position,
            int alertDurationSeconds, long alertId, Action<long> alertClickAction)
        {
            Show(text, image, alertColor, position, alertDurationSeconds, alertId, alertClickAction, null);
        }

        /// <summary>
        /// Shows desktop alert.
        /// </summary>
        /// <param name="text">Text to show on the alert. Text supports text-markup.</param>
        /// <param name="image">Image to display on the alert</param>
        /// <param name="position">Alert screen position</param>
        /// <param name="alertColor">Alert color</param>
        /// <param name="alertDurationSeconds">Duration of alert in seconds</param>
        /// <param name="alertId">Alert ID used to recognize alert if clicked and specified Action is called</param>
        /// <param name="alertClickAction">Action method to call if alert is clicked.</param>
        /// <param name="markupLinkClickHandler">Text-markup link click event handler.</param>
        public static void Show(string text, Image image, eDesktopAlertColor alertColor, eAlertPosition position,
            int alertDurationSeconds, long alertId, Action<long> alertClickAction, MarkupLinkClickEventHandler markupLinkClickHandler)
        {
            Show(text, image, alertColor, position, alertDurationSeconds, alertId, alertClickAction, markupLinkClickHandler, null);
        }

        /// <summary>
        /// Shows desktop alert.
        /// </summary>
        /// <param name="text">Text to show on the alert. Text supports text-markup.</param>
        /// <param name="image">Image to display on the alert</param>
        /// <param name="position">Alert screen position</param>
        /// <param name="alertColor">Alert color</param>
        /// <param name="alertDurationSeconds">Duration of alert in seconds</param>
        /// <param name="alertId">Alert ID used to recognize alert if clicked and specified Action is called</param>
        /// <param name="alertClickAction">Action method to call if alert is clicked.</param>
        /// <param name="markupLinkClickHandler">Text-markup link click event handler.</param>
        /// <param name="referenceControl">Specifies reference control which is used to find target screen alert is displayed on.</param>
        public static void Show(string text, Image image, eDesktopAlertColor alertColor, eAlertPosition position,
            int alertDurationSeconds, long alertId, Action<long> alertClickAction, MarkupLinkClickEventHandler markupLinkClickHandler, Control referenceControl)
        {
            DesktopAlertWindow alert = new DesktopAlertWindow();
            alert.Text = text;
            alert.MaximumSize = Dpi.Size(_MaximumAlertSize);
            alert.AlertPosition = position;
            alert.Image = image;
            alert.AlertId = alertId;
            alert.ClickAction = alertClickAction;
            alert.AutoCloseTimeOut = alertDurationSeconds;
            alert.TextMarkupEnabled = _TextMarkupEnabled;
            if (markupLinkClickHandler != null)
                alert.MarkupLinkClick += markupLinkClickHandler;
            alert.AlertAnimationDuration = _AlertAnimationDuration;
            alert.PlaySound = _PlaySound;
            alert.ReferenceControl = referenceControl;
            SetColors(alert, alertColor);
            alert.Show();
        }

        private static void SetColors(DesktopAlertWindow w, eDesktopAlertColor c)
        {
            if (c == eDesktopAlertColor.Default)
            {
                w.BackColor = ColorScheme.GetColor(0x0078D7);
                w.ForeColor = Color.White;
            }
            else if (c == eDesktopAlertColor.Black)
            {
                w.BackColor = Color.Black;
                w.ForeColor = Color.White;
            }
            else if (c == eDesktopAlertColor.Blue)
            {
                w.BackColor = ColorScheme.GetColor(0x5B9BD5);
                w.ForeColor = Color.White;
            }
            else if (c == eDesktopAlertColor.BlueGray)
            {
                w.BackColor = ColorScheme.GetColor(0x44546A);
                w.ForeColor = Color.White;
            }
            else if (c == eDesktopAlertColor.DarkBlue)
            {
                w.BackColor = ColorScheme.GetColor(0x4472C4);
                w.ForeColor = Color.White;
            }
            else if (c == eDesktopAlertColor.DarkRed)
            {
                w.BackColor = ColorScheme.GetColor(0xC00000);
                w.ForeColor = Color.White;
            }
            else if (c == eDesktopAlertColor.Gold)
            {
                w.BackColor = ColorScheme.GetColor(0xFFC000);
                w.ForeColor = Color.Black;
            }
            else if (c == eDesktopAlertColor.Gray)
            {
                w.BackColor = ColorScheme.GetColor(0xE7E6E6);
                w.ForeColor = Color.Black;
            }
            else if (c == eDesktopAlertColor.Green)
            {
                w.BackColor = ColorScheme.GetColor(0x375623);
                w.ForeColor = Color.White;
            }
            else if (c == eDesktopAlertColor.Orange)
            {
                w.BackColor = ColorScheme.GetColor(0xCA5010);
                w.ForeColor = Color.White;
            }
            else if (c == eDesktopAlertColor.Red)
            {
                w.BackColor = ColorScheme.GetColor(0xE81123);
                w.ForeColor = Color.White;
            }
        }

        private static Size _MaximumAlertSize = new Size(400, 128);
        /// <summary>
        /// Indicates maximum alert size.
        /// </summary>
        public static Size MaximumAlertSize
        {
            get { return _MaximumAlertSize; }
            set { _MaximumAlertSize = value; }
        }

        private static eAlertPosition _AlertPosition = eAlertPosition.BottomRight;
        /// <summary>
        /// Specifies default alert screen position.
        /// </summary>
        public static eAlertPosition AlertPosition
        {
            get { return _AlertPosition; }
            set { _AlertPosition = value; }
        }

        private static eDesktopAlertColor _AlertColor = eDesktopAlertColor.Default;
        /// <summary>
        /// Specifies default alert color.
        /// </summary>
        public static eDesktopAlertColor AlertColor
        {
            get { return _AlertColor; }
            set { _AlertColor = value; }
        }

        private static int _AlertAnimationDuration = 200;
        /// <summary>
        /// Gets or sets the total time in milliseconds alert animation takes.
        /// Default value is 200.
        /// </summary>
        public static int AlertAnimationDuration
        {
            get { return _AlertAnimationDuration; }
            set { _AlertAnimationDuration = value; }
        }

        private static int _AutoCloseTimeOut = 6;
        /// <summary>
        /// Gets or sets time period in seconds after alert closes automatically.
        /// </summary>
        public static int AutoCloseTimeOut
        {
            get { return _AutoCloseTimeOut; }
            set { _AutoCloseTimeOut = value; }
        }

        private static bool _TextMarkupEnabled = true;
        /// <summary>
        /// Gets or sets whether text-markup can be used in alert text, default value is true.
        /// </summary>
        public static bool TextMarkupEnabled
        {
            get { return _TextMarkupEnabled; }
            set { _TextMarkupEnabled = value; }
        }

        private static bool _PlaySound = true;
        /// <summary>
        /// Indicates whether alert plays exclamation sound when shown.
        /// </summary>
        public static bool PlaySound
        {
            get { return _PlaySound; }
            set { _PlaySound = value; }
        }

        /// <summary>
        /// Occurs before alert is displayed and allows access to the alert Window through sender.
        /// </summary>
        [Description("Occurs before alert is displayed and allows access to the alert Window through sender.")]
        public static event EventHandler BeforeAlertDisplayed;

        /// <summary>
        /// Raises BeforeAlertDisplayed event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        internal static void OnBeforeAlertDisplayed(DesktopAlertWindow w, EventArgs e)
        {
            EventHandler h = BeforeAlertDisplayed;
            if (h != null)
                h(w, e);
        }

        /// <summary>
        /// Occurs after alert as been closed.
        /// </summary>
        [Description("Occurs after alert has been closed.")]
        public static event AlertClosedEventHandler AlertClosed;

        /// <summary>
        /// Raises RemovingToken event.
        /// </summary>
        /// <param name="e">Provides event arguments.</param>
        internal static void OnAlertClosed(object sender, AlertClosedEventArgs e)
        {
            AlertClosedEventHandler handler = AlertClosed;
            if (handler != null)
                handler(sender, e);
        }
    }

    /// <summary>
    /// Defines delegate for AlertClosed event.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="args">Event arguments</param>
    public delegate void AlertClosedEventHandler(object sender, AlertClosedEventArgs args);
    /// <summary>
    /// Defines event arguments for AlertClosed event.
    /// </summary>
    public class AlertClosedEventArgs : EventArgs
    {
        /// <summary>
        /// Specifies alert closure source.
        /// </summary>
        public readonly eAlertClosureSource ClosureSource;

        public AlertClosedEventArgs(eAlertClosureSource source)
        {
            ClosureSource = source;
        }
    }

    /// <summary>
    /// Defines predefined desktop alert colors.
    /// </summary>
    public enum eDesktopAlertColor
    {
        Default,
        DarkRed,
        Black,
        Gray,
        BlueGray,
        Blue,
        Orange,
        Gold,
        DarkBlue,
        Green,
        Red
    }
}
