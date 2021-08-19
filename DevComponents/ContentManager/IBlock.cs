using System;
using System.Drawing;
using DevComponents.DotNetBar;

 
namespace DevComponents.UI.ContentManager
 
{
	/// <summary>
	/// Represents a content block interface.
	/// </summary>
	public interface IBlock
	{
		/// <summary>
		/// Gets or sets the bounds of the content block.
		/// </summary>
		Rectangle Bounds {get;set;}
		/// <summary>
		/// Gets or sets whether content block is visible.
		/// </summary>
		bool Visible {get;set;}
        /// <summary>
        /// Gets or sets the block margins.
        /// </summary>
        DevComponents.DotNetBar.Padding Margin { get;set;}
	}
}
