using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace plugEditor
{
    #region ISearchable

    /// <summary>
    /// Searchable controls inherit from ISearchable interface simply to make it easier to invoke
    /// searches from a form's menu or toolbars when multiple searchable controls are used.
    /// </summary>
    public interface ISearchable
    {
        /// <summary>
        /// Return the FindDialog (if any) currently associated with this control
        /// </summary>
        FindDialog FindDialog { get; }
    }

    #endregion

    #region ReplaceEventArgs

    /// <summary>
    /// Search event arguments for a control replace operation
    /// </summary>
    public class ReplaceEventArgs : EventArgs
    {
        /// <summary>
        /// The text to replace the most recent selection
        /// </summary>
        public string ReplaceText
        {
            get { return replaceText; }
            set { replaceText = value; }
        }

        private string replaceText;
    }

    #endregion

    #region ReplaceEventHandler

    /// <summary>
    /// The search dialog is requesting a replace of the last selected text
    /// </summary>
    /// <param name="sender">Sender object (the search dialog)</param>
    /// <param name="e">Replace event parameters</param>
    public delegate void ReplaceEventHandler(object sender, ReplaceEventArgs e);

    #endregion

    #region SearchableControls

    /// <summary>
    /// Place holder for utility functions relating to the SearchableControls collection
    /// </summary>
    /// <remarks>
    /// <para>Part of SearchableControls written by Jim Blackler (jimblackler@gmail.com), August 2006</para>
    /// <para>Note that these functions are only worth using if you have more than one SearchableControl on 
    /// a form.</para>
    /// </remarks>
    public class Utility
    {
        /// <summary>
        /// Returns either the focused control, if it is ISearchable. Otherwise the
        /// searchable control with the lowest TabIndex.
        /// </summary>
        /// <param name="controlCollection">The control collection to search</param>
        /// <remarks>
        /// Provided as a utility function to allow client applications to easily provide a Find option in 
        /// their forms' Edit menus, or on toolbars.
        /// </remarks>
        public static ISearchable FindSearchable(Control.ControlCollection controlCollection)
        {
            ISearchable firstSearchable = null; // keep a record of the first searchable control found

            // Look at each child control to find a focused, searchable control
            //foreach (Control control in controlCollection)
            //{
            //    ISearchable searchable = control as ISearchable;
            //    if (searchable != null)
            //    {
            //        if (control.Focused)
            //        {
            //            return searchable;
            //        }
            //        else if (firstSearchable == null || control.TabIndex < ((Control)firstSearchable).TabIndex)
            //        {
            //            firstSearchable = searchable;
            //        }
            //    }
            //}

            EditorForm ed = new EditorForm();

            foreach (Control control in controlCollection)
            {
                ISearchable searchable = control as ISearchable;
                if (searchable != null)
                {
                    if (control.Focused)
                    {
                        return searchable;
                    }
                    else if (firstSearchable == null || control.TabIndex < ((Control)firstSearchable).TabIndex)
                    {
                        firstSearchable = searchable;
                    }
                }
            }

            // No controls were focused.. return the control with the lowest TabIndex
            return firstSearchable;
        }

        /// <summary>
        /// Opens the find dialog on the most appropriate control in the container
        /// </summary>
        /// <returns>'True' if a find dialog was found to open</returns>
        /// <remarks>
        /// Uses the utility functions FindSearchable() with the form's ControlCollection to find either the
        /// focused control, if it is ISearchable, or the searchable control with the lowest TabIndex.
        /// Calls OpenDialog on the resulting control.
        /// </remarks>
        public static bool OpenFindDialog(Control.ControlCollection controls)
        {
            //ISearchable searchable = Utility.FindSearchable(controls);
            //if (searchable != null)
            //{
            //SearchableTextBox.FindDialog.Show();
                return true;
            //}
            //return false;
        }

        /// <summary>
        /// Calls FindNext() on the most appropriate control in the container
        /// </summary>
        /// <returns>'True' if FindNext executed successfully on a control</returns>
        /// <remarks>
        /// Uses the utility functions FindSearchable() with the form's ControlCollection to find either the
        /// focused control, if it is ISearchable, or the searchable control with the lowest TabIndex.
        /// Calls OpenDialog on the resulting control.
        /// </remarks>
        public static bool FindNext(Control.ControlCollection controls)
        {
            ISearchable searchable = Utility.FindSearchable(controls);
            if (searchable != null)
            {
                searchable.FindDialog.FindNext();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Informs the user if FindNext is available in the supplied control selection
        /// </summary>
        /// <returns>The value of FindNextIsAvailable() on the most appropriate control in the container</returns>
        /// <remarks>
        /// Uses the utility functions FindSearchable() with the form's ControlCollection to find either the
        /// focused control, if it is ISearchable, or the searchable control with the lowest TabIndex.
        /// Calls OpenDialog on the resulting control.
        /// </remarks>
        public static bool FindNextIsAvailable(Control.ControlCollection controls)
        {
            ISearchable searchable = Utility.FindSearchable(controls);
            if (searchable != null)
            {
                return searchable.FindDialog.FindNextIsAvailable();
            }
            return false;
        }
    }

    #endregion

    #region SearchEventArgs

    /// <summary>
    /// Search event arguments for a control search
    /// </summary>
    public class SearchEventArgs : EventArgs
    {
        /// <summary>
        /// Whether or not the search has returned a match
        /// </summary>
        /// <remarks>To be set by the </remarks>
        public bool Successful
        {
            get { return successful; }
            set { successful = value; }
        }

        private bool successful = false;

        /// <summary>
        /// Whether or not the search restarted from the top of the document(for the find dialog to reflect)
        /// </summary>
        public bool RestartedFromDocumentTop
        {
            get { return restartedFromDocumentTop; }
            set { restartedFromDocumentTop = value; }
        }

        private bool restartedFromDocumentTop = false;

        /// <summary>
        /// 'True' if this is the first search performed
        /// </summary>
        /// <remarks>
        /// Optional, to be used by the searching control to remember the selection position 
        /// when searching begins.
        /// </remarks>
        public bool FirstSearch
        {
            get { return firstSearch; }
        }

        private bool firstSearch;

        /// <summary>
        /// The regular expression that performs the search
        /// </summary>
        public Regex SearchRegularExpression
        {
            get { return searchRegularExpression; }
        }

        private Regex searchRegularExpression;

        /// <summary>
        /// Makes search event arguments for a control search
        /// </summary>
        public SearchEventArgs(Regex _searchRegularExpression, bool _firstSearch)
        {
            searchRegularExpression = _searchRegularExpression;
            firstSearch = _firstSearch;
        }
    }

    #endregion

    #region SearchEventHandler

    /// <summary>
    /// The search dialog is requesting a search
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Search event parameters</param>
    public delegate void SearchEventHandler(object sender, SearchEventArgs e);

    #endregion
}