using System;
using System.Text;

 
namespace DevComponents.UI.ContentManager
 
{
    /// <summary>
    /// Represents a extended content block interface for advanced layout information.
    /// </summary>
    public interface IBlockExtended : IBlock
    {
        bool IsBlockElement { get;}
        bool IsNewLineAfterElement { get;}
        bool CanStartNewLine { get; }
        /// <summary>
        /// Returns whether element is an container so it receives full available size of parent control for layout.
        /// </summary>
        bool IsBlockContainer { get; }
    }
}
