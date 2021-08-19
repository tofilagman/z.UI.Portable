using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace DevComponents.DotNetBar.Rendering
{
    /// <summary>
    /// Represents the color table for TokenEditor control tokens.
    /// </summary>
    public class TokenEditorColorTable
    {
        /// <summary>
        /// Initializes a new instance of the TokenEditorColorTable class.
        /// </summary>
        public TokenEditorColorTable()
        {
            
        }
        /// <summary>
        /// Initializes a new instance of the TokenEditorColorTable class.
        /// </summary>
        public TokenEditorColorTable(int normalTextColor, int normalBackground, int mouseOverTextColor, int mouseOverBackground, int focusedTextColor, int focusedBackground)
        {
            Normal = new TokenColorTable(normalTextColor, normalBackground);
            MouseOver = new TokenColorTable(mouseOverTextColor, mouseOverBackground);
            Focused = new TokenColorTable(focusedTextColor, focusedBackground);
        }
        /// <summary>
        /// Initializes a new instance of the TokenEditorColorTable class.
        /// </summary>
        public TokenEditorColorTable(Color normalTextColor, Color normalBackground, Color mouseOverTextColor, Color mouseOverBackground, Color focusedTextColor, Color focusedBackground)
        {
            Normal = new TokenColorTable(normalTextColor, normalBackground);
            MouseOver = new TokenColorTable(mouseOverTextColor, mouseOverBackground);
            Focused = new TokenColorTable(focusedTextColor, focusedBackground);
        }
        /// <summary>
        /// Gets or sets token default state colors.
        /// </summary>
        public TokenColorTable Normal = new TokenColorTable(0x000000, 0xDBE9FC);
        /// <summary>
        /// Gets or sets token mouse over state colors.
        /// </summary>
        public TokenColorTable MouseOver = new TokenColorTable(0x000000, 0xBFDBFF);
        /// <summary>
        /// Gets or sets token focused state colors.
        /// </summary>
        public TokenColorTable Focused = new TokenColorTable(0xFFFFFF, 0x0072C6);
    }

    /// <summary>
    /// Represents the state color table for token in TokenEditor control.
    /// </summary>
    public class TokenColorTable
    {
        /// <summary>
        /// Initializes a new instance of the TokenColorTable class.
        /// </summary>
        public TokenColorTable()
        {
        }
        /// <summary>
        /// Initializes a new instance of the TokenColorTable class.
        /// </summary>
        /// <param name="textColor"></param>
        /// <param name="new"></param>
        public TokenColorTable(Color textColor, Color background)
        {
            TextColor = textColor;
            Background = new LinearGradientColorTable(background);
        }
        /// <summary>
        /// Initializes a new instance of the TokenColorTable class.
        /// </summary>
        /// <param name="textColor"></param>
        /// <param name="new"></param>
        public TokenColorTable(int textColor, int background)
        {
            TextColor = ColorScheme.GetColor(textColor);
            Background = new LinearGradientColorTable(background);
        }
        /// <summary>
        /// Gets or sets token text color.
        /// </summary>
        public Color TextColor = Color.Black;
        /// <summary>
        /// Gets or sets the background color table.
        /// </summary>
        public LinearGradientColorTable Background = new LinearGradientColorTable(0xE5EEF8);
    }
}
